using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRangeGizmos : MonoBehaviour
{
    public float range;

    void OnDrawGizmos(){
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
}
