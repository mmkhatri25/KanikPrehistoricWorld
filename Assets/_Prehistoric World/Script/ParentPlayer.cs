using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlayer : MonoBehaviour {

//	void OnTriggerEnter2D(Collider2D other){
//		if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Player")){
//
//			if(other.gameObject.GetComponent (typeof(ICanTakeDamage))){
//				other.gameObject.layer = LayerMask.NameToLayer("HidingZone");
//			}
//		}
//	}

	void OnTriggerStay2D(Collider2D other){
		if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
			Debug.Log ("OnTriggerStay2D");
			GameManager.Instance.Player.transform.SetParent (transform);
		}
	}

//	void OnTriggerExit2D(Collider2D other){
//		if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
//
//			other.gameObject.transform.parent = null;
//		}
//	}
}
