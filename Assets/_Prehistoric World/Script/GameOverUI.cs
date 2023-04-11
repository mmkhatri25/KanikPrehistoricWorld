using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
	public int RewaredLive = 3;
public GameObject ContinueBut;
	public Text rewardTxt;

	public  void Start ()
	{
		rewardTxt.text = "x" + RewaredLive + " Free live";
		ContinueBut.SetActive (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
         print("GlobalValue.LevelPass-"+GlobalValue.LevelPass);
	}

	public void FreeLive()
	{
		if (!GlobalValue.allowClickUnityAdAgain)
			return;

		SoundManager.Click();

		AdsManager.AdResult += AdsManager_AdResult;
		AdsManager.Instance.ShowRewardedAds();

		return;

	}

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
		GlobalValue.allowClickUnityAdAgain = true;

		AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            OnSuccess();
        }
    }

    public void NoWatch(){
		SoundManager.Click ();
		GameManager.Instance.ResetValue ();
	}

	public void Exit(){
		NoWatch ();
		MenuManager.Instance.ExitGame ();
	}


	public void Restart(){
		NoWatch ();
		MenuManager.Instance.RestartGame ();
	}

	public void OnSuccess ()
	{
		Debug.Log ("get free lives");
        GlobalValue.SavedLives = Mathf.Max(0, GlobalValue.SavedLives);
        GlobalValue.SavedLives += RewaredLive;
		GameManager.Instance.Continues ();

		gameObject.SetActive (false);
	}
}
