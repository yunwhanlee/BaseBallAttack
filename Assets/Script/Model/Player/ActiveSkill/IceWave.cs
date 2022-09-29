using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    GameManager gm;
    BlockMaker bm;
    EffectManager em;
    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bm = gm.bm;
        em = gm.em;
    }
    
    void OnTriggerEnter(Collider col){
        if(col.name.Contains(DM.NAME.Block.ToString())){
            bm.setDecreaseHP(col.gameObject, AtvSkill.ICEWAVE_DMG);
            em.createCritTxtEF(col.transform.position, AtvSkill.ICEWAVE_DMG);
        }
    }
}
