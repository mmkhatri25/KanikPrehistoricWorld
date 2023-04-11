using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour {
	public enum ItemType {Bomb, Live}
	public ItemType item;

	public int amount = 1;
	public GameObject Effect;
	public AudioClip sound;

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null)
			return;

		SoundManager.PlaySfx (sound, 1);

		string weaponName = "";

		switch (item) {
		case ItemType.Bomb:
			GlobalValue.grenade += amount;
			break;
		case ItemType.Live:
			GlobalValue.SavedLives += amount;
			weaponName = "PowerBullet";
			break;
		default:
			break;
		}
			
		if (Effect != null)
			Instantiate (Effect, transform.position, transform.rotation);

		gameObject.SetActive (false);
	}
}
