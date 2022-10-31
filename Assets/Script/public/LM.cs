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
    [Header("THROW BALL")]
    public float THROW_BALL_SPEED = 25;
    [Range(0, 100)] public int SUDDENLY_THORW_PER = 50;

    [Header("HIT BALL")]
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

    [Header("BLOCK SPAN")]
    public int LONG_BLOCK_SPAN = 5;
    public int BOSS_STAGE_SPAN = 10;

    [Header("BLOCK ITEM PERCENT")]
    [Range(0, 100)] public int skipBlockPer = 20;
    [Range(0, 100)] public int itemTypePer = 10;
    [Range(0, 100)] public int treasureChestBlockPer = 5;
    [Range(0, 100)] public int healBlockPer = 5;

    [Header("PLAYER")]
    [Range(0, 1.0f)] public float ATVSKILL_COOLDOWN_UNIT = 0.02f;
    public List<float> MAX_EXP_LIST = new List<float>();

    void Awake() => singleton();

    void Start(){
        Debug.Log("<FIBONACCI SEQUENCE> Player Max Exp");
        MAX_EXP_LIST = Util._.getCalcFibonicciSequence(unit: 100, fibRatio: 1);
        HOMERUN_MIN_POWER = HIT_RANK[B].Power;
    }

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
