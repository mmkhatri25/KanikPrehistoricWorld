using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowGameSpeed : MonoBehaviour {
	[Range(0,1)]
	public float speed = 0.5f;
	bool isWorking = false;
	float ori;
	public bool canUseAgain = true;
	// Use this for initialization
	void Start () {
		ori = Time.timeScale;
	}

	void OnDisable(){
		Time.timeScale = ori;
	}
	
	public void Begin(){


			Time.timeScale = speed;
	
	}



	public void End(){
			Time.timeScale = ori;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (isWorking)
			return;

		if (other.GetComponent<Player> ()) {
			isWorking = true;
			Begin ();
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.GetComponent<Player> ()) {

			isWorking = !canUseAgain;
			End ();
			//			gameObject.SetActive (false);
			//			enabled = false;
		}
	}
}
