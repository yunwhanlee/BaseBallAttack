using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LANG //* LANGUAGE
{
    public enum TP {EN, JP, KR}; //* TYPE
    public const int NAME=0, EXPLAIN=1, HOMERUNBONUS=2;
    const int LEN = 3;

    //* HOME SCENE -----------------------------------
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

    public static List<string[]> CashShopNameList = new List<string[]>(){
        new string[]{"AD Skip", "広告無し", "광고제거"},
        new string[]{"Coin 10000", "コイン 10000", "코인 10000"},
        new string[]{"Coin 50000", "コイン 50000", "코인 50000"},
        new string[]{"Coin 100000", "コイン 100000", "코인 100000"},
        new string[]{"Diamond 10000", "ダイア 10000", "다이아 10000"},
        new string[]{"Diamond 50000", "ダイア 50000", "다이아 50000"},
        new string[]{"Diamond 100000", "ダイア 100000", "다이아 100000"},
    };
    public static List<string[]> CashShopExplainList = new List<string[]>(){
        new string[]{"Skip All Advertising.", "広告をスキップする。", "광고를 스킵한다."},
        new string[]{"Get Coin 10000.", "10000コインを購入。", "10000코인 구입."},
        new string[]{"Get Coin 50000.", "50000コインを購入。", "50000코인 구입."},
        new string[]{"Get Coin 100000.", "100000コインを購入。", "100000코인 구입."},
        new string[]{"Get Diamond 10000.", "10000ダイアを購入。", "10000다이아 구입."},
        new string[]{"Get Diamond 50000.", "50000ダイアを購入。", "50000다이아 구입."},
        new string[]{"Get Diamond 100000.", "100000ダイアを購入。", "100000다이아 구입."},
    };

    //* PLAY SCENE -----------------------------------
    //* UI
    public static string[] Level = new string[]{"LV", "レベル", "레벨"};
    public static string[] Stage = new string[]{"STAGE", "ステージ", "스테이지"};
    public static string[] Combo = new string[]{"COMBO", "コンボー", "콤보"};
    public static string[] Status = new string[]{"STATUS", "状　態", "상　태"};
    public static string[] Back = new string[]{"BACK", "戻す", "뒤로"};
    public static string[] Ready = new string[]{"READY", "準備", "준비"};
    public static string[] Out = new string[]{"OUT", "アウト", "아웃"};
    public static string[] Strike = new string[]{"STRIKE", "ストライク", "스트라이크"};
    public static string[] BestScore = new string[]{"BEST SCORE", "ベストスコア", "베스트 점수"};
    public static string[] LevelUpPanel_Title = new string[]{"LEVEL UP!", "レベル UP!", "레벨 업!"};
    public static string[] LevelUpPanel_Explain = new string[]{"Please, Select Skill", "スキルを選択してください", "스킬을 선택해주세요"};
    //* PSV
    public static string[] Dmg = new string[]{DM.PSV.Dmg.ToString(), "攻撃力", "공격력"};
    public static string[] MultiShot = new string[]{DM.PSV.MultiShot.ToString(), "マルチ弾", "멀티샷"};
    public static string[] Speed = new string[]{DM.PSV.Speed.ToString(), "速度", "속도"};
    public static string[] InstantKill = new string[]{DM.PSV.InstantKill.ToString(), "キル", "킬"};
    public static string[] Critical = new string[]{DM.PSV.Critical.ToString(), "致命打", "치명타"};
    public static string[] Explosion = new string[]{DM.PSV.Explosion.ToString(), "爆発", "폭발"};
    public static string[] ExpUp = new string[]{DM.PSV.ExpUp.ToString(), "経験値％UP", "경험치%UP"};
    public static string[] ItemSpawn = new string[]{DM.PSV.ItemSpawn.ToString(), "アイテム生成UP", "아이탬생성UP"};
    public static string[] VerticalMultiShot = new string[]{DM.PSV.VerticalMultiShot.ToString(), "マルチ弾(縦)", "멀티샷(세로)"};
    public static string[] CriticalDamage = new string[]{DM.PSV.CriticalDamage.ToString(), "致命打ダメージ", "치명타데미지"};
    public static string[] Laser = new string[]{DM.PSV.Laser.ToString(), "レイザー", "레이저"};

    //* LevelScrollPanel.cs

    //* Contructor

    //* 関数
    public static List<string> getTxtListAtHomeScn(string str){
        Debug.Log("getLanguageTxt:: str=" + str);
        const int SPT_TYPE=0, SPT_NAME=1, SPT_EXPLAIN=2, SPT_HOMERUNBONUS=3;
        List<string> resList = new List<string>();

        //* Split String
        var split = str.Split('_');
        string type = split[SPT_TYPE];

        if(type == DM.ITEM.Chara.ToString() || type == DM.ITEM.Bat.ToString()){
            resList.Add(split[SPT_NAME]);
        }
        else if(type == DM.ITEM.Skill.ToString()){
            resList.Add(split[SPT_NAME]);
            resList.Add(split[SPT_EXPLAIN]);
            resList.Add(split[SPT_HOMERUNBONUS]);
        }
        else if(type == DM.ITEM.CashShop.ToString()){
            resList.Add(split[SPT_NAME]);
            resList.Add(split[SPT_EXPLAIN]);
        }
        
        //* Type & Set Language
        switch(type){
            case "Chara":{
                int idx = CharaList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                resList[NAME] = CharaList[idx][(int)DM.ins.Language];
                break;
            }
            case "Bat":{
                int idx = BatList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                resList[NAME] = BatList[idx][(int)DM.ins.Language];
                break;
            }
            case "Skill":{
                int idx = SkillNameList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                Debug.Log("<color=white><<Skill>> idx= " + idx + "</color>");
                resList[NAME] = SkillNameList[idx][(int)DM.ins.Language];
                resList[EXPLAIN] = SkillExplainList[idx][(int)DM.ins.Language];
                resList[HOMERUNBONUS] = SkillHomeRunBonusList[idx][(int)DM.ins.Language];
                break;
            }
            case "CashShop":{
                int idx = CashShopNameList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                Debug.Log("<color=yellow><<CashShop>> idx= " + idx + "</color>");
                resList[NAME] = CashShopNameList[idx][(int)DM.ins.Language];
                resList[EXPLAIN] = CashShopExplainList[idx][(int)DM.ins.Language];
                break;
            }
        }
        return resList;
    }

    public static string getTxtAtPlayScn(string name){
        Debug.Log("getLanguageTxtListAtPlay:: name=" + name);
        string res = null;
        switch(name){
            //* UI
            case "Level" : res = Level[(int)DM.ins.Language];  break;
            case "Stage" : res = Stage[(int)DM.ins.Language];  break;
            case "Combo" : res = Combo[(int)DM.ins.Language];  break;
            case "Status" : res = Status[(int)DM.ins.Language];  break;
            case "Back" : res = Back[(int)DM.ins.Language];  break;
            case "Ready" : res = Ready[(int)DM.ins.Language];  break;
            case "Out" : res = Out[(int)DM.ins.Language];  break;
            case "Strike" : res = Strike[(int)DM.ins.Language];  break;
            case "BestScore" : res = BestScore[(int)DM.ins.Language];  break;
            case "LevelUpPanel_Title" : res = LevelUpPanel_Title[(int)DM.ins.Language]; break;
            case "LevelUpPanel_Explain" : res = LevelUpPanel_Explain[(int)DM.ins.Language]; break;
            //* PSV
            case "Dmg" : res = Dmg[(int)DM.ins.Language];  break;
            case "MultiShot" : res = MultiShot[(int)DM.ins.Language];  break;
            case "Speed" : res = Speed[(int)DM.ins.Language];  break;
            case "InstantKill" : res = InstantKill[(int)DM.ins.Language];  break;
            case "Critical" : res = Critical[(int)DM.ins.Language];  break;
            case "Explosion" : res = Explosion[(int)DM.ins.Language];  break;
            case "ExpUp" : res = ExpUp[(int)DM.ins.Language];  break;
            case "ItemSpawn" : res = ItemSpawn[(int)DM.ins.Language];  break;
            case "VerticalMultiShot" : res = VerticalMultiShot[(int)DM.ins.Language];  break;
            case "CriticalDamage" : res = CriticalDamage[(int)DM.ins.Language];  break;
            case "Laser" : res = Laser[(int)DM.ins.Language];  break;
        }

        if(res == null) Debug.LogError("存在しないTEXTです。");
        return res;
    }
}

