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
    [Header("GAMEMANAGER")][Header("__________________________")]
    public int STAGE_NUM = 1;
    [Header("THROW BALL")][Header("__________________________")]
    public float THROW_BALL_SPEED = 25;
    [Range(0, 100)] public int SUDDENLY_THORW_PER = 50;

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
    public int LONG_BLOCK_SPAN = 5;
    public int BOSS_STAGE_SPAN = 10;
    public int ICE_FREEZE_DURATION = 1;
    public int FIRE_DOT_DMG_DURATION = 2;

    [Header("BLOCK ITEM PERCENT")][Header("__________________________")]
    [Range(0, 100)] public int skipBlockPer = 20;
    [Range(0, 100)] public int itemTypePer = 10;
    [Range(0, 100)] public int treasureChestBlockPer = 5;
    [Range(0, 100)] public int healBlockPer = 5;

    [Header("PLAYER")][Header("__________________________")]
    [Range(0, 1.0f)] public float ATVSKILL_COOLDOWN_UNIT = 0.02f;
    public int MAX_LV = 50;
    public List<float> MAX_EXP_LIST = new List<float>();

    [Header("PSV SKILL")][Header("__________________________")]
    public int LEVELUP_SLOTS_UNIQUE_PER = 20;
    public int DARKORB_SPEED = 250;
    public int GODBLESS_SPAN = 10;

    void Awake() => singleton();

    void Start(){
        Debug.Log("<FIBONACCI SEQUENCE> Player Max Exp");
        MAX_EXP_LIST = Util._.getCalcFibonicciSequenceList(unit: 100, fibRatio: 1, MAX_LV);
        HOMERUN_MIN_POWER = HIT_RANK[B].Power;
        const int OFFSET_CNT = 1;
        GODBLESS_SPAN -= OFFSET_CNT;
    }

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
