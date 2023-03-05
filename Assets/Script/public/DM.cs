using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Purchasing;
using System;

public class DM : MonoBehaviour{
    public static DM ins;    
    public HomeManager hm;
    public GameManager gm;
    public enum DATABASE_KEY {Json};
    public enum QUALITY {Low, Medium, High};
    public enum SCENE {Home, Play, Loading};
    public enum MODE {NORMAL, HARD, NIGHTMARE};
    public enum NAME {
        MainCanvas,
        Coin, Diamond,
        DownWall, Block, FireBallDotEffect, BossDieDropOrbSpot, GrayPanel, Obstacle,
        RightArm, HomeManager, GameManager,
        Ball, BallPreview, Box001, Area, MainBall, SubBall,
        IceMat,
        MainPanel, RewardChest, 
        LevelUp, PsvSkillTicket,
        TitleTxt, ContentTxt,
        IconPanel, IconImg, NameTxt, ValueTxt,
        RemoveAD, PurchasedPanel, AdNoticeTxt, //* 一回限り商品
        IAPBtn_Icon,
        DropBoxStarTrailEF, DropBoxSpeedTrailEF,
        NULL,
    };
    public enum TAG {HitRangeArea, StrikeLine, GameOverLine, Wall, ActiveDownWall, Player,
        NormalBlock, LongBlock, TreasureChestBlock, HealBlock, BossBlock,   
        Obstacle, PlayerBattingSpot, 
        PsvSkillNormal, PsvSkillUnique,
    };
    public enum LAYER {BallPreview};
    public enum ANIM {BossSpawnTxt_Spawn, DoSpawn, 
        DoShake, Swing, DoBossSpawn, Die, IsHit, 
        IsIdle, IsFly, GetHit, Scream, Attack, Touch, HomeRun, Mode, Perfect, BossClear, BossLimitcntAlertTxtEF,
        DoOpen, DoBlur, ThrowBall};
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};
    public enum HITRANK{S, A, B, C, D, E};
    public enum PANEL {Chara, Bat, Skill, CashShop, PsvInfo, Upgrade, NULL};
    public enum ATV {ThunderShot, FireBall, ColorBall, PoisonSmoke, IceWave, NULL};
    public enum PSV {
        //* Normal Passive
        Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage,
        Laser, FireProperty, IceProperty, ThunderProperty,
        //* Unique Passive
        DamageHalfUp, GiantBall, DarkOrb, GodBless, BirdFriend,
        NULL
    };
    public enum UPGRADE {
        Dmg, BallSpeed, Critical, CriticalDamage, BossDamage, CoinBonus, Defence, NULL
    }
    public enum DROPBOX {
        DropBoxShieldPf,  // Barrier from Boss Attack
        DropBoxQuestionPf, // Get Coin
        DropBoxSpeedPf, // Speed Up x 2
        DropBoxStarPf, //Power Up x 2
        DropBoxMagicPf, //Atv Skill Cool Down
    }
    public enum REWARD {
        //* Home
        ROULETTE_TICKET,
        //* Play
        CoinX2, RerotateSkillSlots, Revive, 
        NULL
    }

    [Header("GPGS")][Header("__________________________")]
    [SerializeField] bool isGPGSLogin;  public bool IsGPGSLogin {get => isGPGSLogin; set => isGPGSLogin = value;}

    [Header("STAGE MODE")][Header("__________________________")]
    [SerializeField] int stageNum = 1; public int StageNum {get => stageNum; set => stageNum = value;}
    [SerializeField] MODE mode; public MODE Mode {get => mode; set => mode = value;}

    [Header("GUI")][Header("__________________________")]
    [SerializeField] Sprite coinSpr; public Sprite CoinSpr {get => coinSpr; set => coinSpr = value;}
    [SerializeField] Sprite diamondSpr; public Sprite DiamondSpr {get => diamondSpr; set => diamondSpr = value;}
    [SerializeField] Text coinTxt; public Text CoinTxt {get => coinTxt; set => coinTxt = value;}
    [SerializeField] Text diamondTxt; public Text DiamondTxt {get => diamondTxt; set => diamondTxt = value;}

    [Header("SELECT ITEM")][Header("__________________________")]
    [SerializeField] Material grayItemLockMt;   public Material GrayItemLockMt {get => grayItemLockMt; set => grayItemLockMt = value;}
    [SerializeField] string selectItemType = "";    public string SelectItemType {get => selectItemType; set => selectItemType = value;}
    [SerializeField] RectTransform modelContentPref;   public RectTransform ModelContentPref {get => modelContentPref; set => modelContentPref = value;}
    [SerializeField] RectTransform itemPassivePanel;   public RectTransform ItemPassivePanel {get => itemPassivePanel; set => itemPassivePanel = value;}
    [SerializeField] RectTransform itemSkillBoxPref;   public RectTransform ItemSkillBoxPref {get => itemSkillBoxPref; set => itemSkillBoxPref = value;}

    [Header("SCROLL VIEWS")][Header("__________________________")]
    public ScrollView[] scrollviews; //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop

    [Header("★★【 MY DATE (SAVE & LOAD) 】★★")][Header("__________________________")]
    public PersonalData personalData;

    [Header("ITEM RANK COLOR")][Header("__________________________")]
    [FormerlySerializedAs("rankGeneralColor")] public Color rankGeneralClr;
    [FormerlySerializedAs("rankRareColor")] public Color rankRareClr;
    [FormerlySerializedAs("rankUniqueColor")] public Color rankUniqueClr;
    [FormerlySerializedAs("rankLegendColor")] public Color rankLegendClr;
    [FormerlySerializedAs("rankGodColor")] public Color rankGodClr;
    [FormerlySerializedAs("darkGray")] public Color darkGray;

    [Header("TUTORIAL")][Header("__________________________")]
    public TutorialPanel tutorialPanel;

    [Header("SKY")][Header("__________________________")]
    public Material simpleSkyMt;

    void Awake() {
        Application.targetFrameRate = 40;
        singleton();
    }
    void singleton(){
        //* Singleton
        if(ins == null) {
            Debug.Log("DM::singleton():: Start App Only One Time");
            HomeManager hm = GameObject.Find(NAME.HomeManager.ToString()).GetComponent<HomeManager>(); //* (BUG-50) 再ロードしたら、DM.ins.hmがNULLになり、全てのデータがちゃんと読みだされないバグ対応。
            ins = this;
            //* Google Play Login
            GPGSBinder.Inst.Login((success, user) => {
                Debug.Log($"GPGS::LOGIN= {success}");
                hm.googlePlayLoginTxt.text = $"{success}, {user.userName}, {user.id}, {user.state}, {user.underage}";
                if(success == true) 
                    DM.ins.IsGPGSLogin = true;
                else 
                    hm.errorNetworkDialog.gameObject.SetActive(true);
            });
        }
        else if(ins != null) {
            Debug.Log($"DM::singleton():: Home Scene Repeat, Because this.object is Declared in Home Scene");
            DM.ins.hm = GameObject.Find(NAME.HomeManager.ToString()).GetComponent<HomeManager>(); //* (BUG-50) 再ロードしたら、DM.ins.hmがNULLになり、全てのデータがちゃんと読みだされないバグ対応。
            DM.ins.CoinTxt = this.CoinTxt;
            DM.ins.DiamondTxt = this.DiamondTxt;
            
            int i=0;
            Array.ForEach(DM.ins.scrollviews, sv => {
                sv.ScrollRect = this.scrollviews[i].ScrollRect;
                sv.ContentTf = this.scrollviews[i].ContentTf;
                sv.ItemPrefs = this.scrollviews[i].ItemPrefs;
                i++;
            });

            //! (BUG-防止) "Home"シーンに戻った場合、scrollViewsがnullなくても、ItemPassiveが宣言しないためエラー。
            DM.ins.personalData.ItemPassive = this.personalData.ItemPassive;

            DM.ins.Start();
            
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start(){
        Debug.Log("DM::Start():: DM.ins.hm= " + DM.ins.hm);
        if(DM.ins.hm == null) return;

        LANG.initlanguageList();
        // foreach(DM.ATV list in Enum.GetValues(typeof(DM.ATV)))Debug.LogFormat("Enums GetFindVal:: {0}", list.ToString())

        //* contents Prefab 生成
        scrollviews[(int)DM.PANEL.Chara].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.PANEL.Bat].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.PANEL.Skill].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.PANEL.CashShop].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.PANEL.PsvInfo].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.PANEL.Upgrade].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);

        //* Items of Content (Set UnLockList)
        ItemInfo[] charas = scrollviews[(int)DM.PANEL.Chara].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] bats = scrollviews[(int)DM.PANEL.Bat].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] skills = scrollviews[(int)DM.PANEL.Skill].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] upgrades = scrollviews[(int)DM.PANEL.Upgrade].ContentTf.GetComponentsInChildren<ItemInfo>();

        Debug.Log($"DM::Start():: charas.len= {charas.Length}, bats.len= {bats.Length}, skills.len= {skills.Length}, upgrades= {upgrades.Length}");
        personalData = new PersonalData(charas.Length, bats.Length, skills.Length); //* DataBase
        personalData.load(ref charas, ref bats, ref skills);

        //* アップグレードデータ 初期化。
        scrollviews[(int)DM.PANEL.Upgrade].initUpgradeDt(upgrades);
        scrollviews[(int)DM.PANEL.Skill].initAtvSkillUpgradeDt(skills);

        //* 3DモデルパンネルはsetLanguageないから、ここでタイトルを言語設定。
        scrollviews[(int)DM.PANEL.Chara].ScrollRect.GetComponent<ScrollViewEvent>().TitleTxt.text = LANG.getTxt(LANG.TXT.Character.ToString());
        scrollviews[(int)DM.PANEL.Bat].ScrollRect.GetComponent<ScrollViewEvent>().TitleTxt.text = LANG.getTxt(LANG.TXT.Bat.ToString());

        //* PersonalData後に処理必要なもの（LANGUAGEため）
        scrollviews[(int)DM.PANEL.Skill].setLanguage();
        scrollviews[(int)DM.PANEL.CashShop].setLanguage();
        scrollviews[(int)DM.PANEL.PsvInfo].setLanguage();
        scrollviews[(int)DM.PANEL.Upgrade].setLanguage();

        setUIRemoveAD();

        //* ERROR CHECK
        LANG.checkErrorLangListCounting();
    }

    void Update(){
        if(CoinTxt) CoinTxt.text = personalData.Coin.ToString();
        if(DiamondTxt) DiamondTxt.text = personalData.Diamond.ToString();
    }

