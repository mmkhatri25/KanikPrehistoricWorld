using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformControllerTriggerHelper : MonoBehaviour {
	public bool needKeepContact = true; 	//need contact to keep platform move
	[Header("Move when detect player")]
	public bool moveWhenLedge = true;
	public bool moveWhenClimbVine = true;
	public bool moveWhenStandOn = true;
	bool isDetectPlayer = false;

	public PlatformController platformController;

	[Header("Option when Platform no loop")]
	public bool allowWork=false;
//	public bool sendMessageToPlayerWhenFinish=false;
//	public string messageToPlayer="Action";
	bool sentMessage=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!needKeepContact && platformController.allowMoving)
			return;
		
		if (isDetectPlayer) {
			if ((GameManager.Instance.Player.controller.collisions.below && GameManager.Instance.Player.transform.position.y > platformController.transform.position.y)) {
				platformController.allowMoving = true;
			}else
				platformController.allowMoving = false;
		}else
			platformController.allowMoving = false;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject == GameManager.Instance.Player.gameObject)
			isDetectPlayer = true;
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject == GameManager.Instance.Player.gameObject)
			isDetectPlayer = false;
	}
}
