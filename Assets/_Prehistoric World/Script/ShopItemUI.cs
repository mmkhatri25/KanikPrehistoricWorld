using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public enum ITEM_TYPE { iap1, iap2, iap3, watchVideo, removeAd}

    public ITEM_TYPE itemType;
    public int rewarded = 100;
    public float price = 100;
    public GameObject watchVideocontainer;

    public AudioClip soundRewarded;

    public Text priceTxt, rewardedTxt, rewardTimeCountDownTxt;

    private void Start()
    {
        if (itemType == ITEM_TYPE.watchVideo && AdsManager.Instance)
            rewarded = AdsManager.Instance.getRewarded;
    }

    private void Update()
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        if (itemType == ITEM_TYPE.watchVideo)
        {
            priceTxt.text = "FREE";
            rewardedTxt.text = "+" + rewarded;


            if (watchVideocontainer != null)
            {
                watchVideocontainer.SetActive(AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());

                if (AdsManager.Instance && AdsManager.Instance.TimeWaitingNextWatch() > 0)
                {
                    watchVideocontainer.SetActive(false);
                    rewardTimeCountDownTxt.text =
                    ((int)(AdsManager.Instance.TimeWaitingNextWatch()) / 60).ToString("0") + ":" + ((int)AdsManager.Instance.TimeWaitingNextWatch() % 60).ToString("00");
                }
                else
                {
                    if (rewardTimeCountDownTxt)
                    {
                        rewardTimeCountDownTxt.text = "";

                        if (!AdsManager.Instance || AdsManager.Instance && !AdsManager.Instance.isRewardedAdReady())
                            rewardTimeCountDownTxt.text = "No Ads";
                    }
                }
            }
        }
        else if (itemType == ITEM_TYPE.removeAd)
        {
            if (GlobalValue.RemoveAds)
                gameObject.SetActive(false);

            priceTxt.text = "$" + price;
        }
        else
        {
            priceTxt.text = "$" + price;
            rewardedTxt.text = "+" + rewarded;
        }
    }

    public void Buy()
    {
        switch (itemType)
        {
            case ITEM_TYPE.watchVideo:
                if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
                {
                    AdsManager.AdResult += AdsManager_AdResult;
                    AdsManager.Instance.ShowRewardedAds();
                }
                break;
            case ITEM_TYPE.iap1:
                Purchaser.iAPResult += Purchaser_iAPResult;
                Purchaser.Instance.BuyItem1();
                break;
            case ITEM_TYPE.iap2:
                Purchaser.iAPResult += Purchaser_iAPResult;
                Purchaser.Instance.BuyItem2();
                break;
            case ITEM_TYPE.iap3:
                Purchaser.iAPResult += Purchaser_iAPResult;
                Purchaser.Instance.BuyItem3();
                break;
            case ITEM_TYPE.removeAd:
                Purchaser.Instance.BuyRemoveAds();
                break;
        }
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            GlobalValue.SavedCoins += rewarded;
            SoundManager.PlaySfx(soundRewarded);
            UpdateStatus();
        }
    }

    private void Purchaser_iAPResult(int id)
    {
        Purchaser.iAPResult -= Purchaser_iAPResult;
        GlobalValue.SavedCoins += rewarded;
        SoundManager.PlaySfx(soundRewarded);
        UpdateStatus();
    }
}
