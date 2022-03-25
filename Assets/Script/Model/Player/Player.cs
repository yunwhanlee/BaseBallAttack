using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("【HIT 角度範囲】")]
    public GameObject arrowAxisAnchor;
    public Image swingArcArea;
    private float swingArcRange;
    public int MAX_HIT_DEG; //左右ある方向の最大角度
    public float offsetHitDeg; // Startが０度(←方向)から、↑ →向きに回る。
    
    public GameObject previewBundle;
    public GameObject ballPreviewSphere;

    [Header("【Status】")]
    public bool doSwing = false;
    public bool isLevelUp = false;
    [SerializeField] private int lv = 1;
    [SerializeField] private float maxExp = 100;
    [SerializeField] private int exp = 0;
    [Header("Passive Skill")]
    [SerializeField] private int dmg = 1;
    [SerializeField] private int multiCnt = 1;
    [SerializeField] private int speed = 1;

    //* Component
    private Animator anim;

    void Start(){
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
    public float getMaxExp() => maxExp;
    public void setMaxExp(float _maxExp) => maxExp = _maxExp;
    public int getExp() => exp;
    public void setExp(int _exp) => exp = _exp;
    public void addExp(int _exp) => exp += _exp;
    public bool getIsLevelUp() => isLevelUp;
    public void setIsLevelUp(bool trigger) => isLevelUp = trigger;
    public bool getDoSwing() => doSwing;
    public void setDoSwing(bool trigger) => doSwing = trigger;
    //passive skill
    public int getDmg() => dmg;
    public void setDmg(int _dmg) => dmg = _dmg;
    public int getMultiShot() => multiCnt;
    public void setMultiShot(int _multiCnt) => multiCnt = _multiCnt;
    public int getSpeed() => speed;
    public void setSpeed(int _speed) => speed = _speed;


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
}
