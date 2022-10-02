using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossBlock : Block_Prefab{

    const float STONE_SCALE_X = 2.8f;

    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        animator.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    public void activeBossSkill(){
        Debug.Log("<color=white>activeBossKill()::</color>");
        const float unitX = BlockMaker.SCALE_X;
        const float gap = BlockMaker.CENTER_GAP;
        const float ofX = -5;
        Vector3[] posList = {
            new Vector3(ofX + (unitX * 0),0,-8), new Vector3(ofX + (unitX * 1),0,-8), new Vector3(ofX + (unitX * 2),0,-8), 
            new Vector3(gap + ofX + (unitX * 3),0,-8), new Vector3(gap + ofX + (unitX * 4),0,-8), new Vector3(gap + ofX + (unitX * 5),0,-8), 
            new Vector3(ofX + (unitX * 0),0,-9), new Vector3(ofX + (unitX * 1),0,-9), new Vector3(ofX + (unitX * 2),0,-9), 
            new Vector3(gap + ofX + (unitX * 3),0,-9), new Vector3(gap + ofX + (unitX * 4),0,-9), new Vector3(gap + ofX + (unitX * 5),0,-9), 
            new Vector3(ofX + (unitX * 0),0,-10), new Vector3(ofX + (unitX * 1),0,-10), new Vector3(ofX + (unitX * 2),0,-10), 
            new Vector3(gap + ofX + (unitX * 3),0,-10), new Vector3(gap + ofX + (unitX * 4),0,-10), new Vector3(gap + ofX + (unitX * 5),0,-10)
        };

        // int rand = Random.Range(0, posList.Length);
        // Instantiate(obstacleStonePf, posList[rand], Quaternion.identity, gm.blockGroup);

        Array.ForEach(posList, pos=> {
            Instantiate(obstacleStonePf, pos, Quaternion.identity, gm.blockGroup);
        });
    }
}
