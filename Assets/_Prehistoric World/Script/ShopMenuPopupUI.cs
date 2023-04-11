using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuPopupUI : MonoBehaviour {

	public static ShopMenuPopupUI instance;

	CanvasGroup canvasG;

	public bool isOpen{get;set;}
	public GameObject ShopItem, ShopCharacter, ShopCoin;
	void Awake(){
		if (ShopMenuPopupUI.instance) {
			Destroy (gameObject);
			return;
		}

		instance = this;
		canvasG = GetComponent<CanvasGroup> ();

		Show ();
	}

	//void SetTab(){
	//	if (DefaultValue.Instance.shopItem)
	//		ShopItem.GetComponent<Button> ().onClick.Invoke ();
	//	else if (DefaultValue.Instance.shopCharacter)
	//		ShopCharacter.GetComponent<Button> ().onClick.Invoke ();
	//	else if (DefaultValue.Instance.shopCoin)
	//		ShopCoin.GetComponent<Button> ().onClick.Invoke ();
	//}

	public void HideShop(){
		Debug.Log ("HIDE SHOP");
		SoundManager.Click ();
		instance.canvasG.alpha = 0;
		instance.canvasG.blocksRaycasts = false;
		isOpen = false;
		
		if (GameManager.Instance && GameManager.Instance.Player)
			GameManager.Instance.Player.allowMoving = true;

		if (MenuManager.Instance && Time.timeScale == 0) {
			MenuManager.Instance.Pause ();
		}

		gameObject.SetActive (false);
	}

	public void ShowShop(){
		if (MenuManager.Instance && Time.timeScale != 0) {
			MenuManager.Instance.Pause ();
		}
			canvasG.alpha = 1;
		instance.canvasG.blocksRaycasts = true;
		isOpen = true;
	}

	public static void Show(){
		instance.ShowShop ();
	}

	public static void Hide(){
		SoundManager.Click ();
		instance.HideShop ();
	}
}
