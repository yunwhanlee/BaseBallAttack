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
    [HideInInspector] public float  SKY_MT_MORNING_VALUE = 1.0f;
    [HideInInspector] public float  SKY_MT_DINNER_VALUE = 1.25f;

    [Header("GAME MANAGER")]
    [Tooltip("ステージ")] public int STAGE_NUM = 1;

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
    [Tooltip("マックスステージ数")] public int MAX_STAGE = 300;
    [Tooltip("ステージ当たりコイン手当")] public int STAGE_PER_COIN = 100;
    [Tooltip("ステージ当たりダイア手当")] public int STAGE_PER_DIAMOND = 10;
    [Tooltip("ハードモードコイン掛け算ボーナス")] public int HARDMODE_COIN_BONUS = 2;
    [Tooltip("ハードモードダイア掛け算ボーナス")] public float HARDMODE_DIAMOND_BONUS = 1.5f;

    [Header("VICTORY")][Header("__________________________")]
    [Tooltip("勝利ボースカウンター")] public int VICTORY_BOSSKILL_CNT = 4;
    
    [Header("BLOCK SPAN")][Header("__________________________")]
    [Tooltip("ボース登場ステージ周期")] public int BOSS_STAGE_SPAN = 10;
    [Tooltip("ボース制限ステージ時間")] public int BOSS_LIMIT_SPAN = 20;
    [Tooltip("LONGブロック登場ステージ周期")] public int LONG_BLOCK_SPAN = 5;
    [Tooltip("ブロック フリーズ 持続時間")] public int ICE_FREEZE_DURATION = 1;
    [Tooltip("ブロック 火ドットダメージ 持続時間")] public int FIRE_DOT_DMG_DURATION = 2;

    [Header("BLOCK ITEM DAMAGE PERCENT")][Header("__________________________")]
    [Tooltip("フリーズ ダメージ")] [Range(0, 1)] public float ICE_FREEZE_DMG_PER = 0.1f;
    [Tooltip("火ドット ダメージ")] [Range(0, 1)] public float FIRE_DOT_DMG_PER = 0.2f;
    [Tooltip("ヒールブロック回復％")] [Range(0, 1)] public float HEAL_BLOCK_INCREASE_PER = 0.1f;

    [Header("BLOCK ITEM CREATE PERCENT")][Header("__________________________")]
    [Tooltip("スキップ ブロック％")] [Range(0, 100)] public int SKIP_BLOCK_PER = 20;
    [Tooltip("アイテムタイプ ブロック％")] [Range(0, 100)] public int ITEM_TYPE_PER = 10;
    [Tooltip("宝箱 ブロック％")] [Range(0, 100)] public int TREASURECHEST_BLOCK_PER = 5;
    [Tooltip("宝箱から出るオーブ数")] public int TREASURECHEST_ORB_CNT = 7;
    [Tooltip("ヒール ブロック％")] [Range(0, 100)] public int HEAL_BLOCK_PER = 5;

    [Header("DROP ITEM PERCENT")][Header("__________________________")]
    [Tooltip("リワード箱 ドロップ％(0～1000)")] [Range(0, 1000)] public int REWARD_CHEST_PER = 5;

    [Header("DROP BOX PERCENT")][Header("__________________________")]
    [Tooltip("BUFFドロップボックス ドロップ％(0～1000)")] [Range(0, 1000)] public int DROP_BOX_PER = 5;
    [Tooltip("QuestionPfで得るコイン単位")] public int DROPBOX_COIN_VALUE = 10;
    [Tooltip("生きているターン")] public int DROPBOX_ALIVE_SPAN = 3;

    [Header("PLAYER")][Header("__________________________")]
    
    [Tooltip("プレイヤー最大レベル")] public int MAX_LV = 50;
    public List<float> MAX_EXP_LIST = new List<float>();

    [Header("PSV SKILL")][Header("__________________________")]
    [Tooltip("レベルアップスロット、ユニックスキルの出現％")] [Range(0, 100)] public int LEVELUP_SLOTS_UNIQUE_PER = 10;
    [Tooltip("ダークオーブの速度")] public int DARKORB_SPEED = 250;
    [Tooltip("GODBLESS発動のコンボ倍数")] public int GODBLESS_COMBO_SPAN = 10;

    [Header("ATV SKILL")][Header("__________________________")]
    [Tooltip("アクティブスキル 減少％")] [Range(0, 1.0f)] public float ATV_COOLDOWN_UNIT = 0.01f;
    [Tooltip("サンダーショットクリティカル倍数")] public float THUNDERSHOT_CRIT = 2f;
    [Tooltip("サンダーショットヒットカウント")] public int THUNDERSHOT_HIT_CNT = 5;
    [Tooltip("ファイヤーボールダーメジ％")] public float FIREBALL_DMG_PER = 2f;
    [Tooltip("ファイヤーボールドットダーメジ％")] public float FIREBALL_DOT_DMG_PER = 0.15f;
    [Tooltip("カラーボールポップ破壊するカウント")] public int COLORBALLPOP_CNT = 5;
    [Tooltip("アイスウェイブダーメジ％")] public float ICEWAVE_DMG_PER = 2.5f;
    [Tooltip("ポイズン毒ドットダーメジ％")] public float POISONSMOKE_DOT_DMG_PER = 0.2f;


    [Header("MODEL ITEM PRICE")][Header("__________________________")]
    [Tooltip("一般等級")] public int GENERAL_PRICE;
    [Tooltip("RAER等級")] public int RARE_PRICE;
    [Tooltip("UNIQUE般等級")] public int UNIQUE_PRICE;
    [Tooltip("LEGEND等級")] public int LEGEND_PRICE;
    [Tooltip("GOD等級")] public int GOD_PRICE;

    [Header("UPGRADE PRICE")][Header("__________________________")]
    [Tooltip("ダメージ")] public UpgradePriceCalcSetting UPGRADE_DMG;
    [Tooltip("ボール速度")] public UpgradePriceCalcSetting UPGRADE_BALLSPD;
    [Tooltip("ボースダメージ")] public UpgradePriceCalcSetting UPGRADE_BOSSDMG;
    [Tooltip("コインぼなーす")] public UpgradePriceCalcSetting UPGRADE_COINBONUS;
    [Tooltip("クリティカル")] public UpgradePriceCalcSetting UPGRADE_CRIT;
    [Tooltip("クリティカルダメージ")] public UpgradePriceCalcSetting UPGRADE_CRITDMG;
    [Tooltip("ディフェンス")] public UpgradePriceCalcSetting UPGRADE_DEFENCE;

    [Header("REWARD")][Header("__________________________")]
    [Tooltip("レベルアップスロット、再回転に要するコイン")] public int REROTATE_SKILLSLOTS_PRICE_COIN = 200;
    [Tooltip("復活に要するダイアモンド")] public int REVIVE_PRICE_DIAMOND = 50;
    [Tooltip("ルーレットチケットの待機時間")] public int ROULETTE_TICKET_COOLTIME_MINUTE = 30;

    [Header("PREMIUM PACKAGE")][Header("__________________________")]
    [Tooltip("プレミアムパック購入：ルーレットチケット数")] public int PREM_PACK_ROULETTE_TICKET = 7;
    [Tooltip("プレミアムパック購入：コイン")] public int PREM_PACK_COIN = 50000;
    [Tooltip("プレミアムパック購入：ダイア")] public int PREM_PACK_DIAMOND = 5000;

    [Header("IN-APP PURCHASE PRICE ($ DOLLER)")][Header("__________________________")]
    [Tooltip("プレミアムパック値段")] public int PREM_PACK_PRICE = 10;
    [Header("RATE DIALOG SHOW PLAYTIME CNT")][Header("__________________________")]
    [Tooltip("評価ダイアログを表示するプレイタイム")] public int DISPLAY_RATE_DIALOG_PLAYTIME = 3;

    void Awake() => singleton();

    void Start(){
        Debug.Log("<FIBONACCI SEQUENCE> Player Max Exp");
        MAX_EXP_LIST = Util._.getCalcFibonicciSequenceList(unit: 100, fibRatio: 2, MAX_LV);
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
