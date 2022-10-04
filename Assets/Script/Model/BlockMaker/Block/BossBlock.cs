using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BossBlock : Block_Prefab{

    const float STONE_SCALE_X = 2.8f;
    public int obstacleCnt = 2;

    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        animator.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    public void activeBossSkill(){
        createObstacleStone(obstacleCnt);


    }

    private void createObstacleStone(int cnt){
        Debug.Log("<color=red>activeBossKill():: createObstacleStone()</color>");
        const float UNIT_X = BlockMaker.SCALE_X;
        const float GAP_X = BlockMaker.CENTER_GAP;
        const float OFFSET_X = -5;
        const float OFFSET_Y = -8;

        //* OBSTACLE LIST 準備
        List<Vector3> obstaclePosList = new List<Vector3>(){};
        for(int i=0; i<18;i++){
            const int MASS_ROW_CNT = 6;
            int rowIdx = i % MASS_ROW_CNT;
            int lastIdx = MASS_ROW_CNT/2 - 1;
            float gap = (i % MASS_ROW_CNT) > lastIdx ? GAP_X : 0;

            float x = gap + OFFSET_X + (UNIT_X * rowIdx);
            int z = (int)(OFFSET_Y - (i / MASS_ROW_CNT));
            obstaclePosList.Add(new Vector3(x, 0, z));
        }

        //* OBSTACLE INS 生成
        for(int i=0; i< cnt; i++){
            int randIdx = Random.Range(0, obstaclePosList.Count);
            Instantiate(obstacleStonePf, obstaclePosList[randIdx], Quaternion.identity, gm.obstacleGroup);
            obstaclePosList.RemoveAt(randIdx);
        }

        //* DEBUG用。全体の位置確認。
        // obstaclePosList.ForEach(pos=> Instantiate(obstacleStonePf, pos, Quaternion.identity, gm.obstacleGroup));
    }
}
