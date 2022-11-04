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

    [Header("ARROW")]
    public int MAX_SWING_HIT_DEG; //左右ある方向の最大角度
    public float offsetHitDeg; // Startが０度(←方向)から、↑ →向きに回る。
    public GameObject arrowAxisAnchor;
    public Image swingArcArea;
    private float swingArcRange;
    public GameObject previewBundle;
    public GameObject[] ballPreviewSphere = new GameObject[2]; // 2個
    public HitRank[] hitRank;

    [Header("STATUS")]
    [SerializeField] bool doSwing = false;      public bool DoSwing {get=> doSwing; set=> doSwing=value;}
    [SerializeField] bool isLevelUp = false;    public bool IsLevelUp {get=> isLevelUp; set=> isLevelUp=value;}
    [SerializeField] int befLv = 0;                public int BefLv {get=> befLv; set=> befLv=value;}
    [SerializeField] int lv = 1;                public int Lv {get=> lv; set=> lv=value;}
    [SerializeField] int maxExp = 100;        public int MaxExp {get=> maxExp; set=> maxExp=value;}
    [SerializeField] int exp = 0;               public int Exp {get=> exp; set=> exp=value;}

    [Header("ACTIVE SKILL")]
    public float atvSkillCoolDownUnit = 0.05f;  public float AtvSkillCoolDownUnit {get=> atvSkillCoolDownUnit;}
    public string[] registAtvSkillNames;// = new string[3];
    public AtvSkill[] activeSkills;// = new ActiveSkill[3];
    [SerializeField] Transform batEffectTf;           public Transform BatEffectTf {get => batEffectTf; set => batEffectTf = value;}
    [SerializeField] Transform castEFArrowTf;         public Transform CastEFArrowTf {get => castEFArrowTf;}
    [SerializeField] Transform[] castEFBallPreviewTfs;   public Transform[] CastEFBallPreviewTfs {get => castEFBallPreviewTfs; set => castEFBallPreviewTfs = value;}
    [SerializeField] float thunderCastWidth;     public float ThunderCastWidth {get=> thunderCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float fireBallCastWidth;    public float FireBallCastWidth {get=> fireBallCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float poisonSmokeCastWidth;    public float PoisonSmokeCastWidth {get=> poisonSmokeCastWidth;   set => poisonSmokeCastWidth = value;}
    
    [Header("PASSIVE SKILL")]
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



    public void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em;

        //* Player Model Set Parent
        var playerModel = DM.ins.transform.GetChild(0);
        playerModel.SetParent(modelMovingTf);// Parent TfをPlayerに移動。
        playerModel.SetSiblingIndex(0); // Child INDEXを０番に移動。

        var atvSkillDb = gm.activeSkillDataBase;
        registAtvSkillNames = new string[atvSkillDb.Length];
        activeSkills = new AtvSkill[atvSkillDb.Length];

        //* Player Charaは 必ず０番目のINDEXにすること！！
        var charaTf = modelMovingTf.GetChild(0);
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
        dmg = new PsvSkill<int>(
            LANG.getTxt(DM.PSV.Dmg.ToString()), psvLvArr[(int)DM.PSV.Dmg].lv, 1, 1);
        multiShot = new PsvSkill<int>(
            LANG.getTxt(DM.PSV.MultiShot.ToString()), psvLvArr[(int)DM.PSV.MultiShot].lv, 0, 1);
        speed = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.Speed.ToString()), psvLvArr[(int)DM.PSV.Speed].lv, 1f, 0.35f);
        instantKill = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.InstantKill.ToString()), psvLvArr[(int)DM.PSV.InstantKill].lv, 0f, 0.02f);
        critical = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.Critical.ToString()), psvLvArr[(int)DM.PSV.Critical].lv, 0f, 0.2f);
        explosion = new PsvSkill<Explosion>(
            LANG.getTxt(DM.PSV.Explosion.ToString()), psvLvArr[(int)DM.PSV.Explosion].lv, new Explosion(0f, 0.75f), new Explosion(0.2f, 0.2f));
        expUp = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.ExpUp.ToString()), psvLvArr[(int)DM.PSV.ExpUp].lv, 1f, 0.2f);
        itemSpawn = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.ItemSpawn.ToString()), psvLvArr[(int)DM.PSV.ItemSpawn].lv, 0, 0.05f);
        verticalMultiShot = new PsvSkill<int>(
            LANG.getTxt(DM.PSV.VerticalMultiShot.ToString()), psvLvArr[(int)DM.PSV.VerticalMultiShot].lv, 0, 1);
        criticalDamage = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.CriticalDamage.ToString()), psvLvArr[(int)DM.PSV.CriticalDamage].lv, 0, 0.5f);
        laser = new PsvSkill<int>(
            LANG.getTxt(DM.PSV.Laser.ToString()), psvLvArr[(int)DM.PSV.Laser].lv, 0, 1, 3);
        fireProperty = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.FireProperty.ToString()), psvLvArr[(int)DM.PSV.FireProperty].lv, 0, 1, 3);
        iceProperty = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.IceProperty.ToString()), psvLvArr[(int)DM.PSV.IceProperty].lv, 0, 1, 3);
        thunderProperty = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.ThunderProperty.ToString()), psvLvArr[(int)DM.PSV.ThunderProperty].lv, 0, 1, 3);
        // Unique PSV
        damageTwice = new PsvSkill<int>(
            LANG.getTxt(DM.PSV.DamageTwice.ToString()), psvLvArr[(int)DM.PSV.DamageTwice].lv, 0, 1, 3);
        giantBall = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.GiantBall.ToString()), psvLvArr[(int)DM.PSV.GiantBall].lv, 0, 1, 3);
        darkOrb = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.DarkOrb.ToString()), psvLvArr[(int)DM.PSV.DarkOrb].lv, 0, 1, 3);
        godBless= new PsvSkill<float>(
            LANG.getTxt(DM.PSV.GodBless.ToString()), psvLvArr[(int)DM.PSV.GodBless].lv, 0, 1, 3);
        birdFriend = new PsvSkill<float>(
            LANG.getTxt(DM.PSV.BirdFriend.ToString()), psvLvArr[(int)DM.PSV.BirdFriend].lv, 0, 1, 3);
        

        //* Apply
        dmg.initSkillDt(dmg.Value + dmg.Unit);
        multiShot.initSkillDt(multiShot.Value + multiShot.Unit * multiShot.Level);
        speed.initSkillDt(speed.Value + speed.Unit);
        instantKill.initSkillDt(instantKill.Value + instantKill.Unit * instantKill.Level);
        critical.initSkillDt(critical.Value + critical.Unit * critical.Level);
        explosion.initSkillDt(new Explosion(explosion.Value.per + explosion.Unit.per, explosion.Value.range + explosion.Unit.range));
        expUp.initSkillDt(expUp.Value + expUp.Unit);
        itemSpawn.initSkillDt(itemSpawn.Value + itemSpawn.Unit);
        verticalMultiShot.initSkillDt(verticalMultiShot.Value + verticalMultiShot.Unit * verticalMultiShot.Level);
        criticalDamage.initSkillDt(criticalDamage.Value + criticalDamage.Unit * criticalDamage.Level);
        laser.initSkillDt(laser.Value + laser.Unit * laser.Level);
        fireProperty.initSkillDt(fireProperty.Value + fireProperty.Unit * fireProperty.Level);
        iceProperty.initSkillDt(iceProperty.Value + iceProperty.Unit * iceProperty.Level);
        thunderProperty.initSkillDt(thunderProperty.Value + thunderProperty.Unit * thunderProperty.Level);
        // Unique PSV
        damageTwice.initSkillDt(damageTwice.Value + damageTwice.Unit * damageTwice.Level);
        giantBall.initSkillDt(giantBall.Value + giantBall.Unit * giantBall.Level);
        darkOrb.initSkillDt(darkOrb.Value + darkOrb.Unit * darkOrb.Level);
        godBless.initSkillDt(godBless.Value + godBless.Unit * godBless.Level);
        birdFriend.initSkillDt(birdFriend.Value + birdFriend.Unit * birdFriend.Level);

        
#endregion

        //* Show Psv UI
        gm.displayCurPassiveSkillUI("INGAME");

        //* Set HitRank Data : @params { char rate, float distance, int power }
        // hitRank = new HitRank[6];
        // const int A=0, B=1, C=2, D=3, E=4, F=5;
        // hitRank[A] = new HitRank(0.125f, 10);
        // hitRank[B] = new HitRank(0.25f, 7);
        // hitRank[C] = new HitRank(0.5f, 5);
        // hitRank[D] = new HitRank(0.85f, 4);
        // hitRank[E] = new HitRank(1.125f, 3);
        // hitRank[F] = new HitRank(1.5f, 2);
        
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
        if(exp > maxExp){
            setLevelUp();
        }
    }
    public void setLevelUp(){
        IsLevelUp = true;
        Lv = ++lv;
        Exp = 0;
        MaxExp = (int)LM._.MAX_EXP_LIST[Lv];
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
}
