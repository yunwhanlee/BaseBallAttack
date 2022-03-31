using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaker : MonoBehaviour
{
    //* OutSide
    public GameManager gm;

    private const float WIDTH = 1.9f;
    private const float HEIGHT = 1;
    private const int MAX_HORIZONTAL_CNT = 6;
    private const int FIRST_CREATE_VERTICAL_CNT = 12; //DEAD_MAX-> 13
    private const float SPAWN_POS_X = -5;
    private const float SPWAN_POS_Y = -2;

    public int createPercent = 80;
    public Transform blockBundle;
    public GameObject blockPref;
    public bool isCreateBlock;

    public void Start() {
        //* Init
        var blocks = this.GetComponentsInChildren<Block_Prefab>();
        foreach(var block in blocks) block.onDestroy(true);
        this.transform.position = new Vector3(0, 0.5f, -2);
        createBlockLine(true, FIRST_CREATE_VERTICAL_CNT);
    }

    void Update(){
        if(isCreateBlock){
            isCreateBlock = false;
            moveDownBlock();
        }
    }

    public void setCreateBlockTrigger(bool trigger) => isCreateBlock = trigger;

    public void createBlockLine(bool isFirst = false, int verticalCnt = 1){
        for(int v=0; v<verticalCnt;v++){ //縦
            int offsetCnt = 1;
            for(int h=0; h<MAX_HORIZONTAL_CNT;h++){ //横
                //* ランダムで生成
                int rand = Random.Range(0,100);
                if(createPercent < rand) continue;
                float x = h < 3 ? (SPAWN_POS_X + h * WIDTH) : (SPAWN_POS_X + h * WIDTH + 0.5f * offsetCnt);

                //* Newゲーム開始なのか、次のステージなのか？
                GameObject blockIns;
                if(isFirst){
                    Vector3 pos = new Vector3(x, 0, -v);
                    blockIns = Instantiate(blockPref, pos + blockBundle.position, Quaternion.identity, blockBundle);
                }else{
                    Vector3 pos = new Vector3(x, blockBundle.position.y, SPWAN_POS_Y);
                    blockIns = Instantiate(blockPref, pos, Quaternion.identity, blockBundle);
                }
            }
        }
    }
    private void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓");
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
        createBlockLine();
    }
}
