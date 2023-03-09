using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitRank {
    [SerializeField] [Range(0, 1.5f)] float dist;   public float Dist {get => dist;}
    [SerializeField] int power;   public int Power {get => power;}
    public HitRank(float dist, int power){
        this.dist = dist;
        this.power = power;
    }
}

[System.Serializable]
public class UpgradePriceCalcSetting { //* calcArithmeticProgressionList
    [Tooltip("始め項")] [SerializeField] int start; public int Start {get => start;}
    [Tooltip("共差")]   [SerializeField] int commonDiffrence; public int CommonDiffrence {get => commonDiffrence;}
    [Tooltip("増加量")] [SerializeField] float gradualUpValue; public float GradualUpValue {get => gradualUpValue;}
    public UpgradePriceCalcSetting(int start, int commonDifference, float gradualUpValue){
        this.start = start;
        this.commonDiffrence = commonDifference;
        this.gradualUpValue = gradualUpValue;
    }
}

public class LM : MonoBehaviour //* LEVEING MANAGER
{
    public static LM _;
    //* Value
    [HideInInspector] public readonly float SKY_MT_X_MORNING = 1.0f;
    [HideInInspector] public readonly float SKY_MT_X_DINNER = 1.25f;
    [HideInInspector] public readonly float SKY_MT_X_NIGHT = 1.5f;

    // [Header("GAME MANAGER")]
    // [Tooltip("ステージ")] public int STAGE_NUM = 1;

    [Header("THROW BALL")][Header("__________________________")]
    [Tooltip("ボール投げる速度")] public float THROW_BALL_SPEED = 25;
    [Tooltip("ボールいきなり投げる％")] [Range(0, 100)] public int SUDDENLY_THORW_PER = 50;

    [Header("HIT BALL")][Header("__________________________")]
    [HideInInspector] public float MAX_DISTANCE = 1.5f;
    const int B = 1;
    public HitRank[] HIT_RANK = new HitRank[6]{
        new HitRank(0.125f, 10), //S
        new HitRank(0.3f, 7), //A
        new HitRank(0.5f, 5), //B
        new HitRank(0.85f, 4), //C
        new HitRank(1.125f, 3), //D
        new HitRank(1.5f, 2), //E
    };

    // [Header("UPGRADE MAXLV")][Header("__________________________")]
    // public int UPGRADE_DMG_MAXLV = 100;
    // public int UPGRADE_BALL_SPEED_MAXLV = 20;
    // public int UPGRADE_CRITICAL_MAXLV = 30;
    // public int UPGRADE_CRITICAL_DMG_MAXLV = 20;
    // public int UPGRADE_BOSS_DMG_MAXLV = 30;
    // public int UPGRADE_COIN_BONUS_MAXLV = 20;
    // public int UPGRADE_DEFENCE_MAXLV = 10;
    
    [HideInInspector] public int HOMERUN_MIN_POWER;
    [Header("STAGE")][Header("__________________________")]
    [Tooltip("マックスステージ数")] public int MAX_STAGE = 180;
    [Tooltip("ハードモードコインボーナス")] public float HARDMODE_COIN_BONUS = 2;
    [Tooltip("ハードモードダイアボーナス")] public float HARDMODE_DIAMOND_BONUS = 1.5f;
    [Tooltip("ナイトメアモードコインボーナス")] public float NIGHTMARE_COIN_BONUS = 3;
    [Tooltip("ナイトメアモードダイアボーナス")] public float NIGHTMARE_DIAMOND_BONUS = 2;


    [Header("VICTORY")][Header("__________________________")]
    [Tooltip("勝利ボースカウンター")] public int VICTORY_BOSSKILL_CNT = 4;
    
