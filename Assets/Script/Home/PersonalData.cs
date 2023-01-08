using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

[System.Serializable]
public class AchivementInfo {
    [SerializeField] int val;        public int Val {get => val;}
    [SerializeField] int reward;     public int Reward {get => reward;}
    [SerializeField] bool isAccept;  public bool IsAccept {get => isAccept; set => isAccept = value;}
    [SerializeField] bool isComplete;   public bool IsComplete {get => isComplete; set => isComplete = value;}

    public AchivementInfo(int val, int reward) {
        this.val = val;
        this.reward = reward;
    }
}

[System.Serializable]
public class PersonalData {
    //* Value
    [SerializeField] LANG.TP lang; public LANG.TP Lang {get => lang; set => lang = value;}
    [Header("STATUS")][Header("__________________________")]
    [SerializeField] int playTime; public int PlayTime {get => playTime; set => playTime = value;}

    [Header("GOODS")][Header("__________________________")]
    [SerializeField] int coin; public int Coin {get => coin; set => coin = value;}
    [SerializeField] int diamond; public int Diamond {get => diamond; set => diamond = value;}
    [SerializeField] int rouletteTicket; public int RouletteTicket {get => rouletteTicket; set => rouletteTicket = value;}
    [SerializeField] string rouletteTicketCoolTime; public string RouletteTicketCoolTime {get => rouletteTicketCoolTime; set => rouletteTicketCoolTime = value;}

    [Header("ONE TIME TRIGGER")][Header("__________________________")]
    [SerializeField] bool isHardmodeOn; public bool IsHardmodeOn {get => isHardmodeOn; set => isHardmodeOn = value;}
    [SerializeField] bool isHardmodeEnableNotice; public bool IsHardmodeEnableNotice {get => isHardmodeEnableNotice; set => isHardmodeEnableNotice = value;}
    [SerializeField] bool isRemoveAD; public bool IsRemoveAD {get => isRemoveAD; set => isRemoveAD = value;}
    [SerializeField] bool isSkipTutorial; public bool IsSkipTutorial {get => isSkipTutorial; set => isSkipTutorial = value;}
    [SerializeField] bool isPurchasePremiumPack; public bool IsPurchasePremiumPack {get => isPurchasePremiumPack; set => isPurchasePremiumPack = value;}

    [Header("ACHIVEMENT")][Header("__________________________")]
    [SerializeField] int clearStage; public int ClearStage {get => clearStage; set => clearStage = value;}
    [SerializeField] AchivementInfo[] stageClearArr; public AchivementInfo[] StageClearArr {get => stageClearArr; set => stageClearArr = value;}
    // [SerializeField] AchivementInfo stage10Clear; public AchivementInfo Stage10Clear {get => stage10Clear; set => stage10Clear = value;}
    // [SerializeField] AchivementInfo stage30Clear; public AchivementInfo Stage30Clear {get => stage30Clear; set => stage30Clear = value;}
    // [SerializeField] AchivementInfo stage60Clear; public AchivementInfo Stage60Clear {get => stage60Clear; set => stage60Clear = value;}
    // [SerializeField] AchivementInfo stage100Clear; public AchivementInfo Stage100Clear {get => stage100Clear; set => stage100Clear = value;}
    // [SerializeField] AchivementInfo stage160Clear; public AchivementInfo Stage160Clear {get => stage160Clear; set => stage160Clear = value;}

    [Header("CHARACTOR")][Header("__________________________")]
    [SerializeField] int selectCharaIdx;  public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    [SerializeField] List<bool> charaLockList;  public List<bool> CharaLockList {get => charaLockList; set => charaLockList = value;}
    [Header("BAT")][Header("__________________________")]
    [SerializeField] int selectBatIdx;  public int SelectBatIdx {get => selectBatIdx; set => selectBatIdx = value;}
    [SerializeField] List<bool> batLockList;  public List<bool> BatLockList {get => batLockList; set => batLockList = value;}
    [Header("SKILL")][Header("__________________________")]
    [SerializeField] bool isUnlock2ndSkill;  public bool IsUnlock2ndSkill {get => isUnlock2ndSkill; set => isUnlock2ndSkill = value;}
    [SerializeField] int selectSkillIdx;  public int SelectSkillIdx {get => selectSkillIdx; set => selectSkillIdx = value;}
    [SerializeField] int selectSkill2Idx;  public int SelectSkill2Idx {get => selectSkill2Idx; set => selectSkill2Idx = value;}
    [SerializeField] List<bool> skillLockList;  public List<bool> SkillLockList {get => skillLockList; set => skillLockList = value;}

