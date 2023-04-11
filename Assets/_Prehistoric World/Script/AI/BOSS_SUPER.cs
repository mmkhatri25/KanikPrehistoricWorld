using UnityEngine;
using System.Collections;

public enum BOSSAttackType{Melee, Normal, Throw, Dart, DartBack, Laser, Spread,Track,Bomb}
public class BOSS_SUPER : MonoBehaviour, ICanTakeDamage, IListener {
	public bool isBoss = true;
	public bool activeBossWhenDie = false;

	public float speed = 1f;
	public float distanceDetectPlayer = 10f;
	Rigidbody2D rig;
	Animator anim;
	Player player;

	[Range(10,1000)]
	public float health = 1000f;

	[Header("ATTAXK ORDER")]
	public BossAttackSquence[] BossAttacks;

	[Header("ATTACK SETUP")]
	public LayerMask playerLayer;
	public Transform centerPoint;
//	public float rangeAllowShoot = 7f;
	public float rangeAllowBomb = 3f;
	public float rangeAllowThrow = 3f;

	[Header("Normal Attack")]
	[Range(1,5)]
	public float attackZone = 2f;
	[Range(10,100)]
	public float givePlayerDamage= 30f;
	public float delayHit = 0.1f;
	public float coolDown = 2f;

	[Header("SOUND")]
	public AudioClip hitSound;
	public AudioClip deadSound;
	public AudioClip meleeSound;
	public AudioClip normalSound;
	public AudioClip throwSound;
	public AudioClip dartSound;
	public AudioClip dartBackSound;
	public AudioClip laserSound;
	public AudioClip spreadSound;
	public AudioClip trackSound;
	public AudioClip bombSound;

	[Header("OTHER")]
	public bool useShadowEffect = true;
	public HealthBarEnemy HealthBar; 
	public SpriteRenderer characterImage;



	[Header("damage")]
	public int DamageToPlayer;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2 (0, 10);
	float nextDamage;

	public RangeAttack gunAttack{ get; set; }
	public MeleeAttack meleeAttack{ get; set; }

	public BOSSAttackType currentAttackType{ get; set; }
	int currentAttack=0;
	public GameObject Stone;
	public Transform attackThrowPoint;
	public int speadBullet = 5;
	public GameObject[] ThrowBullet;

	public bool dropBonusItem = true;
	public GameObject dropItem;

	RaycastHit2D hit;
	bool moving;
	float x;
	bool isDead = false;
	float mulSpeed = 1;
	bool disapearing = false;
	bool isStunning = false;
	private GhostSprites ghostSprite;

	IEnumerator UpdateAttackType(){
		if (BossAttacks.Length == 0)
			yield break;

		while (true) {
			float delay = Random.Range (BossAttacks [currentAttack].delayMin, BossAttacks [currentAttack].delayMax);
			yield return new WaitForSeconds (delay);
			currentAttackType = BossAttacks [currentAttack].attackType;

			currentAttack++;
			if (currentAttack >= BossAttacks.Length)
				currentAttack = 0;
		}
	}

	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		player = FindObjectOfType<Player> ();
		ghostSprite = GetComponent<GhostSprites> ();

		x = transform.localScale.x;

		if (HealthBar != null) {
			HealthBar.maxHealth = health;
			HealthBar.currentHealth = health;
		}
	
		currentAttackType = BOSSAttackType.Melee;

