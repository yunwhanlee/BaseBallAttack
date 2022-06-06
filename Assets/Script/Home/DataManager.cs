using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager ins;
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD};

    [Header("--Personal Data--")]
    [SerializeField] Text coinTxt; public Text CoinTxt {get => coinTxt; set => coinTxt = value;}
    [SerializeField] Text diamondTxt; public Text DiamondTxt {get => diamondTxt; set => diamondTxt = value;}

    [Header("--Select Item--")]
    public Material grayBlackNoBuyMt;
    [SerializeField] string selectType = "";    public string SelectType {get => selectType; set => selectType = value;}

    //* [0] : Chara, [1] : Bat, [2] : Skill, [3] : CashShop
    [SerializeField] RectTransform[] scrollViews; public RectTransform[] ScrollViews {get => scrollViews; set => scrollViews = value;}
    [SerializeField] RectTransform contentCharaTf; public RectTransform ContentCharaTf {get => contentCharaTf; set => contentCharaTf = value;}
    [SerializeField] RectTransform contentBatTf; public RectTransform ContentBatTf {get => contentBatTf; set => contentBatTf = value;}
    
    [SerializeField] RectTransform modelParentPref;   public RectTransform ModelParentPref {get => modelParentPref; set => modelParentPref = value;}
    [SerializeField] GameObject[] charaPfs;         public GameObject[] CharaPfs {get => charaPfs; set => charaPfs = value;}
    [SerializeField] GameObject[] batPfs;         public GameObject[] BatPfs {get => batPfs; set => batPfs = value;}

    //* Personal Data Class (Save・Load)
    // [SerializeField] int selectCharaIdx = 0;    public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    
    // [SerializeField] List<bool> charaLockList;

    public PersonalData personalData;

    void Awake() => singleton();
    void Start(){
        //* contents Prefab 生成
        createObject("Chara");
        createObject("Bat");
        //* Items of Content
        ItemInfo[] charas = ContentCharaTf.GetComponentsInChildren<ItemInfo>();
        ItemInfo[] bats = ContentBatTf.GetComponentsInChildren<ItemInfo>();
        personalData = new PersonalData();

        personalData.load(ref charas, ref bats);
        
    }

    void Update(){
        CoinTxt.text = personalData.Coin.ToString();
        DiamondTxt.text = personalData.Diamond.ToString();
    }

    private void OnApplicationQuit(){
        Debug.Log("END GAME:: Scene= " + SceneManager.GetActiveScene().name);
        //* (BUG) SceneがHomeのみセーブできる。
        if(SceneManager.GetActiveScene().name == "Home"){
            ItemInfo[] charas = ContentCharaTf.GetComponentsInChildren<ItemInfo>();
            personalData.save(ref charas);
        }
    }

    public void createObject(string type){
        GameObject[] prefs = null;
        Transform contentTf = null;
        switch(type){
            case "Chara" : 
                prefs = CharaPfs;
                contentTf = ContentCharaTf;
                break;
            case "Bat" :
                prefs = BatPfs;
                contentTf = ContentBatTf;
                break;
        }
        if(prefs == null) return; //* 終了

        //* Prefabs 生成
        Array.ForEach(prefs, obj=>{
            Transform parentTf = Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf).transform;
            GameObject ins = Instantiate(obj, Vector3.zero, Quaternion.identity, parentTf);

            switch(type){
            case "Chara" : 
                break;
            case "Bat" :
                parentTf.GetComponent<RectTransform>().localPosition = new Vector3(0,200,800); //* xとyは自動調整される。
                ins.transform.localPosition = new Vector3(ins.transform.localPosition.x, 0.75f, ins.transform.localPosition.z);
                ins.transform.localRotation = Quaternion.Euler(ins.transform.localRotation.x, ins.transform.localRotation.y, -45);
                break;
        }
            

            Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            ins.name = obj.name;//名前上書き：しないと後ろに(clone)が残る。
        });
    }

    void singleton(){
        //* Singleton
        if(ins == null) ins = this;
        else if(ins != null) return;
        DontDestroyOnLoad(this.gameObject);
    }
}
