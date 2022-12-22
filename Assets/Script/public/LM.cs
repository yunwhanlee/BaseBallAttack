using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitRank{
    [SerializeField] [Range(0, 1.5f)] float dist;   public float Dist {get => dist;}
    [SerializeField] int power;   public int Power {get => power;}
    public HitRank(float dist, int power){
        this.dist = dist;
        this.power = power;
    }
}

public class LM : MonoBehaviour //* LEVEING MANAGER
{
    public static LM _;
    //* Value
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
    
    [HideInInspector] public int HOMERUN_MIN_POWER;

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
    [Tooltip("ヒール ブロック％")] [Range(0, 100)] public int HEAL_BLOCK_PER = 5;

    [Header("DROP ITEM PERCENT")][Header("__________________________")]
    [Tooltip("リワード箱 ドロップ％(0～1000)")] [Range(0, 1000)] public int REWARD_CHEST_PER = 5;

    [Header("DROP BOX PERCENT")][Header("__________________________")]
    [Tooltip("BUFFドロップボックス ドロップ％(0～1000)")] [Range(0, 1000)] public int DROP_BOX_PER = 5;
    [Tooltip("QuestionPfで得るコイン単位")] public int DROPBOX_COIN_VALUE = 10;

    [Header("PLAYER")][Header("__________________________")]
    [Tooltip("アクティブスキル 減少％")] [Range(0, 1.0f)] public float ATV_COOLDOWN_UNIT = 0.02f;
    [Tooltip("プレイヤー最大レベル")] public int MAX_LV = 50;
    public List<float> MAX_EXP_LIST = new List<float>();

    [Header("PSV SKILL")][Header("__________________________")]
    [Tooltip("レベルアップスロット、ユニックスキルの出現％")] [Range(0, 100)] public int LEVELUP_SLOTS_UNIQUE_PER = 10;
    [Tooltip("ダークオーブの速度")] public int DARKORB_SPEED = 250;
    [Tooltip("GODBLESS発動のコンボ倍数")] public int GODBLESS_COMBO_SPAN = 10;

    [Header("REWARD")][Header("__________________________")]
    [Tooltip("レベルアップスロット、再回転に要するコイン")] public int REROTATE_SKILLSLOTS_PRICE_COIN = 200;
    [Tooltip("復活に要するダイアモンド")] public int REVIVE_PRICE_DIAMOND = 50;
    [Tooltip("ルーレットチケットの待機時間")] public int ROULETTE_TICKET_COOLTIME_MINUTE = 30;

    [Header("PREMIUM PACKAGE")][Header("__________________________")]
    [Tooltip("プレミアムパック購入：ルーレットチケット数")] public int PREM_PACK_ROULETTE_TICKET = 7;
    [Tooltip("プレミアムパック購入：コイン")] public int PREM_PACK_COIN = 50000;
    [Tooltip("プレミアムパック購入：ダイア")] public int PREM_PACK_DIAMOND = 5000;

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
