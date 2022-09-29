using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlockDirLineTrailEFPref : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if(col.transform.name.Contains(DM.NAME.Block.ToString())){
            if(col.transform.CompareTag(DM.TAG.BossBlock.ToString())) return;
            if(col == this) return;
            Debug.Log("OnTriggerEnter= " + col.name);
            col.GetComponent<Block_Prefab>().decreaseHp(9999);
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.transform.name.Contains(DM.TAG.Wall.ToString())){
            Destroy(this.gameObject);
        }
    }
}
