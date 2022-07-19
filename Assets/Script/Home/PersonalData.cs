using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PersonalData {
    //* value
    [SerializeField] int coin; public int Coin {get => coin; set => coin = value;}
    [SerializeField] int diamond; public int Diamond {get => diamond; set => diamond = value;}
    [Header("--Charactor--")]
    [SerializeField] int selectCharaIdx;  public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    [SerializeField] List<bool> charaLockList;  public List<bool> CharaLockList {get => charaLockList; set => charaLockList = value;}
    [Header("--Bat--")]
    [SerializeField] int selectBatIdx;  public int SelectBatIdx {get => selectBatIdx; set => selectBatIdx = value;}
    [SerializeField] List<bool> batLockList;  public List<bool> BatLockList {get => batLockList; set => batLockList = value;}
    [Header("--Skill--")]
    [SerializeField] bool isUnlock2ndSkill;  public bool IsUnlock2ndSkill {get => isUnlock2ndSkill; set => isUnlock2ndSkill = value;}
    [SerializeField] int selectSkillIdx;  public int SelectSkillIdx {get => selectSkillIdx; set => selectSkillIdx = value;}
    [SerializeField] int selectSkill2Idx;  public int SelectSkill2Idx {get => selectSkill2Idx; set => selectSkill2Idx = value;}
    [SerializeField] List<bool> skillLockList;  public List<bool> SkillLockList {get => skillLockList; set => skillLockList = value;}
    [Header("--Item Passive--")]
    [SerializeField] ItemPassiveList itemPassive; public ItemPassiveList ItemPassive {get => itemPassive; set => itemPassive = value;}
    
    //* PlayerPrefs キー リスト => privateは jsonには追加しない。
    private List<string> keyList;  public List<string> KeyList {get => keyList; set => keyList = value;}

    //TODO Item OnLock List

    //* constructor
    public PersonalData(){  
        //* 初期化
        KeyList = new List<string>();
        charaLockList = new List<bool>();
        batLockList = new List<bool>();
        skillLockList = new List<bool>();
        ItemPassive = new ItemPassiveList();
    }

    //* method
    public void load(ref ItemInfo[] charas, ref ItemInfo[] bats, ref ItemInfo[] skills){
        Debug.Log("LOAD");
        //* Check Json
        string json = PlayerPrefs.GetString("Json");
        Debug.Log("JSON:: LOAD Data =" + json);

        //* Load Data
        var data = JsonUtility.FromJson<PersonalData>(json); //* Convert Json To Class Data

        //* Set Data
        this.Coin = data.Coin;
        this.Diamond = data.Diamond;

        this.SelectCharaIdx = data.SelectCharaIdx;
        this.CharaLockList = data.CharaLockList;

        this.SelectBatIdx = data.SelectBatIdx;
        this.BatLockList = data.BatLockList;

        this.SelectSkillIdx = data.SelectSkillIdx;
        this.IsUnlock2ndSkill = data.IsUnlock2ndSkill;
        if(this.IsUnlock2ndSkill) this.SelectSkill2Idx = data.SelectSkill2Idx;
        this.SkillLockList = data.SkillLockList;

        // this.ItemPassive = data.ItemPassive;

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
        Debug.Log("SAVE");
        PlayerPrefs.SetString("Json", JsonUtility.ToJson(this, true)); //* Serialize To Json

        //* Print
        string json = PlayerPrefs.GetString("Json");
        Debug.Log("JSON:: SAVE Data =" + json);
    }

    public void reset(){
        Debug.Log("RESET");
        PlayerPrefs.DeleteAll();

        this.Coin = 0;
        this.Diamond = 0;

        this.SelectCharaIdx = 0;
        this.CharaLockList = new List<bool>();

        this.SelectBatIdx = 0;
        this.BatLockList = new List<bool>();

        this.SelectSkillIdx = 0;
        this.IsUnlock2ndSkill = false;
        this.SelectSkill2Idx = 0;
        this.SkillLockList = new List<bool>();

        // this.ItemPassive = new ItemPassiveList();

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Chara].ItemPrefs.Length; i++){
            if(i==0) this.CharaLockList.Add(false);//    items[0].IsLock = false;}
            else     this.CharaLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Bat].ItemPrefs.Length; i++){
            if(i==0) this.BatLockList.Add(false);//    items[0].IsLock = false;}
            else     this.BatLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Skill].ItemPrefs.Length; i++){
            if(i==0) this.SkillLockList.Add(false);//    items[0].IsLock = false;}
            else     this.SkillLockList.Add(true);//     items[i].IsLock = true;}
        }

    }

    public int getSelectIdx(string type){
        return (type == "Chara")? SelectCharaIdx
            :(type == "Bat")? SelectBatIdx
            :(type == "Skill")? SelectSkillIdx :-1;
    }

    public void setSelectIdx(string type, int index){
        switch(type){
            case "Chara" :  SelectCharaIdx = index; break;
            case "Bat" :    SelectBatIdx = index;   break;
            case "Skill" :  SelectSkillIdx = index; break;
        }
    }

    public void setUnLockCurList(string type, int curIdx){
        switch(type){
            case "Chara" :  CharaLockList[curIdx] = false; break;
            case "Bat" :    BatLockList[curIdx] = false;   break;
            case "Skill" :  SkillLockList[curIdx] = false; break;
        }
    }
}
