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

    //* Player BattingSpot Moving 
    private bool isClickBattingSpot = false;
    
    private const int MIN_ARROW_DEG_Y = 30;
    private const int MAX_ARROW_DEG_Y = 150;
    private const float MIN_PLAYER_TF_POS_X = -4.3f;
    private const float MAX_PLAYER_TF_POS_X = 1.7f;

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
        Transform arrowAnchorTf = pl.arrowAxisAnchor.transform;
        Transform playerTf = pl.gameObject.transform;
        float standardPosX = playerTf.position.x;
        if(isClickBattingSpot){ //* Player BattingSpot Translate
            float posRatioX = (stick.localPosition.x / pad.rect.width * 0.4f) * 10;
            Debug.Log("stick.localPosition.x->" + stick.localPosition.x + ", pad.rect.width/4->" + pad.rect.width/4 + ", res=>" + (stick.localPosition.x / pad.rect.width * 0.4f) * 10);
            float playerTfPosX = 0;

            //* Set PlayerTf Position X
            const int SPLIT_CNT = 3;
            const float BEGIN_POS_X = -1;
            const float STICK_RANGE = 2;
            const int LEFT = 0, CENTER = 1, RIGHT = 2;

            float value = (STICK_RANGE / SPLIT_CNT);
            float[] posXArr = new float[3];
            float middlePosX = (STICK_RANGE - value);

            for(int i = 0; i < SPLIT_CNT; i++){
                posXArr[i] = BEGIN_POS_X + value * (i + 1);
                Debug.Log("rangeArr=" + posXArr[i]);
            }
            
            if(BEGIN_POS_X <= posRatioX && posRatioX < posXArr[LEFT]){ //* Right
                playerTfPosX = MIN_PLAYER_TF_POS_X - middlePosX;
                gm.cam2.transform.position = new Vector3(-3.0f, gm.cam2.transform.position.y, gm.cam2.transform.position.z);
            }
            else if(posXArr[LEFT] <= posRatioX && posRatioX < posXArr[CENTER]){ //* Center
                playerTfPosX = 0;
                gm.cam2.transform.position = new Vector3(0, gm.cam2.transform.position.y, gm.cam2.transform.position.z);
            }
            else if(posXArr[CENTER] <= posRatioX && posRatioX < posXArr[RIGHT]){ //* Left
                playerTfPosX = MAX_PLAYER_TF_POS_X + middlePosX;
                gm.cam2.transform.position = new Vector3(3.0f, gm.cam2.transform.position.y, gm.cam2.transform.position.z);
            }
            
            const float offsetX = -1.3f;
            playerTf.transform.position = new Vector3(Mathf.Clamp(playerTfPosX + offsetX, MIN_PLAYER_TF_POS_X, MAX_PLAYER_TF_POS_X), playerTf.position.y, playerTf.position.z);
            drawBallPreviewSphereCast(arrowAnchorTf);
            drawLinePreview(arrowAnchorTf);
            return;
        }
        else{ //* Set Player Model Moving Transform
            if(dir.x < 0){
                //Right
                pl.modelMovingTf.localPosition = new Vector3(3, 0, 0);
                pl.modelMovingTf.localScale = new Vector3(-Mathf.Abs(pl.modelMovingTf.localScale.x), pl.modelMovingTf.localScale.y, pl.modelMovingTf.localScale.z);
            }
            else{
                //Left
                pl.modelMovingTf.localPosition = new Vector3(0, 0, 0);
                pl.modelMovingTf.localScale = new Vector3(+Mathf.Abs(pl.modelMovingTf.localScale.x), pl.modelMovingTf.localScale.y, pl.modelMovingTf.localScale.z);
            }
            
            //* Player矢印(Arrow)角度に適用
            const int offsetDeg2DTo3D = 90;
            pl.arrowAxisAnchor.transform.rotation = Quaternion.Euler(0,offsetDeg2DTo3D - deg, 0);
            
            //* Draw Preview
            drawBallPreviewSphereCast(arrowAnchorTf);
            drawLinePreview(arrowAnchorTf);
        }
    }

    public void OnPointerDown(PointerEventData eventData){
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
                Transform playerTf = pl.gameObject.transform;
                Animator playerTfAnim = playerTf.GetComponent<Animator>();
                playerTfAnim.SetTrigger("Touch");
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
