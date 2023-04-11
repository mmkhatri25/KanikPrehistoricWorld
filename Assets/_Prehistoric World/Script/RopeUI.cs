using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RopeUI : MonoBehaviour {
	public static RopeUI instance;

	public RopeCatcher CurrentRope{ get; set; }

	public GameObject button;

	private Button but;

	// Use this for initialization
	void OnEnable () {
		instance = this;
		CurrentRope = null;
		but = GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void Update () {
		button.SetActive (CurrentRope);
		but.interactable = CurrentRope?true:false;

//		Debug.Log (CurrentRope.name);
	}

	public void Click(){
		if (CurrentRope)
			CurrentRope.CatchTheRope ();
		else
			Debug.LogError ("No ROPE");
	}

	public void ExitRope(){
		if(CurrentRope)
			CurrentRope.Stop ();
	}
}
