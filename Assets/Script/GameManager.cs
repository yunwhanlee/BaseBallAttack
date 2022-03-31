using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State {PLAY, WAIT, GAMEOVER};
    public State state;

    //* CAMERA
    public GameObject cam1, cam2;
    public GameObject cam1Canvas, cam2Canvas;

    //* OutSide
    public Player pl;
    public BallShooter ballShooter;
    public BlockMaker blockMaker;
    public Transform hitRangeStartTf;
    public Transform hitRangeEndTf;
    public Transform ballGroup;

    public Transform deadLineTf;
    public BoxCollider downWall;

    [Header("<---- GUI ---->")]
    public int stage = 1;
    public int bestScore;
    public int strikeCnt = 0;

    public Text stageTxt;
    public Text stateTxt;
    public Text shootCntTxt;

    [Header("-Exp Slider Bar-")]
    public Slider expBar;

    [Header("-View Preview Ball Slider-")] //! あんまり要らないかも。
    public Slider hitRangeSlider;
    private RectTransform hitRangeSliderTf;
    public RectTransform HomeRunRangeTf;
    public float HomeRunRangePer = 0.2f;
    public Image hitRangeHandleImg;

    [Header("-Ball Preview Dir Goal-")]
    public GameObject ballPreviewDirGoal;
    public Image ballPreviewGoalImg;

    [Header("-Strike Ball Image-")]
    public GameObject StrikePanel;
    public Image[] strikeBallImgs;

    [Header("-Level Up-")]
    public GameObject levelUpPanel;

    [Header("-GameOver-")]
    public GameObject gameoverPanel;
    private Text gvStageTxt;
    private Text gvBestScoreTxt;

    [Header("-Pause-")]
    public GameObject pausePanel;


    [Header("-Button-")]
    public Button readyBtn; //Normal
    public Button reGameBtn; //gameoverPanel
    public Button pauseBtn; //pausePanel
    public Button continueBtn; //pausePanel
    public Button homeBtn; //pausePanel

    //---------------------------------------

    void Start() {
        // Debug.Log("deadLine.pos.z=" + deadLineTf.position.z);
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();
        readyBtn = readyBtn.GetComponent<Button>();
        gvStageTxt = gameoverPanel.transform.GetChild(1).GetComponent<Text>();
        gvBestScoreTxt = gameoverPanel.transform.GetChild(2).GetComponent<Text>();

        //* Ball Preview Dir Goal Set Z-Center
        setBallPreviewGoalRandomPos();
    }

    void Update(){
        //* GUI
        expBar.value = Mathf.Lerp(expBar.value, pl.getExp() / pl.getMaxExp(), Time.deltaTime * 10);
        stateTxt.text = state.ToString();
        stageTxt.text = "STAGE : " + stage.ToString();
    }

    public void setState(State st) => state = st;
    public void setShootCntText(string str) => shootCntTxt.text = str;
    public void setBallPreviewGoalImgRGBA(Color color) => ballPreviewGoalImg.color = color;
    public void setNextStage() => ++stage;
    //* GUI Button
    public void onClickReadyButton() => switchCamScene();
    public void onClickReGameButton() => init();
    public void onClickSkillButton() => levelUpPanel.SetActive(false);
    public void onClickSetGameButton(string type) => setGame(type);

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    public void init(){
        state = GameManager.State.WAIT;
        stage = 1;
        strikeCnt = 0;
        pl.setLv(1);
        pl.setExp(0);
        pl.setMaxExp(100);
        gameoverPanel.SetActive(false);
        pl.Start();
        blockMaker.Start();
    }

    public void switchCamScene(){
        if(state == GameManager.State.GAMEOVER) return;
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

            ballShooter.resetCountingTime();

            pl.arrowAxisAnchor.SetActive(false);
            
            StrikePanel.SetActive(true);
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

            pl.arrowAxisAnchor.SetActive(true);
            if(0 < strikeCnt && ballGroup.childCount == 0) pl.previewBundle.SetActive(true); //(BUG)STRIKEになってから、BACKボタン押すと、PreviewLineが消えてしまう。

            StrikePanel.SetActive(false);

            StopCoroutine("corSetStrike");
        }
    }

    //ストライク GUI表示
    public void setStrike(){
        if(strikeCnt < 2)
            StartCoroutine(corSetStrike());
        else
            StartCoroutine(corSetStrike(true));
    }

    private IEnumerator corSetStrike(bool isOut = false){
        strikeBallImgs[strikeCnt++].gameObject.SetActive(true);
        if(isOut){
            strikeCnt = 0;
            setShootCntText("OUT!");
            yield return new WaitForSeconds(1.5f);
            switchCamScene();
            blockMaker.setCreateBlockTrigger(true); //ブロック生成
            foreach(var img in strikeBallImgs) img.gameObject.SetActive(false); //GUI非表示 初期化
            readyBtn.gameObject.SetActive(true);
        }
        else{
            setShootCntText("STRIKE!");
            readyBtn.gameObject.SetActive(true);
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
        //Debug.Log("setBallPreviewImgAlpha:: "+"distance("+dist+")"+ " * unit("+unit+") = " + "alpha(" + alpha + ")");
    }

    public void setBallPreviewGoalRandomPos(){
        float startPosZ = hitRangeStartTf.position.z;
        float endPosZ = hitRangeEndTf.position.z;
        float zCenter = startPosZ + (endPosZ - startPosZ) / 2;
        float v = 0.05f;//0.175f; (BUG) BlockがGameOverまである時に、ボールとぶつかる。
        float rx = Random.Range(-v, v);
        float ry = Random.Range(-v, v);
        ballPreviewDirGoal.transform.position = new Vector3(0 + rx, 0.6f + ry, zCenter);
    }

    public void setGame(string type){
        switch(type){
            case "PAUSE":
                Time.timeScale = 0; pausePanel.SetActive(true);  break;
            case "CONTINUE":
                Time.timeScale = 1; pausePanel.SetActive(false); break;
            case "HOME":
                Debug.Log("FINISH GAME!");
                //TODO
                break;
        }
    }
    public void setGameOver(){
        Debug.Log("--GAME OVER--");
        setState(GameManager.State.GAMEOVER);
        gameoverPanel.SetActive(true);
        gvStageTxt.text = "STAGE : " + stage;
        gvBestScoreTxt.text = "BEST SCORE : " + bestScore;
    }

    public void checkLevelUp(){
        if(pl.getIsLevelUp()){
            pl.setIsLevelUp(false);
            levelUpPanel.SetActive(true);
            levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
        }
    }
}
