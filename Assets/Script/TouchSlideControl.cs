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
    enum POS_X {LEFT, CENTER, RIGHT};
    const float BEGIN_POS_X = -1;
    const float STICK_RANGE = 2;
    const float MIN_PL_TF_POS_X = -4.3f;
    const float MAX_PL_TF_POS_X = 1.7f;
    const int SPLIT_CNT = 3;
    bool isClickBattingSpot = false;
    float playerOffsetX;
    public List<Material> modelOriginMtList;
    public MeshRenderer[] plTfMeshRdrs;
    public Material onClickedMt;

    //* Arrow Axis
    public int offsetDeg2DTo3D = 60;
    public int MIN_ARROW_DEG_Y = 0;
    public int MAX_ARROW_DEG_Y = 120;

    void Start(){
        playerOffsetX = pl.transform.position.x;
    }
    
    //*Event
    public void OnDrag(PointerEventData eventData){
        if(gm.State != GameManager.STATE.WAIT) return;
        if(gm.IsPlayingAnim) return;
        if(pl.IsStun) return;

        Transform playerTf = pl.gameObject.transform;
        Transform arrowAnchorTf = pl.arrowAxisAnchor.transform;

        //*タッチした位置代入
        stick.position = eventData.position;

        //* Stick動き制限
        stick.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.5f);// * 0.25f);

        Vector2 movingDir = (stick.position - pad.gameObject.transform.position);
        
        if(isClickBattingSpot){
            //* Move Player Space
            movePlayerSpace(playerTf, movingDir);
        }
        else{
            //* Rotate Arrow
            moveModelTf(movingDir.normalized);
            rotateArrowTf(movingDir);
        }

        //* Draw Preview
        drawBallPreviewSphereCast(arrowAnchorTf);
        drawLinePreview(arrowAnchorTf);
    }

    public void OnPointerDown(PointerEventData eventData){
        if(gm.State != GameManager.STATE.WAIT) return;
        if(gm.bs.IsBallExist) return;
        if(gm.IsPlayingAnim) return;
        if(pl.IsStun) {
            pad.gameObject.SetActive(false);
            return;
        }

        Debug.Log("OnPointerDown::");

        pad.position = eventData.position;
        pad.gameObject.SetActive(true);

        Vector3 touchPos = new Vector3(eventData.position.x, eventData.position.y, 100);
        if(Util._.checkRayHitTagIsExist(touchPos, DM.TAG.PlayerBattingSpot.ToString())){
            //* 邪魔するボタンUI非活性化
            gm.readyBtn.gameObject.SetActive(false);
            gm.activeSkillBtnGroup.gameObject.SetActive(false);

            Transform playerTf = pl.gameObject.transform;
            List<MeshRenderer> filterList = new List<MeshRenderer>();
            modelOriginMtList = new List<Material>();
            plTfMeshRdrs = playerTf.GetComponentsInChildren<MeshRenderer>();

            isClickBattingSpot = true;
            pl.setAnimTrigger(DM.ANIM.Touch.ToString());

            string[] exceptStrArr = new string[] {
                DM.NAME.BallPreview.ToString(), 
                DM.NAME.Box001.ToString(), 
                DM.NAME.Area.ToString()
            };

            //* 例外Object フィルターリング
            Array.ForEach(plTfMeshRdrs, meshRdr => {
                if(Array.TrueForAll(exceptStrArr, exceptStr => !meshRdr.name.Contains(exceptStr)))
                    filterList.Add(meshRdr);
            });
            plTfMeshRdrs = filterList.ToArray(); 

            //* Originマテリアル保存
            Array.ForEach(plTfMeshRdrs, meshRdr => {
                modelOriginMtList.Add(meshRdr.material);
            });

            //* クリックONモードマテリアルに変更
            Array.ForEach(plTfMeshRdrs, meshRdr => {
                meshRdr.material = onClickedMt;
            });
        }
    }
    public void OnPointerUp(PointerEventData eventData){
        if(gm.IsPlayingAnim) return;
        if(pl.IsStun) {
            backOriginPlayerMeshRdr();
            return;
        }

        //* ボタンUI 活性化
        gm.readyBtn.gameObject.SetActive(true);
        gm.activeSkillBtnGroup.gameObject.SetActive(true);
        
        backOriginPlayerMeshRdr();

        gm.isPointUp = true;
        isClickBattingSpot = false;
        pad.gameObject.SetActive(false);
        stick.localPosition = Vector2.zero;
        if(gm.State != GameManager.STATE.PLAY) return;
        pl.setAnimTrigger(DM.ANIM.Swing.ToString());
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void backOriginPlayerMeshRdr(){
        //* 基のモデルマテリアルに戻す
        int i = 0;
        Array.ForEach(plTfMeshRdrs, meshRdr => {
            // Debug.Log($"OnPointerUp:: {i}:「{meshRdr.transform.gameObject.name}」 ⇐ {modelOriginMtList[i].name} ");
            meshRdr.material = modelOriginMtList[i++];
        });
    }
    private bool checkRayHitTagIsExist(Vector3 touchPos, string findTagName){
        float maxDistance = 100;
        Debug.Log($"checkRayHitTagIsExist:: findTagName={findTagName}");
        Vector3 touchScreenPos = Camera.main.ScreenToWorldPoint(touchPos);
        Ray ray = Camera.main.ScreenPointToRay(touchPos);

        //* Shoot Ray
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, touchScreenPos - ray.origin, maxDistance);
        Debug.DrawLine(ray.origin, touchScreenPos - ray.origin, Color.red, 1);

        //* check Is Exist
        return Array.Exists(hits, hit => hit.transform.CompareTag(findTagName));
    }

    private void movePlayerSpace(Transform playerTf, Vector2 dir){
        float normalX = dir.x / (pad.rect.width * 0.5f); // -1 ~ 1

        const int LEFT = 0, MIDDLE = 1, RIGHT = 2;
        const int SplitCnt = 3;
        const float AmountDist = 2, MinPosX = -1;
        float distUnit = AmountDist / SplitCnt;

        float[] posArr = new float[3];
        float[] plTfPosArr = {-4.3f, -1.3f, 1.8f};
        float[] cam2TfPosArr = {-3, 0, 3.1f};
        for(int i=0; i<3; i++) posArr[i] = MinPosX + distUnit * (i+1);
        // Debug.Log($"movePlayerSpace:: normalX= {Util._.setNumDP(normalX,2)}, posArr=[{Util._.setNumDP(posArr[0],2)}, {Util._.setNumDP(posArr[1],2)}, {Util._.setNumDP(posArr[2],2)}]");

        if(MinPosX < normalX && normalX <= posArr[LEFT]){
            playerTf.transform.position = new Vector3(plTfPosArr[LEFT], playerTf.transform.position.y, playerTf.transform.position.z);
            gm.cam2.transform.position = new Vector3(cam2TfPosArr[LEFT], gm.cam2.transform.position.y, gm.cam2.transform.position.z);
        }
        else if(posArr[LEFT] < normalX && normalX <= posArr[MIDDLE]){
            playerTf.transform.position = new Vector3(plTfPosArr[MIDDLE], playerTf.transform.position.y, playerTf.transform.position.z);
            gm.cam2.transform.position = new Vector3(cam2TfPosArr[MIDDLE], gm.cam2.transform.position.y, gm.cam2.transform.position.z);
        }
        else if(posArr[MIDDLE] < normalX && normalX <= posArr[RIGHT]){
            playerTf.transform.position = new Vector3(plTfPosArr[RIGHT], playerTf.transform.position.y, playerTf.transform.position.z);
            gm.cam2.transform.position = new Vector3(cam2TfPosArr[RIGHT], gm.cam2.transform.position.y, gm.cam2.transform.position.z);
        }
    }
    private void moveModelTf(Vector2 dir){
        if(dir.x < 0){//* Right
            pl.modelMovingTf.localPosition = new Vector3(3.5f, 0, 0);
            pl.modelMovingTf.localScale = new Vector3(-Mathf.Abs(
                pl.modelMovingTf.localScale.x), pl.modelMovingTf.localScale.y, pl.modelMovingTf.localScale.z);
        }
        else{//* Left
            pl.modelMovingTf.localPosition = new Vector3(0, 0, 0);
            pl.modelMovingTf.localScale = new Vector3(+Mathf.Abs(
                pl.modelMovingTf.localScale.x), pl.modelMovingTf.localScale.y, pl.modelMovingTf.localScale.z);
        }
    }
    private void rotateArrowTf(Vector2 dir){
        //* Stick(Arrow)角度によって、Player位置が自動で左右移動。
        float deg = convertDir2DegWithRange(dir, MIN_ARROW_DEG_Y, MAX_ARROW_DEG_Y);
        Debug.Log("OnDrag::rotateArrowTf:: Stick(Arrow) Deg=" + deg + ", dir=" + dir + ", " + ((dir.x < 0)? "Left" : "Right").ToString());

        //* Player矢印(Arrow)角度に適用
        pl.arrowAxisAnchor.transform.rotation = Quaternion.Euler(0, offsetDeg2DTo3D - deg, 0);
        // pl.arrowAxisAnchor.transform.position = new Vector3(pl.arrowAxisAnchor.transform.position.x + Mathf.Cos(deg), pl.arrowAxisAnchor.transform.position.y, pl.arrowAxisAnchor.transform.position.z + Mathf.Sin(deg));
    }
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

        //* 二番目LINEが出たら、一番目 BallPreviewSphereを消して、二番目を活性化
        pl.ballPreviewSphere[1].SetActive((wallNormalVec == Vector3.zero)? false : true);
        bool is2ndBallPreviewOn = pl.ballPreviewSphere[1].activeSelf;

        pl.ballPreviewSphere[0].SetActive(is2ndBallPreviewOn? false : true);
        pl.ballPreviewSphere[1].SetActive(is2ndBallPreviewOn? true : false);
    }

    private float convertDir2DegWithRange(Vector3 dir, int min, int max){
        //! (BUG) タッチをドラッグする時に、Y軸を動かないと角度がちゃんと出来ない。
        //* NormalizeしたX軸の距離(CosΘ)であれば、Y軸の距離(SinΘ)を予想して反映。
        float normalX = dir.x / (pad.rect.width * 0.5f); // -1 ~ 1
        dir = dir.normalized;
        Debug.Log($"normalX= Atan2({Util._.setNumDP(0 <= normalX? 1-normalX : 1+normalX, 2)}, {Util._.setNumDP(normalX, 2)})");

        // (BUG) float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float deg = Mathf.Atan2(((0 <= normalX)? 1-normalX : 1+normalX), normalX) * Mathf.Rad2Deg;

        //*(BUG) Clamp Deg with cur Direction
        deg = Mathf.Clamp(deg, min, max); //! (BUG) 角度が－値になるとClampはminを返す。
        const int LEFT = -1; //const int RIGHT = +1;
        var dirSign = Mathf.Sign(dir.x);
        return deg = (deg == max || (deg == min && dirSign == LEFT)) ? max : deg;
    }
}
