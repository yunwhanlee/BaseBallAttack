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

    [Header("--Select Charactor--")]
    public Material grayBlackNoBuyMt;
    [SerializeField] RectTransform contentTf; public RectTransform ContentTf {get => contentTf; set => contentTf = value;}
    [SerializeField] RectTransform charaParentTf;   public RectTransform CharaParentTf {get => charaParentTf; set => charaParentTf = value;}
    [SerializeField] GameObject[] charaPfs;         public GameObject[] CharaPfs {get => charaPfs; set => charaPfs = value;}

    //* Personal Data Class (Save・Load)
    // [SerializeField] int selectCharaIdx = 0;    public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    
    // [SerializeField] List<bool> charaLockList;

    public PersonalData personalData;

    void Awake() => singleton();
    void Start(){
        //* Charactors Regist
        Array.ForEach(charaPfs, chara=>{
            Transform parentTf = Instantiate(charaParentTf, charaParentTf.localPosition, charaParentTf.localRotation, contentTf).transform;
            GameObject ins = Instantiate(chara, Vector3.zero, Quaternion.identity, parentTf);
            ins.name = chara.name;//名前上書き：しないと後ろに(clone)が残る。
        });

        CharactorInfo[] charaContents = ContentTf.GetComponentsInChildren<CharactorInfo>();
        personalData = new PersonalData(ref charaContents);
    }

    void Update(){
        CoinTxt.text = personalData.Coin.ToString();
        DiamondTxt.text = personalData.Diamond.ToString();
    }

    private void OnApplicationQuit(){
        CharactorInfo[] charaContents = ContentTf.GetComponentsInChildren<CharactorInfo>();
        personalData.save(ref charaContents);
    }

    void singleton(){
        //* Singleton
        if(ins == null) ins = this;
        else if(ins != null) return;
        DontDestroyOnLoad(this.gameObject);
    }
}
