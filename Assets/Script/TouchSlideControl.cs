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
    public BlockMaker bm;
    public LineRenderer line;

    public RectTransform pad;
    public RectTransform stick;
    private Vector3 dir;
    private Vector3 wallNormalVec;
    public GameObject hitBlockByBallPreview;
    
    private const int MIN_ARROW_DEG_Y = 30;
    private const int MAX_ARROW_DEG_Y = 150;

    //*Event
    public void OnDrag(PointerEventData eventData){
        if(gm.State != GameManager.STATE.WAIT) return;
        stick.position = eventData.position;

        //Stick動き制限
        stick.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.25f);
        
        //Stick角度  
        Vector2 dir = (stick.position - pad.gameObject.transform.position).normalized;
        float deg = convertDir2DegWithRange(dir, MIN_ARROW_DEG_Y, MAX_ARROW_DEG_Y);
        //* Stick(Arrow)角度によって、Player位置が自動で左右移動。
        // Debug.Log("OnDrag:: Stick(Arrow) Deg=" + deg + ", dir=" + dir + ", " + ((dir.x < 0)? "left" : "right").ToString());
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
        gm.isPointUp = true;
        pad.gameObject.SetActive(false);
        stick.localPosition = Vector2.zero;
        if(gm.State != GameManager.STATE.PLAY) return;
        pl.setAnimTrigger("Swing");
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------

    private void drawBallPreviewSphereCast(Transform arrowAnchorTf){
        RaycastHit hit;
        float radius = pl.ballPreviewSphere.GetComponent<SphereCollider>().radius * pl.ballPreviewSphere.transform.localScale.x;
        if(Physics.SphereCast(arrowAnchorTf.position, radius, arrowAnchorTf.forward, out hit, 1000, 1 << LayerMask.NameToLayer("BallPreview"))){
            //* BallPreview 1
            Vector3 cetner = hit.point + radius * hit.normal; //☆
            pl.ballPreviewSphere.transform.position = cetner;

            if(hit.transform.CompareTag("Wall")){
                //* Set 法線ベクトル
                wallNormalVec = (hit.transform.position.x < 0)? Vector3.right : Vector3.left;

                //* 反射角
                var originPos = arrowAnchorTf.GetChild(0).transform.position;
                var hitPos = pl.ballPreviewSphere.transform.position;
                var reflectVec = calcReflectVec(originPos, hitPos, wallNormalVec);
                Debug.DrawRay(hit.point, reflectVec, Color.red, 1);

                RaycastHit hit2;
                if(Physics.SphereCast(hit.point, radius, reflectVec, out hit2, 1000, 1 << LayerMask.NameToLayer("BallPreview"))){
                    //* BallPreview 2
                    Vector3 cetner2 = hit2.point + radius * hit2.normal; //☆
                    pl.ballPreviewSphere2.transform.position = cetner2;
                }

            }else{
                wallNormalVec = Vector3.zero;
            }
            // wallNormalVec = (hit.transform.CompareTag("Wall"))? (hit.transform.position.x < 0)? Vector3.right : Vector3.left : Vector3.zero;


            //* 🌟ColorBall ActiveSkill
            bool isColorBallSkill = gm.activeSkillBtnList.Exists(btn => btn.Trigger && btn.Name == DM.ATV.ColorBall.ToString());
            if(isColorBallSkill && hit.transform.CompareTag(BlockMaker.NORMAL_BLOCK)){
                Debug.Log(hit.transform.GetComponent<Block_Prefab>().kind);
                if(hit.transform.GetComponent<Block_Prefab>().kind == BlockMaker.BLOCK.TreasureChest){//* 宝箱は場外
                    return;
                }
                //* Hit Color
                var meshRd = hit.transform.gameObject.GetComponent<MeshRenderer>();
                Color hitColor = meshRd.material.GetColor("_ColorTint");

                //* Find Same Color Blocks
                var blocks = bm.GetComponentsInChildren<Block_Prefab>();
                var sameColorBlocks = Array.FindAll(blocks, bl =>
                    (bl.GetComponent<Block_Prefab>().kind != BlockMaker.BLOCK.TreasureChest) //* 宝箱は場外
                    && (bl.GetComponent<MeshRenderer>().material.GetColor("_ColorTint") == hitColor)
                );

                //* Glow Effect On
                bm.setGlowEF(sameColorBlocks, true);
                
                //* Reset
                if(hitBlockByBallPreview != hit.transform.gameObject){
                    bm.setGlowEF(blocks, false);
                }
                hitBlockByBallPreview = hit.transform.gameObject;
            }
        }
    }

    private void drawLinePreview(Transform arrowAnchorTf){
        var originPos = arrowAnchorTf.GetChild(0).transform.position;
        var hitPos = pl.ballPreviewSphere.transform.position;
        var hitPos2 = pl.ballPreviewSphere2.transform.position;

        line.SetPosition(0, originPos);
        line.SetPosition(1, hitPos);
        line.SetPosition(2, hitPos2);
    }

    private Vector3 calcReflectVec(Vector3 originPos, Vector3 hitPos, Vector3 wallNormalVec){
        //* ★原点
        Vector3 originalToMirrowVector = transform.position - originPos;

        //* 入射角
        Vector3 dir = hitPos - originPos;
        Vector3 incomingVec = dir.normalized;

        //* 法線ベクトル
        Vector3 normalVec = wallNormalVec;

        //* 反射角
        Vector3 reflectVec = hitPos + originalToMirrowVector.magnitude * Vector3.Reflect(incomingVec, normalVec).normalized;

        //* 結果
        Vector3 result = (normalVec == Vector3.zero)? hitPos : hitPos + reflectVec;
        return result;
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
