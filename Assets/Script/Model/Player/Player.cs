using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    //* OutSide
    GameManager gm;  EffectManager em;
    public Animator anim;

    public Transform modelMovingTf;

    [Header("ARROW")][Header("__________________________")]
    public int MAX_SWING_HIT_DEG; //左右ある方向の最大角度
    public float offsetHitDeg; // Startが０度(←方向)から、↑ →向きに回る。
    public GameObject arrowAxisAnchor;
    public Image swingArcArea;
    private float swingArcRange;
    public GameObject previewBundle;
    public GameObject[] ballPreviewSphere = new GameObject[2]; // 2個
    public HitRank[] hitRank;

    [Header("STATUS")][Header("__________________________")]
    [SerializeField] bool doSwing = false;      public bool DoSwing {get=> doSwing; set=> doSwing=value;}
    [SerializeField] bool isLevelUp = false;    public bool IsLevelUp {get=> isLevelUp; set=> isLevelUp=value;}
    [SerializeField] bool isGetRewardChest = false;    public bool IsGetRewardChest {get=> isGetRewardChest; set=> isGetRewardChest=value;}
    [SerializeField] bool isBarrier = false;    public bool IsBarrier {get=> isBarrier; set=> isBarrier=value;}
    [SerializeField] bool isStun = false;    public bool IsStun {get=> isStun; set=> isStun=value;}
    [SerializeField] int befLv;                public int BefLv {get=> befLv; set=> befLv=value;}
    [SerializeField] int lv = 1;                public int Lv {get=> lv; set=> lv=value;}
    [SerializeField] int maxExp = 100;        public int MaxExp {get=> maxExp; set=> maxExp=value;}
    [SerializeField] int exp = 0;               public int Exp {get=> exp; set=> exp=value;}

    [Header("ACTIVE SKILL")][Header("__________________________")]
    public float atvSkillCoolDownUnit = 0.05f;  public float AtvSkillCoolDownUnit {get=> atvSkillCoolDownUnit;}
    public string[] registAtvSkillNames;// = new string[3];
    public AtvSkill[] activeSkills;// = new ActiveSkill[3];
    [SerializeField] Transform batEffectTf;           public Transform BatEffectTf {get => batEffectTf; set => batEffectTf = value;}
    [SerializeField] Transform castEFArrowTf;         public Transform CastEFArrowTf {get => castEFArrowTf;}
    [SerializeField] Transform[] castEFBallPreviewTfs;   public Transform[] CastEFBallPreviewTfs {get => castEFBallPreviewTfs; set => castEFBallPreviewTfs = value;}
    [SerializeField] float thunderCastWidth;     public float ThunderCastWidth {get=> thunderCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float fireBallCastWidth;    public float FireBallCastWidth {get=> fireBallCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float poisonSmokeCastWidth;    public float PoisonSmokeCastWidth {get=> poisonSmokeCastWidth;   set => poisonSmokeCastWidth = value;}
    
    [Header("PASSIVE SKILL")][Header("__________________________")]
    public PsvSkill<int> dmg;
    public PsvSkill<int> multiShot;
    public PsvSkill<float> speed;
    public PsvSkill<float> instantKill;
    public PsvSkill<float> critical;
    public PsvSkill<Explosion> explosion;
    public PsvSkill<float> expUp;
    public PsvSkill<float> itemSpawn;
    public PsvSkill<int> verticalMultiShot;
    public PsvSkill<float> criticalDamage;
    public PsvSkill<int> laser;
    public PsvSkill<float> fireProperty;
    public PsvSkill<float> iceProperty;
    public PsvSkill<float> thunderProperty;
    public PsvSkill<int> damageTwice;
    public PsvSkill<float> giantBall;
    public PsvSkill<float> darkOrb;
    public PsvSkill<float> godBless;
    public PsvSkill<float> birdFriend;
    
    [Header("PASSIVE UNIQUE")][Header("__________________________")]
    [SerializeField] GameObject birdFriendObj;   public GameObject BirdFriendObj {get=>birdFriendObj; set=>birdFriendObj=value;}

    public void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em;

        //* Player Model Set Parent
        BefLv = Lv;
        var playerModel = DM.ins.transform.GetChild(0);
        playerModel.SetParent(modelMovingTf);// Parent TfをPlayerに移動。
        playerModel.SetSiblingIndex(0); // Child INDEXを０番に移動。

        var atvSkillDb = gm.activeSkillDataBase;
        registAtvSkillNames = new string[atvSkillDb.Length];
        activeSkills = new AtvSkill[atvSkillDb.Length];

        //* Player Charaは 必ず０番目のINDEXにすること！！
        var charaTf = modelMovingTf.GetChild(0);

        //* Bat Effect Transform
        Transform tf = Util._.getCharaRightArmPath(charaTf);
        int childLastIdx = tf.childCount - 1;
        Transform bat = tf.GetChild(childLastIdx);
        BatEffectTf = bat.Find("BatEffectTf");
        // Debug.Log("Player:: Start:: RightArm.childCount= " + RightArm.childCount + ", RightArm= " + RightArm);
        // Debug.Log("Player:: charaTf= " + charaTf + ", BatEffectTf= " + BatEffectTf);

        #region Set ATV Skill DataBase
        //* A. Resource
        int i=0;
        Array.ForEach(atvSkillDb, dt=>{
            // Get All Skill Names
            registAtvSkillNames[i] = dt.Name;
            // Get All Skill Data
            activeSkills[i] = new AtvSkill(registAtvSkillNames[i], atvSkillDb);
            // Set EffectManager All Skill Effects
            em.activeSkillBatEFs[i] = activeSkills[i].BatEfPref;
            em.activeSkillShotEFs[i] = activeSkills[i].ShotEfPref;
            em.activeSkillExplosionEFs[i] = activeSkills[i].ExplosionEfPref;
            em.activeSkillCastEFs[i] = activeSkills[i].CastEfPref;
            em.directlyCreateActiveSkillBatEF(i, BatEffectTf);
            i++;
        });
        #endregion

        #region Set PSV Skill Data
        //* Init
        var psvLvArr = DM.ins.personalData.ItemPassive.Arr;
        //* General
        dmg = new PsvSkill<int>(LANG.getTxt(DM.PSV.Dmg.ToString()), psvLvArr[(int)DM.PSV.Dmg].lv, 
            value: 1, unit: 1);
        multiShot = new PsvSkill<int>(LANG.getTxt(DM.PSV.MultiShot.ToString()), psvLvArr[(int)DM.PSV.MultiShot].lv, 
            value: 0, unit: 1, maxLv: 4);
        speed = new PsvSkill<float>(LANG.getTxt(DM.PSV.Speed.ToString()), psvLvArr[(int)DM.PSV.Speed].lv, 
            value: 1f, unit: 0.35f);
        instantKill = new PsvSkill<float>(LANG.getTxt(DM.PSV.InstantKill.ToString()), psvLvArr[(int)DM.PSV.InstantKill].lv, 
            value: 0f, unit: 0.02f);
        critical = new PsvSkill<float>(LANG.getTxt(DM.PSV.Critical.ToString()), psvLvArr[(int)DM.PSV.Critical].lv, 
            value: 0f, unit: 0.2f);
        explosion = new PsvSkill<Explosion>(LANG.getTxt(DM.PSV.Explosion.ToString()), psvLvArr[(int)DM.PSV.Explosion].lv, 
            value: new Explosion(0f, 0.75f), new Explosion(0.2f, 0.2f));
        expUp = new PsvSkill<float>(LANG.getTxt(DM.PSV.ExpUp.ToString()), psvLvArr[(int)DM.PSV.ExpUp].lv, 
            value: 1f, unit: 0.2f);
        itemSpawn = new PsvSkill<float>(LANG.getTxt(DM.PSV.ItemSpawn.ToString()), psvLvArr[(int)DM.PSV.ItemSpawn].lv, 
            value: 0, unit: 0.05f);
        verticalMultiShot = new PsvSkill<int>(LANG.getTxt(DM.PSV.VerticalMultiShot.ToString()), psvLvArr[(int)DM.PSV.VerticalMultiShot].lv, 
            value: 0, unit: 1, maxLv: 4);
        criticalDamage = new PsvSkill<float>(LANG.getTxt(DM.PSV.CriticalDamage.ToString()), psvLvArr[(int)DM.PSV.CriticalDamage].lv, 
            value: 0, unit: 0.5f);
        laser = new PsvSkill<int>(LANG.getTxt(DM.PSV.Laser.ToString()), psvLvArr[(int)DM.PSV.Laser].lv, 
            value: 0, unit: 1, maxLv: 3);
        fireProperty = new PsvSkill<float>(LANG.getTxt(DM.PSV.FireProperty.ToString()), psvLvArr[(int)DM.PSV.FireProperty].lv, 
            value: 0, unit: 0.15f, maxLv: 3);
        iceProperty = new PsvSkill<float>(LANG.getTxt(DM.PSV.IceProperty.ToString()), psvLvArr[(int)DM.PSV.IceProperty].lv, 
            value: 0, unit: 0.15f, maxLv: 3);
        thunderProperty = new PsvSkill<float>(LANG.getTxt(DM.PSV.ThunderProperty.ToString()), psvLvArr[(int)DM.PSV.ThunderProperty].lv, 
            value: 0, unit: 0.15f, maxLv: 3);
        //* Unique
        damageTwice = new PsvSkill<int>(LANG.getTxt(DM.PSV.DamageTwice.ToString()), psvLvArr[(int)DM.PSV.DamageTwice].lv,
            value: 0, unit: 1, maxLv: 1);
        giantBall = new PsvSkill<float>(LANG.getTxt(DM.PSV.GiantBall.ToString()), psvLvArr[(int)DM.PSV.GiantBall].lv,
            value: 1, unit: 2f, maxLv: 1);
        darkOrb = new PsvSkill<float>(LANG.getTxt(DM.PSV.DarkOrb.ToString()), psvLvArr[(int)DM.PSV.DarkOrb].lv, 
            value: 0, unit: 1, maxLv: 1);
        godBless= new PsvSkill<float>(LANG.getTxt(DM.PSV.GodBless.ToString()), psvLvArr[(int)DM.PSV.GodBless].lv, 
            value: 0, unit: 1, maxLv: 1);
        birdFriend = new PsvSkill<float>(LANG.getTxt(DM.PSV.BirdFriend.ToString()), psvLvArr[(int)DM.PSV.BirdFriend].lv, 
            value: 0, unit: 1, maxLv: 1);
        
        //* Apply
        // Upgrade Data Option
        UpgradeDt[] upgradeArr = DM.ins.personalData.Upgrade.Arr;
        int upgradeDmg = (int)upgradeArr[(int)DM.UPGRADE.Dmg].getValue();
        float upgradeBallSpd = upgradeArr[(int)DM.UPGRADE.BallSpeed].getValue();
        float upgradeCrit = upgradeArr[(int)DM.UPGRADE.Critical].getValue();
        float upgradeCritDmg = upgradeArr[(int)DM.UPGRADE.CriticalDamage].getValue();

        //* General
        dmg.initSkillDt(dmg.Val + (dmg.Unit * dmg.Level) + upgradeDmg);
        multiShot.initSkillDt(multiShot.Val + (multiShot.Unit * multiShot.Level));
        speed.initSkillDt(speed.Val + (speed.Unit * speed.Level) + upgradeBallSpd);
        instantKill.initSkillDt(instantKill.Val + instantKill.Unit * instantKill.Level);
        critical.initSkillDt(critical.Val + (critical.Unit * critical.Level) + upgradeCrit);
        explosion.initSkillDt(new Explosion(explosion.Val.per + explosion.Unit.per, explosion.Val.range + explosion.Unit.range));
        expUp.initSkillDt(expUp.Val + (expUp.Unit * expUp.Level));
        itemSpawn.initSkillDt(itemSpawn.Val + (itemSpawn.Unit * itemSpawn.Level));
        verticalMultiShot.initSkillDt(verticalMultiShot.Val + (verticalMultiShot.Unit * verticalMultiShot.Level));
        criticalDamage.initSkillDt(criticalDamage.Val + (criticalDamage.Unit * criticalDamage.Level) + upgradeCritDmg);
        laser.initSkillDt(laser.Val + (laser.Unit * laser.Level));
        fireProperty.initSkillDt(fireProperty.Val + (fireProperty.Unit * fireProperty.Level));
        iceProperty.initSkillDt(iceProperty.Val + (iceProperty.Unit * iceProperty.Level));
        thunderProperty.initSkillDt(thunderProperty.Val + (thunderProperty.Unit * thunderProperty.Level));
        
        //* Unique
        damageTwice.initSkillDt(damageTwice.Val + (damageTwice.Unit * damageTwice.Level));
        float CALC_GIANTBALL_VAL = (giantBall.Level == 1)? (multiShot.Val + verticalMultiShot.Val + giantBall.Val) * giantBall.Unit : giantBall.Val;
        giantBall.initSkillDt(CALC_GIANTBALL_VAL);
        darkOrb.initSkillDt(darkOrb.Val + (darkOrb.Unit * darkOrb.Level));
        godBless.initSkillDt(godBless.Val + (godBless.Unit * godBless.Level));
        birdFriend.initSkillDt(birdFriend.Val + (birdFriend.Unit * birdFriend.Level));
        #endregion

        //* Show Psv UI
        gm.displayCurPassiveSkillUI("INGAME");

        Debug.Log("swingArcArea.rectTransform.localRotation.eulerAngles.z=" + swingArcArea.rectTransform.localRotation.eulerAngles.z);//! (BUG) rotation.eulerAnglesしないと、角度の数値ではなく、小数点が出る。
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("IsIdle", false);
        swingArcRange = MAX_SWING_HIT_DEG * 2;//左右合わせるから * 2
        swingArcArea.fillAmount = swingArcRange / 360;
        swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,offsetHitDeg + 90); 
        arrowAxisAnchor.transform.rotation = Quaternion.Euler(0,-offsetHitDeg,0);
        Debug.Log("Start:: swingArcArea表示角度= " + swingArcRange + ", 角度をfillAmount値(0~1)に変換=" + swingArcRange / 360);
    }

    void Update(){
        // setSwingArcPos();
        calcLevelUpExp();

        //* PSV Uqinue【 Bird Friend 】活性化
        if(birdFriend.Level == 1 && !BirdFriendObj.activeSelf){
            Debug.Log("BirdFriend召喚！");
            BirdFriendObj.SetActive(true);
        }
    }

    public void addExp(int _exp) => Exp += _exp;

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void setSwingArcPos(){
        RectTransform rect = swingArcArea.rectTransform;
        if(this.transform.position.x < 0){
            rect.parent.localScale = new Vector3(Mathf.Abs(rect.parent.localScale.x), rect.parent.localScale.y, rect.parent.localScale.z); //! (BUG) 
            swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,-offsetHitDeg + 90);
        }
        else{
            rect.parent.localScale = new Vector3(-Mathf.Abs(rect.parent.localScale.x), rect.parent.localScale.y, rect.parent.localScale.z);
            swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,+offsetHitDeg + 90);
        }
    }
    public void setAnimTrigger(string name){
        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.SetTrigger(name);
        switch(name){
            case "Swing":
                DoSwing = true;
                break;
            case "HomeRun": 
            case "ActiveSkillBefSpotLight":
            case "Touch":
                anim.updateMode  = AnimatorUpdateMode.UnscaledTime;
                break;
        }
        
    }
    public void setSwingArcColor(string color){
        swingArcArea.color = color == "red" ? new Color(1,0,0,0.4f) : new Color(1,1,0,0.4f); //yellow
    }
    public void calcLevelUpExp(){
        if(exp >= maxExp){
            Debug.Log($"Player:: calcLevelUpExp():: exp=({exp}) > maxExp({maxExp})");
            setLevelUp();
        }
    }
    public void setLevelUp(){
        IsLevelUp = true;
        Lv++;
        Exp = 0;
        MaxExp = (int)LM._.MAX_EXP_LIST[Lv];
        em.createLvUpNovaEF(this.modelMovingTf.transform.position);
        em.createShowExpUITxtEF(gm.showExpUIGroup.transform, $"LEVEL UP!!");
        Debug.Log($"setLevelUp():: Lv={Lv}, MaxExp={MaxExp}");
    }
    public void destroyAllCastEF(){
        //* Arrow Tf
        foreach(Transform child in gm.pl.CastEFArrowTf) GameObject.Destroy(child.gameObject);
        //* BallPreview Tf
        Array.ForEach(gm.pl.CastEFBallPreviewTfs, tf => {
            foreach(Transform child in tf) GameObject.Destroy(child.gameObject);
        });
    }
    public GameObject getBarrierObj(){
        GameObject obj = null;
        for(int i=0; i<modelMovingTf.childCount; i++){
            if(modelMovingTf.GetChild(i).name == ObjectPool.DIC.DropBoxShieldBarrierEF.ToString()){
                obj = modelMovingTf.GetChild(i).gameObject;
                break;
            }
        }
        Debug.Log("getBarrierObj obj= " + obj.name);
        return obj;
    }
}
