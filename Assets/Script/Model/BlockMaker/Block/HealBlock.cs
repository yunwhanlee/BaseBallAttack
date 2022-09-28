using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBlock : Block_Prefab
{
    
    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(this.transform.position, HealRadius);
    }
}
