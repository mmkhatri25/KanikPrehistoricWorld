using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class EnemyAI : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener, IListener, IPlayerContactEvent
{
	[Header("Behavior")]
	public float gravity = 35f;
	[Tooltip("allow push the enemy back when hit by player")]
	public bool pushEnemyBack = true;
	Vector2 pushForce;
	public GameObject spawnItemWhenDead;

	[Header("Health")]
	public HealthType healthType;
	public int maxHitToKill = 1;
	[HideInInspector]
	public int currentHitLeft;
	public float health;
	float currentHealth;
	public GameObject HurtEffect;

	[Header("Moving")]
	public float moveSpeed = 3;
	public bool ignoreCheckGroundAhead = false;
	public GameObject DestroyEffect;
	public enum HealthType{HitToKill, HealthAmount}

	[Header("Patrol")]
	public bool usePatrol = true;
	public float patrolLimitLeft = 2;
	public float patrolLimitRight = 3;
	protected float _patrolLimitLeft, _patrolLimitRight;

	[Header("Sound")]
	public AudioClip hurtSound;
	[Range(0,1)]
	public float hurtSoundVolume = 0.5f;
	public AudioClip deadSound;
	[Range(0,1)]
	public float deadSoundVolume = 0.5f;

	protected bool isStop = false;
	[Header("Projectile")]
	public bool isUseProjectile;
	public LayerMask shootableLayer;
	public Transform PointSpawn;
	public Projectile projectile;
	public float fireRate = 1f;
	public float detectDistance = 10f;
	float _fireIn;

	public bool isPlaying{ get; set; }
	public bool isSocking{ get; set; }
	public bool isDead{ get; set; }

	[HideInInspector]
	public Vector3 velocity;
	private Vector2 _direction;
	private Vector2 _startPosition;	//set this enemy back to the first position when Player spawn to check point
	private Vector2 _startScale;	//set this enemy back to the first position when Player spawn to check point
	[HideInInspector]
	public Controller2D controller;

	float velocityXSmoothing = 0;

	public bool isFacingRight()
	{
		return _direction.x == 1;
	}

	// Use this for initialization
	public virtual void Start () {
		controller = GetComponent<Controller2D> ();
		_direction = Vector2.left;
		_startPosition = transform.position;
		_startScale = transform.localScale;
		_fireIn = fireRate;
		currentHealth = health;
		currentHitLeft = maxHitToKill;

		isPlaying = true;
		isSocking = false;

		_patrolLimitLeft = transform.position.x - patrolLimitLeft;
		_patrolLimitRight = transform.position.x + patrolLimitRight;

		velocity = Vector2.zero;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		if (GameManager.Instance.State == GameManager.GameState.Success)
			enabled = false;
		
		if (!isPlaying || isSocking)
			return;

		_fireIn -= Time.deltaTime;

		if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
			|| (!ignoreCheckGroundAhead && !controller.isGrounedAhead(isFacingRight()) && controller.collisions.below)
			|| (usePatrol && ((!isFacingRight() && transform.position.x <= _patrolLimitLeft) || (isFacingRight() && transform.position.x > _patrolLimitRight)))) {

            velocity.x = 0;
			_direction = -_direction;
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}

		if (isUseProjectile) {
			var position = PointSpawn != null ? PointSpawn.position : transform.position;
			var hit = Physics2D.Raycast (position, _direction, detectDistance, shootableLayer);
			if (hit) {
				if (hit.collider.gameObject.GetComponent<Player> () != null)
					FireProjectile ();
			}
		}
	}

	public virtual void LateUpdate(){
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;


		if (GameManager.Instance.Player == null || !GameManager.Instance.Player.isPlaying)
			return;

		if (isPlaying && !isSocking) {
			float targetVelocityX = _direction.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);
		}

		velocity.y += -gravity * Time.deltaTime;

		if (isStop)
			velocity = Vector2.zero;
		
		controller.Move (velocity * Time.deltaTime, false);

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;
	}

	public void SetForce(float x, float y){
		velocity = new Vector3 (x, y, 0);
	}

	private void FireProjectile(){
		if (_fireIn > 0)
			return;

		_fireIn = fireRate;
		var _projectile = (Projectile) Instantiate (projectile, PointSpawn.position, Quaternion.identity);
		_projectile.Initialize (gameObject, _direction, Vector2.zero);
	}

	bool allowDamage = true;
	public virtual void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		
		if (isDead || !allowDamage)
			return;
		
		pushForce = force;

		if (HurtEffect != null)
			Instantiate (HurtEffect, transform.position, Quaternion.identity);

		if (healthType == HealthType.HitToKill) {
			currentHitLeft--;
			if (currentHitLeft <= 0) {
				isDead = true;
			}
		} else if (healthType == HealthType.HealthAmount) {
			currentHealth -= damage;
			if (currentHealth <= 0) {
				isDead = true;
			}
		}

		if (instigator!= null && instigator.GetComponent<Block> () != null)
			isDead = true;

		HitEvent ();

		StartCoroutine (delayHitCo ());
	}

	IEnumerator delayHitCo(){
		allowDamage = false;
		yield return new WaitForSeconds (0.3f);

		allowDamage = true;

	}

	protected virtual void HitEvent(){
		
		SoundManager.PlaySfx (hurtSound, hurtSoundVolume);

		StopAllCoroutines ();
		StartCoroutine(PushBack (0.35f));
	}


	protected virtual void Dead(){
		isPlaying = false;

		StopAllCoroutines ();
		SoundManager.PlaySfx (deadSound, deadSoundVolume);
		if (DestroyEffect != null)
			Instantiate (DestroyEffect, transform.position, transform.rotation);

		if(spawnItemWhenDead!=null)
			Instantiate (spawnItemWhenDead, PointSpawn.position, PointSpawn.rotation);

		//turn off all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = false;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = false;
		}

	}

	protected virtual void OnRespawn(){

	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		
		currentHealth = health;
		currentHitLeft = maxHitToKill;
		transform.position = _startPosition;
		transform.localScale = _startScale;
		_direction = Vector2.left;
		velocity = Vector3.zero;
		isPlaying = true;
		isDead = false;
		isSocking = false;
		gameObject.SetActive (true);

		//turn on all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = true;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = true;
		}


		OnRespawn ();
	}

	public IEnumerator PushBack(float delay){
		
		isPlaying = false;
		SetForce (GameManager.Instance.Player.transform.localScale.x * pushForce.x, pushForce.y);

		if (isDead) {
			Dead ();
			yield break;
		}
		
		yield return new WaitForSeconds (delay);

		SetForce (0, 0);
		isPlaying = true;
	}

	public void OnDrawGizmosSelected(){
		if (isUseProjectile) {
			Gizmos.color = Color.blue;
			if (_direction.magnitude != 0)
				Gizmos.DrawRay (PointSpawn.position, _direction * detectDistance);
			else
				Gizmos.DrawRay (PointSpawn.position, Vector2.left * detectDistance);
		}
	}

    private void OnDrawGizmos()
    {
		if (usePatrol)
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(new Vector2(_patrolLimitLeft, transform.position.y), 0.3f);
				Gizmos.DrawWireSphere(new Vector2(_patrolLimitRight, transform.position.y), 0.3f);
				Gizmos.DrawLine(new Vector2(_patrolLimitLeft, transform.position.y), new Vector2(_patrolLimitRight, transform.position.y));

			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position - Vector3.right * patrolLimitLeft, 0.3f);
				Gizmos.DrawWireSphere(transform.position + Vector3.right * patrolLimitRight, 0.3f);
				Gizmos.DrawLine(transform.position, transform.position + Vector3.right * patrolLimitRight);
				Gizmos.DrawLine(transform.position, transform.position - Vector3.right * patrolLimitLeft);
			}
		}
	}

    #region IListener implementation

    public void IPlay()
    {
        //		throw new System.NotImplementedException ();
    }

    public void ISuccess()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IUnPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IGameOver()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnRespawn()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }

	#endregion

	[Header("---JUMP ON HEAD---")]
	public bool ignorePlayerThroughEnemy = false;
	public float makeDamage = 30;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2(15, 10);
	float nextDamage;

	public bool canBeKillOnHead = true;
	public bool dealDamageToPlayer = true;
	public void OnPlayerContact(CONTACT_POS contactPos, Vector2 hitPoint)
	{
		if (contactPos == CONTACT_POS.Above)
		{
			if (canBeKillOnHead)
				TakeDamage(1, Vector2.zero, GameManager.Instance.Player.gameObject, hitPoint);
			else if (dealDamageToPlayer)
				DealDamageToPlayer();
		}
		else if (contactPos == CONTACT_POS.Right)
			OnPlayerContactRight();
		else if (contactPos == CONTACT_POS.Left)
			OnPlayerContactLeft();
	}

	public virtual void OnPlayerContactRight()
	{
		if (dealDamageToPlayer)
		{
			DealDamageToPlayer();
		}
	}
	public virtual void OnPlayerContactLeft()
	{
		if (dealDamageToPlayer)
		{
			DealDamageToPlayer();
		}
	}

	void DealDamageToPlayer()
	{
		if (Time.time < nextDamage + rateDamage)
			return;

		nextDamage = Time.time;

		if (makeDamage == 0)
			return;

		var facingDirectionX = Mathf.Sign(GameManager.Instance.Player.transform.position.x - transform.position.x);
		var facingDirectionY = Mathf.Sign(GameManager.Instance.Player.velocity.y);

		Vector2 _makeForce = new Vector2(Mathf.Clamp(Mathf.Abs(GameManager.Instance.Player.velocity.x), 10, 15) * facingDirectionX,
			Mathf.Clamp(Mathf.Abs(GameManager.Instance.Player.velocity.y), 5, 9) * facingDirectionY * -1);

		GameManager.Instance.Player.TakeDamageFromContactEnemy(makeDamage, _makeForce, gameObject, ignorePlayerThroughEnemy);
	}

}
