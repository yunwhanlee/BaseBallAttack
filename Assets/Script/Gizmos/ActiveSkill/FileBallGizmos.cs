using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileBallGizmos : MonoBehaviour
{
    //* OutSide
    [SerializeField] GameManager gm;
    [SerializeField] float width;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.pl.FireBallCastWidth = width;
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("FileBallGizmos:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(true);
        }
    }
    void OnTriggerExit(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("FileBallGizmos:: OnTriggerExit:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(false);
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, width);
    }
}
