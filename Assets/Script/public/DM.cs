using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;

public class DM : MonoBehaviour
{
    public static DM ins;
    public enum DATABASE_KEY {Json};
    public enum SCENE {Home, Play, Loading};
    public enum NAME {
        Coin, diamond,
        DownWall, Block, FireBallDotEffect, BossDieDropOrbSpot, GrayPanel, Obstacle,
        RightArm, HomeManager,
        Ball, BallPreview, Box001, Area,
        IceMat,
        MainPanel, RewardChest, 
        LevelUp, PsvSkillTicket,
    };
    public enum TAG {HitRangeArea, StrikeLine, GameOverLine, Wall, ActiveDownWall, Player,
        NormalBlock, LongBlock, TreasureChestBlock, HealBlock, BossBlock,   
        Obstacle, PlayerBattingSpot, 
        PsvSkillNormal, PsvSkillUnique,
    };
    public enum LAYER {BallPreview};
    public enum ANIM {BossSpawnTxt_Spawn, DoSpawn, DoShake, Swing, DoBossSpawn, Die, IsHit, IsHitBall, IsIdle, IsFly, GetHit, Scream, Attack, Touch, HomeRun, DoOpen};
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};
    public enum HITRANK{S, A, B, C, D, E};
    public enum PANEL {Chara, Bat, Skill, CashShop, PsvInfo, Upgrade, NULL};
    public enum ATV {FireBall, Thunder, ColorBall, PoisonSmoke, IceWave, NULL};
    public enum PSV {
        //* Normal Passive
        Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage,
        Laser, FireProperty, IceProperty, ThunderProperty,
        //* Unique Passive
        DamageTwice, GiantBall, DarkOrb, GodBless, BirdFriend,
        NULL
    };
    public enum UPGRADE {
        Dmg, BallSpeed, Critical, CriticalDamage, BossDamage, CoinBonus, Defence, NULL
    }
    public enum REWARD {
        CoinX2, RerotateSkillSlots, Revive,
    }

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

    [Header("AD : 未完")][Header("__________________________")]
    [SerializeField] bool isRemoveAD;   public bool IsRemoveAD {get => isRemoveAD; set => isRemoveAD = value;}

    void Awake() => singleton();
    void Start(){
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

        personalData = new PersonalData(); //* DataBase
        personalData.load(ref charas, ref bats, ref skills); //TODO Add skills

        //* Init DMデータ ロード。
        for(int i=0; i<upgrades.Length; i++){
            UpgradeDt upgradeDt = personalData.Upgrade.Arr[i];
            upgrades[i].setUpgradeGUI(upgradeDt);
            List<int> priceList = Util._.calcArithmeticProgressionList(start: 100, upgradeDt.maxLv, d: 100, gradualUpValue: 0.1f);
            upgrades[i].price.setValue(priceList[upgradeDt.lv]);
        }

        //* PersonalData後に処理必要なもの（LANGUAGEため）
        scrollviews[(int)DM.PANEL.Skill].setLanguage();
        scrollviews[(int)DM.PANEL.CashShop].setLanguage();
        scrollviews[(int)DM.PANEL.PsvInfo].setLanguage();
        scrollviews[(int)DM.PANEL.Upgrade].setLanguage();

        //* ERROR CHECK
        LANG.checkErrorLangListCounting();
    }

    void Update(){
        if(CoinTxt) CoinTxt.text = personalData.Coin.ToString();
        if(DiamondTxt) DiamondTxt.text = personalData.Diamond.ToString();
    }

#if UNITY_EDITOR
    void OnApplicationQuit(){
        Debug.Log("PC -> OnApplicationQuit():: END GAME:: Scene= " + SceneManager.GetActiveScene().name);
        //* (BUG) SceneがHomeのみセーブできる。
        if(SceneManager.GetActiveScene().name == "Home"){
            personalData.save();
        }
    }
#elif UNITY_ANDROID
#elif UNITY_IPHONE
    void OnApplicationPause(bool paused){
        Debug.Log("Mobile -> OnApplicationPause():: END GAME:: Scene= " + SceneManager.GetActiveScene().name);
        if(paused && SceneManager.GetActiveScene().name == "Home"){
            personalData.save();
        }
    }
