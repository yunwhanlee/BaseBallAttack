using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public void getScrollViewPos(RectTransform pos){ //* －が右側
        // Debug.Log("Scroll View Pos =" + pos.anchoredPosition.x + ", " + pos.anchoredPosition.y);
        float width = Mathf.Abs(540);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        float max = width * charaPfs.Length - width;
        Debug.Log("curPosX=" + (curPosX) + " / " + max + ", idx=" + Mathf.Abs(Mathf.FloorToInt((curPosX) / width)));

        

    }
}
