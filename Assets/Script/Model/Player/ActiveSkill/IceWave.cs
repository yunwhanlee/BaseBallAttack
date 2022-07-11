using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("NormalBlock")){
            col.GetComponent<Block_Prefab>().decreaseHp(100);
        }
    }
}