    [Header("BLOCK SPAN")][Header("__________________________")]
    [Tooltip("ボース登場ステージ周期")] public int BOSS_STAGE_SPAN = 15;
    [Tooltip("ボース制限ステージ時間")] public int BOSS_LIMIT_SPAN = 12;
    [Tooltip("ボース死んだら出るオブ")] public int BOSS_DIE_ORB_CNT = 25;
    [Tooltip("ボース制限ステージお知らせNUMBER")] public int BOSS_LIMIT_CNT_ALERT_NUM = 5;
    [Tooltip("LONGブロック登場ステージ周期")] public int LONG_BLOCK_SPAN = 5;
    [Tooltip("ブロック フリーズ 持続時間")] public int ICE_FREEZE_DURATION = 1;
    [Tooltip("ブロック 火ドットダメージ 持続時間")] public int FIRE_DOT_DMG_DURATION = 2;

    [Header("BLOCK SET MESH HP UNIT(以下)")][Header("__________________________")]
    [Tooltip("PLAIN MT HP単位")] public int BLOCK_MESH_PLAIN_HP_UNIT;
    [Tooltip("PLAIN MT HP単位")] public int BLOCK_MESH_WOOD_HP_UNIT;
    [Tooltip("PLAIN MT HP単位")] public int BLOCK_MESH_SAND_HP_UNIT;
    [Tooltip("PLAIN MT HP単位")] public int BLOCK_MESH_REDBRICK_HP_UNIT;
    [Tooltip("PLAIN MT HP単位")] public int BLOCK_MESH_IRON_HP_UNIT;

    [Header("BLOCK ITEM PROPERTY PERCENT")][Header("__________________________")]
    [Tooltip("フリーズ ダメージ")] [Range(0, 1)] public float ICE_FREEZE_DMG_PER = 0.1f;
    [Tooltip("火ドット ダメージ")] [Range(0, 1)] public float FIRE_DOT_DMG_PER = 0.2f;
    [Tooltip("ヒールブロック回復％")] [Range(0, 1)] public float HEAL_BLOCK_INCREASE_PER = 0.1f;

    [Header("BLOCK ITEM CREATE PERCENT")][Header("__________________________")]
    [Tooltip("スキップ ブロック％")] [Range(0, 100)] public int SKIP_BLOCK_PER = 20;
    [Tooltip("アイテムタイプ ブロック％")] [Range(0, 100)] public int ITEM_TYPE_PER = 10;
    [Tooltip("宝箱 ブロック％")] [Range(0, 100)] public int TREASURECHEST_BLOCK_PER = 5;
    [Tooltip("宝箱から出るオーブ数")] public int TREASURECHEST_ORB_CNT = 7;
    [Tooltip("ヒール ブロック％")] [Range(0, 100)] public int HEAL_BLOCK_PER = 5;
    [Tooltip("ヒール ブロック 生成MAX数")] public int HEAL_BLOCK_CREATE_MAX_CNT = 3;

    [Header("DROP ITEM PERCENT")][Header("__________________________")]
    [Tooltip("リワード箱 ドロップ％(0～1000)")] [Range(0, 1000)] public int REWARD_CHEST_PER = 5;
    [Tooltip("（値が重なる）1. リワード GOODS％")] public int REWARD_GOODS_PER = 40;
    [Tooltip("（値が重なる）2. リワード PSVSKILL_TICKET％")] public int REWARD_PSVSKILL_TICKET_PER = 60;
    [Tooltip("（値が重なる）3. リワード ROULETTE_TICKET％")] public int REWARD_ROULETTE_TICKET_PER = 80;

    [Header("DROP BOX PERCENT")][Header("__________________________")]
    [Tooltip("BUFFドロップボックス ドロップ％(0～1000)")] [Range(0, 1000)] public int DROPBOX_PER = 5;
    [Tooltip("QuestionPfで得るコイン単位")] public int DROPBOX_COIN_VALUE = 10;
    [Tooltip("生きているターン")] public int DROPBOX_ALIVE_SPAN = 3;

    [Header("PLAYER")][Header("__________________________")]
    [Tooltip("プレイヤー最大レベル")] public int MAX_LV = 30;
    public List<int> MAX_EXP_LIST = new List<int>();

