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
    
    //* PlayerPrefs キー リスト => privateは jsonには追加しない。
    private List<string> keyList;  public List<string> KeyList {get => keyList; set => keyList = value;}

    //TODO Item OnLock List

    //* constructor
    public PersonalData(){
        //* 初期化
        KeyList = new List<string>();
        charaLockList = new List<bool>();
        batLockList = new List<bool>();
    }

    //* method
    public void load(){
        Debug.Log("LOAD");
        //* Check Json
        string json = PlayerPrefs.GetString("Json");
        

        //* Load Data
        var data = JsonUtility.FromJson<PersonalData>(json); //* Convert Json To Class Data

        //* Set Data
        this.Coin = data.Coin;
        this.Diamond = data.Diamond;
        this.SelectCharaIdx = data.SelectCharaIdx;
        this.CharaLockList = data.CharaLockList;
        this.SelectBatIdx = data.SelectBatIdx;
        this.BatLockList = data.BatLockList;

        //* Set Item Prefabs.IsLock
        // for(int i=0; i<items.Length; i++){
        //     items[i].GetComponent<ItemInfo>().IsLock = this.CharaLockList[i];
        // }
    }
    
    public void save(ref ItemInfo[] items){
        Debug.Log("SAVE");
        for(int i=0; i<items.Length; i++){
            CharaLockList[i] = items[i].IsLock;
        }

        PlayerPrefs.SetString("Json", JsonUtility.ToJson(this, true)); //* Serialize To Json

        //* Print
        string json = PlayerPrefs.GetString("Json");
        Debug.Log("PersonalData:: SAVE Data =" + json);
    }

    public void reset(ref ItemInfo[] items){
        Debug.Log("RESET");
        PlayerPrefs.DeleteAll();

        this.Coin = 0;
        this.Diamond = 0;
        this.SelectCharaIdx = 0;
        this.CharaLockList = new List<bool>();
        for(int i=0; i<items.Length; i++){
            if(i==0) {this.CharaLockList.Add(false);    items[0].IsLock = false;}
            else     {this.CharaLockList.Add(true);     items[i].IsLock = true;}
        }
    }
}
