using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlock : Block_Prefab
{    
    void Update(){
        base.Update();
    }

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        animator.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    public void activeBossSkill(){
        Debug.Log("<color=white>activeBossKill():: GO!!</color>");
    }
}
