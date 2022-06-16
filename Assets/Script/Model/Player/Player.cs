using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class HitRank{
    [SerializeField] float dist;
    [SerializeField] int power;
    public HitRank(float dist, int power){
        this.dist = dist;
        this.power = power;
    }
    public float Dist {get => dist;}
    public float Power {get => power;}
}

public class Player : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public EffectManager em;

    [Header("<---- HIT 角度範囲 ---->")]
    // CAM1
    public GameObject arrowAxisAnchor;
    public Image swingArcArea;
    private float swingArcRange;
    public int MAX_HIT_DEG; //左右ある方向の最大角度
    public float offsetHitDeg; // Startが０度(←方向)から、↑ →向きに回る。
    public GameObject previewBundle;
    public GameObject ballPreviewSphere;

    [Header("<---- HIT RANK ---->")]
    public HitRank[] hitRank;


    [Header("<---- Status ---->")]
    [SerializeField] bool doSwing = false;      public bool DoSwing {get=> doSwing; set=> doSwing=value;}
    [SerializeField] bool isLevelUp = false;    public bool IsLevelUp {get=> isLevelUp; set=> isLevelUp=value;}
    [SerializeField] int lv = 1;                public int Lv {get=> lv; set=> lv=value;}
    [SerializeField] float maxExp = 100;        public float MaxExp {get=> maxExp; set=> maxExp=value;}
    [SerializeField] int exp = 0;               public int Exp {get=> exp; set=> exp=value;}

    [Header("<---- Active Skill ---->")]
    public float atvSkillCoolDownUnit = 0.05f;  public float AtvSkillCoolDownUnit {get=> atvSkillCoolDownUnit;}
    public string[] registAtvSkillNames;// = new string[3];
    public ActiveSkill[] activeSkills;// = new ActiveSkill[3];
    [SerializeField] Transform batEffectTf;           public Transform BatEffectTf {get => batEffectTf; set => batEffectTf = value;}
    [SerializeField] Transform castEFArrowTf;         public Transform CastEFArrowTf {get => castEFArrowTf;}
    [SerializeField] Transform castEFBallPreviewTf;   public Transform CastEFBallPreviewTf {get => castEFBallPreviewTf;}
    float thunderCastWidth = 1;     public float ThunderCastWidth {get=> thunderCastWidth;}
    float fireBallCastWidth = 3;    public float FireBallCastWidth {get=> fireBallCastWidth;}
    
    
    [Header("<---- Passive Skill ---->")]
    public PassiveSkill<int> dmg;
    public PassiveSkill<int> multiShot;
    public PassiveSkill<float> speed;
    public PassiveSkill<float> instantKill;
    public PassiveSkill<float> critical;
    public PassiveSkill<Explosion> explosion;
    public PassiveSkill<float> expUp;
    public PassiveSkill<float> itemSpawn;

    //* Component
    private Animator anim;

    public void Start(){
        //* Model Load
        var playerModel = DM.ins.transform.GetChild(0);
        playerModel.SetParent(this.gameObject.transform);

        var atvSkillDb = gm.activeSkillDataBase;
        registAtvSkillNames = new string[atvSkillDb.Length];
        activeSkills = new ActiveSkill[atvSkillDb.Length];

        //* Player Charaは 必ず０番目のINDEXにすること！！
        var charaTf = this.transform.GetChild(0);
        BatEffectTf = charaTf.Find("Bone").Find("Bone_R.001").Find("Bone_R.002").Find("RightArm").Find("Bat").Find("BatEffectTf");
        Debug.Log("Player:: charaTf= " + charaTf + ", BatEffectTf= " + BatEffectTf);

        int i=0;
        //* Set Active Skills
        Array.ForEach(atvSkillDb, dt=>{
            // Get All Skill Names
            registAtvSkillNames[i] = dt.Name;
            // Get All Skill Data
            activeSkills[i] = new ActiveSkill(registAtvSkillNames[i], atvSkillDb);
            // Set EffectManager All Skill Effects
            em.activeSkillBatEFs[i] = activeSkills[i].BatEfPref;
            em.activeSkillShotEFs[i] = activeSkills[i].ShotEfPref;
            em.activeSkillExplosionEFs[i] = activeSkills[i].ExplosionEfPref;
            em.activeSkillCastEFs[i] = activeSkills[i].CastEfPref;
            em.createActiveSkillBatEF(i, BatEffectTf);
            i++;
        });

        //* Set Passive Skills : @params { int level, T value, T unit }
        var psvLvArr = DM.ins.personalData.itemPassive.Arr;
        dmg = new PassiveSkill<int>("dmg", psvLvArr[(int)DM.PSV_INDEX.DMG].lv, 1, 1);
        multiShot = new PassiveSkill<int>("multiShot", psvLvArr[(int)DM.PSV_INDEX.MULTISHOT].lv, 0, 1);
        speed = new PassiveSkill<float>("speed", psvLvArr[(int)DM.PSV_INDEX.SPEED].lv, 1f, 0.2f);
        instantKill = new PassiveSkill<float>("instantKill", psvLvArr[(int)DM.PSV_INDEX.INSTANT_KILL].lv, 0f, 0.02f);
        critical = new PassiveSkill<float>("critical", psvLvArr[(int)DM.PSV_INDEX.CRITICAL].lv, 0f, 0.1f);
        explosion = new PassiveSkill<Explosion>("explosion", psvLvArr[(int)DM.PSV_INDEX.EXPLOSION].lv, new Explosion(0f, 0.75f), new Explosion(0.25f, 0.25f));
        expUp = new PassiveSkill<float>("expUp", psvLvArr[(int)DM.PSV_INDEX.EXP_UP].lv, 1f, 0.2f);
        itemSpawn = new PassiveSkill<float>("itemSpawn", psvLvArr[(int)DM.PSV_INDEX.ITEM_SPAWN].lv, 0.1f, 0.05f);

        //* Set HitRank Data : @params { char rate, float distance, int power }
        hitRank = new HitRank[6];
        const int A=0, B=1, C=2, D=3, E=4, F=5;
        hitRank[A] = new HitRank(0.125f, 10);
        hitRank[B] = new HitRank(0.25f, 7);
        hitRank[C] = new HitRank(0.5f, 5);
        hitRank[D] = new HitRank(0.85f, 4);
        hitRank[E] = new HitRank(1.125f, 3);
        hitRank[F] = new HitRank(1.5f, 2);

        
        Debug.Log("swingArcArea.rectTransform.localRotation.eulerAngles.z=" + swingArcArea.rectTransform.localRotation.eulerAngles.z);//! (BUG) rotation.eulerAnglesしないと、角度の数値ではなく、小数点が出る。
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("IsIdle", false);
        swingArcRange = MAX_HIT_DEG * 2;//左右合わせるから * 2
        swingArcArea.fillAmount = swingArcRange / 360;
        swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,offsetHitDeg + 90); 
        arrowAxisAnchor.transform.rotation = Quaternion.Euler(0,-offsetHitDeg,0);
        Debug.Log("Start:: swingArcArea表示角度= " + swingArcRange + ", 角度をfillAmount値(0~1)に変換=" + swingArcRange / 360);
    }

    void Update(){
        setSwingArcPos();
        calcLevelUpExp();
    }


    public void addExp(int _exp) => exp += _exp;

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
        Debug.Log("setLevelUp:: LEVEL UP!");
        IsLevelUp = true;
        Lv = ++lv;
        Exp = 0;
        MaxExp = maxExp * 1.75f;
    }

    public List<int> getAllSkillLvList(){
        List<int> list = new List<int>();
        //set All Skills curLevel
        list.Add(dmg.Level);
        list.Add(multiShot.Level);
        list.Add(speed.Level);
        list.Add(instantKill.Level);
        list.Add(critical.Level);
        list.Add(explosion.Level);
        list.Add(expUp.Level);
        list.Add(itemSpawn.Level);

        return list;
    }
}
