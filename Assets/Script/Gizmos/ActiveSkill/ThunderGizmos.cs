using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderGizmos : MonoBehaviour
{
    [SerializeField]  Player pl;
    [SerializeField] float width;
    [SerializeField] bool isGizmosOn;
    void Start()
    {
        pl = GameObject.Find("Player").GetComponent<Player>();
        pl.ThunderCastWidth = width;
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("ThunderGizmos:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(true);
        }
    }
    void OnTriggerExit(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("ThunderGizmos:: OnTriggerExit:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(false);
        }
    }

    void OnDrawGizmos(){//* ThunderShot Skill Range Preview
        if(!isGizmosOn) return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = this.transform.localToWorldMatrix;//rotation
        const float offset = 1.35f;
        Gizmos.DrawWireCube(Vector3.zero + new Vector3(0,0,12),
            new Vector3(width * offset, width * offset, 20)
        );
    }
}
