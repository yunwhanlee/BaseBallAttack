using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class ScrollView {
    //* Value
    [SerializeField] String type;  public String Type {get => type; set => type = value;} 
    [SerializeField] RectTransform scrollRect;  public RectTransform ScrollRect {get => scrollRect; set => scrollRect = value;}
    [SerializeField] RectTransform contentTf;  public RectTransform ContentTf {get => contentTf; set => contentTf = value;}
    [SerializeField] GameObject[] prefs;  public GameObject[] Prefs {get => prefs; set => prefs = value;}

    public ScrollView(RectTransform scrollRect, RectTransform contentTf, GameObject[] prefs){
        this.type = scrollRect.gameObject.name.Split('_')[1];
        this.scrollRect = scrollRect;
        this.contentTf = contentTf;
        this.prefs = prefs;
    }

    public void createObject(RectTransform modelParentPref){
        //* Prefabs 生成
        Array.ForEach(prefs, obj=>{
            //* 生成
            Transform parentTf = null;
            GameObject ins = null;
            switch(this.type){
                case "Skill" :
                    ins = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, contentTf);
                    break;
                case "Chara" : 
                case "Bat" :
                    parentTf = GameObject.Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf).transform;
                    ins = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, parentTf);
                    break;
            }

            if(!ins) return;

            //* 調整
            switch(this.type){
                case "Chara" : 
                    break;
                case "Bat" :
                    parentTf.GetComponent<RectTransform>().localPosition = new Vector3(0,200,800); //* xとyは自動調整される。
                    ins.transform.localPosition = new Vector3(ins.transform.localPosition.x, 0.75f, ins.transform.localPosition.z);
                    ins.transform.localRotation = Quaternion.Euler(ins.transform.localRotation.x, ins.transform.localRotation.y, -45);
                    break;
                case "Skill" : 
                    ins.transform.localPosition = new Vector3(0,0,0); //* posZがずれるから、調整
                    //* Add "OnClick()" EventListner
                    Button btn = ins.GetComponent<Button>();
                    var svEvent = ScrollRect.GetComponent<ScrollViewEvent>();
                    btn.onClick.AddListener(delegate{svEvent.onClickSkillPanel(ins);});
                    break;
        }
            Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            ins.name = obj.name;//名前上書き：しないと後ろに(clone)が残る。
        });
    }
}

public class DM : MonoBehaviour
{
    public static DM ins;
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};
    public enum ITEM {Chara, Bat, Skill};
    public enum ImgPrefIdx{DMG, MULTISHOT, SPEED, INSTANT_KILL, CRITICAL, EXPLOSION, EXP_UP, ITEM_SPAWN};

    [Header("--Personal Data--")]
    [SerializeField] Text coinTxt; public Text CoinTxt {get => coinTxt; set => coinTxt = value;}
    [SerializeField] Text diamondTxt; public Text DiamondTxt {get => diamondTxt; set => diamondTxt = value;}

    [Header("--Select Item--")]
    public Material grayItemLock;
    [SerializeField] string selectType = "";    public string SelectType {get => selectType; set => selectType = value;}

    [SerializeField] RectTransform modelParentPref;   public RectTransform ModelParentPref {get => modelParentPref; set => modelParentPref = value;}
    
    public ScrollView[] scrollviews; //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop
    public PersonalData personalData;
    

    void Awake() => singleton();
    void Start(){
        //* contents Prefab 生成
        scrollviews[(int)DM.ITEM.Chara].createObject(modelParentPref);
        scrollviews[(int)DM.ITEM.Bat].createObject(modelParentPref);
        scrollviews[(int)DM.ITEM.Skill].createObject(modelParentPref);

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

    private void OnApplicationQuit(){
        Debug.Log("END GAME:: Scene= " + SceneManager.GetActiveScene().name);
        //* (BUG) SceneがHomeのみセーブできる。
        if(SceneManager.GetActiveScene().name == "Home"){
            personalData.save();
        }
    }

    

    void singleton(){
        //* Singleton
        if(ins == null) ins = this;
        else if(ins != null) return;
        DontDestroyOnLoad(this.gameObject);
    }
}
