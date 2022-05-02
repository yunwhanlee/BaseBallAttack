using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderGizmos : MonoBehaviour
{
    public Player pl;
    void Start()
    {
        pl = GameObject.Find("Player").GetComponent<Player>();
    }
    void OnDrawGizmos(){
        //* ThunderShot Skill Range Preview        
        Gizmos.color = Color.yellow;
        Gizmos.matrix = this.transform.localToWorldMatrix;//rotation
        float w = pl.ThunderCastWidth;
        const float offset = 1.35f;
        Gizmos.DrawWireCube(Vector3.zero + new Vector3(0,0,12),
            new Vector3(w * offset,w * offset,20));
    }
}
