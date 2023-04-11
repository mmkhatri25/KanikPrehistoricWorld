using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScreenUI : MonoBehaviour {
	public static BloodScreenUI instance;
	CanvasGroup canvas;
	// Use this for initialization
	void Start () {
		instance = this;
		canvas = GetComponent<CanvasGroup> ();
		canvas.alpha = 0;
	}
	
	public void Work(){
		canvas.alpha = 1;
		StopAllCoroutines ();
		StartCoroutine (MMFade.FadeCanvasGroup (canvas, 1, 0));
	}
}
