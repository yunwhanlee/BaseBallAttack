using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BlockMaker : MonoBehaviour
{
    public const string NORMAL_BLOCK = "NormalBlock";
    public enum BLOCK {Normal, Long, TreasureChest, Null};

    //* OutSide
    public GameManager gm;

    const float WIDTH = 1.9f;
    const float HEIGHT = 1;
    const int MAX_HORIZONTAL_GRID = 6;
    const int FIRST_CREATE_VERTICAL_CNT = 4; //DEAD_MAX-> 13
    // const float SPAWN_POS_X = -5;
    const float SPWAN_POS_Y = -2;
    public float BLOCK2_SPAN = 5;

    public int skipBlockPer = 20;
    public int treasureChestBlockPer = 10;
    public GameObject[] blockPrefs;
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
        createBlockRow(BLOCK.Normal, true, FIRST_CREATE_VERTICAL_CNT);
    }

    void Update(){
        if(IsCreateBlock){
            IsCreateBlock = false;
            moveDownBlock();
        }
    }

    public void createBlockRow(BLOCK type, bool isFirst = false, int verticalCnt = 1){
        //* Value
        float xs = blockPrefs[(int)type].transform.localScale.x;
        float spawnPosX = (type == BLOCK.Normal)? -5 : -3.1f;
        float middleGap = 0.5f; // センターのボールが来る隙間

        switch(type){
            case BLOCK.Normal : 
                for(int v=0; v<verticalCnt;v++){ //縦
                    int offsetCnt = 1;
                    Debug.Log("---------------------");
                    for(int h=0; h<MAX_HORIZONTAL_GRID;h++){ //横
                        var ins = blockPrefs[(int)type];

                        //* #1. Block Skip?
                        int rand = Random.Range(0,100);
                        if(rand < skipBlockPer) continue; //Skip Block

                        //* #2. Block TreasureChest?
                        rand = Random.Range(0,100);
                        if(rand < treasureChestBlockPer)   ins = blockPrefs[(int)BLOCK.TreasureChest];

                        //* #3. Block Normal + Newゲーム開始なのか、次のステージなのか？
                        float x = h < 3 ? (spawnPosX + h * xs) : (spawnPosX + h * xs + middleGap * offsetCnt);
                        float y = (isFirst)? 0 : ins.transform.position.y + gm.blockGroup.position.y;
                        float z = (isFirst)? -v : SPWAN_POS_Y;
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 setPos = (isFirst)? pos + gm.blockGroup.position : pos;
                        Instantiate(ins, setPos, Quaternion.identity, gm.blockGroup);
                    }
                }
                break;
            case BLOCK.Long : 
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
            createBlockRow(BLOCK.Long);
        }else{
            createBlockRow(BLOCK.Normal);
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
