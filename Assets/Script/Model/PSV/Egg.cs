using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Egg : MonoBehaviour
{    
    //* OutSide
    GameManager gm; BlockMaker bm; Player pl; EffectManager em;

    [SerializeField] float explosionRange = 3;
    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bm = gm.bm;
        pl = gm.pl;
        em = gm.em;
    }

    void OnCollisionEnter(Collision col){
        if(col.transform.name.Contains(DM.NAME.Block.ToString())){
            Debug.Log("EGG POP!");
            //* Explosion
            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, explosionRange, Vector3.up, 0);
            Array.ForEach(hits, hit => {
                if(hit.transform.name.Contains(DM.NAME.Block.ToString())){
                    var block = hit.transform.gameObject.GetComponent<Block_Prefab>();
                    int dmg = pl.dmg.Val;
                    block.decreaseHp(dmg);
                    em.createCritTxtEF(hit.transform.position, dmg);
                    em.createEggPopEF(hit.transform.position);
                }
            });

            Destroy(this.gameObject);
        }
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, explosionRange);
    }
}
