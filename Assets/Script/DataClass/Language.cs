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
        Ok, No, Use, Get, Open,
        Start, Level, Stage, BossLimitCnt, Combo, Status, Back, Ready, Out, Strike,
        BestScore, LevelUpPanel_Title, LevelUpPanel_Explain, Play,

        NotEnough, MsgNoSkill, MsgAlreadyRegistedSkill, MsgHardmodeLocked, 
        StageSelectNotice, SettingNotice,
        ADShowFail, PurchaseFail, OneDayUsingMaxCntOver,

        //* Dialog
        DialogUnlock2ndSkill_Title, DialogUnlock2ndSkill_Info,
        ShowAdDialogCoinX2_Title, ShowAdDialogCoinX2_Content,
        ShowAdDialogRerotateSkillSlots_Title, ShowAdDialogRerotateSkillSlots_Content,
        ShowAdDialogRevive_Title, ShowAdDialogRevive_Content,
        ShowAdDialogRouletteTicket_Title, ShowAdDialogRouletteTicket_Content,
        ErrorNetworkDialog_Title, ErrorNetworkDialog_Content,


        Tutorial, OpenTutorial_Content, Skip_NextTime,
        TutorialA_Title, TutorialA_Content,
        TutorialB_Title, TutorialB_Content,
        TutorialC_Title, TutorialC_Content,
        TutorialD_Title, TutorialD_Content,
        TutorialE_Title, TutorialE_Content,
        TutorialF_Title, TutorialF_Content,
        TutorialG_Title, TutorialG_Content,

        Caution, Caution_Content, Caution_Notice,

        Rate, Later, RateDialog_Content1, RateDialog_Content2,
        Reward, GetRewardChestPanel_Content, Empty, Roulette, PsvSkillTicket, RouletteTicket, RouletteSpin, RouletteStop,
        FirstSkill, SecondSkill, PsvSkillInfo, AtvSkill, Character, Bat, Achivement, Upgrade, CashShop, 
        PremiumPack, Coin, Diamond, RemoveAllADs,
        Purchase_Complete,
        HardMode, HardMode_Content1, HardMode_Content2,

        //* PSV
        Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, 
        ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage, 
        Laser, FireProperty, IceProperty, ThunderProperty,

        //* Unique PSV
        DamageTwice, GiantBall, DarkOrb, GodBless, BirdFriend,

        //* Upgrade (Only Needed Language)
        BossDamage, CoinBonus, Defence,
    };
    public const int NAME=0, EXPLAIN=1, HOMERUNBONUS=2;
    const string HR_TXT = "⊡ HomeRunBonus : ";

    //* HOME SCENE -----------------------------------
    public static List<string[]> CharaList = new List<string[]>(){
        // new string[]{"Normal", "ノーマル", "노멀"}, 
        // new string[]{"Penguin", "ペンギン", "펭귄"}, 
        // new string[]{"Bear", "クマ", "곰"}, 
        // new string[]{"Boy1", "男１", "남자1"}, 
        // new string[]{"Cat", "猫", "고양이"}, 
        // new string[]{"Girl1", "女１", "여자1"},
        // new string[]{"MaidA", "メイドA", "메이드A"},
        // new string[]{"MaidB", "メイドB", "메이드B"},
        // new string[]{"MaidC", "メイドC", "메이드C"},
        // new string[]{"MaidD", "メイドD", "메이드D"},
        // new string[]{"PistolMan", "ピストルマン", "피스톨맨"},
        // new string[]{"Dinosaur", "恐竜", "공룡"}, 
    };
    public static List<string[]> BatList = new List<string[]>(){
        // new string[]{"1.Normal", "ノーマル", "노멀"},
        // new string[]{"2.Pink", "ピンク", "핑크"},
        // new string[]{"3.Green", "緑", "초록"},
        // new string[]{"4.Blue", "青", "파랑"},
        // new string[]{"5.Iron", "鉄", "철"},
        // new string[]{"6.Black", "黒", "검정"},
        // new string[]{"7.StripeA", "ストライプA", "줄무늬A"},
        // new string[]{"8.StripeB", "ストライプB", "줄무늬B"},
        // new string[]{"9.StripeC", "ストライプC", "줄무늬C"},
        // new string[]{"10.Cloth", "布", "천"},
        // new string[]{"11.WoodStripe", "木ストライプ", "나무줄무늬"},
        // new string[]{"12.GlowBlue", "輝く青色", "빛나는파랑"},
        // new string[]{"13.GlowYellow", "輝く黄色", "빛나는노랑"},
        // new string[]{"14.GlowRed", "輝く青赤色", "빛나는빨강"},
        // new string[]{"15.Broom", "ホウキ", "빗자루"},
        // new string[]{"16.CandyBar", "キャンディバー", "캔디바"},
        // new string[]{"17.CandyCaneA", "キャンディケインA", "케인A"},
        // new string[]{"18.CandyCaneB", "キャンディケインB", "케인B"},
        // new string[]{"19.CandyCaneC", "キャンディケインC", "케인C"},
        // new string[]{"20.CandyCaneD", "キャンディケインD", "케인D"},
        // new string[]{"21.CandyCaneE", "キャンディケインE", "케인E"}, 
        // new string[]{"22.Guitar", "ギター", "기타"}, 
        // new string[]{"23.Pepero", "ペペロ", "빼빼로"}, 
        // new string[]{"24.ThornBat", "刺バット", "가시배트"}, 
        // new string[]{"25.Wrench", "レンチ", "렌치"}, 
    };
    public static List<string[]> SkillNameList = new List<string[]>(){
        // new string[]{"ThunderShot", "サンダーショット", "썬더샷"},
        // new string[]{"FireBall", "ファイアボール", "파이어볼"},
        // new string[]{"ColorBall", "カラーボール", "칼라볼"},
        // new string[]{"PoisonSmoke", "毒煙", "독구름"},
        // new string[]{"IceWave", "アイスウェーブ", "아이스웨이브"},
    };
    public static List<string[]> SkillExplainList = new List<string[]>(){
        // new string[]{"Erase same color blocks.", 
        //     "同じ色のブロックを消す。", 
        //     "같은색 블록 제거."},
        // new string[]{"Damage within the explosion range.", 
        //     "爆発範囲内にダメージを与える。", 
        //     "폭발범위내 데미지를 준다."},
        // new string[]{"Damage within the Ice sector range.", 
        //     "アイスセクター範囲内にダメージを与える。", 
        //     "아이스섹터범위내에 데미지를 준다."},
        // new string[]{"Dot Damage within the round range. (3 turn) ", 
        //     "該当範囲内にドットダメージを与える。(３ターン)", 
        //     "해당범위내에 도트데미지를 준다."},
        // new string[]{"5 times x2 damage within line range.", 
        //     "ライン範囲内に5回、2倍ダメージを与える。", 
        //     "라인범위내에 5번, 2배 데미지를 준다."},
    };
    public static List<string[]> SkillHomeRunBonusList = new List<string[]>(){
        // new string[]{HR_TXT + "TODO",
        // HR_TXT + "TODO",
        // HR_TXT + "TODO"},
        // new string[]{HR_TXT + "Burn Dot Dmg 10%",
        // HR_TXT + "10%ダメージで燃やす。",
        // HR_TXT + "10퍼센트 데미지로 불태운다."},
        // new string[]{HR_TXT + "TODO",
        // HR_TXT + "TODO",
        // HR_TXT + "TODO"},
        // new string[]{HR_TXT + "+20% range, +2 turn",
        // HR_TXT + "+20% 範囲、+2ターン。",
        // HR_TXT + "+20% 범위, +2 턴."},
        // new string[]{HR_TXT + "x3 damage.",
        // HR_TXT + "３倍 ダメージ。",
        // HR_TXT + "3배 데미지."},
    };
    public static List<string[]> CashShopNameList = new List<string[]>(){
        // new string[]{"AD Skip", "広告無し", "광고제거"},
        // new string[]{"Coin 10000", "コイン 10000", "코인 10000"},
        // new string[]{"Coin 50000", "コイン 50000", "코인 50000"},
        // new string[]{"Coin 100000", "コイン 100000", "코인 100000"},
        // new string[]{"Diamond 10000", "ダイア 10000", "다이아 10000"},
        // new string[]{"Diamond 50000", "ダイア 50000", "다이아 50000"},
        // new string[]{"Diamond 100000", "ダイア 100000", "다이아 100000"},
    };
    public static List<string[]> CashShopExplainList = new List<string[]>(){
        // new string[]{"Skip All Advertising.", "広告をスキップする。", "광고를 스킵한다."},
        // new string[]{"Get Coin 10000.", "10000コインを購入。", "10000코인 구입."},
        // new string[]{"Get Coin 50000.", "50000コインを購入。", "50000코인 구입."},
        // new string[]{"Get Coin 100000.", "100000コインを購入。", "100000코인 구입."},
        // new string[]{"Get Diamond 10000.", "10000ダイアを購入。", "10000다이아 구입."},
        // new string[]{"Get Diamond 50000.", "50000ダイアを購入。", "50000다이아 구입."},
        // new string[]{"Get Diamond 100000.", "100000ダイアを購入。", "100000다이아 구입."},
    };

    //* NOTICE MESSAGE
    public static string[] NotEnough = new string[]{
        "Not Enough", "不足", "부족"
    };
    public static string[] MsgNoSkill = new string[]{
        "Pls purchase more than one skill!", "スキルを二つ以上購入してください！", "스킬을 두개 이상 구매해주세요!"
    };
    public static string[] MsgAlreadyRegistedSkill = new string[]{
        "This Skill is Already Registed",
        "このスキルは既に登録されています。",
        "해당 스킬은 이미 등록되어 있습니다."
    };
    public static string[] MsgHardmodeLocked = new string[]{
        "you have to clear previous step!",
        "以前段階をクリアーしてください!",
        "이전 단계를 클리어 해야됩니다!"
    };
    public static string[] StageSelectNotice = new string[]{
        "After Select the image, press the Play button.",
        "画像を選択したら、プレイボタンを押してください。",
        "이미지를 선택한 후 플레이 버튼을 눌러주세요."
    };
    public static string[] SettingNotice = new string[]{
        "If the gameplay is not smooth with frame drop, try lowering the Quality.",
        "フレームドロップでゲームプレイがスムーズでない場合は、クオリティーを下げてください。",
        "프레임 드롭으로 게임 플레이가 원활하지 않으면, 퀄리티를 낮춰보세요."
    };
    public static string[] ADShowFail = new string[]{
        "AD Failed to show.",
        "広告再生 失敗。",
        "광고재생 실패."
    };
    public static string [] PurchaseFail = new string[]{
        "Purchase Failed.",
        "購入 失敗。",
        "구입 실패."
    };
    public static string [] OneDayUsingMaxCntOver = new string[]{
        "1day Ad views exceeded!",
        "1일 광고 조회수 초과!",
        "1日の広告再生数 超過!"
    };

    //* DIALOG
    public static string[] DialogUnlock2ndSkill_Title = new string[]{
        "-Unlock-", "解錠", "해금"
    };
    public static string[] DialogUnlock2ndSkill_Info = new string[]{
        "One more skill button is added in the in-game. Would you like to purchase it?",
        "インゲームでスキルボタンがもう一つ追加されます。 購入しますか？",
        "인게임에서 스킬버튼이 하나 더 추가됩니다. 구매하시겠습니까?"
    };
    public static string[] ShowAdDialogCoinX2_Title = new string[]{
        "Coin X2", "コイン二倍", "코인 두배"
    };
    public static string[] ShowAdDialogCoinX2_Content = new string[]{
        "Would you like to double the coin?",
        "コインを二倍にもらいますか？",
        "코인을 두배로 받으시겠습니까?"
    };
    public static string[] ShowAdDialogRerotateSkillSlots_Title = new string[]{
        "Rerotate Slots", "スロット再回し", "슬롯 다시돌리기"
    };
    public static string[] ShowAdDialogRerotateSkillSlots_Content = new string[]{
        "Would you like to Rerotate Skill Slots?",
        "スロットを新しく回しますか？",
        "슬롯을 다시 돌리시겠습니까?"
    };
    public static string[] ShowAdDialogRevive_Title = new string[]{
        "Revive", "復活", "부활"
    };
    public static string[] ShowAdDialogRevive_Content = new string[]{
        "Would you like to revive and continue to play the game? (blocks would be initialized.)",
        "復活してゲームを続きますか? (ブロックは初期化されます。)",
        "부활하여 게임을 계속 진행하시겠습니까? (블록은 초기화됩니다.)"
    };
    public static string[] ShowAdDialogRouletteTicket_Title = new string[]{
        "Roulette Ticket", "ルーレットチケット", "룰렛티켓"
    };
    public static string[] ShowAdDialogRouletteTicket_Content = new string[]{
        "Not enough Roulette Ticket.\nWould you like to get a entrance of ticket?",
        "チケットが足りないです。チケットを習得しませんか？",
        "티켓이 모자랍니다. 티켓을 획득하시겠습니까?"
    };
    public static string[] ErrorNetworkDialog_Title = new string[]{
        "Network connection failed.",
        "ネットワーク接続に失敗しました。",
        "네트워크 연결에 실패했습니다."
    };
    public static string[] ErrorNetworkDialog_Content = new string[]{
        "Please check your Wi-Fi connection and retry.",
        "Wi-Fi接続を確認して再試行してください。",
        "Wi-Fi 연결을 확인하고 다시 시도해주세요."
    };
    public static string[] GetRewardChestPanel_Content = new string[]{
        "You get a mysterious treasure chest!",
        "ミステリーな宝箱を得りました!",
        "보물상자를 얻었습니다!"
    };
    public static string[] HardMode_Content1 = new string[]{
        "Congreturation! You can play Hardmode.",
        "おめでとうございます！ハードモードプレイができます。",
        "축하합니다! 하드모드 플레이가 가능합니다."
    };
    public static string[] HardMode_Content2 = new string[]{
        "Show people your power!",
        "皆にあなたのパワーを見せてください！",
        "모두에게 여러분의 힘을 보여주세요!"
    };
    public static string[] Caution = new string[]{
        "Caution",
        "警告",
        "경고"
    };
    public static string[] Caution_Content = new string[]{
        "Are you sure give up the game?",
        "本当にゲームをやめますか？",
        "정말로 게임을 포기하시겠습니까?"
    };
    public static string[] Caution_Notice = new string[]{
        "The played data will not be saved.",
        "プレイしたデータが保存されません。",
        "플레이한 데이터가 저장되지 않습니다."
    };

    //* Tutorial
    public static string[] OpenTutorial_Content = new string[]{
        "Would you like to watch Tutorial?",
        "チュートリアルを見ますか？",
        "튜토리얼을 보시겠습니까?"
    };
    public static string[] Skip_NextTime = new string[]{
        "Skip Next Time",
        "次からスキップする。",
        "다음부터 스킵하기."
    };
    public static string[] TutorialA_Title = new string[]{
        "1. Block", "1. ブロック", "1. 블록"
    };
    public static string[] TutorialA_Content = new string[]{
        "Every turn, blocks are created and approached down.",
        "毎ターン、ブロックが作成され、下に近づきます。",
        "매턴, 블록이 생성되며 아래로 내려옵니다."
    };
    public static string[] TutorialB_Title = new string[]{
        "2. Gameover", "2. ゲームオーバー", "2. 게임오버"
    };
    public static string[] TutorialB_Content = new string[]{
        "If a block reached to Red line, this is GameOver.",
        "レッドラインにブロックが触れると、ゲームオーバ。",
        "레드라인에 블록이 닿으면, 게임오버."
    };
    public static string[] TutorialC_Title = new string[]{
        "3. Ready (1)", "3. 準備 (1)", "3. 준비 (1)"
    };
    public static string[] TutorialC_Content = new string[]{
        "Drag the screen and control the direction of arrow.",
        "画面をドラッグし、矢印の方向を制御します。",
        "화면을 드래그하여 화살표 방향을 정합니다."
    };
    public static string[] TutorialD_Title = new string[]{
        "4. Ready (2)", "4. 準備 (2)", "4. 준비 (2)"
    };
    public static string[] TutorialD_Content = new string[]{
        "Click 'Ready' Button",
        "Readyボタンをクリックします。",
        "Ready버튼을 클릭합니다."
    };
    public static string[] TutorialE_Title = new string[]{
        "5. Shoot", "5. シュート", "5. 슛"
    };
    public static string[] TutorialE_Content = new string[]{
        "Hit the flying ball. (If it's the right timing, Home Run!)",
        "飛んでくるボールを打つ。（タイミングが合えばホームラン！）",
        "날아오는 공을 쳐라! (타이밍 잘 맞추면 홈런!)"
    };
    public static string[] TutorialF_Title = new string[]{
        "6. Move Position", "6. 位置移動", "6. 위치이동"
    };
    public static string[] TutorialF_Content = new string[]{
        "Touch your player and drag left or right. It would be helpful to avoid Boss Attack !",
        "プレーヤーをタッチし、左右にドラッグします。 BossAttackを避けるとき、助かります！",
        "플레이어를 터치하여, 좌우로 드래그합니다. 보스공격을 피할 때 도움이 됩니다!"
    };
    public static string[] TutorialG_Title = new string[]{
        "7. More Strong", "7.より強く", "7. 강해질꼬야"
    };
    public static string[] TutorialG_Content = new string[]{
        "Too weak?\nYou can get stronger with upgrades or characters, bats, and skills!",
        "弱いですか？\nアップグレードやキャラクター、バット、スキルで強くなれる！",
        "약하신가요?\n업그레이드 및 캐릭터, 베트, 스킬들로 더욱 강해지세요!"
    };
    public static string[] RateDialog_Content1 = new string[]{
        "Have you Enjoyed the game?",
        "ゲームはどうでしょうか？",
        "게임은 어떠신가요?"
    };
    public static string[] RateDialog_Content2 = new string[]{
        "please give us your valuable evaluation! It gives me huge strength!",
        "もしよろしければ、大切な評価をお願いします！ とても力になります！",
        "혹시 괜찮으시다면, 소중한 평가 부탁드립니다! 많은 힘이 됩니다!"
    };

    //* UI
    public static string[] Ok = new string[]{"OK", "はい", "네"};
    public static string[] No = new string[]{"No", "いいえ", "아니오"};
    public static string[] Use = new string[]{"USE", "使用", "사용"};
    public static string[] Get = new string[]{"GET", "取る", "얻기"};
    public static string[] Start = new string[]{"START", "スタート", "시작"};
    public static string[] Level = new string[]{"LV", "レベル", "레벨"};
    public static string[] Stage = new string[]{"STAGE", "ステージ", "스테이지"};
    public static string[] BossLimitCnt = new string[]{"BossLimitCnt", "制限時間", "제한시간"};
    public static string[] Combo = new string[]{"COMBO", "コンボ", "콤보"};
    public static string[] Status = new string[]{"STATUS", "状　態", "상　태"};
    public static string[] Back = new string[]{"BACK", "戻す", "뒤로"};
    public static string[] Ready = new string[]{"READY", "準備", "준비"};
    public static string[] Out = new string[]{"OUT", "アウト", "아웃"};
    public static string[] Strike = new string[]{"STRIKE", "ストライク", "스트라이크"};
    public static string[] BestScore = new string[]{"BEST STAGE", "ベストステージ", "베스트 스테이지"};
    public static string[] LevelUpPanel_Title = new string[]{"LEVEL UP!", "レベル UP!", "레벨 업!"};
    public static string[] LevelUpPanel_Explain = new string[]{"Please, Select Skill", "スキルを選択してください", "스킬을 선택해주세요"};
    public static string[] Reward = new string[]{"REWARD", "リワード", "보상"};
    public static string[] Open = new string[]{"Open", "開く", "열기"};
    public static string[] Empty = new string[]{"Empty", "空箱", "빈 상자"};
    public static string[] Roulette = new string[]{"Roulette", "ルーレット", "룰렛"};
    public static string[] PsvSkillTicket = new string[]{"Passive Skill ticket!", "パッシブスキル チケット!", "패시브 스킬 티켓!"};
    public static string[] RouletteTicket = new string[]{"Roulette ticket!", "ルーレット チケット!", "룰렛 티켓!"};
    public static string[] RouletteSpin = new string[]{"Spin!", "回す!", "돌리기!"};
    public static string[] RouletteStop = new string[]{"Stop!", "停止!", "정지!"};
    public static string[] FirstSkill = new string[]{"1st Skill", "一番目 スキル", "첫번째 스킬"};
    public static string[] SecondSkill = new string[]{"2nd Skill", "二番目 スキル", "두번째 스킬"};
    public static string[] PsvSkillInfo = new string[]{"Passive Skill info", "パッシブスキル情報", "패시브 스킬 정보"};
    public static string[] AtvSkill = new string[]{"Active Skill", "アクティブ スキル", "엑티브 스킬"};
    public static string[] Character = new string[]{"Character", "キャラクター", "캐릭터"};
    public static string[] Bat = new string[]{"Bat", "バット", "베트"};
    public static string[] Achivement = new string[]{"Achivement", "業績", "업적"};
    public static string[] Upgrade = new string[]{"Upgrade", "アップグレード", "업그레이드"};
    public static string[] CashShop = new string[]{"Cash Shop", "課金ショップ", "캐시샾"};
    public static string[] Tutorial = new string[]{"Tutorial", "チュートリアル", "튜토리얼"};
    public static string[] PremiumPack = new string[]{"Premium Package", "プレミアム パッケージ", "프리미엄 패키지"};
    public static string[] Coin = new string[]{"Coin", "コイン", "코인"};
    public static string[] Diamond = new string[]{"Diamond", "ダイア", "다이아"};
    public static string[] RemoveAllADs = new string[]{"Remove All ADs", "全ての広告無し", "광고제거"};
    public static string[] Rate = new string[]{"Rate", "評価", "평가"};
    public static string[] Later = new string[]{"Later", "後で", "나중에"};
    public static string[] Purchase_Complete = new string[]{"Purchase_Complete", "購入成功", "구매성공"};
    public static string[] HardMode = new string[]{"HardMode", "ハードモード", "하드모드"};
    public static string[] Play = new string[]{"Play", "プレイ", "플레이"};

    //* PSV
    public static string[] Dmg = new string[]{DM.PSV.Dmg.ToString(), "攻撃力", "공격력"};
    public static string[] MultiShot = new string[]{DM.PSV.MultiShot.ToString(), "マルチ弾", "멀티샷"};
    public static string[] Speed = new string[]{DM.PSV.Speed.ToString(), "速度", "속도"};
    public static string[] InstantKill = new string[]{DM.PSV.InstantKill.ToString(), "キル", "킬"};
    public static string[] Critical = new string[]{DM.PSV.Critical.ToString(), "致命打", "치명타"};
    public static string[] CriticalDamage = new string[]{DM.PSV.CriticalDamage.ToString(), "致命打ダメージ", "치명타데미지"};
    public static string[] Explosion = new string[]{DM.PSV.Explosion.ToString(), "爆発", "폭발"};
    public static string[] ExpUp = new string[]{DM.PSV.ExpUp.ToString(), "経験値％UP", "경험치%UP"};
    public static string[] ItemSpawn = new string[]{DM.PSV.ItemSpawn.ToString(), "アイテム生成UP", "아이탬생성UP"};
    public static string[] VerticalMultiShot = new string[]{DM.PSV.VerticalMultiShot.ToString(), "マルチ弾(縦)", "멀티샷(세로)"};
    public static string[] Laser = new string[]{DM.PSV.Laser.ToString(), "レイザー", "레이저"};
    public static string[] FireProperty = new string[]{DM.PSV.FireProperty.ToString(), "火属性", "불속성"};
    public static string[] IceProperty = new string[]{DM.PSV.IceProperty.ToString(), "氷属性", "빙속성"};
    public static string[] ThunderProperty = new string[]{DM.PSV.ThunderProperty.ToString(), "雷属性", "뇌속성"};
    public static string[] DamageTwice = new string[]{DM.PSV.DamageTwice.ToString(), "ダメージ200%", "데미지200%"};
    public static string[] GiantBall = new string[]{DM.PSV.GiantBall.ToString(), "ジャイアントボール", "자이언트볼"};
    public static string[] DarkOrb = new string[]{DM.PSV.DarkOrb.ToString(), "闇のオーブ", "어둠의오브"};
    public static string[] GodBless = new string[]{DM.PSV.GodBless.ToString(), "神の祝福", "신의축복"};
    public static string[] BirdFriend = new string[]{DM.PSV.BirdFriend.ToString(), "鳥友達", "조류친구"};
    //* Upgrade
    public static string[] BossDamage = new string[]{DM.UPGRADE.BossDamage.ToString(), "ボースダメージ", "보스데미지"};
    public static string[] CoinBonus = new string[]{DM.UPGRADE.CoinBonus.ToString(), "コインボーナス", "코인보너스"};
    public static string[] Defence = new string[]{DM.UPGRADE.Defence.ToString(), "ディフェンス", "디팬스"};

    public static List<string[]> PsvInfoNameList = new List<string[]>(){
        // new string[]{Dmg[(int)TP.EN], Dmg[(int)TP.JP], Dmg[(int)TP.KR]},
        // new string[]{MultiShot[(int)TP.EN], MultiShot[(int)TP.JP], MultiShot[(int)TP.KR]},
        // new string[]{Speed[(int)TP.EN], Speed[(int)TP.JP], Speed[(int)TP.KR]},
        // new string[]{InstantKill[(int)TP.EN], InstantKill[(int)TP.JP], InstantKill[(int)TP.KR]},
        // new string[]{Critical[(int)TP.EN], Critical[(int)TP.JP], Critical[(int)TP.KR]},
        // new string[]{Explosion[(int)TP.EN], Explosion[(int)TP.JP], Explosion[(int)TP.KR]},
        // new string[]{ExpUp[(int)TP.EN], ExpUp[(int)TP.JP], ExpUp[(int)TP.KR]},
        // new string[]{ItemSpawn[(int)TP.EN], ItemSpawn[(int)TP.JP], ItemSpawn[(int)TP.KR]},
        // new string[]{VerticalMultiShot[(int)TP.EN], VerticalMultiShot[(int)TP.JP], VerticalMultiShot[(int)TP.KR]},
        // new string[]{CriticalDamage[(int)TP.EN], CriticalDamage[(int)TP.JP], CriticalDamage[(int)TP.KR]},
        // new string[]{Laser[(int)TP.EN], Laser[(int)TP.JP], Laser[(int)TP.KR]}
    };

    public static List<string[]> PsvInfoExplainList = new List<string[]>(){
        // new string[]{DmgContent[(int)TP.EN], DmgContent[(int)TP.JP], DmgContent[(int)TP.KR]},
        // new string[]{MultiShot[(int)TP.EN], MultiShot[(int)TP.JP], MultiShot[(int)TP.KR]},
        // new string[]{SpeedContent[(int)TP.EN], SpeedContent[(int)TP.JP], SpeedContent[(int)TP.KR]},
        // new string[]{InstantKillContent[(int)TP.EN], InstantKillContent[(int)TP.JP], InstantKillContent[(int)TP.KR]},
        // new string[]{CriticalContent[(int)TP.EN], CriticalContent[(int)TP.JP], CriticalContent[(int)TP.KR]},
        // new string[]{ExplosionContent[(int)TP.EN], ExplosionContent[(int)TP.JP], ExplosionContent[(int)TP.KR]},
        // new string[]{ExpUpContent[(int)TP.EN], ExpUpContent[(int)TP.JP], ExpUpContent[(int)TP.KR]},
        // new string[]{ItemSpawnContent[(int)TP.EN], ItemSpawnContent[(int)TP.JP], ItemSpawnContent[(int)TP.KR]},
        // new string[]{VerticalMultiShotContent[(int)TP.EN], VerticalMultiShotContent[(int)TP.JP], VerticalMultiShotContent[(int)TP.KR]},
        // new string[]{CriticalDamageContent[(int)TP.EN], CriticalDamageContent[(int)TP.JP], CriticalDamageContent[(int)TP.KR]},
        // new string[]{LaserContent[(int)TP.EN], LaserContent[(int)TP.JP], LaserContent[(int)TP.KR]},
    };

    public static List<string[]> UpgradeNameList = new List<string[]>(){

    };

    public static List<string[]> UpgradeExplainList = new List<string[]>(){
        
    };

