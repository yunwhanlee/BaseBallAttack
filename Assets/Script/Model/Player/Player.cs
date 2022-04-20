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

    [Header("【HIT 角度範囲】")]
    // CAM1
    public GameObject arrowAxisAnchor;
    public Image swingArcArea;
    private float swingArcRange;
    public int MAX_HIT_DEG; //左右ある方向の最大角度
    public float offsetHitDeg; // Startが０度(←方向)から、↑ →向きに回る。
    public GameObject previewBundle;
    public GameObject ballPreviewSphere;

    [Header("【HIT ランク】")]
    public HitRank[] hitRank;


    [Header("【Status】")]
    public bool doSwing = false;
    public bool isLevelUp = false;
    [SerializeField] private int lv = 1;
    [SerializeField] private float maxExp = 100;
    [SerializeField] private int exp = 0;

    [Header("【Set Active Bat Effect】")]
    public Transform batEffectTf;
    public Transform castEFArrowTf, castEFBallPreviewTf;
    public string[] registAtvSkillNames = new string[2];
    public ActiveSkill[] activeSkills = new ActiveSkill[2];
    
    
    [Header("【Passive Skill】")]
    public Skill<int> dmg;//private int dmg = 1;
    public Skill<int> multiShot;//private int multiCnt = 0;
    public Skill<float> speed;//private float speed = 1;
    public Skill<float> instantKill;//private float immediateKill = 0;
    public Skill<float> critical;//private float critical = 0;
    public Skill<Explosion> explosion;


    //[SerializeField] private Explosion explosion;

    //* Component
    private Animator anim;

    public void Start(){
        //* Set HitRank Data : @params { char rate, float distance, int power }
        hitRank = new HitRank[6];
        const int A=0, B=1, C=2, D=3, E=4, F=5;
        hitRank[A] = new HitRank(0.125f, 10);
        hitRank[B] = new HitRank(0.25f, 7);
        hitRank[C] = new HitRank(0.5f, 5);
        hitRank[D] = new HitRank(0.85f, 4);
        hitRank[E] = new HitRank(1.125f, 3);
        hitRank[F] = new HitRank(1.5f, 2);
        
        //* Regist Active Skill
        // Set <- From GameManager Table
        activeSkills[0] = new ActiveSkill(registAtvSkillNames[0], gm.activeSkillTable);
        // Set -> To EffectManager Effect
        em.activeSkillBatEFs[0] = activeSkills[0].BatEfPref;
        em.activeSkillShotEFs[0] = activeSkills[0].ShotEfPref;
        em.activeSkillExplosionEFs[0] = activeSkills[0].ExplosionEfPref;
        em.activeSkillCastEFs[0] = activeSkills[0].CastEfPref;
        em.createActiveSkillBatEF(0, batEffectTf);
        
        // Set <- From GameManager Table
        activeSkills[1] = new ActiveSkill(registAtvSkillNames[1], gm.activeSkillTable);
        // Set -> To EffectManager Effect
        em.activeSkillBatEFs[1] = activeSkills[1].BatEfPref;
        em.activeSkillShotEFs[1] = activeSkills[1].ShotEfPref;
        em.activeSkillExplosionEFs[1] = activeSkills[1].ExplosionEfPref;
        em.activeSkillCastEFs[1] = activeSkills[1].CastEfPref;
        em.createActiveSkillBatEF(1, batEffectTf);

        //* Set Passive Skill : @params { int level, T value, T unit }
        dmg = new Skill<int>(0, 1, 1);
        multiShot = new Skill<int>(0, 0, 1);
        speed = new Skill<float>(0, 1f, 0.2f);
        instantKill = new Skill<float>(0, 0f, 0.02f);
        critical = new Skill<float>(0, 0f, 0.1f);
        explosion = new Skill<Explosion>(0, new Explosion(0f, 0.75f), new Explosion(0.25f, 0.25f));
        
        Debug.Log("swingArcArea.rectTransform.localRotation.eulerAngles.z=" + swingArcArea.rectTransform.localRotation.eulerAngles.z);//! (BUG) rotation.eulerAnglesしないと、角度の数値ではなく、小数点が出る。
        anim = GetComponentInChildren<Animator>();
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

    public void setLv(int _lv) => lv = _lv;
    public int getLv() => lv ;
    public float getMaxExp() => maxExp;
    public void setMaxExp(float _maxExp) => maxExp = _maxExp;
    public int getExp() => exp;
    public void setExp(int _exp) => exp = _exp;
    public void addExp(int _exp) => exp += _exp;
    public bool getIsLevelUp() => isLevelUp;
    public void setIsLevelUp(bool trigger) => isLevelUp = trigger;
    public bool getDoSwing() => doSwing;
    public void setDoSwing(bool trigger) => doSwing = trigger;
    public Transform BatEffectTf {get => batEffectTf;}
    public Transform CastEFArrowTf {get => castEFArrowTf;}
    public Transform CastEFBallPreviewTf {get => castEFBallPreviewTf;}

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
                setDoSwing(true);   
                break;
            case "HomeRun": 
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
        isLevelUp = true;
        setLv(++lv);
        setExp(0);
        setMaxExp(maxExp * 1.75f);
    }

    public List<int> getAllSkillLvList(){
        List<int> list = new List<int>();
        //set All Skills curLevel
        list.Add(dmg.getCurLv());
        list.Add(multiShot.getCurLv());
        list.Add(speed.getCurLv());
        list.Add(instantKill.getCurLv());
        list.Add(critical.getCurLv());
        list.Add(explosion.getCurLv());

        return list;
    }
}
