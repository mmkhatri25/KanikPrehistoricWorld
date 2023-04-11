using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour {
	public int amount { get; set;}

	public GameObject button;
	public Shield shield; 
	public Text amountTxt;

	float time;
	int hits;
	GameObject hitFX;
	// Use this for initialization
	void Start () {
		amount = GlobalValue.shieldBullet;
	}

	public void AddAmount(int x=0, float _time = 5, int _hits = 3, bool isUseNow=false, GameObject _hitFX = null){
		time = _time;
		hits = _hits;
		hitFX = _hitFX;

		GlobalValue.shieldBullet += x;
		if (isUseNow)
			UseShield ();
	}
	
	// Update is called once per frame
	void Update () {
		amountTxt.text = GlobalValue.shieldBullet.ToString ();
		//		cv.alpha = amount > 0 ? 1 : 0;
		button.SetActive (GlobalValue.shieldBullet > 0 ? true : false);
	}

	public void UseShield(){
		if (GlobalValue.shieldBullet <= 0)
			return;

		if (GameManager.Instance.isUsingShield)
			return;

		if (shield!=null) {
			GlobalValue.shieldBullet--;
			Instantiate (shield, transform.position, Quaternion.identity).GetComponent<Shield> ().Init (time, hits, hitFX);
		}
	}
}
