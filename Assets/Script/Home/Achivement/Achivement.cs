using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

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

public class Achivement : MonoBehaviour{   
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
    const string BosskillCollection = "BosskillCollection";
    const string UpgradeCnt = "UpgradeCnt";

    //* Value
    enum REWARD_TYPE { COIN, DIAMOND, ROULETTE_TICKET };

    [SerializeField] REWARD_TYPE rewardType = REWARD_TYPE.DIAMOND;
    [Header("言語 [0]:EN, [1]:JP, [2]:KR")] 
    [FormerlySerializedAs("infoTxts")] [SerializeField] protected string[] infoLangTxts = new string[3];
    [SerializeField] protected int cnt, max;
    [SerializeField] protected bool allClear;

    //* Obj
    Image panelImg, rewardIconImg;
    Text infoTxt, valueTxt, rewardTxt;
    Button claimBtn;
    [SerializeField] protected AchivementInfo[] targetArr; // From PersonalData.cs

    void Awake() {
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

        //* Set Target
        setTarget();
    }

    void Start()
    {
        var pDt = DM.ins.personalData;
        
        //* Set Value
        //* To Child Class
    }

    protected void Update(){
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
                case BosskillCollection : cnt = pDt.CollectedBossKill; break;
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
    public virtual void onClickCalimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;
        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }

//*---------------------------------------
//*  Function
//*---------------------------------------
#region Static
    //* To Child Class
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
                : (this.name == BosskillCollection)? pDt.BosskillCollectionArr
                : (this.name == NormalModeClear)? pDt.NormalModeClear
                : (this.name == HardModeClear)? pDt.HardModeClear
                //* 追加
                : null;
        }catch(Exception err){
            Debug.LogError("TYPEに合わせる nameがありません。\n" + err);
        }
    }
    protected void setNext(PersonalData pDt, int idx, bool _allClear = false){
        //* 処理。
        this.max = targetArr[idx].Val;
        this.infoTxt.text = infoLangTxts[(int)DM.ins.personalData.Lang];
        this.rewardTxt.text = targetArr[idx].Reward.ToString();
        if(_allClear) this.allClear = _allClear;
    }
    protected int acceptReward(PersonalData pDt, int idx){
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
