using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DM : MonoBehaviour
{
    public static DM ins;
    public enum NAME {DownWall, Block, FireBallDotEffect};
    public enum TAG {HitRangeArea, StrikeLine, GameOverLine, Wall, ActiveDownWall, Player,
        NormalBlock, LongBlock, TreasureChestBlock, HealBlock, BossBlock,
        Obstacle,
        };
    public enum LAYER {BallPreview};
    public enum ANIM {DoSpawn, DoShake, DoBossSpawn,IsHit, IsIdle, IsFly};
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};
    public enum ITEM {Chara, Bat, Skill, CashShop};
    public enum ATV{FireBall, Thunder, ColorBall, PoisonSmoke, IceWave, NULL};
    public enum PSV{Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage, Laser, NULL};

    [Header("GUI")]
    [SerializeField] Text coinTxt; public Text CoinTxt {get => coinTxt; set => coinTxt = value;}
    [SerializeField] Text diamondTxt; public Text DiamondTxt {get => diamondTxt; set => diamondTxt = value;}

    [Header("SELECT ITEM")]
    public Material grayItemLockMt;
    [SerializeField] string selectItemType = "";    public string SelectItemType {get => selectItemType; set => selectItemType = value;}
    [SerializeField] RectTransform modelContentPref;   public RectTransform ModelContentPref {get => modelContentPref; set => modelContentPref = value;}
    [SerializeField] RectTransform itemPassivePanel;   public RectTransform ItemPassivePanel {get => itemPassivePanel; set => itemPassivePanel = value;}
    [SerializeField] RectTransform itemSkillBoxPref;   public RectTransform ItemSkillBoxPref {get => itemSkillBoxPref; set => itemSkillBoxPref = value;}
    public ScrollView[] scrollviews; //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop

    [Header("SAVE DATE")]
    public PersonalData personalData;

    [Header("AD : 未完")]
    [SerializeField] bool isRemoveAD;   public bool IsRemoveAD {get => isRemoveAD; set => isRemoveAD = value;}

    void Awake() => singleton();
    void Start(){
        // foreach(DM.ATV list in Enum.GetValues(typeof(DM.ATV)))Debug.LogFormat("Enums GetFindVal:: {0}", list.ToString())

        //* contents Prefab 生成
        scrollviews[(int)DM.ITEM.Chara].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Bat].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Skill].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.CashShop].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);

        //* Items of Content (Set UnLockList)
        ItemInfo[] charas = scrollviews[(int)DM.ITEM.Chara].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] bats = scrollviews[(int)DM.ITEM.Bat].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] skills = scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<ItemInfo>();

        personalData = new PersonalData(); //* DataBase
        personalData.load(ref charas, ref bats, ref skills); //TODO Add skills

        //* PersonalData後に処理必要なもの（LANGUAGEため）
        scrollviews[(int)DM.ITEM.Skill].setLanguage();
        scrollviews[(int)DM.ITEM.CashShop].setLanguage();
    }

    void Update(){
        CoinTxt.text = personalData.Coin.ToString();
        DiamondTxt.text = personalData.Diamond.ToString();
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

    public void showAD(string type){
        Debug.Log("<color=yellow> showAD(" + type.ToString() + ")</color>");
        //TODO
        switch(type){
            case "CoinX2":
                break;
            default:
                break;
        }
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



    public int getCurItemType2Idx(){
        return (SelectItemType == DM.ITEM.Chara.ToString())? (int)DM.ITEM.Chara
            : (SelectItemType == DM.ITEM.Bat.ToString())? (int)DM.ITEM.Bat
            : (SelectItemType == DM.ITEM.Skill.ToString())? (int)DM.ITEM.Skill
            : (SelectItemType == DM.ITEM.CashShop.ToString())? (int)DM.ITEM.CashShop
            : -9999;
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
        string[] DMG = LANG.Dmg, MTS = LANG.MultiShot, SPD = LANG.Speed, ITK = LANG.InstantKill,
            CRT = LANG.Critical, EPS = LANG.Explosion, EXP = LANG.ExpUp, ISP = LANG.ItemSpawn,
            MTSV = LANG.VerticalMultiShot, CRTD = LANG.CriticalDamage, LSR = LANG.Laser;
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
