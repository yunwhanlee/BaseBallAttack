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
                if(pDt.StageClearArr[STG160].IsComplete && pDt.StageClearArr[STG160].IsAccept){
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG100].IsComplete && pDt.StageClearArr[STG100].IsAccept){
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG60].IsComplete && pDt.StageClearArr[STG60].IsAccept){
                    max = pDt.StageClearArr[STG100].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG30].IsComplete && pDt.StageClearArr[STG30].IsAccept){
                    max = pDt.StageClearArr[STG60].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG10].IsComplete && pDt.StageClearArr[STG10].IsAccept){
                    max = pDt.StageClearArr[STG30].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                //else if(!pDt.Stage10Clear.IsAccept && !pDt.Stage30Clear.IsAccept && !pDt.Stage60Clear.IsAccept && !pDt.Stage100Clear.IsAccept && !pDt.Stage160Clear.IsAccept){
                else if(!Array.Exists(pDt.StageClearArr, arr => arr.IsAccept)){
                    max = pDt.StageClearArr[STG10].Val;
                    infoTxt.text = $"Stage {max} Clear";
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

        //* Set Color
        panelImg.color = (cnt >= max)? Color.white : Color.gray;
        claimBtn.GetComponent<Image>().color = (cnt >= max)? Color.white : Color.gray;

        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }

    void Update(){
        switch(this.name){
            case "StageClear" : {
                 //* Set Color
                panelImg.color = (cnt >= max)? Color.white : Color.gray;
                claimBtn.GetComponent<Image>().color = (cnt >= max)? Color.white : Color.gray;
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
                    Debug.Log("Stage10Clear");
                    pDt.StageClearArr[STG10].IsAccept = true;
                    max = pDt.StageClearArr[STG30].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG30].IsComplete && !pDt.StageClearArr[STG30].IsAccept){
                    Debug.Log("Stage30Clear");
                    pDt.StageClearArr[STG30].IsAccept = true;
                    max = pDt.StageClearArr[STG60].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG60].IsComplete && !pDt.StageClearArr[STG60].IsAccept){
                    Debug.Log("Stage60Clear");
                    pDt.StageClearArr[STG60].IsAccept = true;
                    max = pDt.StageClearArr[STG100].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG100].IsComplete && !pDt.StageClearArr[STG100].IsAccept){
                    Debug.Log("Stage100Clear");
                    pDt.StageClearArr[STG100].IsAccept = true;
                    max = pDt.StageClearArr[STG160].Val;
                    infoTxt.text = $"Stage {max} Clear";
                }
                else if(pDt.StageClearArr[STG160].IsComplete && !pDt.StageClearArr[STG160].IsAccept){
                    Debug.Log("Stage160Clear");
                    pDt.StageClearArr[STG160].IsAccept = true;
                    infoTxt.text = $"Stage {max} Clear";
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
        if(stage >= 10){
            DM.ins.personalData.StageClearArr[STG10].IsComplete = true;
        }
        if(stage >= 30){
            DM.ins.personalData.StageClearArr[STG30].IsComplete = true;
        }
        if(stage >= 60){
            DM.ins.personalData.StageClearArr[STG60].IsComplete = true;
        }
        if(stage >= 100){
            DM.ins.personalData.StageClearArr[STG100].IsComplete = true;
        }
        if(stage >= 160){
            DM.ins.personalData.StageClearArr[STG160].IsComplete = true;
        }
    }
}
