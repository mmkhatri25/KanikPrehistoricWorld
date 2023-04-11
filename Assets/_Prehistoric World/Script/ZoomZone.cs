using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomZone : MonoBehaviour {
	public float zoomValue = 2.5f;
	float originalZone;
	bool isZooming = false;

	void OnTriggerStay2D(Collider2D other){
		if (isZooming)
			return;
		
		if (other.GetComponent<Player> ()) {
			FindObjectOfType<CameraFollow> ().ZoomIn (zoomValue);
			isZooming = true;
		}
	}



	void OnTriggerExit2D(Collider2D other){
		if (!isZooming)
			return;
		
		if (other.GetComponent<Player> ()) {
			FindObjectOfType<CameraFollow> ().ZoomOut ();
			isZooming = false;
		}
	}
}
