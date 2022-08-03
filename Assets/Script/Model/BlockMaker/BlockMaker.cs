using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BlockMaker : MonoBehaviour
{
    public enum BLOCK {Normal, Long, TreasureChest};

    //* OutSide
    public GameManager gm;

    const float WIDTH = 1.9f;
    const float HEIGHT = 1;
    const int MAX_HORIZONTAL_GRID = 6;
    const int FIRST_CREATE_VERTICAL_CNT = 4; //DEAD_MAX-> 13
    // const float SPAWN_POS_X = -5;
    const float SPWAN_POS_Y = -2;
    public float BLOCK2_SPAN = 5;

    public int createPercent;
    public Transform blockBundle;
    public GameObject[] blockPrefs;
    public bool isCreateBlock;
    public Color[] colors;   public Color[] Colors {get => colors;}
    public Material[] mts;   public Material[] Mts {get => mts;}

    [Header("<---- DROP ITEMS ---->")]
    public Transform dropItemGroup;
    public GameObject dropCoinOrbPf;

    public void Start() {
        //* Init
        var blocks = this.GetComponentsInChildren<Block_Prefab>();
        foreach(var block in blocks) block.onDestroy(block.gameObject, true);
        this.transform.position = new Vector3(0, 0.5f, -2);
        createBlockRow(BLOCK.Normal.ToString(), true, FIRST_CREATE_VERTICAL_CNT);
    }

    void Update(){
        if(isCreateBlock){
            isCreateBlock = false;
            moveDownBlock();
        }
    }

    public void setCreateBlockTrigger(bool trigger) => isCreateBlock = trigger;

    public void createBlockRow(string type, bool isFirst = false, int verticalCnt = 1){
        //* Value
        GameObject ins = (type == BLOCK.Normal.ToString())? blockPrefs[(int)BLOCK.Normal] : blockPrefs[(int)BLOCK.Long];
        float xs = ins.transform.localScale.x;
        float spawnPosX = (type == BLOCK.Normal.ToString())? -5 : -3.1f;
        float middleGap = 0.5f; // センターのボールが来る隙間

        BLOCK blockType = convertBlockStr2Enum(type);

        switch(blockType){
            case BLOCK.Normal : 
                for(int v=0; v<verticalCnt;v++){ //縦
                    int offsetCnt = 1;
                    for(int h=0; h<MAX_HORIZONTAL_GRID;h++){ //横
                        //* ランダムで生成
                        int rand = Random.Range(0,100);
                        if(createPercent < rand) continue;
                        //* Newゲーム開始なのか、次のステージなのか？
                        float x = h < 3 ? (spawnPosX + h * xs) : (spawnPosX + h * xs + middleGap * offsetCnt);
                        float y = (isFirst)? 0 : ins.transform.position.y + blockBundle.position.y;
                        float z = (isFirst)? -v : SPWAN_POS_Y;
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 setPos = (isFirst)? pos + blockBundle.position : pos;
                        Instantiate(ins, setPos, Quaternion.identity, blockBundle);
                    }
                }
                break;
            case BLOCK.Long : 
                for(int h=0; h<2; h++){
                    float x = h < 1 ? spawnPosX + h * xs : spawnPosX + h * xs + middleGap;
                    float y = ins.transform.position.y + blockBundle.position.y;
                    Vector3 pos = new Vector3(x, y, SPWAN_POS_Y);
                    Instantiate(ins, pos, Quaternion.identity, blockBundle);
                }
                break;
        }
    }
    public void createDropItemOrb(Transform blockTf, int resultExp){
        var ins = Instantiate(dropCoinOrbPf, blockTf.position, Quaternion.identity, dropItemGroup) as GameObject;
        var block = blockTf.GetComponent<Block_Prefab>();
        ins.GetComponent<DropItem>().ExpVal = block.Exp;
    }

    public void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓, gm.stage= " + gm.stage);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
        if(gm.stage % BLOCK2_SPAN == 0){
            createBlockRow(BLOCK.Long.ToString());
        }else{
            createBlockRow(BLOCK.Normal.ToString());
        }
    }

    public void setGlowEF(Block_Prefab[] targetBlocks, bool isOn){
        Array.ForEach(targetBlocks, bl => bl.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(isOn));
    }

    public void setGlowEFAllBlocks(bool isOn){ //* Block Grow EF 解除
        var blocks = GetComponentsInChildren<Block_Prefab>();
        setGlowEF(blocks, isOn);
    }

    public BLOCK convertBlockStr2Enum(string name){
        return (name == BLOCK.Normal.ToString())? BLOCK.Normal 
        : BLOCK.Long;
    }
}
