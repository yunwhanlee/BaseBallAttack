using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    EffectManager em;
    void Start()
    {
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
    }
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag(BlockMaker.BLOCK)){
            col.GetComponent<Block_Prefab>().decreaseHp(AtvSkill.ICEWAVE_DMG);
            em.createCritTxtEF(col.transform.position, AtvSkill.ICEWAVE_DMG);
        }
    }
}
