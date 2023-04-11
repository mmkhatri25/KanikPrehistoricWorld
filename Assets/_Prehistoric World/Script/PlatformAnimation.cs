using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAnimation : MonoBehaviour {
	Animator anim;

//	void OnColliderEnter2D(Collision2D other){
//		Debug.Log (other.gameObject);
//		if (other.gameObject != GameManager.Instance.Player.gameObject)
//			return;
//		if (other.gameObject.transform.position.y > transform.position.y) {
//			anim = GetComponent<Animator> ();
//
//			anim.SetBool ("contact", true);
//
//			enabled = false;
//		}
//	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log (other.gameObject);
		if (other.gameObject != GameManager.Instance.Player.gameObject)
			return;
		if (other.gameObject.transform.position.y > transform.position.y) {
			anim = GetComponent<Animator> ();

			anim.SetBool ("contact", true);

			enabled = false;
		}
	}
}
