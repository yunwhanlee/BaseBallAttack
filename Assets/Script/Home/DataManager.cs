using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] string selectType = "";
    [SerializeField] RectTransform contentTf; public RectTransform ContentTf {get => contentTf; set => contentTf = value;}
    [SerializeField] RectTransform modelParentTf;   public RectTransform ModelParentTf {get => modelParentTf; set => modelParentTf = value;}
    [SerializeField] GameObject[] charaPfs;         public GameObject[] CharaPfs {get => charaPfs; set => charaPfs = value;}
    [SerializeField] GameObject[] batPfs;         public GameObject[] BatPfs {get => batPfs; set => batPfs = value;}

    //* Personal Data Class (Save・Load)
    // [SerializeField] int selectCharaIdx = 0;    public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    
    // [SerializeField] List<bool> charaLockList;

    public PersonalData personalData;

    void Awake() => singleton();
    void Start(){
        // ItemInfo[] items = ContentTf.GetComponentsInChildren<ItemInfo>();
        personalData = new PersonalData();
        personalData.load();
    }

    void Update(){
        CoinTxt.text = personalData.Coin.ToString();
        DiamondTxt.text = personalData.Diamond.ToString();
    }

    private void OnApplicationQuit(){
        ItemInfo[] items = ContentTf.GetComponentsInChildren<ItemInfo>();
        personalData.save(ref items);
    }

    public void createObject(string type){
        GameObject[] objs = null;
        switch(type){
            case "Chara" : 
                objs = charaPfs;
                
                break;
            case "Bat" :
                objs = batPfs;
                break;
        }
        if(objs == null) return; //* 終了

        //* Prefabs 生成
        Array.ForEach(objs, obj=>{
            Transform parentTf = Instantiate(modelParentTf, modelParentTf.localPosition, modelParentTf.localRotation, contentTf).transform;
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
            

            Debug.Log("modelParentTf.pos=" + modelParentTf.position + ", modelParentTf.localPos=" + modelParentTf.localPosition);
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
