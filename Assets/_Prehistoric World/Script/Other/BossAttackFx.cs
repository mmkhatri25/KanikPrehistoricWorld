using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackFx : MonoBehaviour {
	public float time = 3;
	public AudioClip sound;
	AudioSource godAudioSource;

	// Use this for initialization
	void Start () {
		godAudioSource = gameObject.AddComponent<AudioSource> ();
		godAudioSource.clip = sound;
		godAudioSource.Play ();
		godAudioSource.loop = true;
		godAudioSource.volume = GlobalValue.isSound ? 1 : 0;
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;

		if (time <= 0) {
			Color color = new Color (1, 1, 1, 0);
			StartCoroutine( MMFade.FadeSprite (GetComponent<SpriteRenderer> (), 1f, color));
			Invoke ("Disappear", 1f);
		}
	}

	void Disappear(){
		Destroy (gameObject);
	}
}
