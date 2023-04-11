using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingTrigger : MonoBehaviour {

	public float speed = 2;
	public GameObject UpContainer;
	public GameObject DownContainer;
	public AudioClip sound;
	public bool useCameraShake = false;
	Animator anim;
	bool isWork = false;
	void Awake(){
		UpContainer.SetActive (false);
		DownContainer.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.speed = speed;
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if (!other.gameObject.GetComponent<Player> () || isWork)
			return;

		isWork = true;
		SoundManager.PlaySfx (sound);
		UpContainer.SetActive (true);
		DownContainer.SetActive (true);
		anim.SetTrigger ("show");
		if (useCameraShake)
			CameraPlay.EarthQuakeShake(1,30, 2);
		enabled = false;
	}
}
