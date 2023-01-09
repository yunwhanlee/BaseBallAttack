using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class AchivementInfo { //* PersonalDataで宣言。
    [SerializeField] int val;        public int Val {get => val; set => val = value;}
    [SerializeField] int reward;     public int Reward {get => reward;}
    [SerializeField] bool isAccept;  public bool IsAccept {get => isAccept; set => isAccept = value;}
    [SerializeField] bool isComplete;   public bool IsComplete {get => isComplete; set => isComplete = value;}

    public AchivementInfo(int val, int reward) {
        this.val = val;
        this.reward = reward;
    }
}

public class Achivement : MonoBehaviour
{   
    //* Outside
    HomeManager hm;

    //* Name
    const string InfoTxt = "InfoTxt", ValueTxt = "ValueTxt", RewardTxt = "RewardTxt", RewardIcon = "RewardIcon", ClaimBtn = "ClaimBtn";
    const string StageClear = "StageClear";
    const string DestroyBlocks = "DestroyBlocks";
    const string CollectedCoin = "CollectedCoin";
    const string CollectedDiamond = "CollectedDiamond";
    const string CollectedRouletteTicket = "CollectedRouletteTicket";
    const string AtvSkillCollection = "AtvSkillCollection";
    const string BatCollection = "BatCollection";
    const string CharactorCollection = "CharactorCollection";
    const string NormalModeClear = "NormalModeClear";
    const string HardModeClear = "HardModeClear";
    const string KillBoss = "KillBoss";
    const string UpgradeCnt = "UpgradeCnt";

    const int STG10 = 0, STG30 = 1, STG60 = 2, STG100 = 3, STG160 = 4; //* Stage Clear 
    const int DTR100 = 0, DTR200 = 1, DTR400 = 2, DTR700 = 3, DTR1000 = 4; //* Destroy Block Cnt
    const int CLTCOIN10000 = 0, CLTCOIN25000 = 1, CLTCOIN50000 = 2, CLTCOIN100000 = 3, CLTCOIN1000000 = 4;
    const int CLTDIA500 = 0, CLTDIA1000 = 1, CLTDIA2000 = 2, CLTDIA5000 = 3, CLTDIA10000 = 4;
    const int CLTTICKET10 = 0, CLTTICKET50 = 1, CLTTICKET100 = 2, CLTTICKET150 = 3, CLTTICKET300 = 4;
    const int CLTCHARAQUATER = 0, CLTCHARAHALF = 1, CLTCHARAALL = 2;
    const int CLTBATQUATER = 0, CLTBATHALF = 1, CLTBATALL = 2;
    const int CLTSKILLHALF = 0, CLTSKILLALL = 1;
    const int UPG20PER = 0, UPG40PER = 1, UPG60PER = 2, UPG80PER = 3, UPGALL = 4;

    //* Value
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
    [SerializeField] AchivementInfo[] targetArr; // From PersonalData.cs

