using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_ChracterChoose : MonoBehaviour {

	[Tooltip("The unique character ID")]
	public int characterID;
	public int price;
	public GameObject CharacterPrefab;

	public bool unlockDefault = false;
	public GameObject UnlockButton;

	public Text pricetxt;
	public Text state; 

	bool isUnlock;
	SoundManager soundManager;
	public AudioClip pickedSound;

	// Use this for initialization
	void Start () {
		soundManager = FindObjectOfType<SoundManager> ();

		if (unlockDefault) {
			PlayerPrefs.SetInt (GlobalValue.Character + characterID, 1);
			isUnlock = true;
		}
		else
			isUnlock = PlayerPrefs.GetInt (GlobalValue.Character + characterID, 0) == 1 ? true : false;

		UnlockButton.SetActive (!isUnlock);

		pricetxt.text = price.ToString ();
	}

	void Update(){
		
		if (!isUnlock)
			return;
		
		if (PlayerPrefs.GetInt (GlobalValue.ChoosenCharacterID, 1) == characterID)
			state.text = "Picked";
		else
			state.text = "Choose";
	}
	
	public void Unlock(){
		SoundManager.PlaySfx (soundManager.soundClick);

		var coins = GlobalValue.SavedCoins;
		if (coins >= price) {
			coins -= price;
			GlobalValue.SavedCoins = coins;
			//Unlock
			PlayerPrefs.SetInt (GlobalValue.Character + characterID, 1);

			isUnlock = true;

			CharacterHolder.Instance.UpdateUnlockCharacter ();
			UnlockButton.SetActive (false);
		} 
	}

	public void Pick(){
		SoundManager.PlaySfx (soundManager.soundClick);

		if (!isUnlock) {
			Unlock ();
			return;
		}

		if (PlayerPrefs.GetInt (GlobalValue.ChoosenCharacterID, 0) == characterID)
			return;

		SoundManager.PlaySfx (pickedSound);
		PlayerPrefs.SetInt (GlobalValue.ChoosenCharacterID, characterID);
		PlayerPrefs.SetInt (GlobalValue.ChoosenCharacterInstanceID, CharacterPrefab.GetInstanceID ());
		CharacterHolder.Instance.CharacterPicked = CharacterPrefab;

	}
}
