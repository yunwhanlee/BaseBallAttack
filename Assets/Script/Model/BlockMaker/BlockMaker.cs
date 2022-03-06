using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaker : MonoBehaviour
{
    private const float WIDTH = 1.9f;
    private const float HEIGHT = 1;
    private const int MAX_HORIZONTAL_CNT = 6;
    private const int MAX_VERTICAL_LINE_CNT = 3;
    private const float SPAWN_POS_X = -5;
    private const float SPWAN_POS_Y = -2;

    [SerializeField] int moveDownSpan = 10;
    [SerializeField] int createPercent = 70;
    public Transform blockBundle;
    public Material[] blockColorMts;
    public GameObject blockPref;

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

        InvokeRepeating("moveDownBlock", moveDownSpan, moveDownSpan);
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
