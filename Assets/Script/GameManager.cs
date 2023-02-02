using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
    public enum STATE {PLAY, WAIT, GAMEOVER, PAUSE, CONTINUE, HOME, NULL};
    [SerializeField] private STATE state;     public STATE State {get => state; set => state = value;}
    [SerializeField] private DM.MODE mode;       public DM.MODE Mode {get => mode; set => mode = value;}
    public float timelineNum;
    public Animator postProcessAnim;
    
    [Header("GROUP")][Header("__________________________")]
    public Transform effectGroup;
    public Transform ballGroup;
    public Transform blockGroup;
    public Transform dropItemGroup;
    public Transform dropBoxGroup;
    public Transform bossGroup;
    public Transform obstacleGroup;
    public Transform activeSkillBtnGroup;
    public RectTransform showExpUIGroup;
    
    [Header("CAMERA")][Header("__________________________")]
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam1Canvas;
    public GameObject cam2Canvas;

    //* OutSide
    [Space]
    [Header("OUTSIDE")][Header("__________________________")]
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
    public TouchSlideControl touchSlideControlPanel;
    public Light light;
    
    [Header("STATUS")][Header("__________________________")]
    public int coin = 0;
    public int diamond = 0;
    public int stage = 1;
    public int bestScore;
    public int strikeCnt = 0;
    public int comboCnt = 0;
    public int bossLimitCnt = 0;
    public int bossKillCnt = 0; public int BossKillCnt { get => bossKillCnt; set => bossKillCnt = value;}
    public int rewardItemCoin = 0;  public int RewardItemCoin { get => rewardItemCoin; set => rewardItemCoin = value;}
    public int rewardItemDiamond = 0;   public int RewardItemDiamond { get => rewardItemDiamond; set => rewardItemDiamond = value;}
    public int rewardItemRouletteTicket = 0;    public int RewardItemRouletteTicket { get => rewardItemRouletteTicket; set => rewardItemRouletteTicket = value;}

    [Header("TRIGGER")][Header("__________________________")]
    public bool isPlayingAnim;  public bool IsPlayingAnim { get => isPlayingAnim; set=> isPlayingAnim = value;}
    public bool isFastPlay;  public bool IsFastPlay { get => isFastPlay; set => isFastPlay = value;}

    [Header("PANEL")][Header("__________________________")]
    public GameObject strikePanel;
    public GameObject levelUpPanel;
    public GameObject pausePanel;
    public GameObject gameoverPanel;
    public GameObject victoryPanel;
    public RectTransform statusFolderPanel;
    public GameObject getRewardChestPanel;

    [Header("DIALOG")][Header("__________________________")]
    public RectTransform showAdDialog;
    public RectTransform openTutorialDialog;
    public Text adDialogTitleTxt;
    public Text adDialogContentTxt;
    public RectTransform askGiveUpDialog;

    [Header("◆GUI◆")][Header("__________________________")]
    public Text stageTxt;
    public Text bossLimitCntTxt;
    public Text stateTxt;
    public Text[] statusTxts = new Text[2];
    [SerializeField] Text shootCntTxt;      public Text ShootCntTxt { get => shootCntTxt; set => shootCntTxt = value;}
    public RectTransform homeRunTxtTf;
    public Slider expBar, bossStageBar;
    public Color bossStageBarColorGray;
    public Color bossStageBarColorPink;
    public Text expBarTxt, bossStageTxt;
    public Toggle skipTutorialToggle;
    public GameObject coinX2Label;
    public Text modeTxt;

    [Header("TEXT EFFECT")][Header("__________________________")]
    public Text comboTxt;
    public Text perfectTxt;
    public GameObject bossSapwnAnimObj;
    public Text bossNameTxt;
    public Text showHitBallInfoTf;

    [Header("PREVIEW BALL SILDER ➡ 現在使っていない")][Header("__________________________")] //! あんまり要らないかも。
    public Slider hitRangeSlider;
    private RectTransform hitRangeSliderTf;

    [Header("BALL PREVIEW DIR GOAL(CAM2)")][Header("__________________________")]
    public GameObject ballPreviewDirGoal;
    public Image ballPreviewGoalImg;

    [Header("STRIKE CNT IMAGE")][Header("__________________________")]
    public Image[] strikeCntImgs;

    [Header("ACTIVE SKILL BTN")][Header("__________________________")]
    public RectTransform activeSkillBtnPf;
    public AtvSkill[] activeSkillDataBase; //* 全てActiveSkillsのデータベース
    [FormerlySerializedAs("activeSkillBtnList")] public List<AtvSkillBtnUI> activeSkillBtnList; //* ActiveSkillボタン
    public Material activeSkillBtnEfMt;
    [SerializeField] int selectAtvSkillBtnIdx;  public int SelectAtvSkillBtnIdx { get=> selectAtvSkillBtnIdx; set=> selectAtvSkillBtnIdx = value;}
    public bool isPointUp; //* SectorGizmos Colliderへ活用するため。
    public Material[] blockGlowColorMts;

    [Header("PASSIVE TABLE")][Header("__________________________")]
    [SerializeField] private GameObject[] psvSkillImgPrefs;    public GameObject[] PsvSkillImgPrefs { get => psvSkillImgPrefs; set => psvSkillImgPrefs = value;}
    public RectTransform psvImgRectTfTemp;  public RectTransform PsvImgRectTfTemp {get => psvImgRectTfTemp; set => psvImgRectTfTemp = value;}
    public RectTransform inGameSkillStatusTableTf;
    public GameObject inGameSkillImgBtnPref;
    
    [Header("PAUSE")][Header("__________________________")]
    public RectTransform pauseSkillStatusTableTf;
    public GameObject skillInfoRowPref;
    public Text levelTxt;
    public Text pauseCoinTxt;
    public Text pauseDiamondTxt;
    public Text pauseModeTxt;

    [Header("GAMEOVER")][Header("__________________________")]
    [FormerlySerializedAs("gvCoinTxt")]   public Text gvCoinTxt;
    [FormerlySerializedAs("gvDiamondTxt")]   public Text gvDiamondTxt;
    [FormerlySerializedAs("gvBestScoreTxt")]   public Text gvBestScoreTxt;
    [FormerlySerializedAs("gvStageTxt")]   public Text gvStageTxt;
    [FormerlySerializedAs("gvRewardItemCoinTxt")]   public Text gvRewardItemCoinTxt;
    [FormerlySerializedAs("gvRewardItemDiamondTxt")]   public Text gvRewardItemDiamondTxt;
    [FormerlySerializedAs("gvRewardItemRouletteTicketTxt")]   public Text gvRewardItemRouletteTicketTxt;

    [Header("VICTORY")][Header("__________________________")]
    [FormerlySerializedAs("vtrCoinTxt")]   public Text vtrCoinTxt;
    [FormerlySerializedAs("vtrDiamondTxt")]   public Text vtrDiamondTxt;
    [FormerlySerializedAs("vtrBestScoreTxt")]   public Text vtrBestScoreTxt;
    [FormerlySerializedAs("vtrStageTxt")]   public Text vtrStageTxt;
    [FormerlySerializedAs("vtrRewardItemCoinTxt")]   public Text vtrRewardItemCoinTxt;
    [FormerlySerializedAs("vtrRewardItemDiamondTxt")]   public Text vtrRewardItemDiamondTxt;
    [FormerlySerializedAs("vtrRewardItemRouletteTicketTxt")]   public Text vtrRewardItemRouletteTicketTxt;

    [Header("STATUS FOLDER")][Header("__________________________")]
    public Transform statusInfoContents;
    public GameObject statusInfoPf;
    public Sprite[] statusIconSprArr;
    public Color[] statusIconColorArr;

    [Header("BUTTON")][Header("__________________________")]
    public Button readyBtn; //normal
    public Button fastPlayBtn; //normal & hit ball
    public Button reGameBtn; //gameoverPanel
    public Button pauseBtn; //pausePanel
    public Button continueBtn; //pausePanel
    public Button homeBtn; //pausePanel
    public Button statusFolderBtn;
    public Button rerotateSkillSlotsBtn; //levelUpPanel
    public Button reviveBtn; //gameoverPanel
    public Button coinX2Btn; //gameoverPanel
    public Button adPricePayBtn;
    public Button adFreeBtn;
    public Button rewardChestOpenBtn;
    public Button rewardChestOkBtn;

    [Header("PSV UNIQUE")][Header("__________________________")]
    public GameObject eggPf;

    [Header("REWARD CHEST")][Header("__________________________")]
    public Text rewardChestTitleTxt;
    public Text rewardChestContentTxt;
    public Image rewardChestIconImg;
    public Sprite defChestSpr;
    public Sprite openChestSpr;
    public Sprite coinBundleSpr;
    public Sprite diamondBundleSpr;
    public Sprite psvSkillTicketSpr;
    public Sprite rouletteTicketSpr;
    public Sprite emptyPoopSpr;

    [Header("SKY")][Header("__________________________")]
    public GameObject skySunObj;
    public GameObject skyMoonObj;
    void Start() {
        // Util._.calcArithmeticProgressionList(start: 100, max: 50, d: 100, gradualUpValue: 0.1f);

        DM.ins.transform.position = Vector3.zero; //* LoadingSceneで、モデルが見えないようにずらした位置を戻す。

        //* スキップに✓がされていない場合は、チュートリアルのダイアログ 表示。
        if(!DM.ins.personalData.IsSkipTutorial){
            displayTutorialDialog();
        }

        stage = LM._.STAGE_NUM;
        Debug.Log("<color=red>----------------------------------------------P L A Y   S C E N E----------------------------------------------</color>");
        //! init()宣言したら、キャラクターモデルを読み込むことができないBUG
        pl = GameObject.Find("Player").GetComponent<Player>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        bs = GameObject.Find("BallShooter").GetComponent<BallShooter>();
        bm = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();

        light = GameObject.Find("Directional Light").GetComponent<Light>();
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();
        readyBtn = readyBtn.GetComponent<Button>();

        bossLimitCntTxt.gameObject.SetActive(false);
        fastPlayBtn.gameObject.SetActive(false);
        Array.ForEach(statusTxts, txt => txt.text = LANG.getTxt(LANG.TXT.Status.ToString()));
        rewardChestTitleTxt.text = LANG.getTxt(LANG.TXT.Reward.ToString());
        rewardChestOpenBtn.GetComponentInChildren<Text>().text = LANG.getTxt(LANG.TXT.Open.ToString());

        //* Ball Preview Dir Goal Set Z-Center
        setBallPreviewGoalRandomPos();

        //* Ready LevelUpPanel Psv All SkillImg Temp
        Array.ForEach(psvSkillImgPrefs, psvSkillImg => {
            var ins = Instantiate(psvSkillImg, Vector3.zero, Quaternion.identity, psvImgRectTfTemp);
            ins.name = psvSkillImg.name;
        });

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
            activeSkillBtnList.Add(new AtvSkillBtnUI(i, LM._.ATV_COOLDOWN_UNIT, pl.activeSkills[idx].Name, btn.GetComponent<Button>(), pl.activeSkills[idx].UISprite, activeSkillBtnEfMt));
        }

        //* Set Active Skill Damage
        new AtvSkill(this, pl);

        //* Sky Style
        Debug.Log("SkyMt.offsetX= " + DM.ins.simpleSkyMt.GetTextureOffset("_MainTex").x);
        if(DM.ins.simpleSkyMt.GetTextureOffset("_MainTex").x == LM._.SKY_MT_MORNING_VALUE)
            skySunObj.SetActive(true);
        else if(DM.ins.simpleSkyMt.GetTextureOffset("_MainTex").x == LM._.SKY_MT_DINNER_VALUE)
            skyMoonObj.SetActive(true);

        //* Set Mode
        Debug.Log($"Set Mode:: {stage} > {LM._.VICTORY_BOSSKILL_CNT} * {LM._.BOSS_STAGE_SPAN} : {stage > LM._.VICTORY_BOSSKILL_CNT * LM._.BOSS_STAGE_SPAN}");
        if(stage > LM._.VICTORY_BOSSKILL_CNT * LM._.BOSS_STAGE_SPAN){
            mode = DM.MODE.HARD;
        }

        //* Pause Panel UI
        pauseCoinTxt.text = DM.ins.personalData.Coin.ToString();
        pauseDiamondTxt.text = DM.ins.personalData.Diamond.ToString();
        pauseModeTxt.text = mode.ToString() + " MODE";
        pauseModeTxt.color = (mode == DM.MODE.HARD)? Color.red : Color.white;

        //* ModeTxt Anim
        modeTxt.text = mode.ToString() + " MODE";
        modeTxt.color = (mode == DM.MODE.HARD)? Color.red : Color.white;
    }

    void Update(){
        // Time.timeScale = timelineNum;

        BossBlock boss = bm.getBoss();

        //* GUI *//
        //* EXP BAR & TEXT
        expBar.value = Mathf.Lerp(expBar.value, (float)pl.Exp / (float)pl.MaxExp, Time.deltaTime * 10);
        expBarTxt.text = $"{pl.Exp} / {pl.MaxExp}";

        //* BOSS BAR & TEXT
        bossStageBar.GetComponent<RectTransform>().anchorMin = new Vector2((boss? 0.15f : 0.5f), 0.5f);
        bossStageBar.value = Mathf.Lerp(bossStageBar.value, 
            boss? ((float)boss.Hp / boss.MaxHp >= 0)? (float)boss.Hp / boss.MaxHp : 0
                : (float)(stage % LM._.BOSS_STAGE_SPAN) / (float)LM._.BOSS_STAGE_SPAN
            , Time.deltaTime * 10);
        bossStageTxt.text = boss? $"{boss.Hp} / {boss.MaxHp}"
            : $"{stage % LM._.BOSS_STAGE_SPAN} / {LM._.BOSS_STAGE_SPAN}";
        
        Image fillImg = Array.Find(bossStageBar.GetComponentsInChildren<Image>(), img => img.transform.name == "Fill");
        fillImg.color = boss? bossStageBarColorPink : bossStageBarColorGray;
        Image iconFrameImg = Array.Find(bossStageBar.GetComponentsInChildren<Image>(), img => img.transform.name == "IconFrame");
        iconFrameImg.color = boss? Color.white : bossStageBarColorGray;
        Image iconImgImg = Array.Find(bossStageBar.GetComponentsInChildren<Image>(), img => img.transform.name == "IconImg");
        iconImgImg.color = boss? Color.white : bossStageBarColorGray;

        //* ANOTHER TEXT
        stateTxt.text = State.ToString();
        levelTxt.text = LANG.getTxt(LANG.TXT.Level.ToString()) + " : " + pl.Lv;
        stageTxt.text = LANG.getTxt(LANG.TXT.Stage.ToString()) + " : " + stage.ToString();
        comboTxt.text = LANG.getTxt(LANG.TXT.Combo.ToString()) + "\n" + comboCnt.ToString();
        bossLimitCntTxt.text = LANG.getTxt(LANG.TXT.BossLimitCnt.ToString()) + " : " + bossLimitCnt.ToString();

        //* ActiveSkill Status
        activeSkillBtnList.ForEach(btn=> btn.setActiveSkillEF());
    }

    public void setBallPreviewGoalImgRGBA(Color color) => ballPreviewGoalImg.color = color;
    public void throwScreenAnimSetTrigger(string name) => throwScreen.GetComponent<Animator>().SetTrigger(name);
    public void setLightDarkness(bool isOn){ //* During Skill Casting ...
        light.color = (isOn)? Color.black : Color.white;
        // light.type = (isOn)? LightType.Spot : LightType.Directional;
    }
