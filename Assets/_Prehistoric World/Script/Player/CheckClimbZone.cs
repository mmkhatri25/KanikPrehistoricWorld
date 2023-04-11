using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckClimbZone : MonoBehaviour {
	public Transform checkPoint;
	public float radius = 0.2f;
	public LayerMask climbableLayer;
	Transform currentVine;

	Vector2 lastPos, currentPos;
	Vector2 offset = Vector2.zero;
	//in case vine moving
	public Vector2 TranslateOffset(){
		if (currentVine) {
			currentPos = currentVine.position;
			offset = currentPos - lastPos;
			lastPos = currentPos;
			Debug.Log (currentVine.gameObject.name);

//			Debug.LogError (offset.x * 10000);
		}
		return offset;
	}

	public bool isInClimbVineZone(){
		RaycastHit2D hit = Physics2D.CircleCast (checkPoint.position, radius, Vector2.zero, 0, climbableLayer);
		if (hit) {
			return true;
		} else {
			return false;
		}
	}

	public bool canClimbVine(){
		RaycastHit2D hit = Physics2D.CircleCast (checkPoint.position, radius, Vector2.zero, 0, climbableLayer);
		if (hit) {
			currentVine = hit.collider.gameObject.transform;
			lastPos = currentVine.position;

			return true;
		} else {
			currentVine = null;
			return false;
		}
	}
}
