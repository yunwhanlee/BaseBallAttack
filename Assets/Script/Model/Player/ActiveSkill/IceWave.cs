using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    GameManager gm;
    BlockMaker bm;
    EffectManager em;
    void Start(){
        gm = DM.ins.gm;
        bm = gm.bm;
        em = gm.em;
    }
    
    void OnTriggerEnter(Collider col){
        if(col.name.Contains(DM.NAME.Block.ToString())){
            Debug.Log($"IceWave::OnTriggerEnter:: col.name= {col.name}, damage= {AtvSkill.IcewaveDmg}");
            if(col.name == "ItemBlockDirLineTrailEF") return;

            bm.decreaseBlockHP(col.gameObject, AtvSkill.IcewaveDmg);
            em.createCritTxtEF(col.transform.position, AtvSkill.IcewaveDmg);
            var block = col.GetComponent<Block_Prefab>();
            block.Freeze.IsOn = true;
            block.Freeze.BefCnt = -1;
        }
    }
}
