using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BlockMaker : MonoBehaviour
{
    public enum KIND {Normal, Long, TreasureChest, Heal, Boss, Null};

    //* OutSide
    public GameManager gm;

    public const int MAX_HORIZONTAL_GRID = 6, FIRST_CREATE_VERTICAL_CNT = 4; //DEAD_MAX-> 13
    public const float SCALE_X = 1.9f, SCALE_Y = 1, SPAWN_POS_X = -5;
    public const float START_POS_Z = -2, CENTER_GAP = 0.5f; // センターのボールが来る隙間

    [Header("STATUS")]
    public bool isCreateBlock;  public bool IsCreateBlock {get => isCreateBlock; set => isCreateBlock = value;}
    public bool isBossSpawn = false;    public bool IsBossSpawn {get => isBossSpawn; set => isBossSpawn = value;}

    [Header("LEVELING (SPAN)")]
    // public int LONG_BLOCK_SPAN; // def 5
    // public int BOSS_STAGE_SPAN; // def 10

    [Header("LEVELING (PERCENT)")]
    // public int skipBlockPer; // def 20
    // public int itemTypePer; // def 10
    // public int treasureChestBlockPer; // def 5
    // public int healBlockPer; // def 5

    [Header("RESOURCE")]
    public GameObject[] blockPrefs;
    public GameObject[] bossPrefs;
    public Color[] colors;   public Color[] Colors {get => colors;}
    public Material[] mts;   public Material[] Mts {get => mts;}

    [Header("DROP ITEM")]
    public GameObject dropItemExpOrbPf;

    public void Start() {
        //* Init
        var blocks = this.GetComponentsInChildren<Block_Prefab>();
        foreach(var block in blocks) block.onDestroy(block.gameObject, true);
        this.transform.position = new Vector3(0, 0.5f, -2);
        createBlockRow(KIND.Normal, true, FIRST_CREATE_VERTICAL_CNT);

    }

    void Update(){
        if(IsCreateBlock){
            IsCreateBlock = false;
            moveDownBlock();
            bossSpawn();
        }
        
    }
//*---------------------------------------
//*  関数
//*---------------------------------------
    public void createBlockRow(KIND type, bool isFirstStage = false, int verticalCnt = 1){
        //* Value
        float xs = blockPrefs[(int)type].transform.localScale.x;
        float startPosX = (type == KIND.Normal)? -5 : -3.1f; //* Pivotが真ん中なので、OffsetPosX設定。

        switch(type){
            case KIND.Normal : 
                for(int v=0; v<verticalCnt;v++){ //* 縦↕
                    Debug.Log("---------------------");
                    for(int h=0; h<MAX_HORIZONTAL_GRID;h++){ //* 横⇔
                        var ins = blockPrefs[(int)type];
                        #region Block Kind or Skip
                        //* #1. Block Skip?
                        int rand = Random.Range(0,100);
                        if(rand < LM._.skipBlockPer) continue; //Skip Block

                        //* #2. Block TreasureChest?
                        rand = Random.Range(0,100);
                        if(rand < LM._.treasureChestBlockPer)   ins = blockPrefs[(int)KIND.TreasureChest];

                        //* #3. Block HealBlock?
                        rand = Random.Range(0,100);
                        if(rand < LM._.healBlockPer)   ins = blockPrefs[(int)KIND.Heal];

                        //* #4. Block生成
                        float x = (h < 3)? (startPosX + h * xs) : (startPosX + h * xs + CENTER_GAP); //* ボールが出る空間考えて調整。
                        float y = (isFirstStage)? 0 : ins.transform.position.y + gm.blockGroup.position.y;
                        float z = (isFirstStage)? -v : START_POS_Z;
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 setPos = (isFirstStage)? pos + gm.blockGroup.position : pos;
                        Instantiate(ins, setPos, Quaternion.identity, gm.blockGroup);
                        #endregion
                    }
                }
                break;
            case KIND.Long : 
                for(int h=0; h<2; h++){
                    var ins = blockPrefs[(int)type];
                    float x = (h < 1)? startPosX + h * xs : startPosX + h * xs + CENTER_GAP;
                    float y = ins.transform.position.y + gm.blockGroup.position.y;
                    Vector3 pos = new Vector3(x, y, START_POS_Z);
                    Instantiate(ins, pos, Quaternion.identity, gm.blockGroup);
                }
                break;
        }
    }

    public void createDropItemExpOrbPf(Transform blockTf, int resultExp, int popPower = 350){
        Debug.Log("createDropItemExpOrbPf:: blockTf= " + blockTf + ", resultExp= " + resultExp);
        var ins = ObjectPool.getObject(ObjectPool.DIC.DropItemExpOrbPf.ToString(), blockTf.position, Quaternion.identity, gm.dropItemGroup);
        var block = blockTf.GetComponent<Block_Prefab>();
        ins.GetComponent<DropItem>().ExpVal = resultExp;
        ins.GetComponent<DropItem>().spawnPopUp(popPower);
    }

    public void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓, gm.stage= " + gm.stage);
        gm.blockGroup.position = new Vector3(gm.blockGroup.position.x, gm.blockGroup.position.y, gm.blockGroup.position.z - 1);

        //* Next Set Block Type
        if(gm.stage % LM._.LONG_BLOCK_SPAN == 0){
            createBlockRow(KIND.Long);
        }else{
            createBlockRow(KIND.Normal);
        }
    }
