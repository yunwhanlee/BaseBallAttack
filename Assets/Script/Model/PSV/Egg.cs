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
            SM.ins.sfxPlay(SM.SFX.EggPop.ToString());
            em.createEggPopEF(this.transform.position);
            Util._.sphereCastAllDecreaseBlocksHp(this.transform, explosionRange, pl.dmg.Val * 3);

            Destroy(this.gameObject);
        }
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, explosionRange);
    }
}
