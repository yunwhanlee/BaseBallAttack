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
    const int STG10 = 0, STG30 = 1, STG60 = 2, STG100 = 3, STG160 = 4; //* Stage Clear 
    const int DTR100 = 0, DTR200 = 1, DTR400 = 2, DTR700 = 3, DTR1000 = 4; //* Destroy Block Cnt

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
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.StageClearArr[STG160].IsAccept)
                    setNext(pDt, STG160, _allClear: true);
                else if(pDt.StageClearArr[STG100].IsAccept)
                    setNext(pDt, STG160);
                else if(pDt.StageClearArr[STG60].IsAccept)
                    setNext(pDt, STG100);
                else if(pDt.StageClearArr[STG30].IsAccept)
                    setNext(pDt, STG60);
                else if(pDt.StageClearArr[STG10].IsAccept)
                    setNext(pDt, STG30);
                else if(!Array.Exists(pDt.StageClearArr, arr => arr.IsAccept))
                    setNext(pDt, STG10);
                break;
            }
            case "DestroyBlocks" :
                cnt = pDt.DestroyBlockCnt;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.DestroyBlockArr[DTR1000].IsAccept)
                    setNext(pDt, DTR1000, _allClear: true);
                else if(pDt.DestroyBlockArr[DTR700].IsAccept)
                    setNext(pDt, DTR1000);
                else if(pDt.DestroyBlockArr[DTR400].IsAccept)
                    setNext(pDt, DTR700);
                else if(pDt.DestroyBlockArr[DTR200].IsAccept)
                    setNext(pDt, DTR400);
                else if(pDt.DestroyBlockArr[DTR100].IsAccept)
                    setNext(pDt, DTR200);
                else if(!Array.Exists(pDt.DestroyBlockArr, arr => arr.IsAccept))
                    setNext(pDt, DTR100);
                break;
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
        //* Set Color
        if(allClear){
            panelImg.color = Color.gray;
            claimBtn.GetComponent<Image>().color = Color.gray;
        }
        else{
            panelImg.color = (cnt >= max)? Color.white : Color.gray;
            claimBtn.GetComponent<Image>().color = (cnt >= max)? Color.white : Color.gray;
        }
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public void onClickCalimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
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
            case "DestroyBlocks" :{
                if(pDt.DestroyBlockArr[DTR100].IsComplete && !pDt.DestroyBlockArr[DTR100].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, DTR100);
                    setNext(pDt, DTR200);
                }
                else if(pDt.DestroyBlockArr[DTR200].IsComplete && !pDt.DestroyBlockArr[DTR200].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, DTR200);
                    setNext(pDt, DTR400);
                }
                else if(pDt.DestroyBlockArr[DTR400].IsComplete && !pDt.DestroyBlockArr[DTR400].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, DTR400);
                    setNext(pDt, DTR700);
                }
                else if(pDt.DestroyBlockArr[DTR700].IsComplete && !pDt.DestroyBlockArr[DTR700].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, DTR700);
                    setNext(pDt, DTR1000);
                }
                else if(pDt.DestroyBlockArr[DTR1000].IsComplete && !pDt.DestroyBlockArr[DTR1000].IsAccept){
                    DM.ins.personalData.Diamond += acceptReward(pDt, DTR1000);
                    setNext(pDt, DTR1000, _allClear: true);
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
            if(stage >= DM.ins.personalData.StageClearArr[i].Val)
                DM.ins.personalData.StageClearArr[i].IsComplete = true;
        }
    }
    static public void addDestroyBlockCnt(){
        DM.ins.personalData.DestroyBlockCnt++;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.DestroyBlockArr.Length; i++){
            if(pDt.DestroyBlockCnt >= pDt.DestroyBlockArr[i].Val)
                pDt.DestroyBlockArr[i].IsComplete = true;
        }
    }

    private void setNext(PersonalData pDt, int idx, bool _allClear = false){
        //* ターゲット設定。
        AchivementInfo target = null;
        try{
            target = (this.name == "StageClear")? pDt.StageClearArr[idx]
                : (this.name == "DestroyBlocks")? pDt.DestroyBlockArr[idx] : null;
        }catch(Exception err){
            Debug.LogError("TYPEに合わせる nameがありません。\n" + err);
        }

        //* 処理。
        this.max = target.Val;
        this.infoTxt.text = "StageClear";
        this.rewardTxt.text = target.Reward.ToString();
        if(_allClear) this.allClear = _allClear;
    }
    private int acceptReward(PersonalData pDt, int idx){
        //* ターゲット設定。
        AchivementInfo target = null;
        try{
            target = (this.name == "StageClear")? pDt.StageClearArr[idx]
                : (this.name == "DestroyBlocks")? pDt.DestroyBlockArr[idx] : null;
        }catch(Exception err){
            Debug.LogError("TYPEに合わせる nameがありません。\n" + err);
        }

        //* 処理。
        target.IsAccept = true;
        int reward = target.Reward;
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

}
