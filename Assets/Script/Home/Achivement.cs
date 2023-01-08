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
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage All Clear";
                    rewardTxt.text = pDt.StageClearArr[STG160].Reward.ToString();
                    allClear = true;
                }
                else if(pDt.StageClearArr[STG100].IsAccept){
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG160].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG60].IsAccept){
                    max = pDt.StageClearArr[STG100].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG100].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG30].IsAccept){
                    max = pDt.StageClearArr[STG60].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG60].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG10].IsAccept){
                    max = pDt.StageClearArr[STG30].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG30].Reward.ToString();
                }
                else if(!Array.Exists(pDt.StageClearArr, arr => arr.IsAccept)){
                    max = pDt.StageClearArr[STG10].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG10].Reward.ToString();
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
                    pDt.StageClearArr[STG10].IsAccept = true;
                    //* Reward Panel
                    int reward = pDt.StageClearArr[STG10].Reward;
                    hm.displayShowRewardPanel(coin: 0, diamond: reward);
                    DM.ins.personalData.Diamond += reward;
                    //* Next Mission
                    max = pDt.StageClearArr[STG30].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG30].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG30].IsComplete && !pDt.StageClearArr[STG30].IsAccept){
                    pDt.StageClearArr[STG30].IsAccept = true;
                    //* Reward Panel
                    int reward = pDt.StageClearArr[STG30].Reward;
                    hm.displayShowRewardPanel(coin: 0, diamond: reward);
                    DM.ins.personalData.Diamond += reward;
                    //* Next Mission
                    max = pDt.StageClearArr[STG60].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG60].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG60].IsComplete && !pDt.StageClearArr[STG60].IsAccept){
                    pDt.StageClearArr[STG60].IsAccept = true;
                    //* Reward Panel
                    int reward = pDt.StageClearArr[STG60].Reward;
                    hm.displayShowRewardPanel(coin: 0, diamond: reward);
                    DM.ins.personalData.Diamond += reward;
                    //* Next Mission
                    max = pDt.StageClearArr[STG100].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG100].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG100].IsComplete && !pDt.StageClearArr[STG100].IsAccept){
                    pDt.StageClearArr[STG100].IsAccept = true;
                    //* Reward Panel
                    int reward = pDt.StageClearArr[STG100].Reward;
                    hm.displayShowRewardPanel(coin: 0, diamond: reward);
                    DM.ins.personalData.Diamond += reward;
                    //* Next Mission
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage {max} Clear";
                    rewardTxt.text = pDt.StageClearArr[STG160].Reward.ToString();
                }
                else if(pDt.StageClearArr[STG160].IsComplete && !pDt.StageClearArr[STG160].IsAccept){
                    pDt.StageClearArr[STG160].IsAccept = true;
                    //* Reward Panel
                    int reward = pDt.StageClearArr[STG160].Reward;
                    hm.displayShowRewardPanel(coin: 0, diamond: reward);
                    DM.ins.personalData.Diamond += reward;
                    //* Next Mission
                    infoTxt.text = $"Stage All Clear";
                    rewardTxt.text = pDt.StageClearArr[STG160].Reward.ToString();
                    allClear = true;
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
    static public void setStageClear(int stage){
        //* Achivement (StageClear IsComplete)
        for(int i=0; i<DM.ins.personalData.StageClearArr.Length; i++){
            if(stage >= DM.ins.personalData.StageClearArr[i].Val){
                DM.ins.personalData.StageClearArr[i].IsComplete = true;
            }
        }
        /*
        if(stage >= DM.ins.personalData.StageClearArr[STG10].Val){
            DM.ins.personalData.StageClearArr[STG10].IsComplete = true;
        }
        if(stage >= DM.ins.personalData.StageClearArr[STG30].Val){
            DM.ins.personalData.StageClearArr[STG30].IsComplete = true;
        }
        if(stage >= DM.ins.personalData.StageClearArr[STG60].Val){
            DM.ins.personalData.StageClearArr[STG60].IsComplete = true;
        }
        if(stage >= DM.ins.personalData.StageClearArr[STG100].Val){
            DM.ins.personalData.StageClearArr[STG100].IsComplete = true;
        }
        if(stage >= DM.ins.personalData.StageClearArr[STG160].Val){
            DM.ins.personalData.StageClearArr[STG160].IsComplete = true;
        }
        */
    }
}
