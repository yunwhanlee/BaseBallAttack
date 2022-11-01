using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum STATE {PLAY, WAIT, GAMEOVER, PAUSE, CONTINUE, HOME, NULL};
    [SerializeField] private STATE state;     public STATE State {get => state; set => state = value;}

    [Header("GROUP")]
    public Transform effectGroup;
    public Transform ballGroup;
    public Transform blockGroup;
    public Transform dropItemGroup;
    public Transform bossGroup;
    public Transform obstacleGroup;
    public Transform activeSkillBtnGroup;


    [Header("CAMERA")]
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam1Canvas;
    public GameObject cam2Canvas;

    //* OutSide
    [Header("OUTSIDE")]
    public Canvas canvasUI;
    public Player pl;
    public EffectManager em;
    public BallShooter bs;
    public BlockMaker bm;
    public GameObject throwScreen;
    public Transform hitRangeStartTf;
    public Transform hitRangeEndTf;
    public Transform deadLineTf;
    public BoxCollider downWallCollider;
    public Light light;

    [Header("STATUS")]
    public int stage = 1;
    public int bestScore;
    public int strikeCnt = 0;
    public int comboCnt = 0;

    [Header("TRIGGER")]
    public bool isPlayingAnim;  public bool IsPlayingAnim { get=> isPlayingAnim; set=> isPlayingAnim = value;}

    [Header("PANEL")]
    public GameObject strikePanel;
    public GameObject levelUpPanel;
    public GameObject pausePanel;
    public GameObject gameoverPanel;
    public RectTransform statusFolderPanel;

    [Header("◆GUI◆")]
    public Text stageTxt;
    public Text stateTxt;
    public Text levelTxt;
    public Text[] statusTxts = new Text[2];
    [SerializeField] Text shootCntTxt;      public Text ShootCntTxt { get => shootCntTxt; set => shootCntTxt = value;}
    public RectTransform homeRunTxtTf;
    public Slider expBar, bossStageBar;
    public Text expTxt, bossStageTxt;

    [Header("TEXT EFFECT")]
    public Text comboTxt;
    public Text perfectTxt;
    public Text bossSpawnTxt;
    public Text showHitBallInfoTf;

    [Header("PREVIEW BALL SILDER ➡ 現在使っていない")] //! あんまり要らないかも。
    public Slider hitRangeSlider;
    private RectTransform hitRangeSliderTf;

    [Header("BALL PREVIEW DIR GOAL(CAM2)")]
    public GameObject ballPreviewDirGoal;
    public Image ballPreviewGoalImg;

    [Header("STRIKE CNT IMAGE")]
    public Image[] strikeCntImgs;

    [Header("ACTIVE SKILL BTN")]
    public RectTransform activeSkillBtnPf;
    public AtvSkill[] activeSkillDataBase; //* 全てActiveSkillsのデータベース
    public List<AtvSkillBtnUI> activeSkillBtnList; //* ActiveSkillボタン
    public Material activeSkillBtnEfMt;
    [SerializeField] int selectAtvSkillBtnIdx;  public int SelectAtvSkillBtnIdx { get=> selectAtvSkillBtnIdx; set=> selectAtvSkillBtnIdx = value;}
    public bool isPointUp; //* SectorGizmos Colliderへ活用するため。
    public Material[] blockGlowColorMts;

    [Header("PASSIVE TABLE")]
    [SerializeField] private GameObject[] psvSkillImgPrefs;    public GameObject[] PsvSkillImgPrefs { get => psvSkillImgPrefs; set => psvSkillImgPrefs = value;}
    public RectTransform inGameSkillStatusTableTf;
    public GameObject inGameSkillImgBtnPref;
    
    [Header("PAUSE")]
    public RectTransform pauseSkillStatusTableTf;
    public GameObject skillInfoRowPref;

    [Header("GAMEOVER")]
    private Text gvStageTxt;
    private Text gvBestScoreTxt;

    [Header("STATUS FOLDER")]
    public Transform statusInfoContents;
    public Text statusInfoTxtPf;

    [Header("BUTTON")]
    public Button readyBtn; //Normal
    public Button reGameBtn; //gameoverPanel
    public Button pauseBtn; //pausePanel
    public Button continueBtn; //pausePanel
    public Button homeBtn; //pausePanel
    public Button statusFolderBtn;

    int val = 1;

    void Start() {
        // Util._.setCalcFibonicciSequence(100, 1.5f);
        

        Debug.Log("<color=red>----------------------------------------------P L A Y   S C E N E----------------------------------------------</color>");
        //! init()宣言したら、キャラクターモデルを読み込むことができないBUG
        pl = GameObject.Find("Player").GetComponent<Player>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        bs = GameObject.Find("BallShooter").GetComponent<BallShooter>();
        bm = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();

        light = GameObject.Find("Directional Light").GetComponent<Light>();
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();
        readyBtn = readyBtn.GetComponent<Button>();
        gvStageTxt = gameoverPanel.transform.GetChild(1).GetComponent<Text>();
        gvBestScoreTxt = gameoverPanel.transform.GetChild(2).GetComponent<Text>();

        Array.ForEach(statusTxts, txt => txt.text = LANG.getTxt(LANG.TXT.Status.ToString()));

        //* Ball Preview Dir Goal Set Z-Center
        setBallPreviewGoalRandomPos();

        //* Active Skill Btns
        int len =  (DM.ins.personalData.IsUnlock2ndSkill)? 2 : 1;
        Debug.Log("GM:: Active Skill Btns len= <color=yellow>" + len + "</color>");
        for(int i=0; i<len; i++){
            Vector3 pos = new Vector3(activeSkillBtnPf.anchoredPosition3D.x, 
                (i == 0)? activeSkillBtnPf.anchoredPosition3D.y : activeSkillBtnPf.anchoredPosition3D.y - 170, 
                activeSkillBtnPf.anchoredPosition3D.z);
            RectTransform btn = Instantiate(activeSkillBtnPf, Vector3.zero, Quaternion.identity, activeSkillBtnGroup);
            btn.anchoredPosition3D = pos;
            // var btn = activeSkillBtnGroup.GetChild(i).GetComponent<Button>();
            // btn.gameObject.SetActive(true);
            int idx = (i==0)? DM.ins.personalData.SelectSkillIdx : DM.ins.personalData.SelectSkill2Idx;
            //* ReActive AddEventListener
            int btnIdx = (idx==DM.ins.personalData.SelectSkillIdx)? 0 : 1;
            btn.GetComponent<Button>().onClick.AddListener(() => onClickActiveSkillButton(btnIdx));
            Debug.LogFormat("activeSkillBtn[{0}].onClick.AddListener => onClickActiveSkillButton({1})", i,btnIdx);
            activeSkillBtnList.Add(new AtvSkillBtnUI(i, LM._.ATVSKILL_COOLDOWN_UNIT, pl.activeSkills[idx].Name, btn.GetComponent<Button>(), pl.activeSkills[idx].UISprite, activeSkillBtnEfMt));
        }
    }

    void Update(){
        //* GUI
        expBar.value = Mathf.Lerp(expBar.value, (float)pl.Exp / (float)pl.MaxExp, Time.deltaTime * 10);
        bossStageBar.value = Mathf.Lerp(bossStageBar.value, (float)(stage % LM._.BOSS_STAGE_SPAN) / (float)LM._.BOSS_STAGE_SPAN, Time.deltaTime * 10);
        expTxt.text = $"{pl.Exp} / {pl.MaxExp}";
        bossStageTxt.text = $"{stage % LM._.BOSS_STAGE_SPAN} / {LM._.BOSS_STAGE_SPAN}";
        stateTxt.text = State.ToString();
        levelTxt.text = LANG.getTxt(LANG.TXT.Level.ToString()) + " : " + pl.Lv;
        stageTxt.text = LANG.getTxt(LANG.TXT.Stage.ToString()) + " : " + stage.ToString();
        comboTxt.text = LANG.getTxt(LANG.TXT.Combo.ToString()) + "\n" + comboCnt.ToString();


        //* ActiveSkill Status
        activeSkillBtnList.ForEach(btn=>{
            btn.setActiveSkillEF();
        });
    }
    public void setBallPreviewGoalImgRGBA(Color color) => ballPreviewGoalImg.color = color;
    public void throwScreenAnimSetTrigger(string name) => throwScreen.GetComponent<Animator>().SetTrigger(name);
    public void setLightDarkness(bool isOn){ //* During Skill Casting ...
        light.type = (isOn)? LightType.Spot : LightType.Directional;
    }
