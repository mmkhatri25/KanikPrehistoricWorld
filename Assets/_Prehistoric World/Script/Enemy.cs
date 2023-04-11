using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ATTACKTYPE {
	RANGE,
	MELEE,
	THROW,
	CALLMINION,
    BOMBER,
	NONE
}

public enum STARTACTION{
	PATROL,
	AIMPLAYER,
	CHASING,
    STAND
}

public enum DETECTPLAYER{
	WalkAndCheckAttack,
	RunAndCheckAttack,
    RushIntoPlayer
}

public enum DISMISSDETECTPLAYER{
	WalkAndPatrol,
	RunAndPatrol,
	Stand
}

public enum ENEMYSTATE {
	IDLE,
	ATTACK,
    RUSHINTOPLAYER,
    WALK,
	RUN,
	DEATH
}

public enum ENEMYEFFECT {
	NONE,
	BURNING,
	FREEZE,
	SHOKING,
	EXPLOSION
}

public enum STARTBEHAVIOR {
	NONE,
    JUMPUP
}

public enum DIEBEHAVIOR {
	NORMAL,
	BLOWUP,
    FALLOUT
}

public enum HITBEHAVIOR {
	ChasePlayer,
	NONE
}

public enum ENEMYTYPE{
	ONGROUND,
	INWATER,
	FLY
}

[RequireComponent(typeof(CheckTargetHelper))]

public class Enemy : MonoBehaviour, IPlayerContactEvent, ICanTakeDamage, IListener
{
	public string information = "Fill some information of this character";
	[Header("Behavier")]
	public STARTACTION startAction;
	public STARTBEHAVIOR startBehavior;
	public ATTACKTYPE attackType;
	public DETECTPLAYER detectPlayerAct;
	public DISMISSDETECTPLAYER dismissDetectPlayerAct;
	public HITBEHAVIOR hitBehavior;
	public DIEBEHAVIOR dieBehavior;

	[Header("Setup")]
	public ENEMYTYPE enemyType;
	
	[Tooltip("If anim = null, get compomnent itself")]
	public Animator anim;
	[Range(1, 1000)]
	public int health = 100;
	public float gravity = 35f;
	public float walkSpeed = 3;
	public float runSpeed = 5;
	public ParticleSystem runSmokeFX;
	[Tooltip("If choose startBehavior = ShowUp => Will jump this force")]
	public float jumpShowUpForce = 10;
	protected Vector2 rushIntoPlayerPoint;

	[Header("Rewarded")]
	public int point = 100;

	[Header("OPTION")]
	[Tooltip("For ONGROUNDED enemy type: only move horizontal when stand on ground")]
	public bool onlyMoveWhenGrounded = false;

	[Header("Detect and Dismiss Player")]
	public float delayAttackWhenDetectPlayer = 0.5f;
	[ReadOnlyAttribute] public CheckTargetHelper checkTarget;
	public float dismissPlayerDistance = 10;
	public float dismissPlayerWhenStandSecond = 5;
	[ReadOnly] public float countingStanding = 0;
	public bool isPlayerDetected { get; set; }
	public GameObject warningIconDetectPlayer;
	public Vector2 chasingOffset = new Vector2(1.2f, 1f);

	public EnemyRangeAttack rangeAttack;
	public EnemyMeleeAttack meleeAttack;
	public EnemyBomberAttack bomberAttack;
	public EnemyThrowAttack throwAttack;
	public EnemyCallMinion callMinion;

	[Header("Rush Into Player Attack for Fly enemy")]
	public float rushIntoPlayerSpeed = 10;
	protected Vector2 rushIntoPlayerDirection;

	[Header("In Water Setup")]
	public LayerMask waterLayer;
	[ReadOnlyAttribute] public float waterLimitUp;

	public float sockingTime = 0.5f;
	public float destroyTime = 1.5f;
	public int pointToGivePlayer = 100;
	public LayerMask playerLayer;   //detect player to attack/chasing
	[Tooltip("if true, the enemy will be fall from the higher platform")]
	public bool canBeFallDown = false;


