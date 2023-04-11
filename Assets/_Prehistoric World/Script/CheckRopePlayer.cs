using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRopePlayer : MonoBehaviour {
//	public Transform checkPoint;
//	public float radius = 0.2f;
//	public LayerMask targetLayer;
//	public Vector2 offset{ get; set; }

	void OnTriggerEnter2D(Collider2D other){
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;
		
		if (other.GetComponent<Player> ()) {
			GameManager.Instance.Player.CatchRopePoint (transform);
		}
	}
}
