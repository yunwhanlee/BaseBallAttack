using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TouchSlideControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{   //*2D Screen *//
    //* OutSide
    public GameManager gm;
    public Player pl;
    public LineRenderer line;

    public RectTransform pad;
    public RectTransform stick;
    private Vector3 dir;
    public GameObject hitBlockByBallPreview;
    private const int MIN_ARROW_DEG_Y = 30;
    private const int MAX_ARROW_DEG_Y = 150;

    //*Event
    public void OnDrag(PointerEventData eventData){
        if(gm.state != GameManager.State.WAIT) return;
        stick.position = eventData.position;

        //Stick動き制限
        stick.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.25f);
        
        //Stick角度  
        Vector2 dir = (stick.position - pad.gameObject.transform.position).normalized;
        float deg = convertDir2DegWithRange(dir, MIN_ARROW_DEG_Y, MAX_ARROW_DEG_Y);
        //* Stick(Arrow)角度によって、Player位置が自動で左右移動。
        Debug.Log("OnDrag:: Stick(Arrow) Deg=" + deg + ", dir=" + dir + ", " + ((dir.x < 0)? "left" : "right").ToString());
        Transform player = pl.gameObject.transform;
        if(dir.x < 0){
            //Right
            player.position = new Vector3(+Mathf.Abs(player.position.x), player.position.y, player.position.z);
            player.localScale = new Vector3(-Mathf.Abs(player.localScale.x),player.localScale.y,player.localScale.z);
        }
        else{
            //Left
            player.position = new Vector3(-Mathf.Abs(player.position.x), player.position.y, player.position.z);
            player.localScale = new Vector3(+Mathf.Abs(player.localScale.x),player.localScale.y,player.localScale.z);
        }
        
        //Player矢印(Arrow)角度に適用
        const int offsetDeg2DTo3D = 90;
        pl.arrowAxisAnchor.transform.rotation = Quaternion.Euler(0,offsetDeg2DTo3D - deg, 0);
        
        //* Draw Preview
        Transform arrowAnchorTf = pl.arrowAxisAnchor.transform;
        drawBallPreviewSphereCast(arrowAnchorTf);
        drawLinePreview(arrowAnchorTf);

        
    }

    public void OnPointerDown(PointerEventData eventData){
        pad.position = eventData.position;
        pad.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData){
        pad.gameObject.SetActive(false);
        stick.localPosition = Vector2.zero;
        if(gm.state != GameManager.State.PLAY) return;
        pl.setAnimTrigger("Swing");
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void drawBallPreviewSphereCast(Transform arrowAnchorTf){
        RaycastHit hit;
        float radius = pl.ballPreviewSphere.GetComponent<SphereCollider>().radius * pl.ballPreviewSphere.transform.localScale.x;
        if(Physics.SphereCast(arrowAnchorTf.position, radius, arrowAnchorTf.forward, out hit, 1000, 1 << LayerMask.NameToLayer("BallPreview"))){
            Vector3 cetner = hit.point + radius * hit.normal; //☆
            pl.ballPreviewSphere.transform.position = cetner;
            // Debug.DrawRay(arrowTf.position, arrowTf.forward * 1000, Color.red, 1);

            //* ColorBall ActiveSkill
            bool isColorBallSkill = gm.activeSkillBtnList.Exists(btn => btn.Trigger && btn.Name == "ColorBall");
            if(isColorBallSkill && hit.transform.CompareTag("NormalBlock")){
                //* Hit Color
                var meshRd = hit.transform.gameObject.GetComponent<MeshRenderer>();
                Color hitColor = meshRd.material.GetColor("_ColorTint");

                //* Find Same Color Blocks
                var blocks = gm.blockMaker.GetComponentsInChildren<Block_Prefab>();
                var sameColorBlocks = Array.FindAll(blocks, bl => 
                    bl.GetComponent<MeshRenderer>().material.GetColor("_ColorTint") == hitColor
                );

                //* Glow Effect On
                Array.ForEach(sameColorBlocks, bl => {
                    bl.setEnabledSpriteGlowEF(true);
                });
                
                //* Reset
                if(hitBlockByBallPreview != hit.transform.gameObject){
                    Array.ForEach(blocks, bl => bl.setEnabledSpriteGlowEF(false));
                }
                hitBlockByBallPreview = hit.transform.gameObject;
            }
        }
    }

    private void drawLinePreview(Transform arrowAnchorTf){
        var arrowPos = arrowAnchorTf.GetChild(0).transform.position;
        line.SetPosition(0, arrowPos);
        line.SetPosition(1, pl.ballPreviewSphere.transform.position);
    }

    private float convertDir2DegWithRange(Vector3 dir, int min, int max){
        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //*(BUG) Clamp Deg with cur Direction
        deg = Mathf.Clamp(deg, MIN_ARROW_DEG_Y, MAX_ARROW_DEG_Y); //! (BUG-2) 角度が－値になるとClampはminを返す。
        const int LEFT = -1; //const int RIGHT = +1;
        var dirSign = Mathf.Sign(dir.x);
        return deg = deg == max || (deg == min && dirSign == LEFT) ? max : deg;
    }
}
