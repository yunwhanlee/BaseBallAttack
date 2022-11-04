using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverLapCheckBoxCollider : MonoBehaviour
{
    [SerializeField] bool isMoved = false;   public bool IsMoved {get => isMoved; set => isMoved = value;}

    private void OnTriggerEnter(Collider col) {
        if(col.CompareTag(DM.TAG.NormalBlock.ToString())
            || col.CompareTag(DM.TAG.LongBlock.ToString())
            || col.CompareTag(DM.TAG.TreasureChestBlock.ToString())
            || col.CompareTag(DM.TAG.HealBlock.ToString())){

                //* 対象をFreezeブロックは除外。
                if(col.GetComponent<Block_Prefab>().Freeze.IsOn){
                    Debug.Log("<color=grey>OverLapCheckBoxCollider col.name= " + col.name + ", pos.z= " + (col.transform.position.z) + "</color>");
                    return;
                }
                else{
                    Debug.Log("<color=green>OverLapCheckBoxCollider col.name= " + col.name + ", pos.z= " + (col.transform.position.z) + "</color>");
                }

                //* Up PosZ
                if(!col.GetComponentInChildren<OverLapCheckBoxCollider>().IsMoved){
                    col.GetComponentInChildren<OverLapCheckBoxCollider>().IsMoved = true;
                    StartCoroutine(coDelaySetPos(col));
                }
            }
    }

    IEnumerator coDelaySetPos(Collider col){
        yield return new WaitForSeconds(0.75f);
        col.transform.position = new Vector3(
            col.transform.position.x, col.transform.position.y, col.transform.position.z + 1
        );
        Debug.Log("<color=yellow>OverLapCheckBoxCollider z+1 col.name= " + col.name + ", pos.z= " + (col.transform.position.z - BlockMaker.OFFSET_POS_Z) + "</color>");
        if(col.transform.position.z - BlockMaker.OFFSET_POS_Z >= 1)
            Destroy(col.transform.gameObject, 0.5f);
    }
}
