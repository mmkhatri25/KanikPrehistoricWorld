using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rate : MonoBehaviour {
	public string rateLink = "YOUR GOOGLE PLAY STORE LINK";
	public void RateUs(){
		SoundManager.Click ();
		Application.OpenURL(rateLink);
	}
}
