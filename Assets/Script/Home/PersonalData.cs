using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

[System.Serializable]
public class PersonalData {
    //* Value
    [SerializeField] LANG.TP lang; public LANG.TP Lang {get => lang; set => lang = value;}
    [SerializeField] int coin; public int Coin {get => coin; set => coin = value;}
    [SerializeField] int diamond; public int Diamond {get => diamond; set => diamond = value;}
    [SerializeField] int rouletteTicket; public int RouletteTicket {get => rouletteTicket; set => rouletteTicket = value;}
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
        this.Coin = data.Coin;
        this.Diamond = data.Diamond;
        this.rouletteTicket = data.RouletteTicket;

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
        Debug.Log("RESET");
        PlayerPrefs.DeleteAll();

        this.Lang = LANG.TP.JP;
        this.Coin = 100000;
        this.Diamond = 0;
        this.rouletteTicket = 1; //* 1つ上げるのは、ボーナス感じ。

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
