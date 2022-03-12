using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamResolution : MonoBehaviour
{   
    //* OutSide Component
    public GameManager gm;
    public Player player;


    //* Inside Component 
    Camera cam;

    //value
    const int DEVICE_WIDTH = 9;
    const int DEVICE_HEIGHT = 16;

    void Awake() {
        setAutoDeviceCamRatio(DEVICE_WIDTH, DEVICE_HEIGHT);
    }

    void Update() {
        if(gm.state != GameManager.State.WAIT) return;
        if(Input.GetMouseButtonDown(0)){
            //* RAY CAST
            const int maxDistance = 50;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(this.transform.position, ray.direction, maxDistance);
            System.Array.Reverse(hits);//(BUG) hitsが逆にIdx順番で帰ることを正しく直す。

            //* 処理
            Transform playerTf = player.gameObject.transform;
            int idx=0;
            foreach (RaycastHit hit in hits){
                Debug.Log("idx="+idx+", hit.collider.tag="+hit.collider.tag);
                switch(hit.collider.tag){
                    case "LeftPosPad": //* 左パッドへプレイヤー配置
                        playerTf.position = new Vector3(-Mathf.Abs(playerTf.position.x), playerTf.position.y, playerTf.position.z);
                        playerTf.localScale = new Vector3(+Mathf.Abs(playerTf.localScale.x),playerTf.localScale.y,playerTf.localScale.z);
                        return;
                    case "RightPosPad": //* 右パッドへプレイヤー配置
                        playerTf.position = new Vector3(+Mathf.Abs(playerTf.position.x), playerTf.position.y, playerTf.position.z);
                        playerTf.localScale = new Vector3(-Mathf.Abs(playerTf.localScale.x),playerTf.localScale.y,playerTf.localScale.z);
                        return;
                }
                idx++;
            };
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 0.5f);
        }
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void setAutoDeviceCamRatio(int w, int h){
        cam = GetComponent<Camera>();
        Rect rect = cam.rect;
        float scaleH = ((float)Screen.width / Screen.height) / ((float)DEVICE_WIDTH / DEVICE_HEIGHT); // (横 / 縦)
        float scaleW = 1f / scaleH;

        //* calculate
        if(scaleH < 1){//上・下が超える
            rect.height = scaleH;
            rect.y = (1f - scaleH) / 2f;
        }else{//左・右が超える
            rect.width = scaleW;
            rect.x = (1f - scaleW) / 2f;
        }

        cam.rect = rect;//apply
    }
    //---------------------------------------
    
}