    [Header("ITEM PASSIVE")][Header("__________________________")]
    [FormerlySerializedAs("itemPassive")]
    [SerializeField] ItemPsvList itemPassive; public ItemPsvList ItemPassive {get => itemPassive; set => itemPassive = value;}

    [Header("UPGRADE ABILITY")][Header("__________________________")]
    [FormerlySerializedAs("upgrade")]
    [SerializeField] UpgradeList upgrade; public UpgradeList Upgrade {get => upgrade; set => upgrade = value;}

    //* PlayerPrefs キー リスト => privateは jsonには追加しない。
    private List<string> keyList;  public List<string> KeyList {get => keyList; set => keyList = value;}

    //TODO Item OnLock List

    //* constructor
    public PersonalData(){
        Debug.Log($"{this}::constructor");
        
        //* 初期化
        this.KeyList = new List<string>();
        this.charaLockList = new List<bool>();
        this.batLockList = new List<bool>();
        this.skillLockList = new List<bool>();
        this.itemPassive = new ItemPsvList();
        this.upgrade = new UpgradeList();

        this.stageClearArr = new AchivementInfo[] {
            new AchivementInfo(10, 10),
            new AchivementInfo(30, 30),
            new AchivementInfo(60, 60),
            new AchivementInfo(100, 100),
            new AchivementInfo(160, 160),
        };

        // Debug.Log("PersonalData::upgrade.Arr[0].lv-->" + upgrade.Arr[0].lv);
    }

    //* method
    public void load(ref ItemInfo[] charas, ref ItemInfo[] bats, ref ItemInfo[] skills){
        Debug.Log($"<size=20><color=green>{this}::LOAD</color></size>");
        //* Check Json
        string json = PlayerPrefs.GetString(DM.DATABASE_KEY.Json.ToString());
        Debug.Log($"<size=15>{this}::JSON:: LOAD Data ={json}</size>");

        //* PlayerPrefsへ保存したデータ ロード
        var data = JsonUtility.FromJson<PersonalData>(json); //* Convert Json To Class Data

        //* Set Data
        this.Lang = data.Lang;

        this.PlayTime = data.PlayTime;

        this.Coin = data.Coin;
        this.Diamond = data.Diamond;
        this.rouletteTicket = data.RouletteTicket;
        this.rouletteTicketCoolTime = (data.RouletteTicketCoolTime != null)? data.RouletteTicketCoolTime : DateTime.Now.ToString();

        this.isHardmodeOn = data.IsHardmodeOn;
        this.isHardmodeEnableNotice = data.IsHardmodeEnableNotice;
        this.isRemoveAD = data.IsRemoveAD;
        this.isSkipTutorial = data.IsSkipTutorial;
        this.isPurchasePremiumPack = data.IsPurchasePremiumPack;

        this.clearStage = data.ClearStage;
        this.stageClearArr = data.stageClearArr;

        this.SelectCharaIdx = data.SelectCharaIdx;
        this.CharaLockList = data.CharaLockList;

        this.SelectBatIdx = data.SelectBatIdx;
        this.BatLockList = data.BatLockList;

        this.SelectSkillIdx = data.SelectSkillIdx;
        this.IsUnlock2ndSkill = data.IsUnlock2ndSkill;
        if(this.IsUnlock2ndSkill) this.SelectSkill2Idx = data.SelectSkill2Idx;
        this.SkillLockList = data.SkillLockList;

        // this.ItemPassive = data.ItemPassive;
        this.Upgrade = data.Upgrade;


        //* Set Real Content Items IsLock
        for(int i=0; i<charas.Length; i++){
            if(i==0)    Debug.Log("<color=green>Chara</color>LockList["+i+"].IsLock=" + this.CharaLockList[i] + ", length= <color=green>" + charas.Length + "</color>");
            charas[i].GetComponent<ItemInfo>().IsLock = this.CharaLockList[i];
        }
        for(int i=0; i<bats.Length; i++){
            if(i==0)    Debug.Log("<color=orange>Bat</color>LockList["+i+"].IsLock=" + this.BatLockList[i] + ", length= <color=orange>" + bats.Length + "</color>");
            bats[i].GetComponent<ItemInfo>().IsLock = this.BatLockList[i];
        }
        for(int i=0; i<skills.Length; i++){
            if(i==0)    Debug.Log("<color=yellow>Skill</color>LockList["+i+"].IsLock=" + this.SkillLockList[i] + ", length= <color=yellow>" + skills.Length + "</color>");
            skills[i].GetComponent<ItemInfo>().IsLock = this.SkillLockList[i];
        }
    }
    
