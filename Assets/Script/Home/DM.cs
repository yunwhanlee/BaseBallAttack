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

    public void createObject(RectTransform modelParentPref, RectTransform itemPassivePanel, RectTransform itemSkillBoxPref){
        //* Prefabs 生成
        Array.ForEach(prefs, obj=>{
            //* 生成
            Transform parentTf = null;
            GameObject model = null;
            Transform psvPanel = null;
            switch(this.type){
                case "Skill" :
                    model = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, contentTf);
                    break;
                case "Chara" : 
                case "Bat" :
                    parentTf = GameObject.Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf).transform;
                    model = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, parentTf);

                    //* Item Passive UI Ready
                    psvPanel = GameObject.Instantiate(itemPassivePanel, itemPassivePanel.localPosition, itemPassivePanel.localRotation, parentTf).transform;
                    var passiveDtArr = DM.ins.personalData.itemPassive.getDtArr();
                    model.GetComponent<ItemInfo>().ItemPassive.setImgPrefs(passiveDtArr);
                    break;
            }

            if(!model) return;
            var itemPassiveList = model.GetComponent<ItemInfo>().ItemPassive.getDtArr();
            //* 調整
            switch(this.type){
                case "Chara" : 
                    showItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case "Bat" :
                    parentTf.GetComponent<RectTransform>().localPosition = new Vector3(0,200,800); //* xとyは自動調整される。
                    model.transform.localPosition = new Vector3(model.transform.localPosition.x, 0.75f, model.transform.localPosition.z);
                    model.transform.localRotation = Quaternion.Euler(model.transform.localRotation.x, model.transform.localRotation.y, -45);

                    showItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case "Skill" : 
                    model.transform.localPosition = new Vector3(0,0,0); //* posZがずれるから、調整
                    //* Add "OnClick()" EventListner
                    Button btn = model.GetComponent<Button>();
                    var svEvent = ScrollRect.GetComponent<ScrollViewEvent>();
                    btn.onClick.AddListener(delegate{svEvent.onClickSkillPanel(model);});
                    break;
        }
            Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            model.name = obj.name;//名前上書き：しないと後ろに(clone)が残る。
        });
    }

    private void showItemPassiveUI(string type, ItemPassiveDt[] itemPassiveList, RectTransform itemSkillBoxPref, Transform psvPanel){
        Array.ForEach(itemPassiveList, list=>{
            if(list.lv > 0){
                Debug.Log(list.imgPref.name + "= " + list.lv);
                var boxPref = GameObject.Instantiate(itemSkillBoxPref, itemSkillBoxPref.localPosition, itemSkillBoxPref.localRotation, psvPanel);
                var imgPref = GameObject.Instantiate(list.imgPref, Vector3.zero, Quaternion.identity, boxPref).transform;
                boxPref.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
                switch(type){
                    case "Chara" : break;
                    case "Bat": psvPanel.GetComponent<RectTransform>().localPosition = new Vector3(2.4f, -2, 6.5f); break;
                }
                boxPref.GetComponentInChildren<Text>().text = list.lv.ToString();
                boxPref.GetComponentInChildren<Text>().transform.SetAsLastSibling();
            }
        });
        Debug.Log("--------------------------------------------");
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
    [SerializeField] RectTransform itemPassivePanel;   public RectTransform ItemPassivePanel {get => itemPassivePanel; set => itemPassivePanel = value;}
    [SerializeField] RectTransform itemSkillBoxPref;   public RectTransform ItemSkillBoxPref {get => itemSkillBoxPref; set => itemSkillBoxPref = value;}
    public ScrollView[] scrollviews; //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop
    public PersonalData personalData;
    

    void Awake() => singleton();
    void Start(){
        //* contents Prefab 生成
        scrollviews[(int)DM.ITEM.Chara].createObject(modelParentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Bat].createObject(modelParentPref, itemPassivePanel, itemSkillBoxPref);
        scrollviews[(int)DM.ITEM.Skill].createObject(modelParentPref, itemPassivePanel, itemSkillBoxPref);

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
