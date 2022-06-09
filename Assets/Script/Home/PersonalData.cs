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
    [SerializeField] int selectSkillIdx;  public int SelectSkillIdx {get => selectSkillIdx; set => selectSkillIdx = value;}
    [SerializeField] List<bool> skillLockList;  public List<bool> SkillLockList {get => skillLockList; set => skillLockList = value;}
    
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
        this.SkillLockList = data.SkillLockList;

        //* Set Real Content Items IsLock
        for(int i=0; i<charas.Length; i++){
            Debug.Log("CharaLockList["+i+"].IsLock=" + this.CharaLockList[i] + ", length= " + charas.Length);
            charas[i].GetComponent<ItemInfo>().IsLock = this.CharaLockList[i];
        }
        for(int i=0; i<bats.Length; i++){
            Debug.Log("BatLockList["+i+"].IsLock=" + this.BatLockList[i] + ", length= " + bats.Length);
            bats[i].GetComponent<ItemInfo>().IsLock = this.BatLockList[i];
        }
        for(int i=0; i<skills.Length; i++){
            Debug.Log("SkillLockList["+i+"].IsLock=" + this.SkillLockList[i] + ", length= " + skills.Length);
            skills[i].GetComponent<ItemInfo>().IsLock = this.SkillLockList[i];
        }
    }
    
    public void save(ref ItemInfo[] items){
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
        this.SkillLockList = new List<bool>();

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Chara].Prefs.Length; i++){
            if(i==0) this.CharaLockList.Add(false);//    items[0].IsLock = false;}
            else     this.CharaLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Bat].Prefs.Length; i++){
            if(i==0) this.BatLockList.Add(false);//    items[0].IsLock = false;}
            else     this.BatLockList.Add(true);//     items[i].IsLock = true;}
        }

        for(int i=0; i<DM.ins.scrollviews[(int)DM.ITEM.Skill].Prefs.Length; i++){
            this.SkillLockList.Add(true);
        }
    }
}
