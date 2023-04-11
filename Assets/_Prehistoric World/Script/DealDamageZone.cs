using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageZone : MonoBehaviour {

//	public Vector2 attackZone;
	public bool damageGoOn=true;
//	public LayerMask CollisionMask;
	public float dealDamage = 30;
	public float damageRate = 2;

	bool allowMakeDamage = true;

	void OnTriggerEnter2D(Collider2D other){
		if (dealDamage == 0)
			return;
		
		if (!allowMakeDamage)
			return;
			
//		if (CollisionMask == (CollisionMask | (1 << other.gameObject.layer))) {
		if (other.gameObject == GameManager.Instance.Player.gameObject) {
			if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
				allowMakeDamage = false;
				other.gameObject.GetComponent<ICanTakeDamage> ().TakeDamage (dealDamage, Vector2.zero, gameObject, other.transform.position);
				if (damageGoOn) {
					Invoke ("AllowDamage", damageRate);
				}
			}
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if (dealDamage == 0)
			return;
		
		if (!allowMakeDamage)
			return;

//		if (CollisionMask == (CollisionMask | (1 << other.gameObject.layer))) {
		if (other.gameObject == GameManager.Instance.Player.gameObject) {
			if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
				allowMakeDamage = false;
				other.gameObject.GetComponent<ICanTakeDamage> ().TakeDamage (dealDamage, Vector2.zero, gameObject, other.transform.position);
				if (damageGoOn) {
					Invoke ("AllowDamage", damageRate);
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (dealDamage == 0)
			return;

//		if (CollisionMask == (CollisionMask | (1 << other.gameObject.layer))) {
		if (other.gameObject == GameManager.Instance.Player.gameObject) {
			CancelInvoke ();
			allowMakeDamage = true;

//			if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
//				allowMakeDamage = false;
//				other.gameObject.GetComponent<ICanTakeDamage> ().TakeDamage (dealDamage, Vector2.zero, gameObject);
//				if (damageGoOn) {
//					Invoke ("AllowDamage", damageRate);
//				}
//			}
		}
	}

	void AllowDamage(){
		allowMakeDamage = true;
	}
}
