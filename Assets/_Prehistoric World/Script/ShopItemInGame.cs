using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemInGame : MonoBehaviour {

	public GameObject tutorial;

    private void Start()
    {
		tutorial.SetActive(false);

	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject == GameManager.Instance.Player.gameObject) {
			tutorial.SetActive(true);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject == GameManager.Instance.Player.gameObject) {
			tutorial.SetActive(false);
		}
	}
}
