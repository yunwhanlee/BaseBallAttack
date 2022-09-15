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

    public static List<string[]> BatList = new List<string[]>(){
        new string[]{"1.Normal", "ノーマル", "노멀"},
        new string[]{"2.Pink", "ピンク", "핑크"},
        new string[]{"3.Green", "緑", "초록"},
        new string[]{"4.Blue", "青", "파랑"},
        new string[]{"5.Iron", "鉄", "철"},
        new string[]{"6.Black", "黒", "검정"},
        new string[]{"7.StripeA", "ストライプA", "줄무늬A"},
        new string[]{"8.StripeB", "ストライプB", "줄무늬B"},
        new string[]{"9.StripeC", "ストライプC", "줄무늬C"},
        new string[]{"10.Cloth", "布", "천"},
        new string[]{"11.WoodStripe", "木ストライプ", "나무줄무늬"},
        new string[]{"12.GlowBlue", "輝く青色", "빛나는파랑"},
        new string[]{"13.GlowYellow", "輝く黄色", "빛나는노랑"},
        new string[]{"14.GlowRed", "輝く青赤色", "빛나는빨강"},
        new string[]{"15.Broom", "ホウキ", "빗자루"},
        new string[]{"16.CandyBar", "キャンディバー", "캔디바"},
        new string[]{"17.CandyCaneA", "キャンディケインA", "케인A"},
        new string[]{"18.CandyCaneB", "キャンディケインB", "케인B"},
        new string[]{"19.CandyCaneC", "キャンディケインC", "케인C"},
        new string[]{"20.CandyCaneD", "キャンディケインD", "케인D"},
        new string[]{"21.CandyCaneE", "キャンディケインE", "케인E"}, 
        new string[]{"22.Guitar", "ギター", "기타"}, 
    };

    //* PLAY SCENE


    //* 関数
    public static string getLanguageTxt(string name){
        List<string[]> tempList = null;
        string itemType = name.Split('_')[0];
        string itemName = name.Split('_')[1];
        Debug.Log("getLanguageTxt:: itemType=" + itemType + ", itemName=" + itemName);
        
        //* Type
        switch(itemType){
            case "Chara":
                tempList = CharaList;
                break;
            case "Bat":
                tempList = BatList;
                break;
            case "Skill":
                //TODO
                break;
        }

        //* Apply
        int idx = tempList.FindIndex(arr => itemName == arr[(int)TP.EN]);
        Debug.Log("idx= " + idx);
        return tempList[idx][(int)DM.ins.Language];
    }
}

