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
    
    //* 必ず、受け取るDataの名をこちに入れること！
    private List<string> keyList;  public List<string> KeyList {get => keyList; set => keyList = value;}
    
    //TODO Chara OnLock List

    //TODO Item OnLock List

    //* constructor
    public PersonalData(){
        KeyList = new List<string>();
    }

    //* method
    //TODO reset()

    public void load(){
        getKeyDtList(PlayerPrefs.GetString("Json"));
        processDt("load");
    }
    
    public void save(){
        processDt("save");
    }

    private void getKeyDtList(string json){
        //* JSON '{' と '}' 削除。
        int strLen = json.ToCharArray().Length-1;
        json = json.Substring(1, strLen-1);

        //* Key受け取る。
        int idxLen = json.Split(',').Length - 1;
        int i = 0;
        Array.ForEach(json.Split(','), arr=>{
            int j = 0;
            Array.ForEach(arr.Split(':'), data=>{
                if(j == 0)  KeyList.Add(data);
                j++;
            });
            i++;
        });
        KeyList.ForEach(key => Debug.Log("key= " + key));
    }

    private void processDt(string type){
        switch(type){
            case "load" : {
                Debug.Log("---------LOAD---------");
                //* Data
                this.Coin = PlayerPrefs.GetInt(keyList[0]);
                this.Diamond = PlayerPrefs.GetInt(keyList[1]);
                this.SelectCharaIdx = PlayerPrefs.GetInt(keyList[2]);
                //* Check Json
                Debug.Log("PersonalData:: Json= " + JsonUtility.ToJson(this));
                break;
            }
            case "save" : {
                Debug.Log("---------SAVE---------");
                //* Data
                PlayerPrefs.SetInt(keyList[0], this.Coin);
                PlayerPrefs.SetInt(keyList[1], this.Diamond);
                PlayerPrefs.SetInt(keyList[2], this.SelectCharaIdx);
                //* Serialize To Json
                PlayerPrefs.SetString("Json", JsonUtility.ToJson(this));
                break;
            }
        }
    }
}
