using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveTrigger : MonoBehaviour {
	public GameObject target;
	public string message = "Stop";
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () != null && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer ("HidingZone")) {
			if (target)
				target.SendMessageUpwards (message, SendMessageOptions.DontRequireReceiver);
		}
	}
}