	public GameObject dieFX, hitFX;
	public float deadForce = 3;
	public float gravityDie = 10;
	public GameObject bloodPuddleFX;
	public GameObject[] explosionFX;
	public Vector2 randomHitPoint = new Vector2(0.2f, 0.2f);
	public Vector2 randomBloodPuddlePoint = new Vector2(0.5f, 0.25f);

	[Header("Patrol")]
	public bool usePatrolPoint = true;
	public float waitingTurn = 0.5f;
	public float patrolLimitLeft = 2;
	public float patrolLimitRight = 3;
	protected bool isWaiting = false;
	protected float waitingTime = 0;
	protected float _patrolLimitLeft, _patrolLimitRight;

	[ReadOnly] public ENEMYSTATE enemyState;
	protected ENEMYEFFECT enemyEffect;
	[Space]
	public Vector2 healthBarOffset = new Vector2(0, 1.5f);

	//[Header("Freeze Option")]
	[HideInInspector] public bool canBeFreeze = true;
	[HideInInspector] public float timeFreeze = 5;
	[HideInInspector] public GameObject dieFrozenFX;

	//[Header("Burning Option")]
	[HideInInspector] public bool canBeBurn = true;
	[HideInInspector] public float timeBurn = 2;
	[HideInInspector] public GameObject dieBurnFX;
	float damageBurningPerFrame;

	//[Header("Shocking Option")]
	[HideInInspector] public bool canBeShock = true;
	[HideInInspector] public float timeShocking = 2f;
	float damageShockingPerFrame;

	[Header("Sound")]
	public AudioClip soundDetectPlayer;
	public AudioClip soundHit;
	public AudioClip soundDie;
	public AudioClip soundRushToPlayerAttack;

	int currentHealth;
	Vector2 hitPos;
	Vector2 knockBackForce;
	public bool isPlaying { get; set; }
	public bool isDead { get; set; }

	public bool isStopping { get; set; }

	protected HealthBarEnemyNew healthBar;
	protected ShakingHelper shakingHelper;

	protected float moveSpeed;
	public bool allowLookAtPlayer { get; set; }

	[HideInInspector]
	public Controller2D controller;
	[HideInInspector]
	protected Vector3 velocity;
	protected float velocityXSmoothing = 0;

	public bool isFacingRight()
	{
		return transform.rotation.y == 0 ? true : false;
	}

	protected virtual void OnEnable()
	{
		isPlaying = true;
	}

	public virtual void Start()
	{
		//if (isMustKillToGo)
		//	GameManager.Instance.RigisterEnemy (gameObject);

		controller = GetComponent<Controller2D>();
		currentHealth = health;
		moveSpeed = walkSpeed;
		_patrolLimitLeft = transform.position.x - patrolLimitLeft;
		_patrolLimitRight = transform.position.x + patrolLimitRight;

		if (enemyType == ENEMYTYPE.FLY)
		{
			controller.HandlePhysic = false;
			gravity = 0;
		}

		//var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
		//healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

		//if (healthBar)
		//	healthBar.Init(transform, (Vector3)healthBarOffset);

		if (anim == null)
			anim = GetComponent<Animator>();
		shakingHelper = GetComponent<ShakingHelper>();
		checkTarget = GetComponent<CheckTargetHelper>();
		if (shakingHelper)
			shakingHelper.enabled = false;

		//set enemy can be hit when player jump on head or make damage to player
		//if (GetComponent<CanBeJumpOn> () == null && canPlayerJumpOnHead) {
		//          gameObject.AddComponent<CanBeJumpOn>();
		//}

		if (enemyType == ENEMYTYPE.INWATER)
		{
			RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.1f, Vector2.zero, 0, waterLayer);
			if (hit)
			{
				waterLimitUp = hit.collider.gameObject.GetComponent<BoxCollider2D>().bounds.max.y - 1;
			}
			else
				Debug.LogError("YOU NEED PLACE THIS: " + gameObject.name + " TO A WATER ZONE");
		}

