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
            bm.decreaseBlockHP(col.gameObject, 1);//AtvSkill.ICEWAVE_DMG);
            em.createCritTxtEF(col.transform.position, 1);//AtvSkill.ICEWAVE_DMG);
            var block = col.GetComponent<Block_Prefab>();
            block.IsFreeze = true;
        }
    }
}
