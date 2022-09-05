using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag(BlockMaker.NORMAL_BLOCK)){
            col.GetComponent<Block_Prefab>().decreaseHp(AtvSkill.ICEWAVE_DMG);
        }
    }
}
