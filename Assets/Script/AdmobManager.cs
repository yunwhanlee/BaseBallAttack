using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;

public class AdmobManager : MonoBehaviour{
    [SerializeField] Text adNoticeTxt;
    Scene scene;
    DM.REWARD adType = DM.REWARD.NULL;

    public bool isTestMode;
    public Button[] RewardAdsBtns;

    void Start(){
        setAdLoadStatusNoticeTxt();

        var requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() {"39F380C85C490BBB"}) // test Device ID
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        LoadRewardAd();
    }

    void Update(){
        Array.ForEach(RewardAdsBtns, adBtn => {
            adBtn.interactable = rewardAd.CanShowAd();
            adNoticeTxt.text = (adBtn.interactable)? "" : "AD Loading..";
        });
    }

    AdRequest GetAdRequest() {
        return new AdRequest.Builder().Build();
    }

    #region REWARD AD
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-4586441545476475/8513799701";
    RewardedAd rewardAd;

    void LoadRewardAd(){
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(GetAdRequest());

        /* 結果についたCallBack処理　*/
        //* 広告ロードが失敗したとき、呼び出す。
        rewardAd.OnAdFailedToLoad += (sender, e) => {
            // SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            // Util._.displayNoticeMsgDialog("AD Load Fail");
        };
        //* 広告表示が失敗したとき、呼び出す。
        rewardAd.OnAdFailedToShow += (sender, e) => {
            SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            Util._.displayNoticeMsgDialog("AD Show Fail");
        };
        //* 広告を最後まで閲覧したとき、呼び出す。(途中で出たら、実行しない)
        rewardAd.OnUserEarnedReward += (sender, e) => {
            Debug.Log($"<color=yellow>admob::rewardAd.OnUserEarnedReward:: {adType}</color>");
            SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
            switch(adType){
                //* Home Scene
                case DM.REWARD.ROULETTE_TICKET: 
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
        };
    }

    public void showRewardAd(DM.REWARD type){
        Debug.Log($"<color=orange>admob::showRewardAd:: {type}</color>");
        adType = type;
        rewardAd.Show();
        LoadRewardAd();
        StartCoroutine(coDelayInit());
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