#region QUIT APP
#if UNITY_EDITOR
    void OnApplicationQuit(){
        Debug.Log("QUIT APP(PC)::OnApplicationQuit():: Scene= " + SceneManager.GetActiveScene().name);
        //* (BUG-42) PLAYシーンで終了しても、購入したデータがセーブできるように。
        // if(SceneManager.GetActiveScene().name == DM.SCENE.Home.ToString())
        personalData.save();
    }
#elif UNITY_ANDROID
    
    //* (BUG-42) PLAYシーンで終了しても、購入したデータがセーブできるように。
    void OnApplicationPause(bool paused){
        //! (BUG-37) ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true){
            Debug.Log("QUIT APP(Mobile)::OnApplicationPause( "+paused+" ):: Scene= " + SceneManager.GetActiveScene().name);
            personalData.save();
        }
    }
#endif
#endregion

    public void setUIRemoveAD(){ //* (BUG-27) DM::setUIRemoveAD:: IsRemoveを購入したら、CashShopにある目録も"購入完了"にする。
        if(personalData.IsRemoveAD){
            var cashCtt = scrollviews[(int)DM.PANEL.CashShop].ContentTf;
            for(int i=0; i<cashCtt.childCount; i++){
                Debug.Log($"DM::Start()::cashCtt.Getchild({i})= " + cashCtt.GetChild(i).name);
                if(cashCtt.GetChild(i).name.Contains(DM.NAME.RemoveAD.ToString())){
                    //* IsLock
                    cashCtt.GetChild(i).GetComponent<ItemInfo>().IsLock = true;
                    //  * PurchasedPanel
                    cashCtt.GetChild(i).Find(DM.NAME.PurchasedPanel.ToString()).gameObject.SetActive(true);
                }
            }
        }
    }

    public TutorialPanel displayTutorialUI(){
        Transform canvas = GameObject.Find(DM.NAME.MainCanvas.ToString()).GetComponent<RectTransform>();
        TutorialPanel tuto = Instantiate(tutorialPanel, canvas, false);
        return tuto;
    }

    public DM.PANEL getCurPanelType2Enum(string name){
        return (name == DM.PANEL.Chara.ToString())? DM.PANEL.Chara
            : (name == DM.PANEL.Bat.ToString())? DM.PANEL.Bat
            : (name == DM.PANEL.Skill.ToString())? DM.PANEL.Skill
            : (name == DM.PANEL.CashShop.ToString())? DM.PANEL.CashShop
            : (name == DM.PANEL.PsvInfo.ToString())? DM.PANEL.PsvInfo
            : (name == DM.PANEL.Upgrade.ToString())? DM.PANEL.Upgrade
            : DM.PANEL.NULL;
    }
    

    public GameManager.STATE convertGameState2Enum(string name){
        return (name == GameManager.STATE.PLAY.ToString())? GameManager.STATE.PLAY
            :(name == GameManager.STATE.WAIT.ToString())? GameManager.STATE.WAIT
            :(name == GameManager.STATE.GAMEOVER.ToString())? GameManager.STATE.GAMEOVER
            :(name == GameManager.STATE.PAUSE.ToString())? GameManager.STATE.PAUSE
            :(name == GameManager.STATE.CONTINUE.ToString())? GameManager.STATE.CONTINUE
            :(name == GameManager.STATE.HOME.ToString())? GameManager.STATE.HOME
            :(name == GameManager.STATE.GIVEUP.ToString())? GameManager.STATE.GIVEUP
            : GameManager.STATE.NULL;
    }

    public PSV convertPsvSkillStr2Enum(string name){
        //* (BUG) All Lang 対応。
        string n = name;
        string[] DMG = LANG.Dmg, 
            MTS = LANG.MultiShot, 
            SPD = LANG.Speed, 
            ITK = LANG.InstantKill,
            CRT = LANG.Critical, 
            EPS = LANG.Explosion, 
            EXP = LANG.ExpUp, 
            ISP = LANG.ItemSpawn,
            MTSV = LANG.VerticalMultiShot, 
            CRTD = LANG.CriticalDamage, 
            LSR = LANG.Laser,
            FPT = LANG.FireProperty,
            IPT = LANG.IceProperty,
            TPT = LANG.ThunderProperty,
            DTW = LANG.DamageHalfUp,
            GTB = LANG.GiantBall,
            DOB = LANG.DarkOrb,
            GBS = LANG.GodBless,
            BFR = LANG.BirdFriend;

        const int EN = (int)LANG.TP.EN, JP = (int)LANG.TP.JP, KR = (int)LANG.TP.KR;

        return (n == DMG[EN] || n == DMG[JP] || n == DMG[KR])? PSV.Dmg
            :(n == MTS[EN] || n == MTS[JP] || n == MTS[KR])? PSV.MultiShot
            :(n == SPD[EN] || n == SPD[JP] || n == SPD[KR])? PSV.Speed
            :(n == ITK[EN] || n == ITK[JP] || n == ITK[KR])? PSV.InstantKill
            :(n == CRT[EN] || n == CRT[JP] || n == CRT[KR])? PSV.Critical
            :(n == EPS[EN] || n == EPS[JP] || n == EPS[KR])? PSV.Explosion
            :(n == EXP[EN] || n == EXP[JP] || n == EXP[KR])? PSV.ExpUp
            :(n == ISP[EN] || n == ISP[JP] || n == ISP[KR])? PSV.ItemSpawn
            :(n == MTSV[EN] || n == MTSV[JP] || n == MTSV[KR])? PSV.VerticalMultiShot
            :(n == CRTD[EN] || n == CRTD[JP] || n == CRTD[KR])? PSV.CriticalDamage
            :(n == LSR[EN] || n == LSR[JP] || n == LSR[KR])? PSV.Laser
            :(n == FPT[EN] || n == FPT[JP] || n == FPT[KR])? PSV.FireProperty
            :(n == IPT[EN] || n == IPT[JP] || n == IPT[KR])? PSV.IceProperty
            :(n == TPT[EN] || n == TPT[JP] || n == TPT[KR])? PSV.ThunderProperty
            //* Unique Psv 
            :(n == DTW[EN] || n == DTW[JP] || n == DTW[KR])? PSV.DamageHalfUp
            :(n == GTB[EN] || n == GTB[JP] || n == GTB[KR])? PSV.GiantBall
            :(n == DOB[EN] || n == DOB[JP] || n == DOB[KR])? PSV.DarkOrb
            :(n == GBS[EN] || n == GBS[JP] || n == GBS[KR])? PSV.GodBless
            :(n == BFR[EN] || n == BFR[JP] || n == BFR[KR])? PSV.BirdFriend

            : PSV.NULL; //-> ダミーデータ
    }

    public ATV convertAtvSkillStr2Enum(string name){
        return (name == ATV.ThunderShot.ToString())? ATV.ThunderShot
            :(name == ATV.FireBall.ToString())? ATV.FireBall
            :(name == ATV.ColorBall.ToString())? ATV.ColorBall
            :(name == ATV.PoisonSmoke.ToString())? ATV.PoisonSmoke
            :(name == ATV.IceWave.ToString())? ATV.IceWave
            : ATV.NULL;
    }

    public UPGRADE convertUpgradeStr2Enum(string name){
        return (name == UPGRADE.Dmg.ToString())? UPGRADE.Dmg
            :(name == UPGRADE.BallSpeed.ToString())? UPGRADE.BallSpeed
            :(name == UPGRADE.BossDamage.ToString())? UPGRADE.BossDamage
            :(name == UPGRADE.CoinBonus.ToString())? UPGRADE.CoinBonus
            :(name == UPGRADE.Critical.ToString())? UPGRADE.Critical
            :(name == UPGRADE.CriticalDamage.ToString())? UPGRADE.CriticalDamage
            :(name == UPGRADE.Defence.ToString())? UPGRADE.Defence
            : UPGRADE.NULL;
    }
}
