using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockNewLevel : MonoBehaviour {
	public AudioClip soundReward;

	public void ShowRewardVideo(){
		SoundManager.Click ();
		ShowRewardedAd ();
	}

	private void ShowRewardedAd()
	{
		if (AdsManager.Instance.isRewardedAdReady())
		{
			GlobalValue.allowClickUnityAdAgain = false;
            AdsManager.AdResult += AdsManager_AdResult;
			AdsManager.Instance.ShowRewardedAds();
		}
	}

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
		AdsManager.AdResult -= AdsManager_AdResult;
		GlobalValue.allowClickUnityAdAgain = true;
		if (isSuccess)
        {
			SoundManager.PlaySfx(soundReward, 0.5f);
			GlobalValue.LevelPass = (GlobalValue.LevelPass + 1);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
    }
}
