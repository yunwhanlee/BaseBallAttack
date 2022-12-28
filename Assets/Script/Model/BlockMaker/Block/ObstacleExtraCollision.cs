using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleExtraCollision : MonoBehaviour
{
    /* Collision Info 
    * ME 
        rigid(X)    => ない。
        collider(O) => isTrigger = true
    * BLOCK
        rigid(O)    => isKinematic = true 
        collider(O) => isTrigger = false
    */

    EffectManager em;

    void Start() {
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
    }

    void OnTriggerEnter(Collider col) {
        //* (BUG-21) Obstacleへ貼ってあるメインコライダーとは衝突しないように。
        if(col.transform.name.Contains(DM.NAME.Obstacle.ToString())) return;

        Debug.Log($"ObstacleExtraCollision:: OnTriggerEnter:: col= {col.name}");

        //* (BUG-21) ObstacleがBlockに当たっても、破壊されないこと対応。
        if(col.transform.name.Contains(DM.NAME.Block.ToString())){
            em.createRockObstacleBrokenEF(this.transform.position);
            Destroy(this.transform.parent.gameObject);
        }
    }
}