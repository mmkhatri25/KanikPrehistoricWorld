using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class EnemyGrounded : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener, IListener, IPlayerContactEvent
{
	[Header("Behavior")]

	public bool isAllowChasingPlayer = false;
	public bool isChasing{ get; set; }		//set by another script
	public bool canBeFallDown = false;	//if true, the enemy will be fall from the higher platform

	[Header("Chasing")]
	public float chaseSpeed = 3;
	public float offsetPlayerY = 0.5f;
	public float finishDistance = 0.5f;

    public float dismissPlayerDistance = 15;

	[Header("Setup")]
	public float gravity = 35f;
	public float moveSpeed = 3;
	public float waitingTurn = 0.5f;
	bool isWaiting = false;
	float waitingTime = 0;
	public float sockingTime = 0.5f;
	public GameObject BonusItem;	//spawn bonus item when it dead
	public GameObject HurtEffect;
	public GameObject DestroyEffect;		//spawn object when dead
	public float destroyTime = 1.5f;
	public LayerMask playerLayer;   //detect player to attack/chasing

	[Header("Patrol")]
	public bool usePatrol = true;
	public float patrolLimitLeft = 2;
	public float patrolLimitRight = 3;
	protected float _patrolLimitLeft, _patrolLimitRight;

	[Header("Health")]
	[Range(0,100)]
	public float health = 50;
	float currentHealth;

	[Header("Sound")]
	public AudioClip soundMeleeAttack;
	public AudioClip soundRangAttack;
	public AudioClip hurtSound;
	[Range(0,1)]
	public float hurtSoundVolume = 0.5f;
	public AudioClip deadSound;
	[Range(0,1)]
	public float deadSoundVolume = 0.5f;


	public enum WeaponsType {None, Melee, Range}
	[Header("Attack")]
	public WeaponsType attackType;

	public float fireRate = 2f;
	float _fireIn;
	bool isDetectingPlayer;
	public float detectDistance = 3f;

	[Header("Melee Attack")]
	public Transform meleePoint;
	public float meleeAttackZone = .7f;
	public float meleeAttackCheckPlayer = 0.1f;
	public float meleeDamage = 20;	//give damage to player

	[Header("Range Attack")]
	public Transform rangePoint;
	public Projectile rangeprojectile;
	public float fireDelay = 0.1f;

	[Header("Grenade")]
	public bool allowThrowGrenade = false;
	public float angleThrow = 60;		//the angle to throw the bomb
	public float throwForce = 300;		//how strong?
	public Transform throwPosition;		//throw the bomb at this position
	public GameObject _Grenade;		//the bomb prefab object


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
	Animator animator;

	float velocityXSmoothing = 0;
	Vector2 pushForce;
	private float _directionFace;
	bool isStop = false;
	bool detectToWaiting = false;		//mean when detect player, player go to hiding zone, wait there and attack player when player go out hiding zone
	Vector3 waitingAfterPlayerGoHidingZone;

	public bool isFacingRight()
	{
		return _direction.x == 1;
	}

	// Use this for initialization
	IEnumerator Start () {
		controller = GetComponent<Controller2D> ();
		animator = GetComponent<Animator> ();
		_direction = Vector2.left;
		if (moveSpeed == 0) {
			_direction = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
		}
		_startPosition = transform.position;
		_startScale = transform.localScale;
		_fireIn = fireRate;
		currentHealth = health;

		isPlaying = true;
		isSocking = false;
		isChasing = false;

		yield return new WaitForEndOfFrame ();
		controller.collisions.faceDir = -1;

		_patrolLimitLeft = transform.position.x - patrolLimitLeft;
		_patrolLimitRight = transform.position.x + patrolLimitRight;
	}


	
	// Update is called once per frame
	void Update () {
		if (isStop)
			return;
		
		HandleAnimation ();
		
		_fireIn -= Time.deltaTime;
//		Debug.Log (isPlaying + "/" + isSocking);

		if (!isPlaying || isSocking || !GameManager.Instance.Player.isPlaying) {
			velocity.x = 0;
			return;
		}

		if (isWaiting) {
			waitingTime += Time.deltaTime;
			if (waitingTime >= waitingTurn) {
				isWaiting = false;
				waitingTime = 0;

				_direction = -_direction;
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		} else {
			if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
				   || (!canBeFallDown && !controller.isGrounedAhead(isFacingRight()) && controller.collisions.below)
				   || (usePatrol && ((!isFacingRight() && transform.position.x <= _patrolLimitLeft) || (isFacingRight() && transform.position.x > _patrolLimitRight))))
			{

				isWaiting = true;
			}
		}


		if (attackType != WeaponsType.None && !isSocking) {
			var hit = Physics2D.Raycast (transform.position, _direction, detectDistance, playerLayer);
			isDetectingPlayer = hit;
			if (hit) {
				if (hit.collider.gameObject.GetComponent<Player> () != null)
				if (_fireIn <= 0) {
					_fireIn = fireRate;

//					StartCoroutine (StopCo (fireRate+0.1f));

					if (attackType == WeaponsType.Range) {
						if (allowThrowGrenade)
							ThrowGrenade ();
						else
							FireProjectile ();
					}
					else
						StartCoroutine (CheckMeleeAttack (meleeAttackCheckPlayer));
				}
			}
		}
	}

	public virtual void LateUpdate(){
		if (isStop)
			return;
		
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;
		
		if (!isPlaying || isSocking || isDetectingPlayer) {
			velocity.x = 0;
			//return;
		}

		if (!GameManager.Instance.Player.isPlaying)
			return;
		
		if (isChasing && isAllowChasingPlayer) {
            if (GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer("HidingZone")) {
                if ((Mathf.Abs(Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position)) > finishDistance) && (Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position) < dismissPlayerDistance)) {
                    transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position + new Vector3(0, offsetPlayerY, 0), chaseSpeed * Time.deltaTime);
                    _directionFace = transform.position.x > GameManager.Instance.Player.transform.position.x ? 1 : -1;
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _directionFace, transform.localScale.y, transform.localScale.z);
                }
            }

			else {
                isChasing = false;
                if (gravity == 0) {
                    detectToWaiting = true; //mean this enemy is flying kind
                    waitingAfterPlayerGoHidingZone = GameManager.Instance.Player.transform.position + Vector3.up * 5;
                } else
                    transform.localScale = new Vector3((velocity.x > 0 ? -1 : 1) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
		
		} else if (detectToWaiting) {
			//transform.position = Vector3.MoveTowards (transform.position, waitingAfterPlayerGoHidingZone, chaseSpeed * Time.deltaTime);
			if (GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer ("HidingZone")) {
				isChasing = true;
				detectToWaiting = false;
			}
		}

		else {
			float targetVelocityX = _direction.x * moveSpeed;
			velocity.x = isWaiting?0: Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);
	

			velocity.y += -gravity * Time.deltaTime;
			if (isStop)
				velocity = Vector2.zero;
			controller.Move (velocity * Time.deltaTime, false);

			if (controller.collisions.above || controller.collisions.below)
				velocity.y = 0;
		}
	}

	void HandleAnimation(){
		animator.SetFloat ("speed", Mathf.Abs (velocity.x));
	}


	IEnumerator CheckMeleeAttack(float delay){
		animator.SetTrigger ("attack");
		isPlaying = false;

		yield return new WaitForSeconds (delay);

		if (isSocking) {
			isPlaying = true;
			yield break;
		}

		var hit = Physics2D.CircleCast (meleePoint.position, meleeAttackZone, Vector2.zero, 0, playerLayer);

		if (!hit) {
			isPlaying = true;
			yield break;
		}

		var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
		if (damage == null) {
			isPlaying = true;
			yield break;
		}
		damage.TakeDamage (meleeDamage, Vector2.zero, gameObject, hit.point);

		yield return new WaitForSeconds (fireRate);
		isPlaying = true;
	}

	public void SetForce(float x, float y){
		velocity = new Vector3 (x, y, 0);
	}

	private void FireProjectile(){
		animator.SetTrigger ("attack");
		StartCoroutine (FireObjectCo ());

	}

	IEnumerator FireObjectCo(){
		yield return new WaitForSeconds (fireDelay);
		var _projectile = (Projectile) Instantiate (rangeprojectile, rangePoint.position, Quaternion.identity);
		_projectile.Initialize (gameObject, _direction, Vector2.zero);
		SoundManager.PlaySfx (soundRangAttack);
	} 

	//This action is called by the Input/ControllerInput
	public void ThrowGrenade(){
		if (!allowThrowGrenade || _Grenade == null)
			return;
		
		GlobalValue.grenade--;
		GameObject obj = Instantiate (_Grenade, throwPosition.position, Quaternion.identity) as GameObject;
		obj.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, transform.localScale.x < 1 ? angleThrow : 180 - angleThrow));
		Debug.Log (obj.transform.eulerAngles.z);
		obj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(throwForce,0));

		animator.SetTrigger ("throw");
	}


	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (isDead)
			return;

		pushForce = force;

		if (HurtEffect != null)
			Instantiate (HurtEffect, hitPoint, Quaternion.identity);

			currentHealth -= damage;
        
		if (currentHealth <= 0) {
			isDead = true;
		}
		

		if (instigator && instigator.GetComponent<Block> () != null)
			isDead = true;

		HitEvent ();

	}

	protected virtual void HitEvent(){

		SoundManager.PlaySfx (hurtSound, hurtSoundVolume);
		StartCoroutine(PushBack (sockingTime));
	}

	protected virtual void Dead(){
		isPlaying = false;

		StopAllCoroutines ();
		SoundManager.PlaySfx (deadSound, deadSoundVolume);

		if(BonusItem!=null)
			Instantiate (BonusItem, transform.position, transform.rotation);

		animator.SetTrigger ("die");

		velocity.x = 0;
		GetComponent<BoxCollider2D> ().enabled = false;

		var isHasSimpleMoving = GetComponent<SimplePathedMoving>();
		if (isHasSimpleMoving)
			isHasSimpleMoving.enabled = false;

		if (gravity == 0)
			gravity = 35;


			Destroy(gameObject, destroyTime);

		controller.collisionMask = 0;
		velocity.y = 6;
        //enabled = false;
	}

	protected virtual void OnRespawn(){

	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{

		currentHealth = health;
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
		isSocking = true;
		SetForce (GameManager.Instance.Player.transform.localScale.x * pushForce.x, pushForce.y);

		if (isDead) {
			Dead ();
			yield break;
		}

		yield return new WaitForSeconds (delay);

		SetForce (0, 0);
		isSocking = false;
		isPlaying = true;
	}

	void OnDrawGizmos(){
        if (isAllowChasingPlayer)
        {
            Gizmos.DrawWireSphere(transform.position, dismissPlayerDistance);
        }
		if (attackType != WeaponsType.None)
		{

			Gizmos.color = Color.white;
			if (_direction.magnitude != 0)
				Gizmos.DrawRay(transform.position, _direction * detectDistance);
			else
				Gizmos.DrawRay(transform.position, Vector2.left * detectDistance);

			if (attackType == WeaponsType.Melee)
			{
				if (meleePoint == null)
					return;

				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(meleePoint.position, meleeAttackZone);
			}
		}

		if (usePatrol)
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(new Vector2( _patrolLimitLeft, transform.position.y), 0.3f);
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

    void OnDestroy(){
		if (DestroyEffect && isDead)
			Instantiate (DestroyEffect, transform.position, Quaternion.identity);
	}

	#region IListener implementation

	public void IPlay ()
	{
//		throw new System.NotImplementedException ();
	}

	public void ISuccess ()
	{
//		throw new System.NotImplementedException ();
	}

	public void IPause ()
	{
//		throw new System.NotImplementedException ();
	}

	public void IUnPause ()
	{
//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
        if (this)
        {
            Debug.Log("IOnStopMovingOn");
            animator.enabled = false;
            isStop = true;
        }
	}

	public void IOnStopMovingOff ()
	{
        if (this)
        {
            animator.enabled = true;
            isStop = false;
        }
	}

	[Header("---JUMP ON HEAD---")]
	public bool ignorePlayerThroughEnemy = false;
	public float makeDamage = 1;
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

	#endregion
}