#endif

    public bool reqShowAD(string type, GameManager gm){
        // Debug.Log("<color=yellow> reqShowAD(" + type.ToString() + ")</color>");
        // if(type == DM.REWARD.CoinX2.ToString()){
        //     gm.coinTxt.text = (int.Parse(gm.coinTxt.text) * 2).ToString();
        //     gm.coinX2Btn.gameObject.SetActive(false);
        // }
        // else if(type == DM.REWARD.RerotateSkillSlots.ToString()){
        //     gm.levelUpPanel.GetComponent<LevelUpPanelAnimate>().Start();
        //     gm.showAdDialog.gameObject.SetActive(false);
        // }
        // else if(type == DM.REWARD.Revive.ToString()){
        //     gm.State = GameManager.STATE.WAIT;
        //     gm.gameoverPanel.SetActive(false);
        //     gm.bm.Start();
        //     BossBlock boss = gm.bm.getBoss();
        //     if(boss) gm.bossLimitCnt = LM._.BOSS_LIMIT_SPAN;
        //     gm.setActiveCam(false); // cam1 ON, cam2 OFF

        //     gm.reviveBtn.gameObject.SetActive(false);
        // }
        // else{
        //     return false;
        // }
        return true;
    }

    void singleton(){
        //* Singleton
        if(ins == null) ins = this;
        else if(ins != null) {
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
            DTW = LANG.DamageTwice,
            GTB = LANG.GiantBall,
            DOB = LANG.DarkOrb,
            GBS = LANG.GodBless,
            BFR = LANG.BirdFriend;

        const int EN = (int)LANG.TP.EN, JP = (int)LANG.TP.JP, KR = (int)LANG.TP.KR;

        return (n == DMG[EN] || n == DMG[JP] || n == DMG[KR])? DM.PSV.Dmg
            :(n == MTS[EN] || n == MTS[JP] || n == MTS[KR])? DM.PSV.MultiShot
            :(n == SPD[EN] || n == SPD[JP] || n == SPD[KR])? DM.PSV.Speed
            :(n == ITK[EN] || n == ITK[JP] || n == ITK[KR])? DM.PSV.InstantKill
            :(n == CRT[EN] || n == CRT[JP] || n == CRT[KR])? DM.PSV.Critical
            :(n == EPS[EN] || n == EPS[JP] || n == EPS[KR])? DM.PSV.Explosion
            :(n == EXP[EN] || n == EXP[JP] || n == EXP[KR])? DM.PSV.ExpUp
            :(n == ISP[EN] || n == ISP[JP] || n == ISP[KR])? DM.PSV.ItemSpawn
            :(n == MTSV[EN] || n == MTSV[JP] || n == MTSV[KR])? DM.PSV.VerticalMultiShot
            :(n == CRTD[EN] || n == CRTD[JP] || n == CRTD[KR])? DM.PSV.CriticalDamage
            :(n == LSR[EN] || n == LSR[JP] || n == LSR[KR])? DM.PSV.Laser
            :(n == FPT[EN] || n == FPT[JP] || n == FPT[KR])? DM.PSV.FireProperty
            :(n == IPT[EN] || n == IPT[JP] || n == IPT[KR])? DM.PSV.IceProperty
            :(n == TPT[EN] || n == TPT[JP] || n == TPT[KR])? DM.PSV.ThunderProperty
            //* Unique Psv 
            :(n == DTW[EN] || n == DTW[JP] || n == DTW[KR])? DM.PSV.DamageTwice
            :(n == GTB[EN] || n == GTB[JP] || n == GTB[KR])? DM.PSV.GiantBall
            :(n == DOB[EN] || n == DOB[JP] || n == DOB[KR])? DM.PSV.DarkOrb
            :(n == GBS[EN] || n == GBS[JP] || n == GBS[KR])? DM.PSV.GodBless
            :(n == BFR[EN] || n == BFR[JP] || n == BFR[KR])? DM.PSV.BirdFriend

            : DM.PSV.NULL; //-> ダミーデータ
    }

    public ATV convertAtvSkillStr2Enum(string name){
        return (name == DM.ATV.Thunder.ToString())? DM.ATV.Thunder
            :(name == DM.ATV.FireBall.ToString())? DM.ATV.FireBall
            :(name == DM.ATV.ColorBall.ToString())? DM.ATV.ColorBall
            :(name == DM.ATV.PoisonSmoke.ToString())? DM.ATV.PoisonSmoke
            :DM.ATV.IceWave;
    }
}