//*-----------------------------------------------------------------------------------------------
//* 関数
//*-----------------------------------------------------------------------------------------------
    public static void initlanguageList(){ //* (BUG) EX)言語とか変えた時に、リストが２倍になるため、初期化する。
        CharaList = new List<string[]>();
        BatList = new List<string[]>();
        SkillNameList = new List<string[]>();
        CashShopNameList = new List<string[]>();
        PsvInfoNameList = new List<string[]>();
        UpgradeNameList = new List<string[]>();
    }
    public static void checkErrorLangListCounting(){
        Debug.Log("checkErrorLangListCounting()::");
        int charaCttCnt = DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf.childCount;
        int batCttCnt = DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf.childCount;
        int skillCttCnt = DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf.childCount;
        int cashShopCttCnt = DM.ins.scrollviews[(int)DM.PANEL.CashShop].ContentTf.childCount;
        int psvInfoCttCnt = DM.ins.scrollviews[(int)DM.PANEL.PsvInfo].ContentTf.childCount;
        int upgradeCttCnt = DM.ins.scrollviews[(int)DM.PANEL.Upgrade].ContentTf.childCount;

        Debug.LogFormat("【Chara】Lang: {0}, ContentTf: {1} => {2}", CharaList.Count, charaCttCnt, (CharaList.Count == charaCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
        Debug.LogFormat("【Bat」Lang: {0}, ContentTf: {1} => {2}", BatList.Count, batCttCnt, (BatList.Count == batCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
        Debug.LogFormat("【SkillName】Lang: {0}, ContentTf: {1} => {2}", SkillNameList.Count, skillCttCnt, (SkillNameList.Count == skillCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
        Debug.LogFormat("【CashShop】Lang: {0}, ContentTf: {1} => {2}", CashShopNameList.Count, cashShopCttCnt, (CashShopNameList.Count == cashShopCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
        Debug.LogFormat("【PsvInfo】Lang: {0}, ContentTf: {1} => {2}", PsvInfoNameList.Count, psvInfoCttCnt, (PsvInfoNameList.Count == psvInfoCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
        Debug.LogFormat("【Upgrade】Lang: {0}, ContentTf: {1} => {2}", UpgradeNameList.Count, upgradeCttCnt, (UpgradeNameList.Count == upgradeCttCnt)? "OK" : "<color=red>ERROR! お互いに数字が合わない！</color>");
    }
    public static string getTxt(string name){
        // Debug.Log("getLangTxtListAtPlay:: name=" + name);
        string res = null;
        int CUR_LANG = (int)DM.ins.personalData.Lang;

        //* NOTICE MESSAGE
        if(name == TXT.NotEnough.ToString()) res = NotEnough[CUR_LANG];
        if(name == TXT.MsgNoSkill.ToString()) res = MsgNoSkill[CUR_LANG];
        if(name == TXT.MsgAlreadyRegistedSkill.ToString()) res = MsgAlreadyRegistedSkill[CUR_LANG];
        if(name == TXT.MsgHardmodeLocked.ToString()) res = MsgHardmodeLocked[CUR_LANG];
        if(name == TXT.StageSelectNotice.ToString()) res = StageSelectNotice[CUR_LANG];
        if(name == TXT.SettingNotice.ToString()) res = SettingNotice[CUR_LANG];
        if(name == TXT.ADShowFail.ToString()) res = ADShowFail[CUR_LANG];
        if(name == TXT.PurchaseFail.ToString()) res = PurchaseFail[CUR_LANG];
        if(name == TXT.OneDayUsingMaxCntOver.ToString()) res = OneDayUsingMaxCntOver[CUR_LANG];

        //* DIALOG
        if(name == TXT.DialogUnlock2ndSkill_Title.ToString()) res = DialogUnlock2ndSkill_Title[CUR_LANG];
        if(name == TXT.DialogUnlock2ndSkill_Info.ToString()) res = DialogUnlock2ndSkill_Info[CUR_LANG];
        if(name == TXT.ShowAdDialogCoinX2_Title.ToString()) res = ShowAdDialogCoinX2_Title[CUR_LANG];
        if(name == TXT.ShowAdDialogCoinX2_Content.ToString()) res = ShowAdDialogCoinX2_Content[CUR_LANG];
        if(name == TXT.ShowAdDialogRerotateSkillSlots_Title.ToString()) res = ShowAdDialogRerotateSkillSlots_Title[CUR_LANG];
        if(name == TXT.ShowAdDialogRerotateSkillSlots_Content.ToString()) res = ShowAdDialogRerotateSkillSlots_Content[CUR_LANG];
        if(name == TXT.ShowAdDialogRevive_Title.ToString()) res = ShowAdDialogRevive_Title[CUR_LANG];
        if(name == TXT.ShowAdDialogRevive_Content.ToString()) res = ShowAdDialogRevive_Content[CUR_LANG];
        if(name == TXT.ShowAdDialogRouletteTicket_Title.ToString()) res = ShowAdDialogRouletteTicket_Title[CUR_LANG];
        if(name == TXT.ShowAdDialogRouletteTicket_Content.ToString()) res = ShowAdDialogRouletteTicket_Content[CUR_LANG];
        if(name == TXT.ErrorNetworkDialog_Title.ToString()) res = ErrorNetworkDialog_Title[CUR_LANG];
        if(name == TXT.ErrorNetworkDialog_Content.ToString()) res = ErrorNetworkDialog_Content[CUR_LANG];
        if(name == TXT.RateDialog_Content1.ToString()) res = RateDialog_Content1[CUR_LANG];
        if(name == TXT.RateDialog_Content2.ToString()) res = RateDialog_Content2[CUR_LANG];
        if(name == TXT.HardMode_Content1.ToString()) res = HardMode_Content1[CUR_LANG];
        if(name == TXT.HardMode_Content2.ToString()) res = HardMode_Content2[CUR_LANG];

        if(name == TXT.Caution.ToString()) res = Caution[CUR_LANG];
        if(name == TXT.Caution_Content.ToString()) res = Caution_Content[CUR_LANG];
        if(name == TXT.Caution_Notice.ToString()) res = Caution_Notice[CUR_LANG];


        //* TUTORIAL
        if(name == TXT.Tutorial.ToString()) res = Tutorial[CUR_LANG];
        if(name == TXT.OpenTutorial_Content.ToString()) res = OpenTutorial_Content[CUR_LANG];
        if(name == TXT.Skip_NextTime.ToString()) res = Skip_NextTime[CUR_LANG];
        if(name == TXT.TutorialA_Title.ToString()) res = TutorialA_Title[CUR_LANG];
        if(name == TXT.TutorialA_Content.ToString()) res = TutorialA_Content[CUR_LANG];
        if(name == TXT.TutorialB_Title.ToString()) res = TutorialB_Title[CUR_LANG];
        if(name == TXT.TutorialB_Content.ToString()) res = TutorialB_Content[CUR_LANG];
        if(name == TXT.TutorialC_Title.ToString()) res = TutorialC_Title[CUR_LANG];
        if(name == TXT.TutorialC_Content.ToString()) res = TutorialC_Content[CUR_LANG];
        if(name == TXT.TutorialD_Title.ToString()) res = TutorialD_Title[CUR_LANG];
        if(name == TXT.TutorialD_Content.ToString()) res = TutorialD_Content[CUR_LANG];
        if(name == TXT.TutorialE_Title.ToString()) res = TutorialE_Title[CUR_LANG];
        if(name == TXT.TutorialE_Content.ToString()) res = TutorialE_Content[CUR_LANG];
        if(name == TXT.TutorialF_Title.ToString()) res = TutorialF_Title[CUR_LANG];
        if(name == TXT.TutorialF_Content.ToString()) res = TutorialF_Content[CUR_LANG];
        if(name == TXT.TutorialG_Title.ToString()) res = TutorialG_Title[CUR_LANG];
        if(name == TXT.TutorialG_Content.ToString()) res = TutorialG_Content[CUR_LANG];
        

        //* UI
        if(name == TXT.Ok.ToString()) res = Ok[CUR_LANG];
        if(name == TXT.No.ToString()) res = No[CUR_LANG];
        if(name == TXT.Use.ToString()) res = Use[CUR_LANG];
        if(name == TXT.Get.ToString()) res = Get[CUR_LANG];
        if(name == TXT.Open.ToString()) res = Open[CUR_LANG];
        if(name == TXT.Start.ToString()) res = Start[CUR_LANG];
        if(name == TXT.Level.ToString()) res = Level[CUR_LANG];
        if(name == TXT.Stage.ToString()) res = Stage[CUR_LANG];
        if(name == TXT.BossLimitCnt.ToString()) res = BossLimitCnt[CUR_LANG];
        if(name == TXT.Combo.ToString()) res = Combo[CUR_LANG];
        if(name == TXT.Status.ToString()) res = Status[CUR_LANG];
        if(name == TXT.Back.ToString()) res = Back[CUR_LANG];
        if(name == TXT.Ready.ToString()) res = Ready[CUR_LANG];
        if(name == TXT.Out.ToString()) res = Out[CUR_LANG];
        if(name == TXT.Strike.ToString()) res = Strike[CUR_LANG];
        if(name == TXT.BestScore.ToString()) res = BestScore[CUR_LANG];
        if(name == TXT.LevelUpPanel_Title.ToString()) res = LevelUpPanel_Title[CUR_LANG];
        if(name == TXT.LevelUpPanel_Explain.ToString()) res = LevelUpPanel_Explain[CUR_LANG];
        if(name == TXT.Reward.ToString()) res = Reward[CUR_LANG];
        if(name == TXT.GetRewardChestPanel_Content.ToString()) res = GetRewardChestPanel_Content[CUR_LANG];
        if(name == TXT.Empty.ToString()) res = Empty[CUR_LANG];
        if(name == TXT.Roulette.ToString()) res = Roulette[CUR_LANG];
        if(name == TXT.PsvSkillTicket.ToString()) res = PsvSkillTicket[CUR_LANG];
        if(name == TXT.RouletteTicket.ToString()) res = RouletteTicket[CUR_LANG];
        if(name == TXT.RouletteSpin.ToString()) res = RouletteSpin[CUR_LANG];
        if(name == TXT.RouletteStop.ToString()) res = RouletteStop[CUR_LANG];
        if(name == TXT.FirstSkill.ToString()) res = FirstSkill[CUR_LANG];
        if(name == TXT.SecondSkill.ToString()) res = SecondSkill[CUR_LANG];
        if(name == TXT.PsvSkillInfo.ToString()) res = PsvSkillInfo[CUR_LANG];
        if(name == TXT.AtvSkill.ToString()) res = AtvSkill[CUR_LANG];
        if(name == TXT.Character.ToString()) res = Character[CUR_LANG];
        if(name == TXT.Bat.ToString()) res = Bat[CUR_LANG];
        if(name == TXT.Achivement.ToString()) res = Achivement[CUR_LANG];
        if(name == TXT.Upgrade.ToString()) res = Upgrade[CUR_LANG];
        if(name == TXT.CashShop.ToString()) res = CashShop[CUR_LANG];
        if(name == TXT.PremiumPack.ToString()) res = PremiumPack[CUR_LANG];
        if(name == TXT.Coin.ToString()) res = Coin[CUR_LANG];
        if(name == TXT.Diamond.ToString()) res = Diamond[CUR_LANG];
        if(name == TXT.RemoveAllADs.ToString()) res = RemoveAllADs[CUR_LANG];
        if(name == TXT.Later.ToString()) res = Later[CUR_LANG];
        if(name == TXT.Rate.ToString()) res = Rate[CUR_LANG];
        if(name == TXT.Purchase_Complete.ToString()) res = Purchase_Complete[CUR_LANG];
        if(name == TXT.HardMode.ToString()) res = HardMode[CUR_LANG];
        if(name == TXT.Play.ToString()) res = Play[CUR_LANG];

        //* PSV
        if(name == TXT.Dmg.ToString()) res = Dmg[CUR_LANG];
        if(name == TXT.MultiShot.ToString()) res = MultiShot[CUR_LANG];
        if(name == TXT.Speed.ToString()) res = Speed[CUR_LANG];
        if(name == TXT.InstantKill.ToString()) res = InstantKill[CUR_LANG];
        if(name == TXT.Critical.ToString()) res = Critical[CUR_LANG];
        if(name == TXT.Explosion.ToString()) res = Explosion[CUR_LANG];
        if(name == TXT.ExpUp.ToString()) res = ExpUp[CUR_LANG];
        if(name == TXT.ItemSpawn.ToString()) res = ItemSpawn[CUR_LANG];
        if(name == TXT.VerticalMultiShot.ToString()) res = VerticalMultiShot[CUR_LANG];
        if(name == TXT.CriticalDamage.ToString()) res = CriticalDamage[CUR_LANG];
        if(name == TXT.Laser.ToString()) res = Laser[CUR_LANG];
        //* Unique PSV
        if(name == TXT.FireProperty.ToString()) res = FireProperty[CUR_LANG];
        if(name == TXT.IceProperty.ToString()) res = IceProperty[CUR_LANG];
        if(name == TXT.ThunderProperty.ToString()) res = ThunderProperty[CUR_LANG];
        if(name == TXT.DamageTwice.ToString()) res = DamageTwice[CUR_LANG];
        if(name == TXT.GiantBall.ToString()) res = GiantBall[CUR_LANG];
        if(name == TXT.DarkOrb.ToString()) res = DarkOrb[CUR_LANG];
        if(name == TXT.GodBless.ToString()) res = GodBless[CUR_LANG];
        if(name == TXT.BirdFriend.ToString()) res = BirdFriend[CUR_LANG];
        //* Upgrade (Only Needed)
        if(name == TXT.BossDamage.ToString()) res = BossDamage[CUR_LANG];
        if(name == TXT.CoinBonus.ToString()) res = CoinBonus[CUR_LANG];
        if(name == TXT.Defence.ToString()) res = Defence[CUR_LANG];


        //* ERROR
        if(res == null) Debug.LogError("存在しないTEXTです。");
        return res;
    }
    public static List<string> getTxtList(string str){
        List<string> resList = new List<string>();
        Debug.Log($"Language::getTxtList(str):: str= {str}"); // ex) Bat_7.StripeA
        const int TYPE=0, NAME=1, EXPLAIN=2, HOMERUNBONUS=3;

        string[] splitList = str.Split('_'); // ex) split = ["Bat", "7.StripeA"];
        string type = splitList[TYPE];
        DM.PANEL itemType = DM.ins.getCurPanelType2Enum(type);

        //* Split Txt String
        switch(itemType){
            case DM.PANEL.Chara: 
            case DM.PANEL.Bat: 
                resList.Add(splitList[NAME]);
                break;
            case DM.PANEL.Skill: 
                resList.Add(splitList[NAME]);
                resList.Add(splitList[EXPLAIN]);
                resList.Add(splitList[HOMERUNBONUS]);
                break;
            case DM.PANEL.CashShop:
            case DM.PANEL.PsvInfo:
            case DM.PANEL.Upgrade:
                resList.Add(splitList[NAME]);
                resList.Add(splitList[EXPLAIN]);
                break;
        }
        
        /* (BUG-9) 
        *  各々モデルPrefabのInspectorViewで、ItemInfoスクリプトのNameTxtsへ書いたのテキストが
        *  自分のObject名と違うと、int idxが nullになるので、エラーが発生します。
        *  EX)「Bat_2.Pink」PrefabオブジェクトのInspectorViewにあるNameTxtsへ「1.Pink」を書いたら、エラー。
        */
        int idx = -1;
        try{
            //* Set Lang
            switch(itemType){
                case DM.PANEL.Chara: {
                    idx = CharaList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    resList[LANG.NAME] = CharaList[idx][(int)DM.ins.personalData.Lang];
                    break;
                }
                case DM.PANEL.Bat: {
                    idx = BatList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    resList[LANG.NAME] = BatList[idx][(int)DM.ins.personalData.Lang];
                    break;
                }
                case DM.PANEL.Skill: {
                    idx = SkillNameList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    // Debug.Log("<color=white><<Skill>> idx= " + idx + "</color>");
                    resList[LANG.NAME] = SkillNameList[idx][(int)DM.ins.personalData.Lang];
                    resList[LANG.EXPLAIN] = SkillExplainList[idx][(int)DM.ins.personalData.Lang];
                    resList[LANG.HOMERUNBONUS] = SkillHomeRunBonusList[idx][(int)DM.ins.personalData.Lang];
                    break;
                }
                case DM.PANEL.CashShop:{
                    idx = CashShopNameList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    // Debug.Log("<color=yellow><<CashShop>> idx= " + idx + "</color>");
                    resList[LANG.NAME] = CashShopNameList[idx][(int)DM.ins.personalData.Lang];
                    resList[LANG.EXPLAIN] = CashShopExplainList[idx][(int)DM.ins.personalData.Lang];
                    break;
                }
                case DM.PANEL.PsvInfo:{
                    idx = PsvInfoNameList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    // Debug.Log("<color=yellow><<PsvInfo>> idx= " + idx + "</color>");
                    resList[LANG.NAME] = PsvInfoNameList[idx][(int)DM.ins.personalData.Lang];
                    resList[LANG.EXPLAIN] = PsvInfoExplainList[idx][(int)DM.ins.personalData.Lang];
                    break;
                }
                case DM.PANEL.Upgrade:{
                    idx = UpgradeNameList.FindIndex(langArr => resList[LANG.NAME] == langArr[(int)TP.EN]);
                    Debug.Log("<color=yellow><<Upgrade>> idx= " + idx + "</color>");
                    resList[LANG.NAME] = UpgradeNameList[idx][(int)DM.ins.personalData.Lang];
                    resList[LANG.EXPLAIN] = Util._.replaceSettingNumber(UpgradeExplainList[idx][(int)DM.ins.personalData.Lang], idx);
                    break;
                }
            }
        }
        catch(Exception errMsg){
            Debug.LogError(errMsg + "\n");
            Debug.LogError("idx= "+idx+"："+itemType+"List.FindIndexメソッドでresListと同じテキストがないでした。\n"
                + "<color=cyan>" + str + "</color>(Prefabオブジェクト名)がそのInspectorViewでItemInfoスクリプトの<color=cyan>NameTxts</color>変数に作成した文字が同じなのかを確認してください！");

        }
        
        return resList;
    }
}