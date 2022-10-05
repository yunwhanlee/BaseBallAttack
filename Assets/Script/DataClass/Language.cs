using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LANG //* LANG
{
    public enum TP {EN, JP, KR}; //* TYPE
    public enum OBJNAME {NameTxt, ExplainTxt, HomeRunBonusTxt};
    public enum TXT {
        //* UI
        Level, Stage, Combo, Status, Back, Ready, Out, Strike, 
        BestScore, LevelUpPanel_Title, LevelUpPanel_Explain,
        DialogNoMoney, DialogNoSkill, 
        DialogUnlock2ndSkill_Title, DialogUnlock2ndSkill_Info,
        //* PSV
        Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, 
        ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage, Laser,
    };
    public const int NAME=0, EXPLAIN=1, HOMERUNBONUS=2;
    

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

    public static string[] DialogNoMoney = new string[]{"NO MONEY!", "お金足りない!", "코인 부족!"};
    public static string[] DialogNoSkill = new string[]{"NO SKILL!", "スキルが足りない!", "스킬 부족!"};
    public static string[] DialogUnlock2ndSkill_Title = new string[]{
        "-Unlock-", "解錠", "해금"
    };
    public static string[] DialogUnlock2ndSkill_Info = new string[]{
        "Active the second skill button in the game.",
        "2番目スキルボタンをアクティブする。",
        "두 번째 스킬 버튼을 액티브 한다."
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
    public static string[] DmgContent = new string[]{"Attack Damage +1","攻撃力 +1","공격력 +1"};
    public static string[] MultiShot = new string[]{DM.PSV.MultiShot.ToString(), "マルチ弾", "멀티샷"};
    public static string[] MultiContent = new string[]{"Extra ball on one side +1","横側へボール追加 +1","옆측에 볼 추가"};
    public static string[] Speed = new string[]{DM.PSV.Speed.ToString(), "速度", "속도"};
    public static string[] SpeedContent = new string[]{"Speed Up +35%","速度アップ +35%","속도 업 +35%"};
    public static string[] InstantKill = new string[]{DM.PSV.InstantKill.ToString(), "キル", "킬"};
    public static string[] InstantKillContent = new string[]{"Immediate destroy +2%","ブロックを即刻破壊 +2%","블록 즉시파괴 +2%"};
    public static string[] Critical = new string[]{DM.PSV.Critical.ToString(), "致命打", "치명타"};
    public static string[] CriticalContent = new string[]{"Critical Up +20%","致命打確率アップ +20%","치명타확률 업 +20%"};
    public static string[] Explosion = new string[]{DM.PSV.Explosion.ToString(), "爆発", "폭발"};
    public static string[] ExplosionContent = new string[]{"Damage within the range +20% (+range75%)","範囲内にダメージを上げる +20% (+範囲75%)",""};
    public static string[] ExpUp = new string[]{DM.PSV.ExpUp.ToString(), "経験値％UP", "경험치%UP"};
    public static string[] ExpUpContent = new string[]{"Experience +20%","経験値 +20%","경험치 +20%"};
    public static string[] ItemSpawn = new string[]{DM.PSV.ItemSpawn.ToString(), "アイテム生成UP", "아이탬생성UP"};
    public static string[] ItemSpawnContent = new string[]{"Item block spawn +5%","アイテムブロックの確率アップ +5%","아이탬 블록 확률 증가 +5%"};
    public static string[] VerticalMultiShot = new string[]{DM.PSV.VerticalMultiShot.ToString(), "マルチ弾(縦)", "멀티샷(세로)"};
    public static string[] VerticalMultiShotContent = new string[]{"extra ball on front side +1","前側へボール追加 +1","앞쪽에 볼 추가"};
    public static string[] CriticalDamage = new string[]{DM.PSV.CriticalDamage.ToString(), "致命打ダメージ", "치명타데미지"};
    public static string[] CriticalDamageContent = new string[]{"Critical Damage Up +50%","致命打ダメージアップ +50%","치명타데미지 +50%"};
    public static string[] Laser = new string[]{DM.PSV.Laser.ToString(), "レイザー", "레이저"};
    public static string[] LaserContent = new string[]{"Shot Laser on one side +1","レイザー攻撃追加 +1","레이저공격 추가 +1"};

    public static List<string[]> PsvInfoNameList = new List<string[]>(){
        new string[]{Dmg[(int)TP.EN], Dmg[(int)TP.JP], Dmg[(int)TP.KR]},
        new string[]{MultiShot[(int)TP.EN], MultiShot[(int)TP.JP], MultiShot[(int)TP.KR]},
        new string[]{Speed[(int)TP.EN], Speed[(int)TP.JP], Speed[(int)TP.KR]},
        new string[]{InstantKill[(int)TP.EN], InstantKill[(int)TP.JP], InstantKill[(int)TP.KR]},
        new string[]{Critical[(int)TP.EN], Critical[(int)TP.JP], Critical[(int)TP.KR]},
        new string[]{Explosion[(int)TP.EN], Explosion[(int)TP.JP], Explosion[(int)TP.KR]},
        new string[]{ExpUp[(int)TP.EN], ExpUp[(int)TP.JP], ExpUp[(int)TP.KR]},
        new string[]{ItemSpawn[(int)TP.EN], ItemSpawn[(int)TP.JP], ItemSpawn[(int)TP.KR]},
        new string[]{VerticalMultiShot[(int)TP.EN], VerticalMultiShot[(int)TP.JP], VerticalMultiShot[(int)TP.KR]},
        new string[]{CriticalDamage[(int)TP.EN], CriticalDamage[(int)TP.JP], CriticalDamage[(int)TP.KR]},
        new string[]{Laser[(int)TP.EN], Laser[(int)TP.JP], Laser[(int)TP.KR]}
    };

    public static List<string[]> PsvInfoExplainList = new List<string[]>(){
        new string[]{DmgContent[(int)TP.EN], DmgContent[(int)TP.JP], DmgContent[(int)TP.KR]},
        new string[]{MultiShot[(int)TP.EN], MultiShot[(int)TP.JP], MultiShot[(int)TP.KR]},
        new string[]{SpeedContent[(int)TP.EN], SpeedContent[(int)TP.JP], SpeedContent[(int)TP.KR]},
        new string[]{InstantKillContent[(int)TP.EN], InstantKillContent[(int)TP.JP], InstantKillContent[(int)TP.KR]},
        new string[]{CriticalContent[(int)TP.EN], CriticalContent[(int)TP.JP], CriticalContent[(int)TP.KR]},
        new string[]{ExplosionContent[(int)TP.EN], ExplosionContent[(int)TP.JP], ExplosionContent[(int)TP.KR]},
        new string[]{ExpUpContent[(int)TP.EN], ExpUpContent[(int)TP.JP], ExpUpContent[(int)TP.KR]},
        new string[]{ItemSpawnContent[(int)TP.EN], ItemSpawnContent[(int)TP.JP], ItemSpawnContent[(int)TP.KR]},
        new string[]{VerticalMultiShotContent[(int)TP.EN], VerticalMultiShotContent[(int)TP.JP], VerticalMultiShotContent[(int)TP.KR]},
        new string[]{CriticalDamageContent[(int)TP.EN], CriticalDamageContent[(int)TP.JP], CriticalDamageContent[(int)TP.KR]},
        new string[]{LaserContent[(int)TP.EN], LaserContent[(int)TP.JP], LaserContent[(int)TP.KR]},
    };

    //* 関数
    public static List<string> getTxtList(string str){
        // Debug.Log("getLangTxt:: str=" + str);
        const int SPT_TYPE=0, SPT_NAME=1, SPT_EXPLAIN=2, SPT_HOMERUNBONUS=3;
        List<string> resList = new List<string>();

        //* Split String
        var split = str.Split('_');
        string type = split[SPT_TYPE];

        if(type == DM.HOME.Chara.ToString() || type == DM.HOME.Bat.ToString()){
            resList.Add(split[SPT_NAME]);
        }
        else if(type == DM.HOME.Skill.ToString()){
            resList.Add(split[SPT_NAME]);
            resList.Add(split[SPT_EXPLAIN]);
            resList.Add(split[SPT_HOMERUNBONUS]);
        }
        else if(type == DM.HOME.CashShop.ToString() || type == DM.HOME.PsvInfo.ToString() ){
            resList.Add(split[SPT_NAME]);
            resList.Add(split[SPT_EXPLAIN]);
        }
        
        //* Type & Set Lang
        switch(type){
            case "Chara":{
                int idx = CharaList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                resList[NAME] = CharaList[idx][(int)DM.ins.personalData.Lang];
                break;
            }
            case "Bat":{
                int idx = BatList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                resList[NAME] = BatList[idx][(int)DM.ins.personalData.Lang];
                break;
            }
            case "Skill":{
                int idx = SkillNameList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                // Debug.Log("<color=white><<Skill>> idx= " + idx + "</color>");
                resList[NAME] = SkillNameList[idx][(int)DM.ins.personalData.Lang];
                resList[EXPLAIN] = SkillExplainList[idx][(int)DM.ins.personalData.Lang];
                resList[HOMERUNBONUS] = SkillHomeRunBonusList[idx][(int)DM.ins.personalData.Lang];
                break;
            }
            case "CashShop":{
                int idx = CashShopNameList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                // Debug.Log("<color=yellow><<CashShop>> idx= " + idx + "</color>");
                resList[NAME] = CashShopNameList[idx][(int)DM.ins.personalData.Lang];
                resList[EXPLAIN] = CashShopExplainList[idx][(int)DM.ins.personalData.Lang];
                break;
            }
            case "PsvInfo":{
                int idx = PsvInfoNameList.FindIndex(langArr => resList[NAME] == langArr[(int)TP.EN]);
                // Debug.Log("<color=yellow><<PsvInfo>> idx= " + idx + "</color>");
                resList[NAME] = PsvInfoNameList[idx][(int)DM.ins.personalData.Lang];
                resList[EXPLAIN] = PsvInfoExplainList[idx][(int)DM.ins.personalData.Lang];
                break;
            }
        }
        return resList;
    }

    public static string getTxt(string name){
        // Debug.Log("getLangTxtListAtPlay:: name=" + name);
        string res = null;
        //* UI
        if(name == TXT.Level.ToString()) res = Level[(int)DM.ins.personalData.Lang];
        if(name == TXT.Stage.ToString()) res = Stage[(int)DM.ins.personalData.Lang];
        if(name == TXT.Combo.ToString()) res = Combo[(int)DM.ins.personalData.Lang];
        if(name == TXT.Status.ToString()) res = Status[(int)DM.ins.personalData.Lang];
        if(name == TXT.Back.ToString()) res = Back[(int)DM.ins.personalData.Lang];
        if(name == TXT.Ready.ToString()) res = Ready[(int)DM.ins.personalData.Lang];
        if(name == TXT.Out.ToString()) res = Out[(int)DM.ins.personalData.Lang];
        if(name == TXT.Strike.ToString()) res = Strike[(int)DM.ins.personalData.Lang];
        if(name == TXT.BestScore.ToString()) res = BestScore[(int)DM.ins.personalData.Lang];
        if(name == TXT.LevelUpPanel_Title.ToString()) res = LevelUpPanel_Title[(int)DM.ins.personalData.Lang];
        if(name == TXT.LevelUpPanel_Explain.ToString()) res = LevelUpPanel_Explain[(int)DM.ins.personalData.Lang];
        if(name == TXT.DialogNoMoney.ToString()) res = DialogNoMoney[(int)DM.ins.personalData.Lang];
        if(name == TXT.DialogNoSkill.ToString()) res = DialogNoSkill[(int)DM.ins.personalData.Lang];
        if(name == TXT.DialogUnlock2ndSkill_Title.ToString()) res = DialogUnlock2ndSkill_Title[(int)DM.ins.personalData.Lang];
        if(name == TXT.DialogUnlock2ndSkill_Info.ToString()) res = DialogUnlock2ndSkill_Info[(int)DM.ins.personalData.Lang];

        //* PSV
        if(name == TXT.Dmg.ToString()) res = Dmg[(int)DM.ins.personalData.Lang];
        if(name == TXT.MultiShot.ToString()) res = MultiShot[(int)DM.ins.personalData.Lang];
        if(name == TXT.Speed.ToString()) res = Speed[(int)DM.ins.personalData.Lang];
        if(name == TXT.InstantKill.ToString()) res = InstantKill[(int)DM.ins.personalData.Lang];
        if(name == TXT.Critical.ToString()) res = Critical[(int)DM.ins.personalData.Lang];
        if(name == TXT.Explosion.ToString()) res = Explosion[(int)DM.ins.personalData.Lang];
        if(name == TXT.ExpUp.ToString()) res = ExpUp[(int)DM.ins.personalData.Lang];
        if(name == TXT.ItemSpawn.ToString()) res = ItemSpawn[(int)DM.ins.personalData.Lang];
        if(name == TXT.VerticalMultiShot.ToString()) res = VerticalMultiShot[(int)DM.ins.personalData.Lang];
        if(name == TXT.CriticalDamage.ToString()) res = CriticalDamage[(int)DM.ins.personalData.Lang];
        if(name == TXT.Laser.ToString()) res = Laser[(int)DM.ins.personalData.Lang];

        //* ERROR
        if(res == null) Debug.LogError("存在しないTEXTです。");
        return res;
    }
}

