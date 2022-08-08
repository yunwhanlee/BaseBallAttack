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
    [SerializeField] int befLv = 0;                public int BefLv {get=> befLv; set=> befLv=value;}
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
    [SerializeField] float thunderCastWidth;     public float ThunderCastWidth {get=> thunderCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float fireBallCastWidth;    public float FireBallCastWidth {get=> fireBallCastWidth;   set => fireBallCastWidth = value;}
    [SerializeField] float poisonSmokeCastWidth;    public float PoisonSmokeCastWidth {get=> poisonSmokeCastWidth;   set => poisonSmokeCastWidth = value;}
    
    
    [Header("<---- Passive Skill ---->")]
    public PassiveSkill<int> dmg;
    public PassiveSkill<int> multiShot;
    public PassiveSkill<float> speed;
    public PassiveSkill<float> instantKill;
    public PassiveSkill<float> critical;
    public PassiveSkill<Explosion> explosion;
    public PassiveSkill<float> expUp;
    public PassiveSkill<float> itemSpawn;
    public PassiveSkill<int> verticalMultiShot;

    //* Component
    private Animator anim;

    public void Start(){
        //* Player Model Set Parent
        var playerModel = DM.ins.transform.GetChild(0);
        playerModel.SetParent(this.gameObject.transform);// Parent TfをPlayerに移動。
        playerModel.SetSiblingIndex(0); // Child INDEXを０番に移動。

        var atvSkillDb = gm.activeSkillDataBase;
        registAtvSkillNames = new string[atvSkillDb.Length];
        activeSkills = new ActiveSkill[atvSkillDb.Length];

        //* Player Charaは 必ず０番目のINDEXにすること！！
        var charaTf = this.transform.GetChild(0);
        Transform RightArm = charaTf.Find("Bone").Find("Bone_R.001").Find("Bone_R.002").Find("RightArm");
        int childLastIdx = RightArm.childCount - 1;
        Transform bat = RightArm.GetChild(childLastIdx);
        BatEffectTf = bat.Find("BatEffectTf");
        Debug.Log("Player:: Start:: RightArm.childCount= " + RightArm.childCount + ", RightArm= " + RightArm);
        Debug.Log("Player:: charaTf= " + charaTf + ", BatEffectTf= " + BatEffectTf);

        int i=0;
        //* Set Active Skills DataBase
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
        var psvLvArr = DM.ins.personalData.ItemPassive.Arr;
        dmg = new PassiveSkill<int>(
            DM.PSV.Dmg.ToString(), psvLvArr[(int)DM.PSV.Dmg].lv, 1, 1);
        multiShot = new PassiveSkill<int>(
            DM.PSV.MultiShot.ToString(), psvLvArr[(int)DM.PSV.MultiShot].lv, 0, 1);
        speed = new PassiveSkill<float>(
            DM.PSV.Speed.ToString(), psvLvArr[(int)DM.PSV.Speed].lv, 1f, 0.35f);
        instantKill = new PassiveSkill<float>(
            DM.PSV.InstantKill.ToString(), psvLvArr[(int)DM.PSV.InstantKill].lv, 0f, 0.02f);
        critical = new PassiveSkill<float>(
            DM.PSV.Critical.ToString(), psvLvArr[(int)DM.PSV.Critical].lv, 0f, 0.2f);
        explosion = new PassiveSkill<Explosion>(
            DM.PSV.Explosion.ToString(), psvLvArr[(int)DM.PSV.Explosion].lv, new Explosion(0f, 0.75f), new Explosion(0.25f, 0.25f));
        expUp = new PassiveSkill<float>(
            DM.PSV.ExpUp.ToString(), psvLvArr[(int)DM.PSV.ExpUp].lv, 1f, 0.2f);
        itemSpawn = new PassiveSkill<float>(
            DM.PSV.ItemSpawn.ToString(), psvLvArr[(int)DM.PSV.ItemSpawn].lv, 0.1f, 0.05f);
        verticalMultiShot = new PassiveSkill<int>(
            DM.PSV.VerticalMultiShot.ToString(), psvLvArr[(int)DM.PSV.VerticalMultiShot].lv, 0, 1);

        //* Apply Psv Data
        Debug.Log("dmg " + dmg.Value + " + " + dmg.Unit);
        dmg.initPsvSkillDt(dmg.Value + dmg.Unit);
        multiShot.initPsvSkillDt(multiShot.Value + multiShot.Unit);
        Debug.Log("speed " + speed.Value + " + " + speed.Unit);
        speed.initPsvSkillDt(speed.Value + speed.Unit);
        Debug.Log("instantKill " + instantKill.Value + " + " + instantKill.Unit);
        instantKill.initPsvSkillDt(instantKill.Value + instantKill.Unit * instantKill.Level);
        Debug.Log("critical " + critical.Value + " + " + critical.Unit);
        critical.initPsvSkillDt(critical.Value + critical.Unit * critical.Level);
        explosion.initPsvSkillDt(new Explosion(explosion.Value.per + explosion.Unit.per, explosion.Value.range + explosion.Unit.range));
        expUp.initPsvSkillDt(expUp.Value + expUp.Unit);
        itemSpawn.initPsvSkillDt(itemSpawn.Value + itemSpawn.Unit);
        verticalMultiShot.initPsvSkillDt(verticalMultiShot.Value + verticalMultiShot.Unit);

        //* Show Psv UI
        gm.displayCurPassiveSkillUI("INGAME");

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
        list.Add(verticalMultiShot.Level);

        return list;
    }
}