    void Start()
    {
        //* Assign Object
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();

        panelImg = this.GetComponent<Image>();
        infoTxt = this.transform.Find(InfoTxt).GetComponent<Text>();
        valueTxt = this.transform.Find(ValueTxt).GetComponent<Text>();
        rewardTxt = this.transform.Find(RewardTxt).GetComponent<Text>();
        rewardIconImg = this.transform.Find(RewardIcon).GetComponent<Image>();
        claimBtn = this.transform.Find(ClaimBtn).GetComponent<Button>();

        //* Set Icon
        rewardIconImg.sprite = (rewardType == REWARD_TYPE.COIN)? hm.rewardIconSprs[(int)REWARD_TYPE.COIN]
            : (rewardType == REWARD_TYPE.DIAMOND)? hm.rewardIconSprs[(int)REWARD_TYPE.DIAMOND]
            : (rewardType == REWARD_TYPE.ROULETTE_TICKET)? hm.rewardIconSprs[(int)REWARD_TYPE.ROULETTE_TICKET] : null;

        var pDt = DM.ins.personalData;
        //* Set Target
        setTarget();

        //* Set Value
        switch(this.name){
            case StageClear : {
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
            case DestroyBlocks :
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
            case CollectedCoin :
                cnt = pDt.CollectedCoin;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.CollectedCoinArr[CLTCOIN1000000].IsAccept)
                    setNext(pDt, CLTCOIN1000000, _allClear: true);
                else if(pDt.CollectedCoinArr[CLTCOIN100000].IsAccept)
                    setNext(pDt, CLTCOIN1000000);
                else if(pDt.CollectedCoinArr[CLTCOIN50000].IsAccept)
                    setNext(pDt, CLTCOIN100000);
                else if(pDt.CollectedCoinArr[CLTCOIN25000].IsAccept)
                    setNext(pDt, CLTCOIN50000);
                else if(pDt.CollectedCoinArr[CLTCOIN10000].IsAccept)
                    setNext(pDt, CLTCOIN25000);
                else if(!Array.Exists(pDt.CollectedCoinArr, arr => arr.IsAccept))
                    setNext(pDt, CLTCOIN10000);
                break;
            case CollectedDiamond :
                cnt = pDt.CollectedDiamond;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.CollectedDiamondArr[CLTDIA10000].IsAccept)
                    setNext(pDt, CLTDIA10000, _allClear: true);
                else if(pDt.CollectedDiamondArr[CLTDIA5000].IsAccept)
                    setNext(pDt, CLTDIA10000);
                else if(pDt.CollectedDiamondArr[CLTDIA2000].IsAccept)
                    setNext(pDt, CLTDIA5000);
                else if(pDt.CollectedDiamondArr[CLTDIA1000].IsAccept)
                    setNext(pDt, CLTDIA2000);
                else if(pDt.CollectedDiamondArr[CLTDIA500].IsAccept)
                    setNext(pDt, CLTDIA1000);
                else if(!Array.Exists(pDt.CollectedDiamondArr, arr => arr.IsAccept))
                    setNext(pDt, CLTDIA500);
                break;
            case CollectedRouletteTicket :
                cnt = pDt.CollectedRouletteTicket;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.CollectedRouletteTicketArr[CLTTICKET300].IsAccept)
                    setNext(pDt, CLTTICKET300, _allClear: true);
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET150].IsAccept)
                    setNext(pDt, CLTTICKET300);
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET100].IsAccept)
                    setNext(pDt, CLTTICKET150);
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET50].IsAccept)
                    setNext(pDt, CLTTICKET100);
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET10].IsAccept)
                    setNext(pDt, CLTTICKET50);
                else if(!Array.Exists(pDt.CollectedRouletteTicketArr, arr => arr.IsAccept))
                    setNext(pDt, CLTTICKET10);
                break;
            case CharactorCollection : {
                List<bool> lockList = DM.ins.personalData.CharaLockList;
                cnt = lockList.FindAll(list => list == false).Count;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.CharaCollectionArr[CLTCHARAALL].IsAccept)
                    setNext(pDt, CLTCHARAALL, _allClear: true);
                else if(pDt.CharaCollectionArr[CLTCHARAHALF].IsAccept)
                    setNext(pDt, CLTCHARAALL);
                else if(pDt.CharaCollectionArr[CLTCHARAQUATER].IsAccept)
                    setNext(pDt, CLTCHARAHALF);
                else if(!Array.Exists(pDt.CharaCollectionArr, arr => arr.IsAccept))
                    setNext(pDt, CLTCHARAQUATER);
                break;
                }
            case BatCollection : {
                List<bool> lockList = DM.ins.personalData.BatLockList;
                cnt = lockList.FindAll(list => list == false).Count;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.BatCollectionArr[CLTBATALL].IsAccept)
                    setNext(pDt, CLTBATALL, _allClear: true);
                else if(pDt.BatCollectionArr[CLTBATHALF].IsAccept)
                    setNext(pDt, CLTBATALL);
                else if(pDt.BatCollectionArr[CLTBATQUATER].IsAccept)
                    setNext(pDt, CLTBATHALF);
                else if(!Array.Exists(pDt.BatCollectionArr, arr => arr.IsAccept))
                    setNext(pDt, CLTBATQUATER);
                break;
                }
            case AtvSkillCollection : {
                List<bool> lockList = DM.ins.personalData.SkillLockList;
                cnt = lockList.FindAll(list => list == false).Count;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.AtvSkillCollectionArr[CLTSKILLALL].IsAccept)
                    setNext(pDt, CLTSKILLALL, _allClear: true);
                else if(pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsAccept)
                    setNext(pDt, CLTSKILLALL);
                else if(!Array.Exists(pDt.AtvSkillCollectionArr, arr => arr.IsAccept))
                    setNext(pDt, CLTSKILLHALF);
                break;
                }
            case UpgradeCnt : {
                cnt = pDt.UpgradeCnt;
                Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
                //* Init
                if(pDt.UpgradeCntArr[UPGALL].IsAccept)
                    setNext(pDt, UPGALL, _allClear: true);
                else if(pDt.UpgradeCntArr[UPG80PER].IsAccept)
                    setNext(pDt, UPGALL);
                else if(pDt.UpgradeCntArr[UPG60PER].IsAccept)
                    setNext(pDt, UPG80PER);
                else if(pDt.UpgradeCntArr[UPG40PER].IsAccept)
                    setNext(pDt, UPG60PER);
                else if(pDt.UpgradeCntArr[UPG20PER].IsAccept)
                    setNext(pDt, UPG40PER);
                else if(!Array.Exists(pDt.UpgradeCntArr, arr => arr.IsAccept))
                    setNext(pDt, UPG20PER);
                break;
                }
            case KillBoss :
                break;
            case NormalModeClear :
                if(pDt.NormalModeClear[0].IsComplete) cnt = 1;
                setNext(pDt, idx: 0, pDt.NormalModeClear[0].IsAccept);
                break;
            case HardModeClear :
                if(pDt.HardModeClear[0].IsComplete) cnt = 1;
                setNext(pDt, idx: 0, pDt.HardModeClear[0].IsAccept);
                break;
        }
    }

    void Update(){
        //* Set Color
        if(allClear){
            panelImg.color = Color.gray;
            claimBtn.GetComponent<Image>().color = Color.gray;
        }
        else{
            var pDt = DM.ins.personalData;
            switch(this.name){
                // case StageClear : cnt = pDt.ClearStage; break;
                // case DestroyBlocks : cnt = pDt.DestroyBlockCnt; break;
                case CollectedCoin : cnt = pDt.CollectedCoin; break;
                case CollectedDiamond : cnt = pDt.CollectedDiamond; break;
                case CollectedRouletteTicket : cnt = pDt.CollectedRouletteTicket; break;
                case CharactorCollection : cnt = pDt.CollectedChara; break;
                case BatCollection : cnt = pDt.CollectedBat; break;
                case AtvSkillCollection : cnt = pDt.CollectedAtvSkill; break;
                case UpgradeCnt : cnt = pDt.UpgradeCnt; break;
                //* 追加
            }
            panelImg.color = (cnt >= max)? Color.white : Color.gray;
            claimBtn.GetComponent<Image>().color = (cnt >= max)? Color.white : Color.gray;
        }

        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public void onClickCalimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;
        switch(this.name){
            case StageClear : {
                if(pDt.StageClearArr[STG10].IsComplete && !pDt.StageClearArr[STG10].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG10));
                    setNext(pDt, STG30);
                }
                else if(pDt.StageClearArr[STG30].IsComplete && !pDt.StageClearArr[STG30].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG30));
                    setNext(pDt, STG60);
                }
                else if(pDt.StageClearArr[STG60].IsComplete && !pDt.StageClearArr[STG60].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG60));
                    setNext(pDt, STG100);
                }
                else if(pDt.StageClearArr[STG100].IsComplete && !pDt.StageClearArr[STG100].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG100));
                    setNext(pDt, STG160);
                }
                else if(pDt.StageClearArr[STG160].IsComplete && !pDt.StageClearArr[STG160].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG160));
                    setNext(pDt, STG160, _allClear: true);
                }
                //* 追加
                break;
            }
            case DestroyBlocks : {
                if(pDt.DestroyBlockArr[DTR100].IsComplete && !pDt.DestroyBlockArr[DTR100].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, DTR100));
                    setNext(pDt, DTR200);
                }
                else if(pDt.DestroyBlockArr[DTR200].IsComplete && !pDt.DestroyBlockArr[DTR200].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, DTR200));
                    setNext(pDt, DTR400);
                }
                else if(pDt.DestroyBlockArr[DTR400].IsComplete && !pDt.DestroyBlockArr[DTR400].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, DTR400));
                    setNext(pDt, DTR700);
                }
                else if(pDt.DestroyBlockArr[DTR700].IsComplete && !pDt.DestroyBlockArr[DTR700].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, DTR700));
                    setNext(pDt, DTR1000);
                }
                else if(pDt.DestroyBlockArr[DTR1000].IsComplete && !pDt.DestroyBlockArr[DTR1000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, DTR1000));
                    setNext(pDt, DTR1000, _allClear: true);
                }
                //* 追加
                break;
            }
            case CollectedCoin : {
                if(pDt.CollectedCoinArr[CLTCOIN10000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN10000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN10000));
                    setNext(pDt, CLTCOIN25000);
                }
                else if(pDt.CollectedCoinArr[CLTCOIN25000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN25000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN25000));
                    setNext(pDt, CLTCOIN50000);
                }
                else if(pDt.CollectedCoinArr[CLTCOIN50000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN50000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN50000));
                    setNext(pDt, CLTCOIN100000);
                }
                else if(pDt.CollectedCoinArr[CLTCOIN100000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN100000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN100000));
                    setNext(pDt, CLTCOIN1000000);
                }
                else if(pDt.CollectedCoinArr[CLTCOIN1000000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN1000000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN1000000));
                    setNext(pDt, CLTCOIN1000000, _allClear: true);
                }
                //* 追加
                break;
            }
            case CollectedDiamond : {
                if(pDt.CollectedDiamondArr[CLTDIA500].IsComplete && !pDt.CollectedDiamondArr[CLTDIA500].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA500));
                    setNext(pDt, CLTDIA1000);
                }
                else if(pDt.CollectedDiamondArr[CLTDIA1000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA1000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA1000));
                    setNext(pDt, CLTDIA2000);
                }
                else if(pDt.CollectedDiamondArr[CLTDIA2000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA2000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA2000));
                    setNext(pDt, CLTDIA5000);
                }
                else if(pDt.CollectedDiamondArr[CLTDIA5000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA5000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA5000));
                    setNext(pDt, CLTDIA10000);
                }
                else if(pDt.CollectedDiamondArr[CLTDIA10000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA10000].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA10000));
                    setNext(pDt, CLTDIA10000, _allClear: true);
                }
                //* 追加
                break;
            }
            case CollectedRouletteTicket : {
                if(pDt.CollectedRouletteTicketArr[CLTTICKET10].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET10].IsAccept){
                    DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET10));
                    setNext(pDt, CLTTICKET50);
                }
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET50].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET50].IsAccept){
                    DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET50));
                    setNext(pDt, CLTTICKET100);
                }
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET100].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET100].IsAccept){
                    DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET100));
                    setNext(pDt, CLTTICKET150);
                }
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET150].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET150].IsAccept){
                    DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET150));
                    setNext(pDt, CLTTICKET300);
                }
                else if(pDt.CollectedRouletteTicketArr[CLTTICKET300].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET300].IsAccept){
                    DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET300));
                    setNext(pDt, CLTTICKET300, _allClear: true);
                }
                //* 追加
                break;
            }
            case CharactorCollection : {
                if(pDt.CharaCollectionArr[CLTCHARAQUATER].IsComplete && !pDt.CharaCollectionArr[CLTCHARAQUATER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAQUATER));
                    setNext(pDt, CLTCHARAHALF);
                }
                else if(pDt.CharaCollectionArr[CLTCHARAHALF].IsComplete && !pDt.CharaCollectionArr[CLTCHARAHALF].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAHALF));
                    setNext(pDt, CLTCHARAALL);
                }
                else if(pDt.CharaCollectionArr[CLTCHARAALL].IsComplete && !pDt.CharaCollectionArr[CLTCHARAALL].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAALL));
                    setNext(pDt, CLTCHARAALL, _allClear: true);
                }
                break;
            }
            case BatCollection : {
                if(pDt.BatCollectionArr[CLTBATQUATER].IsComplete && !pDt.BatCollectionArr[CLTBATQUATER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATQUATER));
                    setNext(pDt, CLTBATHALF);
                }
                else if(pDt.BatCollectionArr[CLTBATHALF].IsComplete && !pDt.BatCollectionArr[CLTBATHALF].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATHALF));
                    setNext(pDt, CLTBATALL);
                }
                else if(pDt.BatCollectionArr[CLTBATALL].IsComplete && !pDt.BatCollectionArr[CLTBATALL].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATALL));
                    setNext(pDt, CLTBATALL, _allClear: true);
                }
                break;
            }
            case AtvSkillCollection : {
                if(pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsComplete && !pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTSKILLHALF));
                    setNext(pDt, CLTSKILLALL);
                }
                else if(pDt.AtvSkillCollectionArr[CLTSKILLALL].IsComplete && !pDt.AtvSkillCollectionArr[CLTSKILLALL].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, CLTSKILLALL));
                    setNext(pDt, CLTSKILLALL, _allClear: true);
                }
                break;
            }
            case UpgradeCnt : {
                if(pDt.UpgradeCntArr[UPG20PER].IsComplete && !pDt.UpgradeCntArr[UPG20PER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, UPG20PER));
                    setNext(pDt, UPG40PER);
                }
                else if(pDt.UpgradeCntArr[UPG40PER].IsComplete && !pDt.UpgradeCntArr[UPG40PER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, UPG40PER));
                    setNext(pDt, UPG60PER);
                }
                else if(pDt.UpgradeCntArr[UPG60PER].IsComplete && !pDt.UpgradeCntArr[UPG60PER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, UPG60PER));
                    setNext(pDt, UPG80PER);
                }
                else if(pDt.UpgradeCntArr[UPG80PER].IsComplete && !pDt.UpgradeCntArr[UPG80PER].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, UPG80PER));
                    setNext(pDt, UPGALL);
                }
                else if(pDt.UpgradeCntArr[UPGALL].IsComplete && !pDt.UpgradeCntArr[UPGALL].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, UPGALL));
                    setNext(pDt, UPGALL, _allClear: true);
                }
                break;
            }
            case NormalModeClear : {
                if(pDt.NormalModeClear[0].IsComplete && !pDt.NormalModeClear[0].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, idx: 0));
                    setNext(pDt, idx: 0, _allClear: true);
                }
                break;
            }
            case HardModeClear : {
                if(pDt.HardModeClear[0].IsComplete && !pDt.HardModeClear[0].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, idx: 0));
                    setNext(pDt, idx: 0, _allClear: true);
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
#region Static
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
    static public void setGoodsCompletedList(){
        collectCoin(0);
        collectDiamond(0);
        collectRouletteTicket(0);
    }
    static public void collectCoin(int amount){
        DM.ins.personalData.CollectedCoin += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedCoinArr.Length; i++){
            if(pDt.CollectedCoin >= pDt.CollectedCoinArr[i].Val)
                pDt.CollectedCoinArr[i].IsComplete = true;
        }
    }
    static public void collectDiamond(int amount){
        DM.ins.personalData.CollectedDiamond += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedDiamondArr.Length; i++){
            if(pDt.CollectedDiamond >= pDt.CollectedDiamondArr[i].Val)
                pDt.CollectedDiamondArr[i].IsComplete = true;
        }
    }
    static public void collectRouletteTicket(int amount){
        DM.ins.personalData.CollectedRouletteTicket += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedRouletteTicketArr.Length; i++){
            if(pDt.CollectedRouletteTicket >= pDt.CollectedRouletteTicketArr[i].Val)
                pDt.CollectedRouletteTicketArr[i].IsComplete = true;
        }
    }
    static public void collectChara() {
        DM.ins.personalData.CollectedChara++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CharaCollectionArr.Length; i++){
            if(pDt.CollectedChara >= pDt.CharaCollectionArr[i].Val)
                pDt.CharaCollectionArr[i].IsComplete = true;
        }
    }
    static public void collectBat() {
        DM.ins.personalData.CollectedBat++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.BatCollectionArr.Length; i++){
            if(pDt.CollectedBat >= pDt.BatCollectionArr[i].Val)
                pDt.BatCollectionArr[i].IsComplete = true;
        }
    }
    static public void collectAtvSkill() {
        DM.ins.personalData.CollectedAtvSkill++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.AtvSkillCollectionArr.Length; i++){
            if(pDt.CollectedAtvSkill >= pDt.AtvSkillCollectionArr[i].Val)
                pDt.AtvSkillCollectionArr[i].IsComplete = true;
        }
    }
    static public void addUpgradeCnt() {
        DM.ins.personalData.UpgradeCnt++;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.UpgradeCntArr.Length; i++){
            if(pDt.UpgradeCnt >= pDt.UpgradeCntArr[i].Val)
                pDt.UpgradeCntArr[i].IsComplete = true;
        }
    }
    static public void setNormalModeClear(){
        DM.ins.personalData.NormalModeClear[0].IsComplete = true;
    }
    static public void setHardModeClear(){
        DM.ins.personalData.HardModeClear[0].IsComplete = true;
    }
