using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileBallGizmos : ActiveSkillGizmos
{
#if UNITY_EDITOR
    protected override void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, width);
    }
#endif
}
