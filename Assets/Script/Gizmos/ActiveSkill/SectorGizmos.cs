using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SectorGizmos : MonoBehaviour
{
    GameManager gm;
    TouchSlideControl touchSlideCtr;

    public float angleRange = 45f;
    public float distance = 5f;
    public bool isCollision = false;

    Color _blue = new Color(0f, 0f, 1f, 0.1f);
    Color _red = new Color(1f, 0f, 0f, 0.1f);

    public List<Transform> blockList;
    public List<Transform> hitList;
    Vector3 direction;
    [SerializeField]    float dotValue = 0f;
    public float offsetZ = 0.7f;

    Vector3 pos;

    void Start(){
        Debug.Log("SectorGizoms::Start():: DM.ins.gm= " + DM.ins.gm);
        gm = DM.ins.gm;
        touchSlideCtr = gm.touchSlideControlPanel;
        var childs = gm.bm.GetComponentsInChildren<Transform>();
        var blocks = Array.FindAll(childs, child => child.name.Contains("Block") && child.name != "BlockMaker");
        blockList = new List<Transform>(blocks);
        hitList = new List<Transform>();
    }

    void Update(){
        pos = new Vector3(transform.position.x, transform.position.y, transform.position.z - offsetZ);

        //* 🍧 IceWave BallPreviewSphere[0],[1]の角度　調整。
        if(this.transform.name.Contains(DM.ATV.IceWave.ToString())){
            float rotY = 0;
            if(touchSlideCtr.wallNormalVec == Vector3.zero){
                rotY = gm.pl.arrowAxisAnchor.transform.rotation.eulerAngles.y;
            }else{
                gm.pl.ballPreviewSphere[0].transform.LookAt(gm.pl.ballPreviewSphere[1].transform);
                Vector3 rot = gm.pl.ballPreviewSphere[0].transform.rotation.eulerAngles;
                rotY = rot.y;
            }
            this.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }

        if(gm.isPointUp){
            var childs = gm.blockGroup.GetComponentsInChildren<Transform>();
            var blocks = Array.FindAll(childs, child => child.name.Contains("Block") && child.name != "BlockGroup");
            blockList = new List<Transform>(blocks);
            blockList.ForEach(block =>block.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(false));
            hitList = new List<Transform>();

            dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));
            for(int i=0; i<blockList.Count; i++){
                direction = blockList[i].position - new Vector3(pos.x, pos.y, pos.z - offsetZ);
                if (direction.magnitude < distance){
                    if (Vector3.Dot(direction.normalized, this.transform.forward) > dotValue){
                        hitList.Add(blockList[i]);
                        blockList.RemoveAt(i);
                        --i;
                    }
                }
            }
            
            if(hitList.Count > 0)   isCollision = true;
            else                    isCollision = false;

            gm.isPointUp = false;

            hitList.ForEach(block =>{
                block.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(true);
            });
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos(){
        Handles.color = isCollision ? _red : _blue;
        Handles.DrawSolidArc(new Vector3(pos.x, pos.y, pos.z - offsetZ), Vector3.up, transform.forward, angleRange/2, distance);
        Handles.DrawSolidArc(new Vector3(pos.x, pos.y, pos.z - offsetZ), Vector3.up, transform.forward, -angleRange/2, distance);
    }
#endif
}
