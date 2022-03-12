
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State {PLAY, WAIT, GAMEOVER};
    public State state = State.PLAY;

    //* CAMERA
    public GameObject cam1, cam2;
    public GameObject cam1Canvas, cam2Canvas;

    //* OutSide
    public BallShooter ballShooter;
    public BlockMaker blockMaker;
    public Transform hitRangeStartTf;
    public Transform hitRangeEndTf;

    public Transform deadLineTf;
    public BoxCollider downWall;

    [Header("<---- GUI ---->")]
    public Text stateTxt;
    public Text shootCntTxt;

    [Header("-View Slider-")]
    public Slider hitRangeSlider;
    private RectTransform hitRangeSliderTf;
    public RectTransform HomeRunRangeTf;
    public float HomeRunRangePer = 0.2f;
    public Image hitRangeHandleImg;

    [Header("-Ball Preview Dir Goal-")]
    public GameObject ballPreviewDirGoal;
    public Image ballPreviewGoalImg;

    [Header("-Button-")]
    public Button readyBtn;

    

    void Start() {
        // Debug.Log("deadLine.pos.z=" + deadLineTf.position.z);
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();
        readyBtn = readyBtn.GetComponent<Button>();

        //* Ball Preview Dir Goal Set Z-Center
        float startPosZ = hitRangeStartTf.position.z;
        float endPosZ = hitRangeEndTf.position.z;
        float zCenter = startPosZ + (endPosZ - startPosZ) / 2;
        ballPreviewDirGoal.transform.position = new Vector3(0, 0.6f, zCenter);


        //* Set UI HomeRunRange
        // float HomeRunRangeWidth = hitRangeSliderTf.rect.width * HomeRunRangePer;
        // HomeRunRangeTf.sizeDelta = new Vector2(HomeRunRangeWidth, 12);

        // float randPosX = Mathf.RoundToInt(Random.Range(0, hitRangeSliderTf.rect.width - HomeRunRangeWidth));
        // float parentPivotOffset = hitRangeSliderTf.rect.width / 2;
        // Debug.Log("random.Range("+0+"～"+ (hitRangeSliderTf.rect.width - HomeRunRangeWidth).ToString() +")"+ ", randPosX=" + randPosX + ", parentPivotOffset=" + parentPivotOffset);
        // HomeRunRangeTf.localPosition = new Vector3(randPosX - parentPivotOffset,0,0);
    }


    void Update()
    {
        //* GUI
        //* State Txt
        stateTxt.text = state.ToString();

        //* HitBox Degree Slider
        // float degY = hitBox.transform.rotation.eulerAngles.y;
        // degY = (degY < 180) ? -degY : +(360 - degY);
        // float v = degY + hitBoxDegOffset;
        //Debug.Log("GUI:: HitBox degY v=" + v);

        // int hitBoxDegSliderMax = hitBoxDegOffset * 2;
        // hitBoxDegSlider.value = v / hitBoxDegSliderMax;
    }

    public void setState(State st) => state = st;
    public void setShootCntText(string str) => shootCntTxt.text = str;
    public void setBallPreviewGoalImgRGBA(Color color) => ballPreviewGoalImg.color = color;
    
    //* GUI Button
    public void onClickReadyButton() => switchCamScene();

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    public void switchCamScene(){
        if(!cam2.activeSelf){//* CAM2 On
            state = GameManager.State.PLAY;
            cam1.SetActive(false);
            cam1Canvas.SetActive(false);
            cam2.SetActive(true);
            cam2Canvas.SetActive(true);

            shootCntTxt.gameObject.SetActive(true);
            readyBtn.gameObject.GetComponentInChildren<Text>().text = "BACK";

            ballPreviewGoalImg.gameObject.SetActive(true);
            setBallPreviewGoalImgRGBA(new Color(0.8f,0.8f,0.8f, 0.2f));

            ballShooter.resetBallShootCnt();
            
        }
        else{//* CAM1 On
            state = GameManager.State.WAIT;
            cam1.SetActive(true);
            cam1Canvas.SetActive(true);
            cam2.SetActive(false);
            cam2Canvas.SetActive(false);
            
            shootCntTxt.gameObject.SetActive(false);
            readyBtn.gameObject.GetComponentInChildren<Text>().text = "READY";

            ballPreviewGoalImg.gameObject.SetActive(false);

            StopCoroutine("corSetStrike");
        }
    }

    //ストライク GUI表示
    public void setStrike(){
        if(ballShooter.strikeCnt < 2)
            StartCoroutine(corSetStrike());
        else
            StartCoroutine(corSetStrike(true));
    }

    private IEnumerator corSetStrike(bool isOut = false){
        if(isOut){
            ballShooter.strikeCnt = 0;
            setShootCntText("OUT!");
            yield return new WaitForSeconds(1.5f);
            switchCamScene();
            blockMaker.setCreateBlock(true); //ブロック生成
        }
        else{
            ballShooter.strikeCnt++;
            setShootCntText("STRIKE!");
            yield return new WaitForSeconds(1.5f);
        }
        ballShooter.setIsBallExist(false); //ボール発射準備 On
        setBallPreviewGoalImgRGBA(new Color(0.8f,0.8f,0.8f, 0.2f));
    }

    //" ボールがHit領域に来ることに当たって、ボール予想イメージ透明度を調整
    public void setBallPreviewImgAlpha(float dist){
        if(dist > 10) return;
        float alphaApplyMax = 200;
        float distResponseMax = 10;
        float unit = alphaApplyMax / distResponseMax;
        float alpha = (unit * dist) / 255;
        setBallPreviewGoalImgRGBA(new Color(0.8f,0.8f,0.8f, 1-alpha));
        //Debug.Log("setBallPreviewImgAlpha:: "+"distance("+dist+")"+ " * unit("+unit+") = " + alpha);
    }
}
