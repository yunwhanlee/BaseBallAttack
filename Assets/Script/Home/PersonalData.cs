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
    public PersonalData(GameObject[] charaPfs){
        //* 初期化
        KeyList = new List<string>();

        charaLockList = new List<bool>();
        Array.ForEach(charaPfs, chara => charaLockList.Add(chara.GetComponent<CharactorInfo>().IsLock));

        // int i=0;
        // CharaLockList.ForEach(charaLock => {Debug.Log("chara["+(i++)+"].IsLock= " + charaLock);});
    }

    //* method
    //TODO reset()

    public void load(){
        // PlayerPrefs.DeleteAll();

        // getKeyDtList(PlayerPrefs.GetString("Json"));
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
    }

    private void processDt(string type){
        switch(type){
            case "load" : {
                Debug.Log("LOAD");
                //* Check Json
                string json = PlayerPrefs.GetString("Json");
                Debug.Log("PersonalData:: Load Data =" + json);

                //* Load Data
                var data = JsonUtility.FromJson<PersonalData>(json); //* Convert Json To Class Data
                //* Set Data
                this.Coin = data.Coin;
                this.Diamond = data.Diamond;
                this.SelectCharaIdx = data.SelectCharaIdx;
                this.charaLockList = data.CharaLockList;

                break;
            }
            case "save" : {
                Debug.Log("SAVE");
                PlayerPrefs.SetString("Json", JsonUtility.ToJson(this, true)); //* Serialize To Json
                break;
            }
        }
    }


}
