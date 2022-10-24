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
    public Vector3 wallNormalVec;
    public GameObject hitBlockByBallPreview;

    private bool isClickBattingSpot = false;
    
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

        if(isClickBattingSpot){

            return;
        }
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
        Debug.Log("");

        Vector3 touchPos = new Vector3(eventData.position.x, eventData.position.y, 100);
        Vector3 touchScreenPos = Camera.main.ScreenToWorldPoint(touchPos);
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        float maxDistance = 100;
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, touchScreenPos - ray.origin, maxDistance);
        Debug.DrawLine(ray.origin, touchScreenPos - ray.origin, Color.red, 1);

        Array.ForEach(hits, hit => {
            
            if(hit.transform.name == "BattingSpot"){
                Debug.Log("OnPointerDown::hit.tag= BattingSpot:: hit.transform.name=" + hit.transform.name);
                isClickBattingSpot = true;
                return;
            }
        });

        pad.position = eventData.position;
        pad.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData){
        gm.isPointUp = true;
        isClickBattingSpot = false;
        pad.gameObject.SetActive(false);
        stick.localPosition = Vector2.zero;
        if(gm.State != GameManager.STATE.PLAY) return;
        pl.setAnimTrigger("Swing");
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void drawBallPreviewSphereCast(Transform arrowAnchorTf){
        RaycastHit hit, hit2;
        float radius = pl.ballPreviewSphere[0].GetComponent<SphereCollider>().radius * pl.ballPreviewSphere[0].transform.localScale.x;
        if(Physics.SphereCast(arrowAnchorTf.position, radius, arrowAnchorTf.forward, out hit, 1000, 1 << LayerMask.NameToLayer(DM.LAYER.BallPreview.ToString()))){
            setBallPreviewCenterPos(ref pl.ballPreviewSphere[0], hit, radius);
            if(hit.transform.CompareTag(DM.TAG.Wall.ToString())){
                //* Set 法線ベクトル
                wallNormalVec = (hit.transform.position.x < 0)? Vector3.right : Vector3.left;

                //* 反射角
                var originPos = arrowAnchorTf.GetChild(0).transform.position;
                var hitPos = pl.ballPreviewSphere[0].transform.position;
                var reflectVec = calcReflectVec(originPos, hitPos, wallNormalVec);
                // Debug.DrawRay(hit.point, reflectVec, Color.red, 1);

                // RaycastHit
                if(Physics.SphereCast(hit.point, radius, reflectVec, out hit2, 1000, 1 << LayerMask.NameToLayer(DM.LAYER.BallPreview.ToString()))){
                    setBallPreviewCenterPos(ref pl.ballPreviewSphere[1], hit2, radius);
                }
                
                //* 🌟ColorBall ActiveSkill
                if(hit2.transform)
                    gm.activeSkillDataBase[0].setColorBallSkillGlowEF(gm, ref bm, hit2, ref hitBlockByBallPreview);
                return;
            }
            else{
                wallNormalVec = Vector3.zero;
            }
            //* 🌟ColorBall ActiveSkill
            gm.activeSkillDataBase[0].setColorBallSkillGlowEF(gm, ref bm, hit, ref hitBlockByBallPreview);
            
        }
    }
    private void setBallPreviewCenterPos(ref GameObject ballPrevObj, RaycastHit hit, float radius){
        Vector3 center = hit.point + radius * hit.normal;
        ballPrevObj.transform.position = center;
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
        return (normalVec == Vector3.zero)? hitPos : hitPos + reflectVec;
    }

    private void drawLinePreview(Transform arrowAnchorTf){
        var originPos = arrowAnchorTf.GetChild(0).transform.position;
        var hitPos = pl.ballPreviewSphere[0].transform.position;
        var hitPos2 = pl.ballPreviewSphere[1].transform.position;

        line.SetPosition(0, originPos);
        line.SetPosition(1, hitPos);
        line.SetPosition(2, (wallNormalVec == Vector3.zero)? hitPos : hitPos2);

        //* Second BallPreviewSphere 活性化
        pl.ballPreviewSphere[1].SetActive((wallNormalVec == Vector3.zero)? false : true);
        bool is2ndBallPreviewOn = pl.ballPreviewSphere[1].activeSelf;

        pl.ballPreviewSphere[0].SetActive(is2ndBallPreviewOn? false : true);
        pl.ballPreviewSphere[1].SetActive(is2ndBallPreviewOn? true : false);
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
