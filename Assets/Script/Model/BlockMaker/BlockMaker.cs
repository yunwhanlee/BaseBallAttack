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
    private const int MAX_VERTICAL_LINE_CNT = 7;
    private const float SPAWN_POS_X = -5;
    private const float SPWAN_POS_Y = -2;

    public int createPercent = 80;
    public Transform blockBundle;
    public Material[] blockColorMts;
    public GameObject blockPref;
    public bool isCreateBlock;

    void Start() {
        for(int v=0; v<MAX_VERTICAL_LINE_CNT;v++){ //縦
            int offsetCnt = 1;
            for(int h=0; h<MAX_HORIZONTAL_CNT;h++){ //横
                //* ランダムで生成
                int rand = Random.Range(0,100);
                if(createPercent < rand) continue;

                float x = h < 3 ? (SPAWN_POS_X + h * WIDTH) : (SPAWN_POS_X + h * WIDTH + 0.5f * offsetCnt);
                Vector3 pos = new Vector3(x, 0, -v);
                GameObject blockIns = Instantiate(blockPref, pos + blockBundle.position, Quaternion.identity, blockBundle);
                int randIdx = Random.Range(0, blockColorMts.Length);
                blockIns.GetComponent<Renderer>().material = blockColorMts[randIdx];
            }
        }
    }
    void Update(){
        if(isCreateBlock){
            isCreateBlock = false;
            moveDownBlock();
        }
    }

    public void setCreateBlock(bool boolen){
        isCreateBlock = boolen;
    }

    private void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓");
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);

        //* Create BlockLine
        int offsetCnt = 1;
        for(int h=0; h<MAX_HORIZONTAL_CNT;h++){ //横
                //* ランダムで生成
                int rand = Random.Range(0,100);
                if(createPercent < rand) continue;

                float x = h < 3 ? (SPAWN_POS_X + h * WIDTH) : (SPAWN_POS_X + h * WIDTH + 0.5f * offsetCnt);
                Vector3 pos = new Vector3(x, blockBundle.position.y, SPWAN_POS_Y);
                GameObject blockIns = Instantiate(blockPref, pos, Quaternion.identity, blockBundle);
                int randIdx = Random.Range(0, blockColorMts.Length);
                blockIns.GetComponent<Renderer>().material = blockColorMts[randIdx];
        }
    }
}