#region BOSS
    public BossBlock getBoss(){
        return (gm.bossGroup.childCount > 0)?
            gm.bossGroup.GetChild(0).GetComponent<BossBlock>() : null;
    }
    public void bossSpawn(){
        if(gm.stage % LM._.BOSS_STAGE_SPAN == 0 && !IsBossSpawn){
            IsBossSpawn = true;
            if(IsBossSpawn && gm.bossGroup.childCount == 0){
                Debug.Log("BOSS SPAWN!!");
                IsBossSpawn = false;
                var pos = new Vector3(0, 0, bossPrefs[0].transform.position.z + 2);
                Instantiate(bossPrefs[0], pos, bossPrefs[0].transform.rotation, gm.bossGroup);

                StartCoroutine(coPlayBossSpawnAnim());
            }
        }
    }
    private IEnumerator coPlayBossSpawnAnim(){
        //* 再生時間 習得
        var camAnim = gm.cam1.GetComponent<Animator>();
        var txtAnim = gm.bossSpawnTxt.GetComponent<Animator>();
        
        const int SPAWN = 1; // IDLE = 0
        float playSec = Util._.getAnimPlayTime(SPAWN, txtAnim);

        Time.timeScale = 0.1f;
        camAnim.SetTrigger(DM.ANIM.DoBossSpawn.ToString());
        txtAnim.SetTrigger(DM.ANIM.DoSpawn.ToString());
        yield return new WaitForSecondsRealtime(playSec);
        Time.timeScale = 1;
    }
    public void setGlowEF(Block_Prefab[] blocks, bool isOn){
        Array.ForEach(blocks, bl => bl.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(isOn));
    }
    public void eraseObstacle(){
        if(gm.obstacleGroup.childCount <= 0) return; 
        
        for(int i=0; i<gm.obstacleGroup.childCount; i++){
            // Debug.Log($"eraseObstacle():: obstacleGroup.GetChild({i})= {gm.obstacleGroup.GetChild(i)}");
            var childTf = gm.obstacleGroup.GetChild(i);
            Destroy(childTf.gameObject);
        }
    }
#endregion

    public void setGlowEFAllBlocks(bool isOn){ //* Block Grow EF 解除
        var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>();
        setGlowEF(blocks, isOn);
    }

    public KIND convertBlockStr2Enum(string name){
        return (name == KIND.Normal.ToString())? KIND.Normal 
        : KIND.Long;
    }
    public void checkIsHealBlock(){
        var blocks = gm.blockGroup.GetComponentsInChildren<HealBlock>();
        Array.ForEach(blocks, block => {
            if(block.kind == KIND.Heal){
                block.IsHeal = true;
            }
        });
    }
    public void setDecreaseHP(GameObject obj, int dmg){
        //* TAGで親と子の正しいCLASSを判別してから、処理
        if(obj.CompareTag(DM.TAG.BossBlock.ToString()))
            obj.GetComponent<BossBlock>().decreaseHp(dmg);
        else
            obj.GetComponent<Block_Prefab>().decreaseHp(dmg);
    }
}
