using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemReward : MonoBehaviour {
	public int itemID = 1;
	public enum ItemType{Live, NormalBullet, Grenade}
	public ItemType itemType;

	public int rewardedUnit = 1;

	public Text rewardedAmountTxt;
	public Text currentAmountTxt;
	public AudioClip sound;

	public int coinPrice = 1;
	public Text coinTxt;
	public GameObject watchVideoBut;

	void OnEnable(){
		UpdateAmount ();
	}

	void Start(){
		UpdateAmount ();

		rewardedAmountTxt.text = "x" + rewardedUnit;
		coinTxt.text = coinPrice.ToString ();
	}

	public void UseCoin(){
		var coins = GlobalValue.SavedCoins;
		if (coins >= coinPrice) {
			coins -= coinPrice;
			GlobalValue.SavedCoins = coins;
			DoReward ();
		} 
	}

    public void ShowRewardAd()
    {
        if (!GlobalValue.allowClickUnityAdAgain)
            return;

        //		#if UNITY_ADS

        if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
        {
                GlobalValue.allowClickUnityAdAgain = false;
            AdsManager.AdResult += AdsManager_AdResult;
			AdsManager.Instance.ShowRewardedAds();
        }
        SoundManager.Click();
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
	{
		GlobalValue.allowClickUnityAdAgain = true;
		AdsManager.AdResult -= AdsManager_AdResult;
		if (isSuccess)
        {
			DoReward();
		}
    }

	private void DoReward(){
        switch (itemType)
        {
            case ItemType.Live:
                GlobalValue.SavedLives += rewardedUnit;
                break;
            case ItemType.NormalBullet:
                GlobalValue.normalBullet += rewardedUnit;
                break;
            case ItemType.Grenade:
                GlobalValue.grenade += rewardedUnit;
                break;
            default:
                break;
        }

		UpdateAmount ();
		SoundManager.PlaySfx (sound);
	}

    private void UpdateAmount()
    {
        switch (itemType)
        {
			case ItemType.Live:
				currentAmountTxt.text = "current: " + GlobalValue.SavedLives;
				break;
            case ItemType.NormalBullet:
                currentAmountTxt.text = "current: " + GlobalValue.normalBullet;
                break;
            case ItemType.Grenade:
                currentAmountTxt.text = "current: " + GlobalValue.grenade;
                break;
            default:
                break;
        }
    }
}