#endregion
#region Private
    private void setTarget(){
        var pDt = DM.ins.personalData;
        try{
            targetArr = (this.name == StageClear)? pDt.StageClearArr
                : (this.name == DestroyBlocks)? pDt.DestroyBlockArr
                : (this.name == CollectedCoin)? pDt.CollectedCoinArr
                : (this.name == CollectedDiamond)? pDt.CollectedDiamondArr
                : (this.name == CollectedRouletteTicket)? pDt.CollectedRouletteTicketArr
                : (this.name == CharactorCollection)? pDt.CharaCollectionArr
                : (this.name == BatCollection)? pDt.BatCollectionArr
                : (this.name == AtvSkillCollection)? pDt.AtvSkillCollectionArr
                : (this.name == UpgradeCnt)? pDt.UpgradeCntArr
                : (this.name == NormalModeClear)? pDt.NormalModeClear
                : (this.name == HardModeClear)? pDt.HardModeClear
                //* 追加
                : null;
        }catch(Exception err){
            Debug.LogError("TYPEに合わせる nameがありません。\n" + err);
        }
    }
    private void setNext(PersonalData pDt, int idx, bool _allClear = false){
        //* 処理。
        this.max = targetArr[idx].Val;
        this.infoTxt.text = this.name;
        this.rewardTxt.text = targetArr[idx].Reward.ToString();
        if(_allClear) this.allClear = _allClear;
    }
    private int acceptReward(PersonalData pDt, int idx){
        //* 処理。
        targetArr[idx].IsAccept = true;
        int reward = targetArr[idx].Reward;
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
#endregion
}