//* --------------------------------------------------------------------------------------
//* GUI Button
//* --------------------------------------------------------------------------------------
    public void onClickReadyButton() => switchCamera();
    public void onClickReGameButton() => init();
    public void onClickSkillButton() => levelUpPanel.SetActive(false);
    public void onClickSetGameButton(string type) => setGame(type);
    public void onClickActiveSkillButton(int i) {
        SelectAtvSkillBtnIdx = i; //* 最新化
        bool isActive = activeSkillBtnList[i].SelectCircleEF.gameObject.activeSelf;
        Debug.LogFormat("onClickActiveSkillButton({0}), isActive= {1}", i, isActive);
        //(BUG)再クリック。Cancel Selected Btn
        if(isActive){
            activeSkillBtnList[i].init(this, true);
            return;
        }
        else{
            bm.setGlowEFAllBlocks(false); //* Btn Offしたら、全てGlowもOFF。
        }
        //(BUG)重複選択禁止。初期化
        activeSkillBtnList.ForEach(btn => {
            btn.init(this, true);
        });

        if(ballGroup.childCount == 0){
            activeSkillBtnList[i].onTriggerActiveSkillBtn(this);
        }
    } //(BUG)途中でスキル活性化ダメ

    public void onClickStatusFolderButton(bool isTrigger){
        float pivotX = statusFolderPanel.pivot.x;
        if(pivotX == -1) isTrigger = false;
        else isTrigger = true;
        Debug.Log("onClickStatusFolderButton():: isTrigger= " + isTrigger + ", pivotX=" + pivotX);
        statusFolderPanel.pivot = new Vector2(isTrigger? -1 : 0, 0.5f);

        // Init
        for(int i=0;i<statusInfoContents.childCount; i++)
            Destroy(statusInfoContents.GetChild(i).gameObject);

        // Set InfoTxt List
        List<string> infoTxtList = PsvSkill<int>.getPsvInfo2Str(pl);

        // Apply InfoTxt List
        infoTxtList.ForEach(infoTxt => {
            statusInfoTxtPf.text = infoTxt;
            Instantiate(statusInfoTxtPf, Vector3.zero, Quaternion.identity, statusInfoContents);
        });
    }

    public void onClickBtnShowAD(string type){
        DM.ins.showAD(type);
    }