//* --------------------------------------------------------------------------------------
//* GUI Button
//* --------------------------------------------------------------------------------------
    // public void onClickReGameButton() => init();
    public void onClickSetGameButton(string type) => setGame(type);
    public void onClickReadyButton() { switchCamera(); SM.ins.sfxPlay(SM.SFX.BtnClick.ToString()); }
    public void onClickSkillButton() { levelUpPanel.SetActive(false); SM.ins.sfxPlay(SM.SFX.BtnClick.ToString()); }
    public void onClickGiveUpButton() => displayAskGiveUpDialog();
    public void onClickOpenTutorialButton() {
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        DM.ins.displayTutorialUI();
        openTutorialDialog.gameObject.SetActive(false);
    }
    public void onClickSkipTutorialNextTimeToggle() {DM.ins.personalData.IsSkipTutorial = skipTutorialToggle.isOn; SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());}
    public void onClickRewardChestOpenButton() {
        Debug.Log("onClickRewardChestOpenButton()::");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        StartCoroutine(coRewardChestOpen());
    }
    public void onClickFastPlayButton(){
        Debug.Log("onClickFastPlayButton::");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        fastPlayBtn.GetComponent<Image>().color = Color.red;
        isFastPlay = true;
        Time.timeScale = 2.5f;
    }
