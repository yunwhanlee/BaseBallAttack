using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSmokeGizmos : ActiveSkillGizmos
{
    protected override void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, width);
    }
}