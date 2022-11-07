using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkOrb : MonoBehaviour
{
    //* OutSide
    GameManager gm; BlockMaker bm; Player pl; EffectManager em;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bm = gm.bm;
        pl = gm.pl;
        em = gm.em;
    }

    void Update(){
        var target = this.transform.parent.transform.position;
        this.transform.RotateAround(target, Vector3.up, Time.deltaTime * LM._.DARKORB_SPEED);
    }

    private void OnTriggerEnter(Collider col) {
        if(col.transform.name.Contains(DM.NAME.Block.ToString())){
            var block = col.GetComponent<Block_Prefab>();
            Debug.Log($"DarkOrb::OnTriggerEnter:: HIT->{block}");
            em.createDarkOrbHitEF(col.transform.position);
            bm.decreaseBlockHP(col.gameObject, pl.dmg.Val);
        }
    }
}
