using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PersonalData {
    //* value
    [SerializeField] int coin; public int Coin {get => coin; set => coin = value;}
    [SerializeField] int diamond; public int Diamond {get => diamond; set => diamond = value;}
    [SerializeField] int selectCharaIdx;  public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    [SerializeField] List<bool> charaLockList;  public List<bool> CharaLockList {get => charaLockList; set => charaLockList = value;}
    
    //* PlayerPrefs キー リスト => privateは jsonには追加しない。
    private List<string> keyList;  public List<string> KeyList {get => keyList; set => keyList = value;}

    //TODO Item OnLock List

    //* constructor
    public PersonalData(ref GameObject[] charaPfs){
        //* 初期化
        KeyList = new List<string>();
        charaLockList = new List<bool>();
        
        load(ref charaPfs);
    }

    //* method
    public void load(ref GameObject[] charaPfs){
        Debug.Log("LOAD");
        //* Check Json
        string json = PlayerPrefs.GetString("Json");
        Debug.Log("PersonalData:: LOAD Data =" + json + (json == ""));

        if(json == ""){
            this.Coin = 222;
            this.Diamond = 0;
            this.SelectCharaIdx = 0;
            for(int i=0; i<charaPfs.Length; i++){
                if(i==0) this.CharaLockList.Add(true);
                else     this.CharaLockList.Add(false);
            }
            return;
        }

        //* Load Data
        var data = JsonUtility.FromJson<PersonalData>(json); //* Convert Json To Class Data
        //* Set Data
        this.Coin = data.Coin;
        this.Diamond = data.Diamond;
        this.SelectCharaIdx = data.SelectCharaIdx;
        this.CharaLockList = data.CharaLockList;

        //* Set Charactor Prefab IsLock
        for(int i=0; i<charaPfs.Length; i++){
            charaPfs[i].GetComponent<CharactorInfo>().IsLock = this.CharaLockList[i];
        }
    }
    
    public void save(ref CharactorInfo[] charaContents){
        Debug.Log("SAVE");

        for(int i=0; i<charaContents.Length; i++){
            CharaLockList[i] = charaContents[i].IsLock;
        }

        PlayerPrefs.SetString("Json", JsonUtility.ToJson(this, true)); //* Serialize To Json
        string json = PlayerPrefs.GetString("Json");
        Debug.Log("PersonalData:: SAVE Data =" + json);
    }

    public void reset(){
        PlayerPrefs.DeleteAll();
    }
    


    // private void getKeyDtList(string json){
    //     //* JSON '{' と '}' 削除。
    //     int strLen = json.ToCharArray().Length-1;
    //     json = json.Substring(1, strLen-1);

    //     //* Key受け取る。
    //     int idxLen = json.Split(',').Length - 1;
    //     int i = 0;
    //     Array.ForEach(json.Split(','), arr=>{
    //         int j = 0;
    //         Array.ForEach(arr.Split(':'), data=>{
    //             if(j == 0)  KeyList.Add(data);
    //             j++;
    //         });
    //         i++;
    //     });
    // }
}