    [Header("UPGRADE UNIT")][Header("__________________________")]
    public int UPG_DMG_UNIT = 1;    public int UPG_DMG_MAXLV = 100;
    public float UPG_BALL_SPEED_UNIT = 0.05f;    public int UPG_SPEED_MAXLV = 20;
    public float UPG_CRIT_UNIT = 0.01f;    public int UPG_CRIT_MAXLV = 30;
    public float UPG_CRIT_DMG_UNIT = 0.1f;    public int UPG_CRIT_DMG_MAXLV = 20;
    public float UPG_BOSS_DMG_UNIT = 0.05f;    public int UPG_BOSS_DMG_MAXLV = 30;
    public float UPG_COIN_BONUS_UNIT = 0.05f;    public int UPG_COIN_BONUS_MAXLV = 20;
    public float UPG_DEFENCE_UNIT = 0.05f;    public int UPG_DEFENCE_MAXLV = 10;

    [Header("PSV SKILL")][Header("__________________________")]
    [Tooltip("レベルアップスロット、ユニックスキルの出現％")] [Range(0, 100)] public int LEVELUP_SLOTS_UNIQUE_PER = 10;
    [Tooltip("ダークオーブの速度")] public int DARKORB_SPEED = 250;
    [Tooltip("GODBLESS発動のコンボ倍数")] public int GODBLESS_COMBO_SPAN = 10;

    [Header("ATV SKILL")][Header("__________________________")]
    [Tooltip("アクティブスキル 減少値％")] [Range(0, 1.0f)] public float ATV_COOLDOWN_UNIT = 0.01f;
    [Tooltip("アクティブスキル MAXレベル値")] public int ATVSKILL_MAXLV = 20;
    [Space(10)]
    [Tooltip("サンダーショット クリティカル 倍数")] public float THUNDERSHOT_CRIT = 1f;
    [Tooltip("サンダーショット デフォルト ヒットカウント")] public int THUNDERSHOT_DEF_HIT = 5;
    [Tooltip("サンダーショット アップグレード ヒットカウント")] public int THUNDERSHOT_UPG_HIT = 1;
    [Space(10)]
    [Tooltip("カラーボール デフォルト カウント")] public int COLORBALLPOP_DEF_CNT = 5;
    [Tooltip("カラーボール アップグレード カウント")] public int COLORBALLPOP_UPG_CNT = 1;
    [Space(10)]
    [Tooltip("ファイヤーボール デフォルト ダーメジ％")] public float FIREBALL_DEF_DMG_PER = 3f;
    [Tooltip("ファイヤーボール アップグレード ダーメジ％")] public float FIREBALL_UPG_DMG_PER = 0.5f;
    [Tooltip("ファイヤーボール ドット ダーメジ％")] public float FIREBALL_DOT_DMG_PER = 0.15f;
    [Space(10)]
    [Tooltip("ポイズン毒ドット デフォルト ダーメジ％")] public float POISONSMOKE_DEF_DMG_PER = 0.2f;
    [Tooltip("ポイズン毒ドット アップグレード ダーメジ％")] public float POISONSMOKE_UPG_DMG_PER = 0.05f;
    [Space(10)]
    [Tooltip("アイスウェイブ デフォルト ダーメジ％")] public float ICEWAVE_DEF_DMG_PER = 2.5f;
    [Tooltip("アイスウェイブ アップグレード％")] public float ICEWAVE_UPG_DMG_PER = 0.4f;

    [Header("STAGE FINISH PRICE")][Header("__________________________")]
    [Tooltip("ステージ当たりコイン手当")] public int STAGE_PER_COIN_PRICE = 100;
    [Tooltip("ステージ当たりダイア手当")] public int STAGE_PER_DIAMOND_PRICE = 10;

    [Header("MODEL ITEM PRICE")][Header("__________________________")]
    [Tooltip("一般等級")] public int GENERAL_PRICE; //COIN
    [Tooltip("RAER等級")] public int RARE_PRICE; //COIN
    [Tooltip("UNIQUE般等級")] public int UNIQUE_PRICE; //COIN
    [Tooltip("LEGEND等級")] public int LEGEND_PRICE; //COIN
    [Tooltip("GOD等級")] public int GOD_PRICE; //DIAMOND

