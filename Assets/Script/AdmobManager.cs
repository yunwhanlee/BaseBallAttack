using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : MonoBehaviour{
    static public AdmobManager ins;

    public bool isTestMode;
    public Text LogText;
    public Button[] RewardAdsBtns;
    public bool isAdClosed;
    
    void Awake() => singleton();

    void Start(){
        var requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() {"39F380C85C490BBB"}) // test Device ID
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        LoadRewardAd();
    }

    void Update(){
        Array.ForEach(RewardAdsBtns, adBtn => {
            if(adBtn.gameObject.activeSelf)
                adBtn.interactable = rewardAd.CanShowAd();
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

        rewardAd.OnAdOpening += (sender, e) => LogText.text = "WATCHING...";

        rewardAd.OnAdClosed += (sender, e) => {
            isAdClosed = true;
            LogText.text = "CLOSED";
        };
        
        rewardAd.OnUserEarnedReward += (sender, e) => {
            isAdClosed = false;
            LogText.text = "SUCCESS";
            Debug.Log("SUCCESS showRewardAd:: " + LogText.text);
        };

    }

    public void showRewardAd(){
        rewardAd.Show();
        LoadRewardAd();
        coDelayInit(1);
    }
	#endregion

    IEnumerator coDelayInit(float delay){
        yield return new WaitForSeconds(delay);
        LogText.text = "";
        isAdClosed = false;
    }
    private void singleton(){
        if(ins == null) {
            ins = this;
            DontDestroyOnLoad(ins);
        }
        else
            Destroy(this.gameObject);
    }
}
