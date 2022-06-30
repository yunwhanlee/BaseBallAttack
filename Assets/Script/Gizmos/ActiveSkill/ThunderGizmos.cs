using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderGizmos : ActiveSkillGizmos
{
    protected override void OnDrawGizmos(){//* ThunderShot Skill Range Preview
        if(!isGizmosOn) return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = this.transform.localToWorldMatrix;//rotation
        const float offset = 1.35f;
        Gizmos.DrawWireCube(Vector3.zero + new Vector3(0,0,12),
            new Vector3(width * offset, width * offset, 20)
        );
    }
}
