using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour {
	SoundManager soundManager;
	public bool ResetRemoveAd = false;

	void Start(){
		soundManager = FindObjectOfType<SoundManager> ();
	}

	public void Reset(){
		SoundManager.PlaySfx (soundManager.soundClick);
        GlobalValue.LevelPass = 0;

		if(ShopMenuPopupUI.instance)
			Destroy (ShopMenuPopupUI.instance.gameObject);
	}

	public void ResetGame(){
		SoundManager.Click ();

		bool isRemoveAd = GlobalValue.RemoveAds;

		PlayerPrefs.DeleteAll ();

        if (CharacterHolder.Instance)
            CharacterHolder.Instance.UpdateUnlockCharacter();


        GlobalValue.RemoveAds = ResetRemoveAd ? false : isRemoveAd;

		SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
	}
}