/// --------------------------------------------------------------------------
/// REWARD CHEST RANDOM OPEN
/// --------------------------------------------------------------------------
    IEnumerator coRewardChestOpen(){
        Debug.Log("coRewardChestOpen()::");
        initRewardChestPanelUI(isOpen: true);
        getRewardChestPanel.GetComponentInChildren<Animator>().SetTrigger(DM.ANIM.DoOpen.ToString());
        yield return new WaitForSecondsRealtime(1); //* (BUG-10) RewardChestPanelが表示されるとき、Time.Scaleが０のため、RealTimeで動く。
        
        SM.ins.sfxPlay(SM.SFX.Upgrade.ToString());
        rewardChestOkBtn.gameObject.SetActive(true);
        const int GOODS = 0, PSVSKILL_TICKET = 1, ROULETTE_TICKET = 2, EMPTY = 3;
        var goodsPriceDic = new Dictionary<string, int>();
        int rand = Random.Range(0, 100);
        int reward = 
            (rand < LM._.REWARD_GOODS_PER)? GOODS
            : (rand < LM._.REWARD_PSVSKILL_TICKET_PER)? PSVSKILL_TICKET
            : (rand < LM._.REWARD_ROULETTE_TICKET_PER)? ROULETTE_TICKET
            : EMPTY;

        //* (BUG-12)LevelUpPanelが最初に表示するとき、Start()が実行される、PSVスキル選びがLevelに上書きされるバグ対応。
        if(levelUpPanel.activeSelf && reward == PSVSKILL_TICKET)
            reward = GOODS;
        //* (BUG-16)UIスタイル変更が上書きされ、できない。-> Lv2まではPSV_TICKETができないように設定。
        if(pl.Lv < 2 && reward == PSVSKILL_TICKET)
            reward = GOODS;

        switch(reward){
            case GOODS:
                const int COIN = 0, DIAMOND = 1;
                rand = Random.Range(0, 100);
                int kind = (rand < 50)? COIN : DIAMOND;
                switch(kind){
                    case COIN:{
                        int[] priceArr = LM._.REWARD_CHEST_COINARR;
                        rand = Random.Range(0, priceArr.Length);
                        setRewardChestPanelUI(coinBundleSpr, priceArr[rand].ToString(), LANG.getTxt(LANG.TXT.Get.ToString()));
                        goodsPriceDic.Add(DM.NAME.Coin.ToString(), priceArr[rand]); //* Reward
                        break;
                    }
                    case DIAMOND:{
                        int[] priceArr = LM._.REWARD_CHEST_DIAMONDARR;
                        rand = Random.Range(0, priceArr.Length);
                        setRewardChestPanelUI(diamondBundleSpr, priceArr[rand].ToString(), LANG.getTxt(LANG.TXT.Get.ToString()));
                        goodsPriceDic.Add(DM.NAME.Diamond.ToString(), priceArr[rand]); //* Reward
                        break;
                    }
                }
                break;
            case PSVSKILL_TICKET:
                setRewardChestPanelUI(psvSkillTicketSpr, LANG.getTxt(LANG.TXT.PsvSkillTicket.ToString()), LANG.getTxt(LANG.TXT.Use.ToString()));
                break;
            case ROULETTE_TICKET:
                setRewardChestPanelUI(rouletteTicketSpr, LANG.getTxt(LANG.TXT.RouletteTicket.ToString()), LANG.getTxt(LANG.TXT.Get.ToString()));
                break;
            case EMPTY:
                setRewardChestPanelUI(emptyPoopSpr, LANG.getTxt(LANG.TXT.Empty.ToString()), LANG.getTxt(LANG.TXT.Ok.ToString()));
                break;
        }

        //* AddEventListener('onClick')
        rewardChestOkBtn.onClick.RemoveAllListeners(); //* (BUG-13) リワート種類によって処理が変わるのでAddListenerをしましたが、繰り返したら重なるバグを初期化する形で対応。
        rewardChestOkBtn.onClick.AddListener(() => onClickRewardChestOkButton(reward, goodsPriceDic));
    }
    private void initRewardChestPanelUI(bool isOpen){
        rewardChestOpenBtn.gameObject.SetActive(!isOpen? true : false);
        rewardChestIconImg.sprite = !isOpen? defChestSpr : openChestSpr;
        rewardChestContentTxt.text = !isOpen? LANG.getTxt(LANG.TXT.GetRewardChestPanel_Content.ToString()) : "";
    }
    public void initRewardChestPanel(){
        getRewardChestPanel.SetActive(true);
        initRewardChestPanelUI(isOpen: false);
        rewardChestOkBtn.gameObject.SetActive(false);
    }
    private void setRewardChestPanelUI(Sprite iconSpr, string contentTxt, string okTxt){
        rewardChestIconImg.sprite = iconSpr;
        rewardChestContentTxt.text = contentTxt;
        rewardChestOkBtn.GetComponentInChildren<Text>().text = okTxt;
    }
    public void onClickRewardChestOkButton(int reward, Dictionary<string, int> goodsPriceDic){
        Debug.Log("onClickRewardChestOkButton:: reward= " + reward);
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        Time.timeScale = 1;
        const int GOODS = 0, PSVSKILL_TICKET = 1, ROULETTE_TICKET = 2, EMPTY = 3;
        switch(reward){
            case GOODS:
                if(goodsPriceDic.TryGetValue(DM.NAME.Coin.ToString(), out int rewardCoin))
                    // coin += rewardCoin;
                    rewardItemCoin += rewardCoin;
                else if(goodsPriceDic.TryGetValue(DM.NAME.Diamond.ToString(), out int rewardDiamond))
                    // diamond += rewardDiamond;
                    rewardItemDiamond += rewardDiamond;

                break;
            case PSVSKILL_TICKET:
                Debug.Log("onClickRewardChestOkButton:: PSVSKILL_TICKET!");
                levelUpPanel.GetComponent<LevelUpPanelAnimate>().IsPsvSkillTicket = true;
                levelUpPanel.SetActive(true);
                levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
                levelUpPanel.GetComponent<LevelUpPanelAnimate>().setUI(DM.NAME.PsvSkillTicket.ToString());
                break;
            case ROULETTE_TICKET:
                // DM.ins.personalData.addRouletteTicket(1);
                rewardItemRouletteTicket++;
                break;
            case EMPTY:
                //何もしない。
                break;
        }
        getRewardChestPanel.SetActive(false); //* パンネル閉じる。
    }

    public void onClickActiveSkillButton(int i) {
        if(pl.IsStun) return;
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
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
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        float pivotX = statusFolderPanel.pivot.x;
        if(pivotX == -1) isTrigger = false;
        else isTrigger = true;
        Debug.Log("onClickStatusFolderButton():: isTrigger= " + isTrigger + ", pivotX=" + pivotX);
        statusFolderPanel.pivot = new Vector2(isTrigger? -1 : 0, 0.5f);

        //* Init
        for(int i=0;i<statusInfoContents.childCount; i++)
            Destroy(statusInfoContents.GetChild(i).gameObject);

        //* Set InfoTxt List
        List<string> infoTxtList = PsvSkill<int>.getPsvStatusInfo2Str(pl);
        List<string> nameList = new List<string>();
        List<string> valueList = new List<string>();

        for(int i=0; i<infoTxtList.Count; i++){
            if(i % 2 == 0) nameList.Add(infoTxtList[i]);
            else valueList.Add(infoTxtList[i]);
        }

        //* Set statusInfo UI Style
        {
            const int BLUE = 0, RED = 1, ORANGE = 2, GREEN = 3;
            int i=0;
            nameList.ForEach(list => {
                if(i == 0 || i == 1) statusInfoPf.transform.Find("IconPanel").GetComponent<Image>().color = statusIconColorArr[BLUE];
                else if(i == 2 || i == 3) statusInfoPf.transform.Find("IconPanel").GetComponent<Image>().color = statusIconColorArr[RED];
                else if(i == 4 || i == 5 || i == 6 || i == 7) statusInfoPf.transform.Find("IconPanel").GetComponent<Image>().color = statusIconColorArr[ORANGE];
                else statusInfoPf.transform.Find("IconPanel").GetComponent<Image>().color = statusIconColorArr[GREEN];

                statusInfoPf.transform.Find("IconPanel").transform.Find("IconImg").GetComponentInChildren<Image>().sprite = statusIconSprArr[i];
                statusInfoPf.transform.Find("NameTxt").GetComponent<Text>().text = nameList[i];
                statusInfoPf.transform.Find("ValueTxt").GetComponent<Text>().text = valueList[i];
                Instantiate(statusInfoPf, Vector3.zero, Quaternion.identity, statusInfoContents);
                i++;
            });
        }
    }
    public void onClickBtnOpenShowAdDialog(string type) {
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        showAdDialog.gameObject.SetActive(true);
        adPricePayBtn.gameObject.SetActive(true);
        Text price = adPricePayBtn.GetComponentInChildren<Text>();
        Image icon = Array.Find(adPricePayBtn.GetComponentsInChildren<Image>(), chd => chd.name.Contains("Icon"));
        
        //* AddEventListener('onClick')
        adPricePayBtn.onClick.AddListener(() => onClickPayButton(type));
        adFreeBtn.onClick.AddListener(() => onClickShowADButton(type));

        //* Set Language
        if(type == DM.REWARD.CoinX2.ToString()){
            adDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogCoinX2_Title.ToString());
            adDialogContentTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogCoinX2_Content.ToString());
            adPricePayBtn.gameObject.SetActive(false);
        }
        else if(type == DM.REWARD.RerotateSkillSlots.ToString()){
            adDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRerotateSkillSlots_Title.ToString());
            adDialogContentTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRerotateSkillSlots_Content.ToString());
            icon.sprite = DM.ins.CoinSpr;
            price.text = LM._.REROTATE_SKILLSLOTS_PRICE_COIN.ToString();
        }
        else if(type == DM.REWARD.Revive.ToString()){
            adDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRevive_Title.ToString());
            adDialogContentTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRevive_Content.ToString());
            icon.sprite = DM.ins.DiamondSpr;
            price.text = LM._.REVIVE_PRICE_DIAMOND.ToString();
        }
    }
    public void onClickPayButton(string rewardType){
        Debug.Log($"onClickPayButton:: {rewardType}");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        if(rewardType == DM.REWARD.RerotateSkillSlots.ToString()){
            setRerotateSkillSlots();
        }
        else if(rewardType == DM.REWARD.Revive.ToString()){
            setRevive();
        }
        showAdDialog.gameObject.SetActive(false); //* ダイアログ閉じる
    }
    public void onClickShowADButton(string rewardType){
        Debug.Log("<color=yellow> onClickShowADButton(" + rewardType.ToString() + ")</color>");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        //* 広告
        bool success = DM.ins.reqShowAD();
        if(success){
            setShowADReward(rewardType);
        }
        else{
            SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            Util._.displayNoticeMsgDialog("ERROR！");
        }
    }