		if (!isBoss)
			Play ();
	}

	public void Play(){
		moving = true;

		StartCoroutine (UpdateAttackType ());
	}

	void DisableAllEffect(){
		mulSpeed = 1;
		anim.speed = 1;
		if (ghostSprite)
			ghostSprite.allowGhost = false;

		if (characterImage)
			characterImage.color = new Color (1, 1, 1, 1);

		disapearing = false;
	}

	void Update () {
		if (isDead || player.isFinish || disapearing || isStunning || isStop || GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		anim.SetBool ("isJump", Mathf.Abs( rig.velocity.y) > 0.1f);
		
		if (moving) {
			hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + 0.5f), Vector2.left, distanceDetectPlayer, playerLayer);
			if (hit) {
				transform.Translate (new Vector3 (-speed * mulSpeed * Time.deltaTime, 0));
				transform.localScale = new Vector3 (x, transform.localScale.y, transform.localScale.z);
			} else {
				hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + 0.5f), Vector2.right, distanceDetectPlayer, playerLayer);
				if (hit) {
					transform.localScale = new Vector3 (-x, transform.localScale.y, transform.localScale.z);
					transform.Translate (new Vector3 (speed * mulSpeed * Time.deltaTime, 0));
				}
			}
		}
		
		if (Physics2D.CircleCast(centerPoint.position,attackZone,Vector2.zero,0,playerLayer) && moving) {
			StartCoroutine (Attack ());
			StartCoroutine (IdleDelay (1, 3));
		}

		if (moving && hit)
			anim.SetBool ("walk", true);
		else
			anim.SetBool ("walk", false);
	}

	IEnumerator IdleDelay(float min, float max){
		moving = false;
		var delay = Random.Range (min, max);
		yield return new WaitForSeconds (delay);
		moving = true;
	}

	IEnumerator Attack(){
		if (isDead)
			yield break;
		
		SoundManager.PlaySfx (meleeSound);
		anim.SetTrigger ("attack");
		yield return new WaitForSeconds (delayHit);
		//check if player still in range and give damage
		RaycastHit2D hit = Physics2D.CircleCast(centerPoint.position, attackZone, Vector2.zero, 0, playerLayer);
		if (hit)
			player.TakeDamage (givePlayerDamage, new Vector2 (0, 3), gameObject, hit.point);

	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigato, Vector3 hitPoint)
	{
		if (isDead)
			return;

		SoundManager.PlaySfx (hitSound);
		anim.SetTrigger ("hit");
		health -= damage;
		isDead = health <= 0 ? true : false;
		if (HealthBar != null)
			HealthBar.currentHealth = health;
		if (isDead) {
			SoundManager.PlaySfx (deadSound);
			anim.SetTrigger ("die");
			anim.SetBool ("isDead", true);
			if(HealthBar)
			HealthBar.gameObject.SetActive (false);
			var boxCo = GetComponents<BoxCollider2D> ();
			foreach (var box in boxCo) {
				box.enabled = false;
			}
			var CirCo = GetComponents<CircleCollider2D> ();
			foreach (var cir in CirCo) {
				cir.enabled = false;
			}
			rig.isKinematic = true;



			StopAllCoroutines ();
			CancelInvoke ();

			if (isBoss && !activeBossWhenDie) {
				GameManager.Instance.State = GameManager.GameState.Success;
				Invoke ("FinishLevel", delayFinishLevel);
			}

			ghostSprite.allowGhost = false;
			StartCoroutine ("DisableCo", 0.2f);
			
			if (destroyFX)
				Instantiate (destroyFX, transform.position, Quaternion.identity);

			if (dropBonusItem && dropItem)
				Instantiate (dropItem, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		}
	}

	public float delayFinishLevel = 1.5f;
	void FinishLevel(){
		GameManager.Instance.GameFinish ();
	}

	public GameObject destroyFX;

	void OnTriggerStay2D(Collider2D other){
		var Player = other.GetComponent<Player> ();
		if (Player == null)
			return;

		if (!Player.isPlaying)
			return;


        if (Time.time < nextDamage + rateDamage)
			return;

		nextDamage = Time.time;

//		if (canBeKillOnHead && (Player.transform.position.y > transform.position.y)) {
//
//			Player.SetForce (new Vector2 (transform.localScale.x > 0 ? -pushPlayer.x : pushPlayer.x, pushPlayer.y));
//			var canTakeDamage = (ICanTakeDamage) GetComponent (typeof(ICanTakeDamage));
//			if (canTakeDamage != null)
//				canTakeDamage.TakeDamage (damageJumpOnHead, Vector2.zero, gameObject);
//
//			return;
//		}



		//Push player back
		//		var facingDirectionX = Mathf.Sign (Player.transform.localScale.x);
		//		var facingDirectionY = Mathf.Sign (Player.velocity.y);


		var facingDirectionX = Mathf.Sign (Player.transform.position.x - transform.position.x);
		var facingDirectionY = Mathf.Sign (Player.velocity.y);

		Player.SetForce(new Vector2 (Mathf.Clamp (Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
			Mathf.Clamp (Mathf.Abs(Player.velocity.y), 5, 15) * facingDirectionY * -1));

		if (DamageToPlayer == 0)
			return;
		Player.TakeDamage (DamageToPlayer, Vector2.zero, gameObject,Player.transform.position);
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay (new Vector2 (transform.position.x, transform.position.y +  0.5f), Vector2.left*distanceDetectPlayer);
		Gizmos.DrawRay (new Vector2 (transform.position.x, transform.position.y +  0.5f), Vector2.right*distanceDetectPlayer);
		Gizmos.DrawWireSphere (centerPoint.position, attackZone);
	}

	//Throw Attack
	IEnumerator Attack(float delay){
		
		yield return new WaitForSeconds (delay);

		if (isDead)
			yield break;
		
		transform.localScale = new Vector3 (GameManager.Instance.Player.transform.position.x > transform.position.x ? (-x) : x, transform.localScale.y, transform.localScale.z);
		SoundManager.PlaySfx (throwSound);
		anim.SetTrigger ("Throw");

//		if (GameManager.Instance.State == GameManager.GameState.Playing)
//			StartCoroutine (Attack (Random.Range (MinAttackTime, MaxAttackTime)));
//		TurnCoroutines();
	}

	//
//	IEnumerator SuperAttack(float delay){
//		if (isDead)
//			yield break;
//		
//		yield return new WaitForSeconds (delay);
//
//		if (isFlying) {
//			StartCoroutine (SuperAttack (Random.Range (MinSuperAttackTime, MaxSuperAttackTime)));
//			yield break;
//		}
//
//		StopAllCoroutines ();
//		DisableAllEffect ();
//
//		transform.localScale = new Vector3 (GameManager.Instance.Player.transform.position.x > transform.position.x ? (-x) : x, transform.localScale.y, transform.localScale.z);
//
//
//
//		superAttackCurrentBulletNumber = 0;
//		Invoke ("SuperAttackAction", 0);
//
//
//
////		if (GameManager.Instance.State == GameManager.GameState.Playing)
////			StartCoroutine (SuperAttack (Random.Range (MinSuperAttackTime, MaxSuperAttackTime)));
//	}

	int superAttackCurrentBulletNumber = 0;

//	void SuperAttackAction(){
//		if (isDead)
//			return;
//		
//		superAttackCurrentBulletNumber++;
//			if (SuperAttackBullet)
//				Instantiate (SuperAttackBullet);
//
//			if (useWaveAnimation)
//				anim.SetTrigger ("SuperAttack");
//
//			SoundManager.PlaySfx (superAttackSound);
//
//		if (superAttackCurrentBulletNumber >= attackTimes)
//			TurnCoroutines ();
//		else
//			Invoke ("SuperAttackAction", delayPerAttack);
//	}

	bool isBanana = false;

//	IEnumerator AttackBanana(float delay){
//		yield return new WaitForSeconds (delay);
//		if (isDead)
//			yield break;
//		
//		transform.localScale = new Vector3 (GameManager.Instance.Player.transform.position.x > transform.position.x ? (-x) : x, transform.localScale.y, transform.localScale.z);
//
//		SoundManager.PlaySfx (throwSound);
//		anim.SetTrigger ("Throw");
//
//		isBanana = true;
//		isStunning = true;
//
//		yield return new WaitForSeconds (2);
//
//		isStunning = false;
//
//		if (GameManager.Instance.State == GameManager.GameState.Playing)
//			StartCoroutine (AttackBanana (Random.Range (MinAttackTime, MaxAttackTime)));
//	}

	//Called by animation event trigger
	public void ThrowStone(){
		if (isDead)
			return;
		
		if (isBanana) {
			Instantiate (ThrowBullet[Random.Range(0,ThrowBullet.Length)], transform.position, Quaternion.identity);
			isBanana = false;
			return;
		}

		int j = speadBullet / 2;
		for (int i = 0; i < speadBullet; i++) {
			GameObject bullet = Instantiate (Stone, attackThrowPoint.position, Quaternion.identity) as GameObject;
			bullet.GetComponent<ChasingStone> ().Init (j);
			j--;
		}

	}

//	IEnumerator DisappearShowCo(){
//		yield return new WaitForSeconds (timeActiveDisappear);
//
//		disapearing = true;
////		float alpha = 1;
////		while (alpha > 0) {
////			alpha -= Time.deltaTime;
////			characterImage.color = new Color (1, 1, 1, alpha);
////			yield return new WaitForEndOfFrame ();
////			Debug.Log ("1");
////		}
//
//		_2dxFX_NewTeleportation fx = characterImage.GetComponent<_2dxFX_NewTeleportation> ();
//		if (characterImage && characterImage.GetComponent<_2dxFX_NewTeleportation> () !=null) {
////			yield return new WaitForSeconds (1);
//
//			float delay = 1f, currentFX = 0f;
//			while (currentFX < delay) {
//				currentFX += Time.deltaTime;
//				fx._Fade = currentFX/delay;
//				yield return new WaitForEndOfFrame ();
//			}
//		}
//
//		transform.position = points [Random.Range (0, points.Length)].position;
////		while (alpha < 1) {
////			alpha += Time.deltaTime;
////			characterImage.color = new Color (1, 1, 1, alpha);
////			yield return new WaitForEndOfFrame ();
////			Debug.Log ("2");
////		}
//
//		if (characterImage && characterImage.GetComponent<_2dxFX_NewTeleportation> () !=null) {
//			//			yield return new WaitForSeconds (1);
//
//			float delay = 1f, currentFX = 1f;
//			while (currentFX > 0) {
//				currentFX -= Time.deltaTime;
//				fx._Fade = currentFX/delay;
//				yield return new WaitForEndOfFrame ();
//			}
//		}
//
//		disapearing = false;
//
////		StartCoroutine (DisappearShowCo ());
//		TurnCoroutines();
//	}

	bool isStop = false;
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
		isStop = true;
	}

	public void IUnPause ()
	{
		isStop = false;
		//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
		
		Debug.Log ("BOSS STOP");
		if (this) {
			StopAllCoroutines ();
			DisableAllEffect ();
			isStop = true;
		}
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
//		TurnCoroutines ();
		isStop = false;
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
		anim.enabled = false;
		isStop = true;
		//		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	public void IOnStopMovingOff ()
	{
		anim.enabled = true;
		isStop = false;
		//		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

	#endregion

	//This action is called by the Input/ControllerInput
	//public void MeleeAttack(){
	//	if (meleeAttack != null) {
	//		meleeAttack.Attack ();
	//	}
	//}

	private void CheckGunAttack(){
		switch (currentAttackType) {
		case BOSSAttackType.Normal:
			RangeAttack (false, true);
			break;
		case BOSSAttackType.Dart:

			break;

		case BOSSAttackType.DartBack:

			break;
		case BOSSAttackType.Laser:

			break;
		case BOSSAttackType.Spread:

			break;
		case BOSSAttackType.Track:

			break;
		default:

			break;
		}
	}

//	/This action is called by the Input/ControllerInput
	//buttonUp means allow fire
	public void RangeAttack(bool powerBullet, bool buttonUP){
		if (gunAttack!=null) {
			if (buttonUP) {
				if (gunAttack.Fire (powerBullet)) {
					SoundManager.PlaySfx (BossAttacks[currentAttack].sound);
				}
			} 
		}
	}

	[Header("Grenade")]
	public float angleThrow = 60;		//the angle to throw the bomb
	public float throwForce = 300;		//how strong?
	public Transform throwPosition;		//throw the bomb at this position
	public GameObject _Grenade;		//the bomb prefab object
	//This action is called by the Input/ControllerInput
	public void ThrowGrenade(){
//		Debug.Log (isJumpCombox3);
//		if (!allowThrowGrenade || _Grenade == null || GlobalValue.grenade <= 0 || isJumpCombox3)
//			return;
//
//		GlobalValue.grenade--;

		Vector3 throwPos = attackThrowPoint.position;
//		if (wallSliding) {
//			float throwX = transform.position.x;
//			float offsetThrow = throwPosition.localPosition.x;
//
//			throwX += transform.position.x > throwPosition.position.x ? offsetThrow : -offsetThrow;
//			throwPos.x = throwX;
//		}

		GameObject obj = Instantiate (_Grenade, throwPos, Quaternion.identity) as GameObject;

		float angle; 
		//		angle = controller.collisions.faceDir == 1 ? angleThrow : 180 - angleThrow;
		angle = transform.localScale.x > 0 ? angleThrow : 180 - angleThrow;

//		if (wallSliding)
//			angle = 180 - angle;

//		Debug.Log ("Wallsliding" + wallSliding);

		obj.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		Debug.Log (obj.transform.eulerAngles.z);
		obj.GetComponent<Rigidbody2D> ().AddRelativeForce (new Vector2 (throwForce, 0));

		anim.SetTrigger ("throw");
	}

	[System.Serializable]
	public class BossAttackSquence{
		public BOSSAttackType attackType;
		public float delayMin = 3;
		public float delayMax = 5;
		public float rate;
		public AudioClip sound;
	}
}
