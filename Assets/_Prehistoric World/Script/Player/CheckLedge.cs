using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLedge : MonoBehaviour {
	public Transform checkPoint;
	public float radius = 0.2f;
	public LayerMask targetLayer;
	public LayerMask checkGroundLayer;
	public Vector2 offset{ get; set; }

	public Transform newLedgePos;
	Transform ledgePos;

	void LateUpdate(){
		if (ledgePos) {
			offset = ledgePos.position - checkPoint.position;
		}
		Debug.LogWarning ("offset" + offset);
	}

	public bool isLedge(){
		
		
		RaycastHit2D hit = Physics2D.CircleCast (checkPoint.position, radius, Vector2.zero, 0, targetLayer);
		if (hit) {
			offset = hit.collider.gameObject.transform.position - checkPoint.position;
			ledgePos = hit.collider.gameObject.transform;
			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// if above player no collider then allow player climb up
	/// </summary>
	/// <returns><c>true</c>, if can climb ledge was ised, <c>false</c> otherwise.</returns>
	public bool isCanClimbLedge(){
		RaycastHit2D hit = Physics2D.Raycast (checkPoint.position + Vector3.up, GameManager.Instance.Player.isFacingRight ? Vector2.right : Vector2.left, 0.5f,checkGroundLayer);
//		Debug.Log (hit.collider.gameObject.name);
		if (hit) {
			return false;
		}
		else
			return true;
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(checkPoint.position,radius);
		Gizmos.color = Color.white;
		Gizmos.DrawRay (checkPoint.position + Vector3.up, Vector2.right);
	}
}
