using UnityEngine;
using System.Collections;

public class BirdDetectPlayerHelper : MonoBehaviour {
	public EnemyGrounded bird;

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null || GameManager.Instance.Player.gameObject.layer == LayerMask.NameToLayer ("HidingZone"))
			return;

		if (!bird.isDead && bird.isAllowChasingPlayer)
			bird.isChasing = true;
	}
}
