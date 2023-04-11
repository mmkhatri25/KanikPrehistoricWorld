using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowGameSpeedTrigger : MonoBehaviour {
	public bool isStart = true;
	public SlowGameSpeed target;

	public void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> ()) {
			if (isStart)
				target.Begin ();
			else
				target.End ();
		}
	}
}
