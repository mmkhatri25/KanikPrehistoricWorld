using UnityEngine;
using System.Collections;
public enum RocketType {Track, Normal}
public class Rocket : MonoBehaviour, ICanTakeDamage {
	
	public RocketType typeBullet;
	public LayerMask targetLayer;
	public float speed = 1;
	public bool isUseRadar = true;
	public float radarRadius = 3;
	float spreadAngle = 0;

	public GameObject ExplosionFX;
	public AudioClip soundExplosion;

	public float damageToGive = 100f;
	public float timeDestroy = 10;
	bool isDetect;
	Transform target;
	Vector2 direction = Vector2.right;
	GameObject Owner;
	int originalLayer;
	void OnEnable(){
//		gameObject.transform.SetParent (null);
		isDetect = false;
//		transform.eulerAngles = Vector3.zero;
		originalLayer = gameObject.layer;
	}

	public void Init (Vector2 _direction, GameObject owner = null){
//		Debug.LogError (_direction);
		direction = _direction;
		Owner = owner;
	}

	public void SetAngle(float angle){
		spreadAngle = angle;
	}

	public void LookAtTarget(Vector3 _target){
		Vector3 diff = _target - transform.position;
		diff.Normalize();

		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
	}

	void Start(){
		Destroy (gameObject, timeDestroy);

		if (typeBullet == RocketType.Track) {
			if (direction == Vector2.up)
				transform.rotation = Quaternion.AngleAxis (90, Vector3.forward);
			else if(direction == Vector2.right)
				transform.rotation = Quaternion.AngleAxis (0, Vector3.forward);
			else if(
				direction == Vector2.left)
				transform.rotation = Quaternion.AngleAxis (180, Vector3.forward);
			else if(direction == Vector2.down)
				transform.rotation = Quaternion.AngleAxis (-90, Vector3.forward);

			transform.Rotate (Vector3.forward, spreadAngle);
		}
	}

	// Update is called once per frame
	void  Update () {
		if (!isDetect && isUseRadar) {
			
			RaycastHit2D hit = Physics2D.CircleCast ((Vector2)transform.position + direction*radarRadius, radarRadius, Vector2.zero, 0, targetLayer);
				if (hit) {
					isDetect = true;
					target = hit.collider.gameObject.transform;
				}
			
			transform.Translate (speed * Time.deltaTime * (GameManager.Instance.Player.isRunning? 1.5f:1), 0, 0,Space.Self);
		} else if (target) {
			
				
			transform.position = Vector2.MoveTowards (transform.position, target.position, speed * Time.deltaTime);

			//rotate the rocket look to the target
			Vector3 dir = target.position - transform.position;
			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

			angle = Mathf.Lerp (transform.eulerAngles.z > 180?transform.eulerAngles.z-360:transform.eulerAngles.z, angle, 1f);

			var finalAngle = angle < 0 ? angle - 360 : angle;
//			finalAngle *= GameManager.Instance.Player.isFacingRight ? 1 : -1;
			transform.rotation = Quaternion.AngleAxis (finalAngle, Vector3.forward);
		} else {
			if (isDetect) {
				if (ExplosionFX != null)
					Instantiate (ExplosionFX, transform.position, Quaternion.identity);

//				Debug.LogError ("THIS");
				Destroy (gameObject);
			}
			transform.Translate (speed * Time.deltaTime * (GameManager.Instance.Player.isRunning? 1.5f:1), 0, 0,Space.Self);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (Owner!=null &&  other.gameObject.GetComponent<Rocket>()!=null && other.gameObject.GetComponent<Rocket>().Owner == Owner)
			return;

		if (other.gameObject == Owner)
			return;
		
		var damage = (ICanTakeDamage)other.gameObject.GetComponent (typeof(ICanTakeDamage));
		if (damage == null || other.GetComponent<Player>()) {
			if (target) {
				if (other.gameObject == target.gameObject) {
					if (ExplosionFX != null)
						Instantiate (ExplosionFX, other.gameObject.transform.position, Quaternion.identity);


//					Debug.LogError ("THIS");
					Destroy (gameObject);
				}
			}

			if (damage != null && targetLayer == (targetLayer | (1 << other.gameObject.layer))) {
				damage.TakeDamage (damageToGive, Vector2.zero, GameManager.Instance.Player.gameObject, other.gameObject.transform.position);
				if (ExplosionFX != null)
					Instantiate (ExplosionFX, other.gameObject.transform.position, Quaternion.identity);

//				Debug.LogError ("THIS");
				Destroy (gameObject);
			}
			
			return;
		}

		damage.TakeDamage (damageToGive, Vector2.zero, GameManager.Instance.Player.gameObject, other.gameObject.transform.position);

		if (ExplosionFX != null)
			Instantiate (ExplosionFX, other.gameObject.transform.position, Quaternion.identity);

//		Debug.LogError ("THIS");
		Destroy (gameObject);
//		Debug.LogError ("HIT: " + other.gameObject.name + "/" + Owner);
//		var Enemy = other.GetComponent<Enemy> ();
//
//		if (Enemy) {
//			Enemy.Hit (damage);
//
//			SoundManager.PlaySfx (soundExplosion);
//			if (ExplosionFX != null)
//				Instantiate (ExplosionFX, other.gameObject.transform.position, Quaternion.identity);
//
////			GameManager.Instance.ScoreBonus += scoreHit;
//
//			Hide ();
//		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(gameObject.layer == LayerMask.NameToLayer("HidingZone"))
			gameObject.layer = originalLayer;
//		}
	}

//	void OnBecameInvisible(){
//		Hide ();
//	}

//	public virtual void Hide(){
//		if (GameManager.Instance.RocketHolder && gameObject.activeInHierarchy) {
//			gameObject.transform.SetParent (rocketType == RocketType.Rocket ? GameManager.Instance.RocketHolder.transform : GameManager.Instance.BulletHolder.transform);
//			gameObject.SetActive (false);
//		} else
//			Destroy (gameObject);
//	}

	void OnDrawGizmosSelected(){
		if (typeBullet == RocketType.Normal)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere ((Vector2)transform.position + direction * radarRadius, radarRadius);
	}

	#region ICanTakeDamage implementation
	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (ExplosionFX != null)
			Instantiate (ExplosionFX, transform.position, Quaternion.identity);

//		Debug.LogError ("THIS");
		Destroy (gameObject);
	}
	#endregion
}
