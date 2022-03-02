using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaker : MonoBehaviour
{
    private const float WIDTH = 1.9f;
    private const float HEIGHT = 1;
    private const int MAX_HORIZONTAL_CNT = 6;
    private const int MAX_VERTICAL_CNT = 12;
    private const float START_POS_X = -5;

    public Transform blockBundle;
    public Material[] blockColorMts;
    public GameObject blockPref;

    void Start() {
        for(int v=0; v<MAX_VERTICAL_CNT;v++){
            int offsetCnt = 1;
            for(int h=0; h<MAX_HORIZONTAL_CNT;h++){
                if(h == 1) continue;
                float x = (h < 3)? START_POS_X + h * WIDTH : START_POS_X + h * WIDTH + 0.5f * offsetCnt;
                Vector3 pos = new Vector3(x, 0, -v);
                GameObject blockIns = Instantiate(blockPref, pos + blockBundle.position, Quaternion.identity, blockBundle);
                int randIdx = Random.Range(0, blockColorMts.Length);
                blockIns.GetComponent<Renderer>().material = blockColorMts[randIdx];
            }
        }
    }
}
