using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitRank{
    [SerializeField] [Range(0, 1.5f)] float dist;   public float Dist {get => dist;}
    [SerializeField] int power;   public float Power {get => power;}
    public HitRank(float dist, int power){
        this.dist = dist;
        this.power = power;
    }
}

public class LM : MonoBehaviour //* LEVEING MANAGER
{
    public static LM _;
    //* Value
    [Header("PLAYER")]
    [Range(0, 1.0f)] public float ATVSKILL_COOLDOWN_UNIT = 0.02f;

    [Header("THROW BALL")]
    public float THROW_BALL_SPEED = 25;
    [Range(0, 100)] public int SUDDENLY_THORW_PER = 50;

    [Header("HIT BALL")]
    public HitRank[] HIT_BALL_RANK = new HitRank[6]{
        new HitRank(0.125f, 10),
        new HitRank(0.25f, 7),
        new HitRank(0.5f, 5),
        new HitRank(0.85f, 4),
        new HitRank(1.125f, 3),
        new HitRank(1.5f, 2),
    };

    [Header("BLOCK SPAN")]
    public int LONG_BLOCK_SPAN = 5;
    public int BOSS_STAGE_SPAN = 10;

    [Header("BLOCK ITEM PERCENT")]
    [Range(0, 100)] public int skipBlockPer = 20;
    [Range(0, 100)] public int itemTypePer = 10;
    [Range(0, 100)] public int treasureChestBlockPer = 5;
    [Range(0, 100)] public int healBlockPer = 5;



    void Awake() => singleton();

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
