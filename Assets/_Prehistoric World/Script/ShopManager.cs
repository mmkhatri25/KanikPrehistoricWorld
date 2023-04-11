using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

	public CanvasGroup[] canvasGroups;

	public GameObject RemoveAdBut;
	void Start () {
		DisableCanvas ();
		foreach(var obj in canvasGroups)
        {
			obj.gameObject.SetActive(true);
        }

		ActivePanel (canvasGroups[0]);
	}
		
	void Update(){
		if(RemoveAdBut)
		RemoveAdBut.SetActive ((GlobalValue.RemoveAds ? false : true));
	}

	void DisableCanvas(){
		foreach (var obj in canvasGroups) {
			obj.alpha = 0;
			obj.blocksRaycasts = false;
		}
	}

	void ActivePanel(CanvasGroup canv){
		canv.alpha = 1;
		canv.blocksRaycasts = true;
	}
	
	public void SwichPanel(CanvasGroup canv){
		for (int i = 0; i < canvasGroups.Length; i++) {
			if (canv == canvasGroups [i]) {
				DisableCanvas ();
				ActivePanel (canvasGroups [i]);
				break;
			}
		}
		SoundManager.Click ();
	}
}