//*---------------------------------------
//*  関数
//*---------------------------------------
#region AD REWARD
    private void setShowADReward(string rewardType){
        if(rewardType == DM.REWARD.RerotateSkillSlots.ToString()){
            setRerotateSkillSlots();
        }
        else if(rewardType == DM.REWARD.Revive.ToString()){
            setRevive();
        }
        else if(rewardType == DM.REWARD.CoinX2.ToString()){
            setCoinX2();
        }
        //* ダイアログ閉じる
        showAdDialog.gameObject.SetActive(false);
    }

    private void setRerotateSkillSlots(){
        Debug.Log("setRerotateSkillSlots::");
        levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
        rerotateSkillSlotsBtn.gameObject.SetActive(false);
    }

    private void setRevive(){
        State = GameManager.STATE.WAIT;
        gameoverPanel.SetActive(false);
        BossBlock boss = bm.getBoss();
        if(boss) bossLimitCnt = LM._.BOSS_LIMIT_SPAN;
        setActiveCam(false); // cam1 ON, cam2 OFF
        reviveBtn.gameObject.SetActive(false);
        Invoke("collectDropOrb", 0.5f);
        SM.ins.sfxPlay(SM.SFX.Revive.ToString());
        em.activeUI_EF("Revive");
        bm.Start();
    }

    private void setCoinX2(){
        gvCoinTxt.text = (int.Parse(gvCoinTxt.text) * 2).ToString();
        coinX2Btn.gameObject.SetActive(false);
        coinX2Label.gameObject.SetActive(true);
    }
