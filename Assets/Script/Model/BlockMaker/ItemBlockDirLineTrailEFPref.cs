using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlockDirLineTrailEFPref : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.name.Contains(DM.NAME.Block.ToString())){
            if(col.transform.name.Contains(BlockMaker.KIND.Boss.ToString())) return;
            col.GetComponent<Block_Prefab>().decreaseHp(9999);
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.name.Contains(DM.TAG.Wall.ToString())){
            Destroy(this.gameObject);
        }
    }
}
