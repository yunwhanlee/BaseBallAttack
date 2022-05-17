using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager ins;

    [Header("--Select Charactor--")]
    public RectTransform scrollContentTf;
    public RectTransform charaRectTf;
    public GameObject[] charaPfs;
    public int selectCharaIdx = 0;
    void Awake() => singleton();
    void Start()
    {
        //* Charactors Regist
        Array.ForEach(charaPfs, chara=>{
            Transform parentTf = Instantiate(charaRectTf, charaRectTf.localPosition, charaRectTf.localRotation, scrollContentTf).transform;
            Instantiate(chara, Vector3.zero, Quaternion.identity, parentTf);
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
