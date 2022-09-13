using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DM : MonoBehaviour
{
    public static DM ins;
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};
    public enum ITEM {Chara, Bat, Skill, CashShop};
    public enum ATV{FireBall, Thunder, ColorBall, PoisonSmoke, IceWave, NULL};
    public enum PSV{Dmg, MultiShot, Speed, InstantKill, Critical, Explosion, ExpUp, ItemSpawn, VerticalMultiShot, CriticalDamage, Laser, NULL};

    [Header("--Personal Data--")]
    [SerializeField] Text coinTxt; public Text CoinTxt {get => coinTxt; set => coinTxt = value;}
    [SerializeField] Text diamondTxt; public Text DiamondTxt {get => diamondTxt; set => diamondTxt = value;}

    [Header("--Select Item--")]
    public Material grayItemLock;
    [SerializeField] string selectItemType = "";    public string SelectItemType {get => selectItemType; set => selectItemType = value;}

    [SerializeField] RectTransform modelContentPref;   public RectTransform ModelContentPref {get => modelContentPref; set => modelContentPref = value;}
    [SerializeField] RectTransform itemPassivePanel;   public RectTransform ItemPassivePanel {get => itemPassivePanel; set => itemPassivePanel = value;}
    [SerializeField] RectTransform itemSkillBoxPref;   public RectTransform ItemSkillBoxPref {get => itemSkillBoxPref; set => itemSkillBoxPref = value;}
    public ScrollView[] scrollviews; //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop
    public PersonalData personalData;

    [Header("--AD--")]
    [SerializeField] bool isRemoveAD;   public bool IsRemoveAD {get => isRemoveAD; set => isRemoveAD = value;}

    void Awake() => singleton();
    void Start(){
        foreach(DM.ATV list in Enum.GetValues(typeof(DM.ATV))){
            Debug.LogFormat("Enums GetFindVal:: {0}", list.ToString());
        }
        //* contents Prefab 生成
        scrollviews[(int)DM.ITEM.Chara].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Bat].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Skill].createItem(modelContentPref, itemPassivePanel, itemSkillBoxPref);

        //* Items of Content
        ItemInfo[] charas = scrollviews[(int)DM.ITEM.Chara].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] bats = scrollviews[(int)DM.ITEM.Bat].ContentTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] skills = scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<ItemInfo>();
        
        personalData = new PersonalData();
        personalData.load(ref charas, ref bats, ref skills); //TODO Add skills
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



    public int convertItemType2Idx(){
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
        return (name == DM.PSV.Dmg.ToString())? DM.PSV.Dmg
            :(name == DM.PSV.MultiShot.ToString())? DM.PSV.MultiShot
            :(name == DM.PSV.Speed.ToString())? DM.PSV.Speed
            :(name == DM.PSV.InstantKill.ToString())? DM.PSV.InstantKill
            :(name == DM.PSV.Critical.ToString())? DM.PSV.Critical
            :(name == DM.PSV.Explosion.ToString())? DM.PSV.Explosion
            :(name == DM.PSV.ExpUp.ToString())? DM.PSV.ExpUp
            :(name == DM.PSV.ItemSpawn.ToString())? DM.PSV.ItemSpawn
            :(name == DM.PSV.VerticalMultiShot.ToString())? DM.PSV.VerticalMultiShot
            :(name == DM.PSV.CriticalDamage.ToString())? DM.PSV.CriticalDamage
            :(name == DM.PSV.Laser.ToString())? DM.PSV.Laser
            : DM.PSV.NULL; //-> ダミーデータ
    }

    public ATV convertAtvSkillStr2Enum(string name){
        return (name == DM.ATV.Thunder.ToString())? DM.ATV.Thunder
            :(name == DM.ATV.FireBall.ToString())? DM.ATV.FireBall
            :(name == DM.ATV.ColorBall.ToString())? DM.ATV.ColorBall
            :(name == DM.ATV.PoisonSmoke.ToString())? DM.ATV.PoisonSmoke
            :DM.ATV.IceWave;
    }

    // public BlockMaker.BLOCK convertBlockKind2Enum(string name){
    //     var res = gameObject.name.Contains(BlockMaker.BLOCK.Normal.ToString())? kind = BlockMaker.BLOCK.Normal
    //         : gameObject.name.Contains(BlockMaker.BLOCK.Long.ToString())? kind = BlockMaker.BLOCK.Long
    //         : gameObject.name.Contains(BlockMaker.BLOCK.TreasureChest.ToString())? kind = BlockMaker.BLOCK.TreasureChest
    //         : gameObject.name.Contains(BlockMaker.BLOCK.Heal.ToString())? kind = BlockMaker.BLOCK.Heal
    //         : BlockMaker.BLOCK.Null;
    //     return res;
    // }
}