#endregion

    public void init(){
        State = GameManager.STATE.WAIT;
        gameoverPanel.SetActive(false);
        stage = 1;
        strikeCnt = 0;
        comboCnt = 0;
        pl.Lv = 1;
        pl.Exp = 0;
        pl.MaxExp = 100;
        pl.Start();
        bm.Start();
        //* Collect Drop Items Exp
        var dropObjs = dropItemGroup.GetComponentsInChildren<DropItem>();
    }

    public void switchCamera(){
        if(State == GameManager.STATE.GAMEOVER) return;
        if(pl.IsStun) return;
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
    
    public void setActiveCam(bool isOnCam2){
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
        SM.ins.sfxPlay(SM.SFX.CountDownStrike.ToString());
        strikeCntImgs[strikeCnt++].gameObject.SetActive(true);
        if(isOut){ //* アウト
            strikeCnt = 0;
            ShootCntTxt.text = LANG.getTxt(LANG.TXT.Out.ToString()) + "!";
            yield return new WaitForSeconds(1.5f);
            bs.init();
            switchCamera();
            foreach(var img in strikeCntImgs) img.gameObject.SetActive(false); //GUI非表示 初期化
            // stage++;
            // bm.DoCreateBlock = true; //ブロック生成
            // readyBtn.gameObject.SetActive(true);
            // pl.previewBundle.SetActive(true);
            setNextStage();
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

    public void setGameOver(){
        Debug.Log("<size=30> --- G A M E O V E R --- </size>");
        SM.ins.sfxPlay(SM.SFX.Defeat.ToString());
        setFinishGame(
            gameoverPanel, gvBestScoreTxt, gvStageTxt, gvCoinTxt, gvDiamondTxt
            , gvRewardItemCoinTxt, gvRewardItemDiamondTxt, gvRewardItemRouletteTicketTxt);
    }

    public void setVictory(){
        Debug.Log("<size=30> --- V I C T O R Y --- </size>");
        SM.ins.sfxPlay(SM.SFX.Victory.ToString());
        setFinishGame(
            victoryPanel, vtrBestScoreTxt, vtrStageTxt, vtrCoinTxt, vtrDiamondTxt
            , vtrRewardItemCoinTxt, vtrRewardItemDiamondTxt, vtrRewardItemRouletteTicketTxt);

        //* HardMode unlocked
        if(!DM.ins.personalData.IsHardmodeOn)
            DM.ins.personalData.IsHardmodeOn = true;

        //* Achivement
        if(mode == DM.MODE.NORMAL)
            AcvNormalModeClear.setNormalModeClear();
        else if(mode == DM.MODE.HARD)
            AcvHardModeClear.setHardModeClear();
    }

    private void setFinishGame(GameObject panel, Text scoreTxt, Text stageTxt, Text coinTxt, Text diamondTxt,
        Text rewardItemCoinTxt, Text rewardItemDiamondTxt, Text rewardItemRouletteTicketTxt){
        State = GameManager.STATE.GAMEOVER;
        panel.SetActive(true);

        //* UI
        scoreTxt.text = LANG.getTxt(LANG.TXT.BestScore.ToString()) + " : " + bestScore;
        stageTxt.text = LANG.getTxt(LANG.TXT.Stage.ToString()) + " : " + stage;

        //* Set Stage
        if(mode == DM.MODE.HARD)
            stage -= LM._.VICTORY_BOSSKILL_CNT * LM._.BOSS_STAGE_SPAN;

        //* Coin & Diamond
        coin += stage * LM._.STAGE_PER_COIN_PRICE;
        diamond += stage * LM._.STAGE_PER_DIAMOND_PRICE;
        int extraUpgradeCoin = Mathf.RoundToInt(coin * DM.ins.personalData.Upgrade.Arr[(int)DM.UPGRADE.CoinBonus].getValue());

        //* Show Goods => setGameでも使う。
        coinTxt.text = (coin + extraUpgradeCoin * (mode == DM.MODE.HARD? LM._.HARDMODE_COIN_BONUS : 1)).ToString();
        diamondTxt.text = (diamond * (mode == DM.MODE.HARD? LM._.HARDMODE_DIAMOND_BONUS : 1)).ToString();

        //* Show Reward Item Goods
        rewardItemCoinTxt.text = rewardItemCoin.ToString();
        rewardItemDiamondTxt.text = rewardItemDiamond.ToString();
        rewardItemRouletteTicketTxt.text = rewardItemRouletteTicket.ToString();

        //* Add Reward Goods
        int resultCoin = int.Parse(coinTxt.text) + rewardItemCoin;
        int resultDiamond = int.Parse(diamondTxt.text) + rewardItemDiamond;
        int resultRouletteTicket = rewardItemRouletteTicket;

        DM.ins.personalData.addCoin(resultCoin); // DM.ins.personalData.Coin += int.Parse(coinTxt.text);
        DM.ins.personalData.addDiamond(resultDiamond);
        DM.ins.personalData.addRouletteTicket(resultRouletteTicket);
        
        //* Rate Dialogを表示するため
        DM.ins.personalData.PlayTime++;
    }

    public void setGame(string type){
        Debug.Log($"setGame(type= {type})::");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
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
                
                resetSkillStatusTable();
                DM.ins.personalData.save();
                SceneManager.LoadScene(DM.SCENE.Home.ToString());
                break;
        }
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

    public List<DropBox> dropBoxList;
    public void setNextStage() {
        if(IsFastPlay){
            IsFastPlay = false;
            Time.timeScale = 1;
        }

        //* Victory
        if(bossKillCnt >= LM._.VICTORY_BOSSKILL_CNT){
            setVictory();
            return;
        }

        State = GameManager.STATE.WAIT;
        BossBlock boss = bm.getBoss();

        //* (BUG-34) nextStage()に行く前、ballCountが1になり、downWallCollider.isTriggerがfalseのままになるBUG。
        Debug.Log($"<color=white>setNextStage:: ballCnt={ballGroup.childCount}</color>");

        //* Set
        ++stage;
        comboCnt = 0;
        bm.DoCreateBlock = true; //* Block 生成
        downWallCollider.isTrigger = true; //* 下壁 物理 無視。
        Debug.Log($"<color=yellow>setNextStage:: downWallCollider.isTrigger={downWallCollider.isTrigger}</color>");
        bs.IsBallExist = false;
        pl.IsStun = false;
        readyBtn.gameObject.SetActive(true);
        fastPlayBtn.gameObject.SetActive(false);
        fastPlayBtn.GetComponent<Image>().color = Color.white;
        pl.previewBundle.SetActive(true);
        destroyEveryBalls();
        setBallPreviewGoalRandomPos();

        //* Check Event
        bm.checkIsHealBlock();
        bm.setBlockPropertyDuration();

        activeSkillDataBase[0].checkBlocksIsDotDmg(this);
        StartCoroutine(coCheckPerfectBonus(boss));
        StartCoroutine(coCheckLevelUp());
        StartCoroutine(coCheckGetRewardChest());

        //* オーブを集める
        Invoke("collectDropOrb", 0.5f);
        
        //* BossSkill
        if(boss){
            stage--; //ステージは同じく維持
            bossLimitCnt--;
            //* ボース制限時間が０になったら、GAMEOVER!!
            if(bossLimitCnt <= 0){
                setGameOver();
                return;
            }
            boss.activeBossSkill();
        }

        //* Achivement (StageClear)
        if(DM.ins.personalData.ClearStage <= stage){
            DM.ins.personalData.ClearStage = stage;
        }

        //* Achivement (StageClear IsComplete)
        AcvStageClear.setStageClear(stage);
    }

    private void destroyEveryBalls(){
        if(ballGroup.childCount > 0){
            for(int i=0; i<ballGroup.childCount; i++)
                Destroy(ballGroup.GetChild(i).gameObject);
        }
    }

    private void collectDropOrb(){
        //* (BUG-6) 後で破壊したブロックからでるOrbがPlayerに行かない。Invokeで0.5秒を待た後で収集。
        var orbs = dropItemGroup.GetComponentsInChildren<DropItem>();
        Debug.Log("setNextStage():: Invoke(collectDropOrb):: MoveToPlayer ON -> orbs.Length= " + orbs.Length);
        Array.ForEach(orbs, orb => orb.IsMoveToPlayer = true);
    }

    private IEnumerator coCheckPerfectBonus(BossBlock boss){
        if(blockGroup.childCount == 0){
            perfectTxt.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
            em.activeUI_EF("Perfect");
            //* One More Next Stage (ボスがいなければ)
            yield return new WaitForSeconds(1);
            if(!boss)
                ++stage;
            bm.DoCreateBlock = true;
        }
    }
    public IEnumerator coCheckLevelUp(){
        //* Coroutineの中、while文でUpdate()ように活用：ExpOrbがPlayerに全て届くまで待つ。
        while(true){
            if(pl.IsLevelUp){ //* <- Player::setLevelUp()
                yield return new WaitForSeconds(1);
                pl.IsLevelUp = false;
                levelUpPanel.SetActive(true);
                rerotateSkillSlotsBtn.gameObject.SetActive(true);
                levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
                break;
            }
            yield return null;
        }
    }
    public IEnumerator coCheckGetRewardChest(){
        //* Coroutineの中、while文でUpdate()ように活用：RewardChestがPlayerに届くまで待つ。
        while(true){
            if(pl.IsGetRewardChest){
                pl.IsGetRewardChest = false;
                initRewardChestPanel();
                break;
            }
            yield return null;
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

    public void displayTutorialDialog(){
        openTutorialDialog.gameObject.SetActive(true);

        //* Language
        Transform dialog = openTutorialDialog.Find("Dialog");
        Text title = dialog.Find(DM.NAME.TitleTxt.ToString()).GetComponent<Text>();
        Text content = dialog.Find(DM.NAME.ContentTxt.ToString()).GetComponent<Text>();
        Text okBtn = dialog.Find("OkBtn").GetComponentInChildren<Text>();

        title.text = LANG.getTxt(LANG.TXT.Tutorial.ToString());
        content.text = LANG.getTxt(LANG.TXT.OpenTutorial_Content.ToString());
        okBtn.text = LANG.getTxt(LANG.TXT.Ok.ToString());
    }

    public void displayAskGiveUpDialog(){
        askGiveUpDialog.gameObject.SetActive(true);

        //* Language
        Transform dialog = askGiveUpDialog.Find("Dialog");
        Text title = dialog.Find(DM.NAME.TitleTxt.ToString()).GetComponent<Text>();
        Text content = dialog.Find(DM.NAME.ContentTxt.ToString()).GetComponent<Text>();
        Text notice = dialog.Find("NoticeTxt").GetComponent<Text>();
        Text okBtn = dialog.Find("OkBtn").GetComponentInChildren<Text>();

        title.text = LANG.getTxt(LANG.TXT.Caution.ToString());
        content.text = LANG.getTxt(LANG.TXT.Caution_Content.ToString());
        notice.text = LANG.getTxt(LANG.TXT.Caution_Notice.ToString());
        okBtn.text = LANG.getTxt(LANG.TXT.Ok.ToString());
    }
}