    [Header("UPGRADE PRICE")][Header("__________________________")]
    [Tooltip("ダメージ")] public UpgradePriceCalcSetting UPGRADE_DMG;
    [Tooltip("ボール速度")] public UpgradePriceCalcSetting UPGRADE_BALLSPD;
    [Tooltip("クリティカル")] public UpgradePriceCalcSetting UPGRADE_CRIT;
    [Tooltip("クリティカルダメージ")] public UpgradePriceCalcSetting UPGRADE_CRITDMG;
    [Tooltip("ボースダメージ")] public UpgradePriceCalcSetting UPGRADE_BOSSDMG;
    [Tooltip("コインぼなーす")] public UpgradePriceCalcSetting UPGRADE_COINBONUS;
    [Tooltip("ディフェンス")] public UpgradePriceCalcSetting UPGRADE_DEFENCE;

    [Header("ATV SKILL UPGRADE PRICE")][Header("__________________________")]
    [Tooltip("アクティブスキル UnLock価格")] public int PURCHASE_ATVSKILL_PRICE;
    [Tooltip("UnLock2ndAtvスキル値段")] public int UNLOCK_2ND_ATVSKILL_PRICE;
    [Tooltip("アクティブスキルアップグレード値段クラス")] public UpgradePriceCalcSetting UPGRADE_ATVSKILL;
    [Tooltip("アクティブスキルアップグレード値段リスト")] public List<int> UPGRADE_ATVSKILL_PRICE_LIST;

    [Header("REWARD CHEST")][Header("__________________________")]
    [Tooltip("REWARD CHEST GOODS COIN配列")] public int[] REWARD_CHEST_COINARR = {100, 250, 500, 1000, 2000, 5000};
    [Tooltip("REWARD CHEST GOODS DIAMOND配列")] public int[] REWARD_CHEST_DIAMONDARR = {10, 50, 100, 150, 250, 400};

    [Header("AD REWARD")][Header("__________________________")]
    [Tooltip("レベルアップスロット、再回転に要するコイン")] public int REROTATE_SKILLSLOTS_PRICE_COIN = 200;
    [Tooltip("復活に要するダイアモンド")] public int REVIVE_PRICE_DIAMOND = 50;
    [Tooltip("ルーレットチケットの待機時間")] public int ROULETTE_TICKET_COOLTIME_MINUTE = 60;
    [Tooltip("一日、ルーレットチケットAD制限数")] public int ROULETTE_TICKET_ONEDAY_AD_PLAY_MAX_CNT = 5;

    [Header("PREMIUM PACKAGE")][Header("__________________________")]
    [Tooltip("プレミアムパック購入：ルーレットチケット数")] public int PREM_PACK_ROULETTE_TICKET = 7;
    [Tooltip("プレミアムパック購入：コイン")] public int PREM_PACK_COIN = 50000;
    [Tooltip("プレミアムパック購入：ダイア")] public int PREM_PACK_DIAMOND = 5000;

    [Header("IN-APP PURCHASE PRICE(￥)")][Header("__________________________")]
    [Tooltip("プレミアムパック価額")] public int PREM_PACK_PRICE = 999;
    [Header("RATE DIALOG SHOW PLAYTIME CNT")][Header("__________________________")]
    [Tooltip("評価ダイアログを表示するプレイタイム")] public int DISPLAY_RATE_DIALOG_PLAYTIME = 3;

    void Awake() => singleton();

    void Start(){
        Debug.Log("<FIBONACCI SEQUENCE> Player Max Exp");
        // MAX_EXP_LIST = Util._.getCalcFibonicciSequenceList(unit: 200, fibRatio: 2, MAX_LV);
        MAX_EXP_LIST = Util._.calcArithmeticProgressionList(
            start: 300, 
            max: 30, 
            d: 100, 
            gradualUpValue: 1f
        );

        HOMERUN_MIN_POWER = HIT_RANK[B].Power;
        const int OFFSET_CNT = 1;
        GODBLESS_COMBO_SPAN -= OFFSET_CNT;
    }

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
