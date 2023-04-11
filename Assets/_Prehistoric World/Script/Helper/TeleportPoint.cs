using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour {
	public bool useTeleport = true;
	public Teleport Teleport;

	void OnTriggerEnter2D(Collider2D other){
		if (!useTeleport)
			return;

		if (!GameManager.Instance.Player.isPlaying)
			return;
		
		if (other.GetComponent<Player> ()) {
			Teleport.TeleportPlayer (transform.position);
			return;
		}
		
		if (other.gameObject.GetComponent<CanTeleport>()) {
			Teleport.TeleportObj (transform.position, other.gameObject);
		}
	}
}
