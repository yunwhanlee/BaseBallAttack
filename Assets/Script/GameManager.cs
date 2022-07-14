using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State {PLAY, WAIT, GAMEOVER};
    public State state;     public State STATE {get => state; set => state = value;}

    //* CAMERA
    public GameObject cam1, cam2;
    public GameObject cam1Canvas, cam2Canvas;

    //* OutSide
    [Header("<---- OutSide ---->")]
    public EffectManager em;
    public Canvas canvasUI;
    public Player pl;
    public BallShooter ballShooter;
    public BlockMaker bm;
    public GameObject throwScreen;
    public Transform hitRangeStartTf;
    public Transform hitRangeEndTf;
    public Transform ballGroup;
    public Transform deadLineTf;
    public BoxCollider downWall;
    public Light light;

    [Header("<---- GUI ---->")]
    public int stage = 1;
    public int bestScore;
    public int strikeCnt = 0;
    public int comboCnt = 0;

    public Text stageTxt;
    public Text stateTxt;
    public Text levelTxt;
    public Text shootCntTxt;
    public Text comboTxt;

    [Header("--Exp Slider Bar--")]
    public Slider expBar;

    [Header("--View Preview Ball Slider--")] //! あんまり要らないかも。
    public Slider hitRangeSlider;
    private RectTransform hitRangeSliderTf;
    public RectTransform HomeRunRangeTf;
    public float HomeRunRangePer = 0.2f;
    public Image hitRangeHandleImg;

    [Header("--Ball Preview Dir Goal (CAM2)--")]
    public GameObject ballPreviewDirGoal;
    public Image ballPreviewGoalImg;

    [Header("--Strike Ball Image--")]
    public GameObject StrikePanel;
    public Image[] strikeBallImgs;

    [Header("--Level Up--")]
    public GameObject levelUpPanel;

    [Header("--Active Skill Btn--")]
    public ActiveSkill[] activeSkillDataBase; //* 全てActiveSkillsのデータベース
    public List<ActiveSkillBtnUI> activeSkillBtnList; //* ActiveSkillボタン
    public Material activeSkillBtnEfMt;
    public Material[] blockGlowColorMts;
    public bool isPointUp; //* SectorGizmos Colliderへ活用するため。

    [Header("--Passive Skill Table InGame--")]
    public GameObject[] passiveSkillImgObjPrefs;
    public RectTransform inGameSkillStatusTableTf;
    public GameObject inGameSkillImgBtnPref;
    
    [Header("--Pause--")]
    public GameObject pausePanel;
    public RectTransform pauseSkillStatusTableTf;
    public GameObject skillInfoRowPref;

    [Header("--GameOver--")]
    public GameObject gameoverPanel;
    private Text gvStageTxt;
    private Text gvBestScoreTxt;

    [Header("--Button--")]
    public Button readyBtn; //Normal
    public Transform activeSkillBtnGroup; //Normal
    public Button reGameBtn; //gameoverPanel
    public Button pauseBtn; //pausePanel
    public Button continueBtn; //pausePanel
    public Button homeBtn; //pausePanel

    //---------------------------------------
    void Start() {
        light = GameObject.Find("Directional Light").GetComponent<Light>();
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();
        readyBtn = readyBtn.GetComponent<Button>();
        gvStageTxt = gameoverPanel.transform.GetChild(1).GetComponent<Text>();
        gvBestScoreTxt = gameoverPanel.transform.GetChild(2).GetComponent<Text>();

        //* Ball Preview Dir Goal Set Z-Center
        setBallPreviewGoalRandomPos();

        //* Active Skill Btns(現在は１個、最大2個)
        int i=0;
        Debug.Log("GM:: activeSkillBtnGroup.childCount=" + activeSkillBtnGroup.childCount);
        foreach(Transform child in activeSkillBtnGroup){
            Button btn = child.GetComponent<Button>();
            activeSkillBtnList.Add(new ActiveSkillBtnUI(
                i,
                pl.AtvSkillCoolDownUnit,
                pl.activeSkills[DM.ins.personalData.SelectSkillIdx].Name,
                btn,
                pl.activeSkills[DM.ins.personalData.SelectSkillIdx].UISprite,
                activeSkillBtnEfMt)
            );
            i++;
        }
    }

    void Update(){
        //* GUI
        expBar.value = Mathf.Lerp(expBar.value, pl.Exp / pl.MaxExp, Time.deltaTime * 10);
        stateTxt.text = STATE.ToString();
        levelTxt.text = "LV : " + pl.Lv;
        stageTxt.text = "STAGE : " + stage.ToString();
        comboTxt.text = "COMBO\n" + comboCnt.ToString();

        //* (BUG) ボールが複数ある時、同時に消えたら、次に進まないこと対応
        // if(ballGroup.childCount == 0 && !downWall.isTrigger){
        //     Debug.Log("GM:: Update:: setNextStage()");
        //     setNextStage();
        // }

        //* ActiveSkill Status
        activeSkillBtnList.ForEach(btn=>{
            btn.setActiveSkillEF();
        });
        
    }

    public void setShootCntText(string str) => shootCntTxt.text = str;
    public void setBallPreviewGoalImgRGBA(Color color) => ballPreviewGoalImg.color = color;
    public GameObject[] getSkillImgObjPrefs() => passiveSkillImgObjPrefs;
    public void throwScreenAnimSetTrigger(string name) => throwScreen.GetComponent<Animator>().SetTrigger(name);
    public void setLightDarkness(bool isOn){ //* During Skill Casting ...
        light.type = (isOn)? LightType.Spot : LightType.Directional;
    }
    
    //* --------------------------------------------------------------------------------------
    //* GUI Button
    //* --------------------------------------------------------------------------------------
    public void onClickReadyButton() => switchCamScene();
    public void onClickReGameButton() => init();
    public void onClickSkillButton() => levelUpPanel.SetActive(false);
    public void onClickSetGameButton(string type) => setGame(type);
    public void onClickActiveSkillButton(int i) {
        //(BUG)再クリック。Cancel Selected Btn
        if(activeSkillBtnList[i].SelectCircleEF.gameObject.activeSelf){
            activeSkillBtnList[i].init(this, true);
            return;
        }
        //(BUG)重複選択禁止。初期化
        activeSkillBtnList.ForEach(btn=>{
            btn.init(this, true);
        });

        if(ballGroup.childCount == 0){
            activeSkillBtnList[i].onTriggerActive(i, this);
        }
    } //(BUG)途中でスキル活性化ダメ

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    public void init(){
        STATE = GameManager.State.WAIT;
        stage = 1;
        strikeCnt = 0;
        comboCnt = 0;
        pl.Lv = 1;
        pl.Exp = 0;
        pl.MaxExp = 100;
        gameoverPanel.SetActive(false);
        pl.Start();
        bm.Start();
        
    }

    public void switchCamScene(){
        if(STATE == GameManager.State.GAMEOVER) return;
        if(!cam2.activeSelf){//* CAM2 On
            STATE = GameManager.State.PLAY;
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

            foreach(Transform child in activeSkillBtnGroup){
                Button btn = child.GetComponent<Button>();
                btn.gameObject.SetActive(false);
            }

            setLightDarkness(false); //* Light -> Normal
        }
        else{//* CAM1 On
            STATE = GameManager.State.WAIT;
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

            foreach(Transform child in activeSkillBtnGroup){
                Button btn = child.GetComponent<Button>();
                btn.gameObject.SetActive(true);
            }
        
            //* Check Before Cam1 Light : Darkness or Normal
            bool isBefLightDark = activeSkillBtnList.Exists(btn => btn.SelectCircleEF.gameObject.activeSelf);
            if(isBefLightDark)  setLightDarkness(true);

            //* ActiveSkill Status
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
            bm.setCreateBlockTrigger(true); //ブロック生成
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
        setBallPreviewGoalImgRGBA(new Color(alpha, alpha, 0.8f, 1-alpha));
        // Debug.Log("setBallPreviewImgAlpha:: "+"distance("+dist+")"+ " * unit("+unit+") = " + "alpha(" + alpha + ")");
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
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                displayCurPassiveSkillUI(type);
                break;
            case "CONTINUE":
                Time.timeScale = 1;
                resetSkillStatusTable();
                pausePanel.SetActive(false);
                break;
            case "HOME":
                Time.timeScale = 1;
                Debug.Log("FINISH GAME!");
                resetSkillStatusTable();
                SceneManager.LoadScene("Home");
                break;
        }
    }
    public void setGameOver(){
        Debug.Log("--GAME OVER--");
        STATE = GameManager.State.GAMEOVER;
        gameoverPanel.SetActive(true);
        gvStageTxt.text = "STAGE : " + stage;
        gvBestScoreTxt.text = "BEST SCORE : " + bestScore;
    }

    public void checkLevelUp(){
        if(pl.IsLevelUp){
            pl.IsLevelUp = false;
            levelUpPanel.SetActive(true);
            levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
        }
    }

    public void displayCurPassiveSkillUI(string type){
        GameObject pref = (type == "PAUSE")? skillInfoRowPref : inGameSkillImgBtnPref;
        Transform parentTf = (type == "PAUSE")? pauseSkillStatusTableTf : inGameSkillStatusTableTf;

        //(BUG) 情報が重ならないように、一回 初期化する。
        if(type != "PAUSE" && parentTf.childCount > 0)
            foreach(Transform childTf in parentTf){Destroy(childTf.gameObject);}

        List<int> lvList = pl.getAllSkillLvList();
        int i=0;
        lvList.ForEach(lv => {
            if(lv > 0){
                String levelTxt = (type == "PAUSE")? ("x " + lv.ToString()) : lv.ToString();
                var rowTf = Instantiate(pref, Vector3.zero, Quaternion.identity, parentTf).transform;
                var imgObj = Instantiate(getSkillImgObjPrefs()[i], Vector3.zero, Quaternion.identity, rowTf);
                if(type != "PAUSE"){
                    imgObj.transform.localScale = Vector3.one * 0.3f;
                    imgObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    int index = imgObj.transform.GetSiblingIndex();
                    imgObj.transform.SetSiblingIndex(index - 1);
                }
                rowTf.GetComponentInChildren<Text>().text = levelTxt;
            }
            i++;
        });
    }
    public void resetSkillStatusTable(){
        foreach(RectTransform child in pauseSkillStatusTableTf){
            Destroy(child.gameObject);
        }
    }

    public void setNextStage() {
        Debug.Log("<color=black>setNextStage:: NEXT STAGE(Ball Is Destroyed)</color>");
        ++stage;
        STATE = GameManager.State.WAIT;
        downWall.isTrigger = true; //*下壁 物理X
        readyBtn.gameObject.SetActive(true);
        bm.setCreateBlockTrigger(true);
        comboCnt = 0;
        if(bm.transform.childCount == 0){
            Debug.Log("PERFECT!");
            ++stage;
            bm.moveDownBlock();
        }

        ballShooter.setIsBallExist(false);
        pl.previewBundle.SetActive(true);
        StartCoroutine(coWaitPlayerCollectOrb());
        setBallPreviewGoalRandomPos();
        checkBlocksIsDotDmg();

        //* Collect Drop Items Exp
        var dropObjs = bm.dropItemGroup.GetComponentsInChildren<DropItem>();
        Debug.Log("setNextStage:: dropObjs.Length= " + dropObjs.Length);
        Array.ForEach(dropObjs, dropObj=>dropObj.moveToTarget(pl.transform));

        
    }

    private IEnumerator coWaitPlayerCollectOrb(){
        float sec = 1f;
        yield return new WaitForSeconds(sec);
        Debug.LogFormat("<color=black>coWaitCollectOrb:: checkLevelUp() wait: {0}sec</color>",sec);
        checkLevelUp();
    }
    private void checkBlocksIsDotDmg(){
        var blocks = bm.GetComponentsInChildren<Block_Prefab>();
        Array.ForEach(blocks, block => {
            if(block.IsDotDmg)  block.decreaseHp(block.getDotDmg(2));
        });
    }
}