    public void save(){
        Debug.Log($"<size=20><color=red>{this}::SAVE</color></size>");
        PlayerPrefs.SetString(DM.DATABASE_KEY.Json.ToString(), JsonUtility.ToJson(this, true)); //* Serialize To Json

        //* Print
        string json = PlayerPrefs.GetString(DM.DATABASE_KEY.Json.ToString());
        Debug.Log($"<size=15>{this}::JSON:: SAVE Data ={json}</size>");
    }

    public void reset(){
        Debug.Log("<size=30>RESET</size>");
        PlayerPrefs.DeleteAll();

        this.Lang = LANG.TP.JP;

        this.PlayTime = 0;

        this.Coin = 100000;
        this.Diamond = 0;
        this.rouletteTicket = 1; //* 1つ上げるのは、ボーナス感じ。
        this.RouletteTicketCoolTime = DateTime.Now.ToString();

        this.isHardmodeOn = false;
        this.isHardmodeEnableNotice = false;
        this.isRemoveAD = false;
        this.isSkipTutorial = false;
        this.isPurchasePremiumPack = false;

        this.clearStage = 1;
        this.stageClearArr = new AchivementInfo[] {
            new AchivementInfo(10, 10),
            new AchivementInfo(30, 30),
            new AchivementInfo(60, 60),
            new AchivementInfo(100, 100),
            new AchivementInfo(160, 160),
        };

        this.SelectCharaIdx = 0;
        this.CharaLockList = new List<bool>();

        this.SelectBatIdx = 0;
        this.BatLockList = new List<bool>();

        this.SelectSkillIdx = 0;
        this.IsUnlock2ndSkill = false;
        this.SelectSkill2Idx = -1;
        this.SkillLockList = new List<bool>();

        // this.ItemPassive = new ItemPassiveList();
        this.Upgrade = new UpgradeList();

        for(int i=0; i<DM.ins.scrollviews[(int)DM.PANEL.Chara].ItemPrefs.Length; i++){
            if(i==0) this.CharaLockList.Add(false);//    items[0].IsLock = false;}
            else     this.CharaLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.PANEL.Bat].ItemPrefs.Length; i++){
            if(i==0) this.BatLockList.Add(false);//    items[0].IsLock = false;}
            else     this.BatLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.PANEL.Skill].ItemPrefs.Length; i++){
            if(i==0) this.SkillLockList.Add(false);//    items[0].IsLock = false;}
            else     this.SkillLockList.Add(true);//     items[i].IsLock = true;}
        }
    }

    public int getSelectIdx(string type){
        return (type == DM.PANEL.Chara.ToString())? SelectCharaIdx
            :(type == DM.PANEL.Bat.ToString())? SelectBatIdx
            :(type == DM.PANEL.Skill.ToString())? SelectSkillIdx :-1;
    }

    public void setSelectIdx(int index){
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        switch(itemType){
            case DM.PANEL.Chara :  SelectCharaIdx = index; break;
            case DM.PANEL.Bat :    SelectBatIdx = index;   break;
            case DM.PANEL.Skill :  
                var hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
                if(hm.selectedSkillBtnIdx == 0)
                    SelectSkillIdx = index; 
                else
                    SelectSkill2Idx = index; 
                break;
        }
    }

    public void setUnLockCurList(int curIdx){
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        switch(itemType){
            case DM.PANEL.Chara :  CharaLockList[curIdx] = false; break;
            case DM.PANEL.Bat :    BatLockList[curIdx] = false;   break;
            case DM.PANEL.Skill :  SkillLockList[curIdx] = false; break;
        }
    }
}
