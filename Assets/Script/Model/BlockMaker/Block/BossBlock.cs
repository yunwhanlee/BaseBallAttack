using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlock : Block_Prefab{
    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        animator.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    public void activeBossSkill(){
        Debug.Log("<color=white>activeBossKill()::</color>");
        const float unitX = BlockMaker.SCALE_X;
        const float offsetX = -5;
        Vector3[] posList = {
            new Vector3(offsetX + (unitX * 0),0,-8), new Vector3(offsetX + (unitX * 1),0,-8), new Vector3(offsetX + (unitX * 2),0,-8), 
            new Vector3(offsetX + (unitX * 3),0,-8), new Vector3(offsetX + (unitX * 4),0,-8), new Vector3(offsetX + (unitX * 5),0,-8), 
            new Vector3(offsetX + (unitX * 0),0,-9), new Vector3(offsetX + (unitX * 1),0,-9), new Vector3(offsetX + (unitX * 2),0,-9), 
            new Vector3(offsetX + (unitX * 3),0,-9), new Vector3(offsetX + (unitX * 4),0,-9), new Vector3(offsetX + (unitX * 5),0,-9), 
            new Vector3(offsetX + (unitX * 0),0,-10), new Vector3(offsetX + (unitX * 1),0,-10), new Vector3(offsetX + (unitX * 2),0,-10), 
            new Vector3(offsetX + (unitX * 3),0,-10), new Vector3(offsetX + (unitX * 4),0,-10), new Vector3(offsetX + (unitX * 5),0,-10)
        };

        int rand = Random.Range(0, posList.Length);


        Instantiate(obstacleStonePf, posList[rand], Quaternion.identity, gm.blockGroup);
    }
}