		if (warningIconDetectPlayer)
			warningIconDetectPlayer.SetActive(false);

		switch (startBehavior)
		{
			case STARTBEHAVIOR.NONE:
				SetEnemyState(ENEMYSTATE.WALK);
				break;
			case STARTBEHAVIOR.JUMPUP:
				SetEnemyState(ENEMYSTATE.WALK);
				velocity.y = jumpShowUpForce;
				break;

		}


		//set up start action
		switch (startAction)
		{
			case STARTACTION.PATROL:

				break;
			case STARTACTION.AIMPLAYER:
				moveSpeed = 0;
				LookAtPlayer();
				break;
			case STARTACTION.CHASING:
				//			cooldownDetect = int.MaxValue;
				DetectPlayer();
				break;
			default:
				break;
		}

		if(runSmokeFX)
			runSmokeFX.enableEmission = false;
	}

	void LookAtPlayer()
	{
		allowLookAtPlayer = true;
	}

	public void AnimSetTrigger(string name)
	{
		anim.SetTrigger(name);
	}

	public void AnimSetBool(string name, bool value)
	{
		anim.SetBool(name, value);
	}

	public void AnimSetFloat(string name, float value)
	{
		anim.SetFloat(name, value);
	}

	public void SetEnemyState(ENEMYSTATE state)
	{

		enemyState = state;
	}

	public void SetEnemyEffect(ENEMYEFFECT effect)
	{
		enemyEffect = effect;
	}

	public virtual void Update()
	{
		if(runSmokeFX)
			runSmokeFX.enableEmission = Mathf.Abs( velocity.x )> 0.1f && moveSpeed == runSpeed && enemyType == ENEMYTYPE.ONGROUND && controller.collisions.below;

		if (enemyEffect == ENEMYEFFECT.BURNING)
			CheckDamagePerFrame(damageBurningPerFrame);

		if (enemyEffect == ENEMYEFFECT.SHOKING)
			CheckDamagePerFrame(damageShockingPerFrame);
		if (healthBar != null)
			healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);

		//		GameManager.Instance.isPlayerDetected |= isPlayerDetected;
		//		else {
		//			cooldownDetectCouting -= Time.deltaTime;
		//			if (cooldownDetectCouting <= 0)
		//				isPlayerDetected = false;
		//		}
	}


	//can call by Alarm action of other Enemy
	public virtual void DetectPlayer(float delayChase = 0)
	{
		if (isPlayerDetected)
			return;

		isPlayerDetected = true;
		SoundManager.PlaySfx(soundDetectPlayer);
		StartCoroutine(DelayBeforeChasePlayer(delayChase));
	}


	protected IEnumerator DelayBeforeChasePlayer(float delay)
	{
		SetEnemyState(ENEMYSTATE.IDLE);
		if (warningIconDetectPlayer)
			warningIconDetectPlayer.SetActive(true);
		yield return new WaitForSeconds(delay);
		if (warningIconDetectPlayer)
			warningIconDetectPlayer.SetActive(false);
		if (enemyState == ENEMYSTATE.ATTACK)
		{
			yield break;
		}

		SetEnemyState(ENEMYSTATE.WALK);


		if (detectPlayerAct == DETECTPLAYER.RushIntoPlayer)
		{
			AnimSetTrigger("rushIntoPlayer");
			rushIntoPlayerPoint = GameManager.Instance.Player.transform.position + Vector3.up * 0.5f;
			rushIntoPlayerDirection = (rushIntoPlayerPoint - (Vector2)transform.position).normalized;
			SetEnemyState(ENEMYSTATE.RUSHINTOPLAYER);
			SoundManager.PlaySfx(soundRushToPlayerAttack);

			if ((isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x) || (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x))
				transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 180 : 0, transform.rotation.z));

			Destroy(gameObject, 2);
		}
	}

	public virtual void DismissDetectPlayer()
	{
		if (!isPlayerDetected)
			return;

		isPlayerDetected = false;
	}

	public virtual void FixedUpdate()
	{

	}

	public virtual void Hit()
	{
		SoundManager.PlaySfx(soundHit);
		switch (hitBehavior)
		{
			case HITBEHAVIOR.ChasePlayer:
				DetectPlayer();
				break;
			case HITBEHAVIOR.NONE:

				break;
			default:
				break;
		}
	}

	public virtual void Die()
	{
		isPlaying = false;
		isDead = true;
		isPlayerDetected = false;
		SetEnemyState(ENEMYSTATE.DEATH);

		if (warningIconDetectPlayer)
			warningIconDetectPlayer.SetActive(false);

		if (dieFX)
			SpawnSystemHelper.GetNextObject(dieFX, true, transform.position);


		if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
			SpawnSystemHelper.GetNextObject(dieFrozenFX, true, hitPos);

		if (enemyEffect == ENEMYEFFECT.SHOKING)
			UnShock();

		if (enemyEffect == ENEMYEFFECT.EXPLOSION)
		{
			if (bloodPuddleFX)
			{
				for (int i = 0; i < Random.Range(2, 5); i++)
				{
					SpawnSystemHelper.GetNextObject(bloodPuddleFX, true,
						(Vector2)transform.position + new Vector2(Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2), Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y)));
				}
			}
			if (explosionFX.Length > 0)
			{
				for (int i = 0; i < Random.Range(1, 3); i++)
				{
					SpawnSystemHelper.GetNextObject(explosionFX[Random.Range(0, explosionFX.Length)], true, transform.position);
				
				}
			}
		}
		else
			SoundManager.PlaySfx(soundDie);
	}

	public virtual void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (isDead)
			return;

		if (enemyState == ENEMYSTATE.DEATH)
			return;

		if (isStopping)
			return;

		bool isExplosion = false;

		hitPos = hitPoint;
		currentHealth -= (int)damage;
		knockBackForce = force;
		if (hitFX)
			SpawnSystemHelper.GetNextObject(hitFX, true,
				hitPos + new Vector2(Random.Range(-randomHitPoint.x, randomHitPoint.x), Random.Range(-randomHitPoint.y, randomHitPoint.y)));
		if (bloodPuddleFX)
			SpawnSystemHelper.GetNextObject(bloodPuddleFX, true,
				(Vector2)transform.position + new Vector2(Random.Range(-randomBloodPuddlePoint.x, randomBloodPuddlePoint.x), Random.Range(-randomBloodPuddlePoint.y, randomBloodPuddlePoint.y)));

		//Debug.LogError(dieBehavior);

		if (healthBar)
			healthBar.UpdateValue(currentHealth / (float)health);
		//		Debug.LogError (isExplosion + "BLOW" + (dieBehavior == DIEBEHAVIOR.BLOWUP));
		if (currentHealth <= 0)
		{
			if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP || attackType == ATTACKTYPE.BOMBER)
			{
				//SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
				if (explosionFX.Length > 0)
					Instantiate(explosionFX[Random.Range(0, explosionFX.Length)], transform.position, Quaternion.identity);

				//check if have bomb then active it
				if (attackType == ATTACKTYPE.BOMBER && bomberAttack != null)
				{
					bomberAttack.BlowUp();
				}

				Destroy(gameObject);
			}
			else if (dieBehavior == DIEBEHAVIOR.FALLOUT)
			{
				velocity = new Vector2(0, deadForce);
				controller.HandlePhysic = false;
				gravity = gravityDie;
				Die();
			}
			else
				Die();
		}
		else
			Hit();
	}

	private void CheckDamagePerFrame(float _damage)
	{
		if (enemyState == ENEMYSTATE.DEATH)
			return;

		currentHealth -= (int)_damage;
		if (healthBar)
			healthBar.UpdateValue(currentHealth / (float)health);

		if (currentHealth <= 0)
			Die();
	}

	#region IListener implementation

	public virtual void IPlay()
	{
		//		isPlaying = true;
	}

	public virtual void ISuccess()
	{
		//		isPlaying = false;
	}

	public virtual void IPause()
	{
	}

	public virtual void IUnPause()
	{
	}

	public virtual void IGameOver()
	{
		isPlaying = false;
	}

	public virtual void IOnRespawn()
	{
		isPlaying = true;
		SetEnemyState(ENEMYSTATE.WALK);
		DismissDetectPlayer();
	}

	public virtual void IOnStopMovingOn()
	{
	}

	public virtual void IOnStopMovingOff()
	{
	}

	#endregion

	#region ICanFreeze implementation
	//	_2dxFX_Frozen[] iceFX;
	public virtual void Freeze(GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.FREEZE)
			return;

		if (enemyEffect == ENEMYEFFECT.BURNING)
			BurnOut();

		if (enemyEffect == ENEMYEFFECT.SHOKING)
		{
			UnShock();
		}

		if (canBeFreeze)
		{
			enemyEffect = ENEMYEFFECT.FREEZE;


			anim.enabled = false;
			//			Invoke ("UnFreeze", timeFreeze);
			StartCoroutine(UnFreezeCo());
		}
	}

	IEnumerator UnFreezeCo()
	{
		if (enemyEffect != ENEMYEFFECT.FREEZE)
			yield break;

		float wait = timeFreeze - 1;
		yield return new WaitForSeconds(wait);

		float time = 1;


		UnFreeze();
	}

	void UnFreeze()
	{
		if (enemyEffect != ENEMYEFFECT.FREEZE)
			return;

		enemyEffect = ENEMYEFFECT.NONE;

	}

	#endregion



	#region ICanBurn implementation

	//	protected _2dxFX_AL_Fire[] burnFX;
	public virtual void Burning(float damage, GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.BURNING)
			return;

		if (enemyEffect == ENEMYEFFECT.FREEZE)
		{
			UnFreeze();
		}

		if (enemyEffect == ENEMYEFFECT.SHOKING)
		{
			UnShock();
		}

		if (canBeBurn)
		{
			damageBurningPerFrame = damage;
			enemyEffect = ENEMYEFFECT.BURNING;


			StartCoroutine(BurnOutCo());
		}
	}

	IEnumerator BurnOutCo()
	{
		if (enemyEffect != ENEMYEFFECT.BURNING)
			yield break;

		float wait = timeBurn - 1;
		yield return new WaitForSeconds(wait);

		if (enemyState == ENEMYSTATE.DEATH)
		{
			BurnOut();
			Destroy(gameObject);
		}

		BurnOut();
	}

	void BurnOut()
	{
		if (enemyEffect != ENEMYEFFECT.BURNING)
			return;

		enemyEffect = ENEMYEFFECT.NONE;

	}
	#endregion

	#region ICanShock implementation


	public virtual void Shoking(float damage, GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.SHOKING)
			return;

		if (enemyEffect == ENEMYEFFECT.FREEZE)
		{
			UnFreeze();
		}

		if (enemyEffect == ENEMYEFFECT.BURNING)
			BurnOut();

		if (canBeShock)
		{
			damageShockingPerFrame = damage;
			enemyEffect = ENEMYEFFECT.SHOKING;


			if (shakingHelper)
			{
				if (!shakingHelper.enabled)
					shakingHelper.enabled = true;

				shakingHelper.DoShake(true); ;
			}

			StartCoroutine(UnShockCo());
		}
	}

	IEnumerator UnShockCo()
	{
		if (enemyEffect != ENEMYEFFECT.SHOKING)
			yield break;

		yield return new WaitForSeconds(timeShocking);

		UnShock();
	}

	void UnShock()
	{
		if (enemyEffect != ENEMYEFFECT.SHOKING)
			return;

		enemyEffect = ENEMYEFFECT.NONE;


		if (shakingHelper)
		{
			shakingHelper.StopShake(); ;

			if (shakingHelper.enabled)
				shakingHelper.enabled = false;
		}


	}


	#endregion

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
}
