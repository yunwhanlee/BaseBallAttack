using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlockDirLineTrailEFPref : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.CompareTag("NormalBlock")){
            col.GetComponent<Block_Prefab>().decreaseHp(9999);
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.CompareTag("Wall")){
            Destroy(this.gameObject);
        }
    }
}
