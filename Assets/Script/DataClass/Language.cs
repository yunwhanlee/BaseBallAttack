using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LANG //* LANGUAGE
{
    public enum TP {EN, JP, KR}; //* TYPE
    const int LEN = 3;

    //* HOME SCENE
    public static List<string[]> CharaList = new List<string[]>(){
        new string[]{"Normal", "ノーマル", "노멀"}, 
        new string[]{"Penguin", "ペンギン", "펭귄"}, 
        new string[]{"Bear", "クマ", "곰"}, 
        new string[]{"Boy1", "男１", "남자1"}, 
        new string[]{"Cat", "猫", "고양이"}, 
        new string[]{"Girl1", "女１", "여자1"}
    };
    

    //* PLAY SCENE


    //* 関数
    public static string getLanguageTxt(string name){
        List<string[]> tempList = null;
        string itemType = name.Split('_')[0];
        string itemName = name.Split('_')[1];
        Debug.Log("getLanguageTxt:: itemType=" + itemType + ", itemName=" + itemName);
        
        switch(itemType){
            case "Chara": 
                tempList = CharaList;
                break;
            case "Bat": 
                //TODO
                break;
            case "Skill": 
                //TODO
                break;
        }

        int idx = tempList.FindIndex(arr => itemName == arr[(int)TP.EN]);
        Debug.Log("idx= " + idx);
        return tempList[idx][(int)DM.ins.Language];
    }
}

