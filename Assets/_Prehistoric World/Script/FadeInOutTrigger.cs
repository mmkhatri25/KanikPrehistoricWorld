using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutTrigger : MonoBehaviour {
	public bool overPlayer = false;
	public float timeShow = 1;
	public float timeHold = 0.5f;
	public float timeHide = 1;
	public bool useItAgain = false;
	public Color color = Color.white;
    public AudioClip sound;
	void OnTriggerEnter2D(Collider2D other){
		//		GetComponent<BoxCollider2D> ().enabled = false;

		if (other.GetComponent<Player> ()) {
			FadeInOutEffect.Instance.Work (color, timeShow, timeHold, timeHide, overPlayer);
            SoundManager.PlaySfx(sound);
			if (!useItAgain)
				gameObject.SetActive (false);
		}
	}
}
