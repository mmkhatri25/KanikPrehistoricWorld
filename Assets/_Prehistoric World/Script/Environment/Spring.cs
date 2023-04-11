using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public float pushHeight = 5;
	public AudioClip soundEffect;
	[Range(0,1)]
	public float soundEffectVolume = 0.5f;

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}

	public void Push(){
		if (anim != null)
			anim.SetTrigger ("jump");
		SoundManager.PlaySfx (soundEffect, soundEffectVolume);
	}
}
