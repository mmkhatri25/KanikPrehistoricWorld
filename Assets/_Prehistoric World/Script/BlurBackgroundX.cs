using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurBackgroundX : MonoBehaviour {
	public Renderer sprite;



	public Camera paraCamera;
	GameObject backgroundObj;

	// Use this for initialization
	void Start () {
		TurnOff ();
		backgroundObj = GameObject.Find ("Background");
	}

	public void TurnOn(bool beforePlayer){
		sprite.gameObject.SetActive (true);
		if (beforePlayer)
			paraCamera.depth = 1;
		if(backgroundObj)
			backgroundObj.SetActive (false);
	}

	public void TurnOff(){
		sprite.gameObject.SetActive (false);
			paraCamera.depth = -0.5f;

		if(backgroundObj)
			backgroundObj.SetActive (true);
	}


	public void ChangeColor(Color color, float speed){
		Debug.Log ("ChangeColor");
//		color.a = color == Color.white ? 0 : anpha;
		StartCoroutine (MMFade.FadeTexture (sprite.material, speed, color));
//		sprite.color = color;
	}
}
