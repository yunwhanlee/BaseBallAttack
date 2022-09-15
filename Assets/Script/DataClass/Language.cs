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

    public static List<string[]> SkillNameList = new List<string[]>(){
        new string[]{"ThunderShot", "サンダーショット", "썬더샷"},
        new string[]{"FireBall", "ファイアボール", "파이어볼"},
        new string[]{"ColorBall", "カラーボール", "칼라볼"},
        new string[]{"PoisonSmoke", "毒煙", "독구름"},
        new string[]{"IceWave", "アイスウェーブ", "아이스웨이브"},
    };
    public static List<string[]> SkillExplainList = new List<string[]>(){
        new string[]{"Erase same color blocks.", 
            "同じ色のブロックを消す。", 
            "같은색 블록 제거."},
        new string[]{"Damage within the explosion range.", 
            "爆発範囲内にダメージを与える。", 
            "폭발범위내 데미지를 준다."},
        new string[]{"Damage within the Ice sector range.", 
            "アイスセクター範囲内にダメージを与える。", 
            "아이스섹터범위내에 데미지를 준다."},
        new string[]{"Dot Damage within the round range. (3 turn) ", 
            "該当範囲内にドットダメージを与える。(３ターン)", 
            "해당범위내에 도트데미지를 준다."},
        new string[]{"5 times x2 damage within line range.", 
            "ライン範囲内に5回、2倍ダメージを与える。", 
            "라인범위내에 5번, 2배 데미지를 준다."},
    };

    const string HR_TXT = "⊡ HomeRunBonus : ";
    public static List<string[]> SkillHomeRunBonusList = new List<string[]>(){
        new string[]{HR_TXT + "TODO",
        HR_TXT + "TODO",
        HR_TXT + "TODO"},
        new string[]{HR_TXT + "Burn Dot Dmg 10%",
        HR_TXT + "10%ダメージで燃やす。",
        HR_TXT + "10퍼센트 데미지로 불태운다."},
        new string[]{HR_TXT + "TODO",
        HR_TXT + "TODO",
        HR_TXT + "TODO"},
        new string[]{HR_TXT + "+20% range, +2 turn",
        HR_TXT + "+20% 範囲、+2ターン。",
        HR_TXT + "+20% 범위, +2 턴."},
        new string[]{HR_TXT + "x3 damage.",
        HR_TXT + "３倍 ダメージ。",
        HR_TXT + "3배 데미지."},
    };

    //* PLAY SCENE


    //* 関数
    public static List<string> getLanguageTxtList(string str){
        Debug.Log("getLanguageTxt:: str=" + str);
        const int SPT_TYPE=0, SPT_NAME=1, SPT_EXPLAIN=2, SPT_HOMERUNBONUS=3;
        const int NAME=0, EXPLAIN=1, HOMERUNBONUS=2;
        List<string> resultList = new List<string>();

        //* Split String
        var split = str.Split('_');
        string type = split[SPT_TYPE];

        if(type == DM.ITEM.Chara.ToString() || type == DM.ITEM.Bat.ToString()){
            resultList.Add(split[SPT_NAME]);
        }
        else if(type == DM.ITEM.Skill.ToString()){
            resultList.Add(split[SPT_NAME]);
            resultList.Add(split[SPT_EXPLAIN]);
            resultList.Add(split[SPT_HOMERUNBONUS]);
        }
        
        //* Type & Set Language
        switch(type){
            case "Chara":{
                int idx = CharaList.FindIndex(langArr => resultList[NAME] == langArr[(int)TP.EN]);
                resultList[NAME] = CharaList[idx][(int)DM.ins.Language];
                break;
            }
            case "Bat":{
                int idx = BatList.FindIndex(langArr => resultList[NAME] == langArr[(int)TP.EN]);
                resultList[NAME] = BatList[idx][(int)DM.ins.Language];
                break;
            }
            case "Skill":{
                int idx = SkillNameList.FindIndex(langArr => resultList[NAME] == langArr[(int)TP.EN]);
                Debug.Log("<color=white><<Skill>> idx= " + idx + "</color>");
                resultList[NAME] = SkillNameList[idx][(int)DM.ins.Language];
                resultList[EXPLAIN] = SkillExplainList[idx][(int)DM.ins.Language];
                resultList[HOMERUNBONUS] = SkillHomeRunBonusList[idx][(int)DM.ins.Language];
                break;
            }
        }
        return resultList;
    }
}

