using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager ins;

    [Header("--Select Charactor--")]
    [SerializeField] RectTransform scrollContentTf; public RectTransform ScrollContentTf {get => scrollContentTf; set => scrollContentTf = value;}
    [SerializeField] RectTransform charaParentTf;   public RectTransform CharaParentTf {get => charaParentTf; set => charaParentTf = value;}
    [SerializeField] GameObject[] charaPfs; public GameObject[] CharaPfs {get => charaPfs; set => charaPfs = value;}
    [SerializeField] int selectCharaIdx = 0;  public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    [SerializeField] string selectCharaName = ""; public string SelectCharaName {get => selectCharaName; set => selectCharaName = value;}

    void Awake() => singleton();
    void Start()
    {
        //* Charactors Regist
        Array.ForEach(charaPfs, chara=>{
            Transform parentTf = Instantiate(charaParentTf, charaParentTf.localPosition, charaParentTf.localRotation, scrollContentTf).transform;
            GameObject ins = Instantiate(chara, Vector3.zero, Quaternion.identity, parentTf);
            ins.name = chara.name;//名前上書き：しないと後ろに(clone)が残る。
        });
    }

    void Update()
    {
    }

    void singleton(){
        //* Singleton
        if(ins == null) ins = this;
        else if(ins != null) return;
        DontDestroyOnLoad(this.gameObject);
    }
}
