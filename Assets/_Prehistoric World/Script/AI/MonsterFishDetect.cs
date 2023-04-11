using UnityEngine;
using System.Collections;

public class MonsterFishDetect : MonoBehaviour {
	public MonsterFishJump monster;

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player>() && GameManager.Instance.Player.gameObject.layer != LayerMask.NameToLayer ("HidingZone")) {
			monster.Attack ();
			Destroy (gameObject);
		}
	}
}
