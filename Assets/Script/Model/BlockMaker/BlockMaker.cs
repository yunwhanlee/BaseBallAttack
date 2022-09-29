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

    const float WIDTH = 1.9f;
    const float HEIGHT = 1;
    const int MAX_HORIZONTAL_GRID = 6;
    const int FIRST_CREATE_VERTICAL_CNT = 4; //DEAD_MAX-> 13
    // const float SPAWN_POS_X = -5;
    const float SPWAN_POS_Y = -2;
    public float BLOCK2_SPAN = 5;
    public int BOSS_STAGE_SPAN = 10;
    public bool isBossSpawn = false;    public bool IsBossSpawn {get => isBossSpawn; set => isBossSpawn = value;}

    [Header("<---- Special Block Spawn Percent  ---->")]
    public int skipBlockPer = 20;
    public int treasureChestBlockPer = 10;
    public int healBlockPer = 10;

    public GameObject[] blockPrefs;
    public GameObject[] bossPrefs;
    public bool isCreateBlock;  public bool IsCreateBlock {get => isCreateBlock; set => isCreateBlock = value;}
    public Color[] colors;   public Color[] Colors {get => colors;}
    public Material[] mts;   public Material[] Mts {get => mts;}

    [Header("<---- DROP ITEMS ---->")]
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
        }

        //* Boss
        if(gm.stage % BOSS_STAGE_SPAN == 0 && !IsBossSpawn){
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

    IEnumerator coPlayBossSpawnAnim(){
        //* 再生時間 習得
        const int IDLE = 0, SPAWN = 1;
        var anim = gm.bossSpawnTxt.GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        // Array.ForEach(clips, clip=> Debug.Log($"clip= {clip.name}, clip.length= {clip.length}"));
        float playTimeSec = clips[SPAWN].length;


        Time.timeScale = 0.1f;
        Debug.Log("coPlayBossSpawnAnim():: Time Stop");
        anim.SetTrigger(DM.ANIM.DoSpawn.ToString());
        yield return new WaitForSecondsRealtime(playTimeSec);
        Debug.Log("coPlayBossSpawnAnim():: Time Go");
        Time.timeScale = 1;
    }

    public void createBlockRow(KIND type, bool isFirst = false, int verticalCnt = 1){
        //* Value
        float xs = blockPrefs[(int)type].transform.localScale.x;
        float spawnPosX = (type == KIND.Normal)? -5 : -3.1f;
        float middleGap = 0.5f; // センターのボールが来る隙間

        switch(type){
            case KIND.Normal : 
                for(int v=0; v<verticalCnt;v++){ //縦
                    int offsetCnt = 1;
                    Debug.Log("---------------------");
                    for(int h=0; h<MAX_HORIZONTAL_GRID;h++){ //横
                        var ins = blockPrefs[(int)type];
#region Block Kind or Skip
                        //* #1. Block Skip?
                        int rand = Random.Range(0,100);
                        if(rand < skipBlockPer) continue; //Skip Block

                        //* #2. Block TreasureChest?
                        rand = Random.Range(0,100);
                        if(rand < treasureChestBlockPer)   ins = blockPrefs[(int)KIND.TreasureChest];

                        //* #3. Block HealBlock?
                        rand = Random.Range(0,100);
                        if(rand < healBlockPer)   ins = blockPrefs[(int)KIND.Heal];

                        //* #3. Block Normal + Newゲーム開始なのか、次のステージなのか？
                        float x = h < 3 ? (spawnPosX + h * xs) : (spawnPosX + h * xs + middleGap * offsetCnt);
                        float y = (isFirst)? 0 : ins.transform.position.y + gm.blockGroup.position.y;
                        float z = (isFirst)? -v : SPWAN_POS_Y;
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 setPos = (isFirst)? pos + gm.blockGroup.position : pos;
                        Instantiate(ins, setPos, Quaternion.identity, gm.blockGroup);
#endregion
                    }
                }
                break;
            case KIND.Long : 
                for(int h=0; h<2; h++){
                    var ins = blockPrefs[(int)type];
                    float x = h < 1 ? spawnPosX + h * xs : spawnPosX + h * xs + middleGap;
                    float y = ins.transform.position.y + gm.blockGroup.position.y;
                    Vector3 pos = new Vector3(x, y, SPWAN_POS_Y);
                    Instantiate(ins, pos, Quaternion.identity, gm.blockGroup);
                }
                break;
        }
    }
    public void createDropItemExpOrbPf(Transform blockTf, int resultExp){
        Debug.Log("createDropItemExpOrbPf:: blockTf= " + blockTf + ", resultExp= " + resultExp);
        var ins = ObjectPool.getObject(ObjectPool.DIC.DropItemExpOrbPf.ToString(), blockTf.position, Quaternion.identity, gm.dropItemGroup);
        var block = blockTf.GetComponent<Block_Prefab>();
        ins.GetComponent<DropItem>().ExpVal = block.Exp;
    }

    public void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓, gm.stage= " + gm.stage);
        gm.blockGroup.position = new Vector3(gm.blockGroup.position.x, gm.blockGroup.position.y, gm.blockGroup.position.z - 1);

        //* Next Set Block Type
        if(gm.stage % BLOCK2_SPAN == 0){
            createBlockRow(KIND.Long);
        }else{
            createBlockRow(KIND.Normal);
        }
    }

    public void setGlowEF(Block_Prefab[] blocks, bool isOn){
        Array.ForEach(blocks, bl => bl.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(isOn));
    }

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
}
