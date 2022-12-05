using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverLapCheckBoxCollider : MonoBehaviour
{

    GameManager gm;
    [SerializeField] bool isMoved = false;   public bool IsMoved {get => isMoved; set => isMoved = value;}

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider col) {
        // if(col.CompareTag(DM.TAG.NormalBlock.ToString()) || col.CompareTag(DM.TAG.LongBlock.ToString())|| col.CompareTag(DM.TAG.TreasureChestBlock.ToString())|| col.CompareTag(DM.TAG.HealBlock.ToString()))
        if(Util._.isColBlockOrObstacle(col)){
            //* 対象をFreezeブロックは除外。
            if(col.GetComponent<Block_Prefab>().Freeze.IsOn){
                Debug.Log("<color=grey>OverLapCheckBoxCollider col.name= " + col.name + ", pos.z= " + (col.transform.position.z) + "</color>");
                return;
            }
            else{
                Debug.Log("<color=green>this="+ this.transform + "::OverLapCheckBoxCollider col.name= " + col.name + ", pos.z= " + (col.transform.position.z) + "</color>");
            }

            //* Up PosZ
            var overLapColliderObj = col.GetComponentInChildren<OverLapCheckBoxCollider>();
            if(overLapColliderObj && !overLapColliderObj.IsMoved){
                //* ぶつかったのが障害物なら、障害物を破壊して終了。
                if(this.transform.parent.name.Contains(DM.NAME.Obstacle.ToString())){
                    gm.em.createRockObstacleBrokenEF(this.transform.position);
                    Destroy(this.transform.parent.gameObject);
                    return;
                }
                //* 上に移動して戻す。
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