//*---------------------------------------
//*  関数
//*---------------------------------------
    public void init(){
        State = GameManager.STATE.WAIT;
        stage = 1;
        strikeCnt = 0;
        comboCnt = 0;
        pl.Lv = 1;
        pl.Exp = 0;
        pl.MaxExp = 100;
        gameoverPanel.SetActive(false);
        pl.Start();
        bm.Start();
        //* Collect Drop Items Exp
        var dropObjs = dropItemGroup.GetComponentsInChildren<DropItem>();
    }

    public void switchCamera(){
        if(State == GameManager.STATE.GAMEOVER) return;
        bool isOnCam2 = !cam2.activeSelf;

        if(isOnCam2){//* CAM2 On
            State = GameManager.STATE.PLAY;
            ManageActiveObjects(isOnCam2);
            setTextReadyBtn(LANG.getTxt(LANG.TXT.Back.ToString()));
            setBallPreviewGoalImgRGBA(new Color(0.8f,0.8f,0.8f, 0.2f));
            bs.init();
        }
        else{//* CAM1 On
            State = GameManager.STATE.WAIT;
            ManageActiveObjects(isOnCam2);
            setTextReadyBtn(LANG.getTxt(LANG.TXT.Ready.ToString()));
            //* (BUG)STRIKEになってから、BACKボタン押すと、PreviewLineが消えてしまう。
            setActivePreviewBendle(true);
            //* ActiveSkill Status
            StopCoroutine("corSetStrike");
        }
    }
    
    private void setActiveCam(bool isOnCam2){
        cam1.SetActive(!isOnCam2);
        cam1Canvas.SetActive(!isOnCam2);
        cam2.SetActive(isOnCam2);
        cam2Canvas.SetActive(isOnCam2);
    }
    public void setActiveSkillBtns(bool trigger){
        foreach(Transform child in activeSkillBtnGroup){
            Button btn = child.GetComponent<Button>();
            btn.gameObject.SetActive(trigger);
        }
    }
    private void setTextReadyBtn(string str){
        readyBtn.GetComponentInChildren<Text>().text = str;
    }
    private void setActivePreviewBendle(bool trigger){
        if(0 < strikeCnt && ballGroup.childCount == 0)  
            pl.previewBundle.SetActive(trigger); 
    }
    private void ManageActiveObjects(bool trigger){
        bool isBefLightDark = activeSkillBtnList.Exists(btn => btn.SelectCircleEF.gameObject.activeSelf);
        BossBlock boss = bm.getBoss();
        // TOGGLE A
        setActiveCam(trigger);
        shootCntTxt.gameObject.SetActive(trigger);
        ballPreviewGoalImg.gameObject.SetActive(trigger);
        strikePanel.SetActive(trigger);
        if(boss) boss.setBossComponent(trigger);//* Boss Collider OFF：Ball投げる時、ぶつかるから。
        // TOGGLE B
        setActiveSkillBtns(!trigger);
        pl.arrowAxisAnchor.SetActive(!trigger);
        statusFolderPanel.gameObject.SetActive(!trigger);
        if(isBefLightDark)  setLightDarkness(!trigger); //* Light <-> Normal
    }

    public void setStrike(){//ストライク GUI表示
        if(strikeCnt < 2)
            StartCoroutine(corSetStrike());
        else
            StartCoroutine(corSetStrike(true));
    }

    private IEnumerator corSetStrike(bool isOut = false){
        strikeCntImgs[strikeCnt++].gameObject.SetActive(true);
        if(isOut){ //* アウト
            stage++;
            strikeCnt = 0;
            ShootCntTxt.text = LANG.getTxt(LANG.TXT.Out.ToString()) + "!";
            yield return new WaitForSeconds(1.5f);
            bs.init();
            switchCamera();
            bm.DoCreateBlock = true; //ブロック生成
            foreach(var img in strikeCntImgs) img.gameObject.SetActive(false); //GUI非表示 初期化
            readyBtn.gameObject.SetActive(true);
        }
        else{ //* ストライク
            ShootCntTxt.text = LANG.getTxt(LANG.TXT.Strike.ToString()) + "!";
            readyBtn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            bs.init();
        }
        bs.IsBallExist = false; //ボール発射準備 On
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
        ballPreviewDirGoal.transform.position = new Vector3(ballPreviewDirGoal.transform.position.x + rx, 0.6f + ry, zCenter);
    }

    public void setGame(string type){
        STATE state = DM.ins.convertGameState2Enum(type);
        switch(state){
            case STATE.PAUSE:
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                displayCurPassiveSkillUI(type);
                break;
            case STATE.CONTINUE:
                Time.timeScale = 1;
                resetSkillStatusTable();
                pausePanel.SetActive(false);
                break;
            case STATE.HOME:
                Time.timeScale = 1;
                Debug.Log("FINISH GAME!");
                resetSkillStatusTable();
                DM.ins.personalData.save();
                SceneManager.LoadScene("Home");
                break;
        }
    }
    public void setGameOver(){
        Debug.Log("--GAME OVER--");
        State = GameManager.STATE.GAMEOVER;
        gameoverPanel.SetActive(true);
        gvStageTxt.text = LANG.getTxt(LANG.TXT.Stage.ToString()) + " : " + stage;
        gvBestScoreTxt.text = LANG.getTxt(LANG.TXT.BestScore.ToString()) + " : " + bestScore;
    }

    public void displayCurPassiveSkillUI(string type){
        GameObject pref = (type == STATE.PAUSE.ToString())? skillInfoRowPref : inGameSkillImgBtnPref;
        Transform parentTf = (type == STATE.PAUSE.ToString())? pauseSkillStatusTableTf : inGameSkillStatusTableTf;

        //(BUG) 情報が重ならないように、一回 初期化する。
        if(type != STATE.PAUSE.ToString() && parentTf.childCount > 0)
            foreach(Transform childTf in parentTf){Destroy(childTf.gameObject);}

        List<KeyValuePair<string, int>> psvlvList = PsvSkill<int>.getPsvLVList(pl);
        int i=0;
        psvlvList.ForEach(lv => {
            if(lv.Value > 0){
                String levelTxt = (type == STATE.PAUSE.ToString())? ("x " + lv.Value.ToString()) : lv.Value.ToString();
                var rowTf = Instantiate(pref, Vector3.zero, Quaternion.identity, parentTf).transform;

                var imgObj = Instantiate(PsvSkillImgPrefs[i], Vector3.zero, Quaternion.identity, rowTf);
                // Debug.Log("displayCurPassiveSkillUI:: imgObj.name= " + imgObj.name);
                if(type != STATE.PAUSE.ToString()){
                    imgObj.transform.localScale = Vector3.one * 0.3f;
                    imgObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    int index = imgObj.transform.GetSiblingIndex();
                    imgObj.transform.SetSiblingIndex(index - 1);
                }
                //* Max Level TXT 表示。
                var maxLv = PsvSkill<int>.getPsvMaxLVList(pl).Find(maxLv => (maxLv.Key == lv.Key));
                if(lv.Value < maxLv.Value){
                    rowTf.GetComponentInChildren<Text>().text = levelTxt;
                }else{
                    rowTf.GetComponentInChildren<Text>().text = "MAX";
                    Color orange = new Color(1.0f, 0.4f, 0.0f);
                    rowTf.GetComponentInChildren<Text>().color = orange;
                }
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
        State = GameManager.STATE.WAIT;
        BossBlock boss = bm.getBoss();
        Debug.Log($"<color=white>setNextStage:: ballCnt={ballGroup.childCount}</color>");

        //* Set
        ++stage;
        comboCnt = 0;
        bm.DoCreateBlock = true; //* Block 生成
        downWallCollider.isTrigger = true; //* 下壁 物理 
        bs.IsBallExist = false;
        readyBtn.gameObject.SetActive(true);
        pl.previewBundle.SetActive(true);
        destroyEveryBalls();
        setBallPreviewGoalRandomPos();

        //* Check Event
        bm.checkIsHealBlock();
        activeSkillDataBase[0].checkBlocksIsDotDmg(this);
        StartCoroutine(coCheckPerfectBonus(boss));
        StartCoroutine(coCheckLevelUp());

        //* オーブを集める
        collectDropOrb();
        
        //* BossSkill
        bm.eraseObstacle();
        if(boss){ //* ボスが生きていると
            boss.activeBossSkill();
            stage--;
        }
    }

    private void destroyEveryBalls(){
        if(ballGroup.childCount > 0){
            for(int i=0; i<ballGroup.childCount; i++)
                Destroy(ballGroup.GetChild(i).gameObject);
        }
    }

    private void collectDropOrb(){
        var orbs = dropItemGroup.GetComponentsInChildren<DropItem>();
        Debug.Log("setNextStage:: orbs.Length= " + orbs.Length);
        Array.ForEach(orbs, dropObj => dropObj.IsMoveToPlayer = true);
    }

    private IEnumerator coCheckPerfectBonus(BossBlock boss){
        if(blockGroup.childCount == 0){
            perfectTxt.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
            em.enableUITxtEF("Perfect");
            //* One More Next Stage (ボスがいなければ)
            yield return new WaitForSeconds(1);
            if(!boss)
                ++stage;
            bm.DoCreateBlock = true;
        }
    }
    public IEnumerator coCheckLevelUp(){
        Debug.Log($"coCheckLevelUp()::checkLevelUp():: pl.BefLv= {pl.BefLv}, pl.Lv= {pl.Lv}");
        yield return new WaitForSeconds(0.8f);
        if(pl.IsLevelUp){
            pl.IsLevelUp = false;
            levelUpPanel.SetActive(true);
            levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
        }
    }
    public int getCurSkillIdx(){
        return (SelectAtvSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx : DM.ins.personalData.SelectSkill2Idx;
    }
    public float setHitPower(float distance, HitRank[] hitRank){
        float power = (distance <= hitRank[(int)DM.HITRANK.S].Dist) ? hitRank[(int)DM.HITRANK.S].Power //-> BEST HIT (HOMERUH!)
            : (distance <= hitRank[(int)DM.HITRANK.A].Dist)? hitRank[(int)DM.HITRANK.A].Power
            : (distance <= hitRank[(int)DM.HITRANK.B].Dist)? hitRank[(int)DM.HITRANK.B].Power
            : (distance <= hitRank[(int)DM.HITRANK.C].Dist)? hitRank[(int)DM.HITRANK.C].Power
            : (distance <= hitRank[(int)DM.HITRANK.D].Dist)? hitRank[(int)DM.HITRANK.D].Power
            : hitRank[(int)DM.HITRANK.E].Power; //-> WORST HIT (distance <= 1.5f)
        return power;
    }
    public string setHitRankTxt(float power, HitRank[] hitRank){
        string rankTxt = ((power == hitRank[(int)DM.HITRANK.S].Power)? DM.HITRANK.S.ToString()
            : (power == hitRank[(int)DM.HITRANK.A].Power)? DM.HITRANK.A.ToString()
            : (power == hitRank[(int)DM.HITRANK.B].Power)? DM.HITRANK.B.ToString()
            : (power == hitRank[(int)DM.HITRANK.C].Power)? DM.HITRANK.C.ToString()
            : (power == hitRank[(int)DM.HITRANK.D].Power)? DM.HITRANK.D.ToString() : DM.HITRANK.E.ToString()).ToString();
        return rankTxt;
    }
}