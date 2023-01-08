using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Achivement : MonoBehaviour
{   
    //* Outside
    HomeManager hm;

    //* Value
    const int STG10 = 0, STG30 = 1, STG60 = 2, STG100 = 3, STG160 = 4;

    enum REWARD_TYPE { COIN, DIAMOND, ROULETTE_TICKET };
    [SerializeField] REWARD_TYPE rewardType = REWARD_TYPE.DIAMOND;
    [SerializeField] int step = 1, maxStep;
    [SerializeField] int cnt, max;
    [SerializeField] int rewardValue;
    [SerializeField] bool allClear;

    //* Obj
    Image panelImg, rewardIconImg;
    Text infoTxt, valueTxt, rewardTxt;
    Button claimBtn;

    void Start()
    {
        //* Assign Object
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();

        panelImg = this.GetComponent<Image>();
        infoTxt = this.transform.Find("InfoTxt").GetComponent<Text>();
        valueTxt = this.transform.Find("ValueTxt").GetComponent<Text>();
        rewardTxt = this.transform.Find("RewardTxt").GetComponent<Text>();
        rewardIconImg = this.transform.Find("RewardIcon").GetComponent<Image>();
        claimBtn = this.transform.Find("ClaimBtn").GetComponent<Button>();

        //* Set Icon
        rewardIconImg.sprite = (rewardType == REWARD_TYPE.COIN)? hm.rewardIconSprs[(int)REWARD_TYPE.COIN]
            : (rewardType == REWARD_TYPE.DIAMOND)? hm.rewardIconSprs[(int)REWARD_TYPE.DIAMOND]
            : (rewardType == REWARD_TYPE.ROULETTE_TICKET)? hm.rewardIconSprs[(int)REWARD_TYPE.ROULETTE_TICKET] : null;

        //* Set Value
        var pDt = DM.ins.personalData;
        switch(this.name){
            case "StageClear" : {
                cnt = pDt.ClearStage;
                Debug.Log($"Achivement:: StageClear:: cnt= {cnt}");

                if(pDt.StageClearArr[STG160].IsAccept){
                    setNext(pDt, STG160, _allClear: true);
                }
                else if(pDt.StageClearArr[STG100].IsAccept){
                    setNext(pDt, STG160);
                }
                else if(pDt.StageClearArr[STG60].IsAccept){
                    setNext(pDt, STG100);
                }
                else if(pDt.StageClearArr[STG30].IsAccept){
                    setNext(pDt, STG60);
                }
                else if(pDt.StageClearArr[STG10].IsAccept){
                    setNext(pDt, STG30);
                }
                else if(!Array.Exists(pDt.StageClearArr, arr => arr.IsAccept)){
                    setNext(pDt, STG10);
                }
                break;
            }
            case "AtvSkillCollector" : {
                List<bool> lockList = DM.ins.personalData.SkillLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "BatCollector" : {
                List<bool> lockList = DM.ins.personalData.BatLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "CharactorCollector" : {
                List<bool> lockList = DM.ins.personalData.CharaLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "DestroyBlocks" :
                break;
            case "CoinRich" :
                break;
            case "DiamondRich" :
                break;
            case "NormalModeClear" :
                break;
            case "HardModeClear" :
                break;
            case "KillBoss" :
                break;
            case "RouletteTicketCollector" :
                break;
            case "UpgradeMaster" :
                break;
        }

        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }

    void Update(){
        switch(this.name){
            case "StageClear" : {
                 //* Set Color
                if(allClear){
                    panelImg.color = Color.gray;
                    claimBtn.GetComponent<Image>().color = Color.gray;
                }
                else{
                    panelImg.color = (cnt >= max)? Color.white : Color.gray;
                    claimBtn.GetComponent<Image>().color = (cnt >= max)? Color.white : Color.gray;
                }
                break;
            }
        }
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public void onClickCalimBtn(){
        var pDt = DM.ins.personalData;
        switch(this.name){
            case "StageClear" : {
                if(pDt.StageClearArr[STG10].IsComplete && !pDt.StageClearArr[STG10].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, STG10);
                    setNext(pDt, STG30);
                }
                else if(pDt.StageClearArr[STG30].IsComplete && !pDt.StageClearArr[STG30].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, STG30);
                    setNext(pDt, STG60);
                }
                else if(pDt.StageClearArr[STG60].IsComplete && !pDt.StageClearArr[STG60].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, STG60);
                    setNext(pDt, STG100);
                }
                else if(pDt.StageClearArr[STG100].IsComplete && !pDt.StageClearArr[STG100].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, STG100);
                    setNext(pDt, STG160);
                }
                else if(pDt.StageClearArr[STG160].IsComplete && !pDt.StageClearArr[STG160].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, STG160);
                    setNext(pDt, STG160, _allClear: true);
                }
                break;
            }
        }
        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }

//*---------------------------------------
//*  Function
//*---------------------------------------
    private void setNext(PersonalData pDt, int idx, bool _allClear = false){
        this.max = pDt.StageClearArr[idx].Val;
        this.infoTxt.text = $"Stage Clear";
        this.rewardTxt.text = pDt.StageClearArr[idx].Reward.ToString();
        if(_allClear) this.allClear = _allClear;
    }
    private int acceptReward(PersonalData pDt, int idx){
        pDt.StageClearArr[idx].IsAccept = true;
        //* Reward Panel
        int reward = pDt.StageClearArr[idx].Reward;
        //* Reward Type
        switch(this.rewardType){
            case REWARD_TYPE.COIN :
                hm.displayShowRewardPanel(coin: reward); break;
            case REWARD_TYPE.DIAMOND :
                hm.displayShowRewardPanel(coin: 0, diamond: reward); break;
            case REWARD_TYPE.ROULETTE_TICKET :
                hm.displayShowRewardPanel(coin: 0, diamond: 0, rouletteTicket: reward); break;
        }
        return reward;
    }
    static public void setStageClear(int stage){
        //* Achivement (StageClear IsComplete)
        for(int i=0; i<DM.ins.personalData.StageClearArr.Length; i++){
            if(stage >= DM.ins.personalData.StageClearArr[i].Val){
                DM.ins.personalData.StageClearArr[i].IsComplete = true;
            }
        }
    }
}
