using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM : MonoBehaviour //* LEVEING MANAGER
{
    public static LM _;

    //* Value
    [Header("PLAYER")]
    [Range(0, 1.0f)] public float ATVSKILL_COOLDOWN_UNIT = 0.05f;

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
