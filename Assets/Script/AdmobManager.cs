using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour{
    [SerializeField] Text adNoticeTxt;
    Scene scene;
    DM.REWARD adType = DM.REWARD.NULL;

    public bool isTestMode;
    public Button[] RewardAdsBtns;

    #region REWARD AD
    const string RewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string RewardID = "ca-app-pub-3908204064369314/3519376638";
    RewardedAd RewardAd;

    void Awake(){
        setAdLoadStatusNoticeTxt();

        // Init Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) => 
            LoadRewardAd()
        );
    }

    void Update(){
        Array.ForEach(RewardAdsBtns, adBtn => {
            adBtn.interactable = RewardAd.CanShowAd();
            adNoticeTxt.text = (adBtn.interactable)? "" : "AD Loading..";
        });
    }

    void LoadRewardAd(){
        //* Clean old ad
        if (RewardAd != null) {
                RewardAd.Destroy();
                RewardAd = null;
        }

        //* 広告要請
        var adRequest = new AdRequest();

        //* Unit ID
        string unitId = isTestMode? RewardTestID : RewardID;

        //* リワードロード
        RewardedAd.Load(unitId, adRequest, (RewardedAd ad, LoadAdError error) => {
            //* Fail
            if (error != null || ad == null) {
                Debug.LogError("Rewarded ad failed to load an ad " +　"with error : " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

            RewardAd = ad;

            /* 結果についたCallBack処理　*/
            RegisterEventHandlers(RewardAd);
        });
    }

    /// <summary>
    ///*リワード報告が終わったら、結果処理
    /// </summary>
    private void SetResultRewardedAd() {
        Debug.Log($"<color=yellow>admob::rewardAd.OnUserEarnedReward:: {adType}</color>");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        switch(adType){
            //* Home Scene
            case DM.REWARD.ROULETTE_TICKET:                     
                DM.ins.personalData.RouletteTicketOneDayAdPlayCnt++;
                DM.ins.personalData.RouletteTicket++;
                DM.ins.hm.showRoulettePanel();
                break;
            //* Home Scene
            case DM.REWARD.CoinX2: 
                DM.ins.gm.setCoinX2();
                break;
            case DM.REWARD.RerotateSkillSlots: 
                DM.ins.gm.setRerotateSkillSlots();
                break;
            case DM.REWARD.Revive: 
                DM.ins.gm.setRevive();
                break;
        }
    }

    private void RegisterEventHandlers(RewardedAd ad) {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) => {Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));};
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () => {Debug.Log("Rewarded ad recorded an1 impression.");};
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () => {Debug.Log("Rewarded ad was clicked.");};
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () => { Debug.Log("Rewarded ad full screen content opened.");};
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("Rewarded ad full screen content closed.");
            SetResultRewardedAd();
            LoadRewardAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) => {
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
            SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.ADShowFail.ToString()));
            LoadRewardAd();
        };
    }

    /// <summary>
    ///*リワード広告再生。
    /// </summary>
    /// <param name="type">Reward Type Name: ex) "Revive"</param>
    public void ShowRewardAd(DM.REWARD type){
        Debug.Log($"<color=orange>admob::ShowRewardAd:: {type}</color>");
        adType = type;

        //* 広告表示
        if (RewardAd != null && RewardAd.CanShowAd()) {
            RewardAd.Show((Reward reward) => {
                Debug.Log($"SHOW REWARD!");
            });
        }
        else {
            LoadRewardAd();
            StartCoroutine(coDelayInit());
        }
    }
	#endregion
/// -----------------------------------------------------------------------
/// 関数
/// -----------------------------------------------------------------------
    IEnumerator coDelayInit(){
        yield return Util.delay1;
        adType = DM.REWARD.NULL;
    }

    private void setAdLoadStatusNoticeTxt(){
        Array.ForEach(RewardAdsBtns, adBtn => {
            adNoticeTxt = Array.Find(adBtn.GetComponentsInChildren<Text>(), (txtObj) => 
                txtObj.transform.name == DM.NAME.AdNoticeTxt.ToString());
        });
    }
}
