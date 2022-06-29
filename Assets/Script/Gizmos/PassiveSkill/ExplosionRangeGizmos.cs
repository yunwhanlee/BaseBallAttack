using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRangeGizmos : MonoBehaviour
{
    public Player pl;

    void Start(){
        pl = GameObject.Find("Player").GetComponent<Player>();
    }
    public float range;

    void OnDrawGizmos(){
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, pl.explosion.Value.range);
    }
}
