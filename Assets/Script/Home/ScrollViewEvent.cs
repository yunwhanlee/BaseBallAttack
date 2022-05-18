using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;
using UnityEngine.EventSystems;

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    float rectWidth;
    float curIdxBasePos;
    int curIdx;

    [Header("--Scroll Speed--")]
    [SerializeField] float scrollStopSpeed;
    float scrollSpeed;
    float scrollBefFramePosX;

    [Header("--UI--")]
    public SpriteRenderer boxSprRdr;
    public Text rankTxt;
    public Text priceTxt;
    public Text nameTxt;


    

    void Start(){
        rectWidth = DataManager.ins.CharaParentTf.rect.width;
    }

    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData){
        // Debug.Log("<color=white> ScrollViewEvent:: Drag Begin </color>");
    }

    public void OnEndDrag(PointerEventData eventData){
        //* (BUG) スクロールが先端と末端だったら、イベント起動しない。
        // int posX = (int)DataManager.ins.ScrollContentTf.anchoredPosition.x;
        // Debug.Log("<color=white> ScrollViewEvent:: Drag End:: PosX=" + posX + ", curIdx=" + curIdx + "Charas.Len=" + (DataManager.ins.CharaPfs.Length - 1) + "</color>");

        // if(curIdx == 0 || curIdx == DataManager.ins.CharaPfs.Length-1){
        //     setScrollStopCharaInfo();
        // }
    }

    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        float width = Mathf.Abs(rectWidth);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        float max = width * DataManager.ins.CharaPfs.Length - width;
        curIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        curIdxBasePos = -((curIdx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);

        Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", idx=" + curIdx + " (curIdxBasePos=" + curIdxBasePos + "), scrollSpeed=" + scrollSpeed);

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1){
            setScrollStopCharaInfo();
        }

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    private void setScrollStopCharaInfo(){
        //* Set PosX
        DataManager.ins.ScrollContentTf.anchoredPosition = new Vector2(curIdxBasePos, -500);
        
        // var charaPref = DataManager.ins.CharaPfs[curIdx];
        var charaPrefs = DataManager.ins.ScrollContentTf.GetComponentsInChildren<CharactorInfo>();
        var charaInfo = charaPrefs[curIdx];

        //* Show Rank Text
        rankTxt.text = charaInfo.Rank.ToString();

        //* Set Price
        priceTxt.text = charaInfo.Price.ToString();

        //* Set Name
        string name = charaInfo.name.Split('_')[1];
        nameTxt.text = name;

        //* Set Rank UI Color
        Color color = Color.white;
        var boxGlowEf = boxSprRdr.GetComponent<SpriteGlowEffect>();
        float brightness = 0;
        int outline = 2;
        switch(charaInfo.Rank){
            case DataManager.RANK.GENERAL : color = Color.white; brightness=1; outline=2;   break;
            case DataManager.RANK.RARE : color = Color.blue; brightness=8; outline=2;   break;
            case DataManager.RANK.UNIQUE : color = Color.red; brightness=8.5f; outline=5; break;
            case DataManager.RANK.LEGEND : color = Color.magenta; brightness=9; outline=8; break;
            case DataManager.RANK.GOD : color = Color.yellow; brightness=10; outline=5; break;
        }
        rankTxt.color = color;
        boxSprRdr.color = color;
        boxGlowEf.GlowColor = color;
        boxGlowEf.GlowBrightness = brightness;
        boxGlowEf.OutlineWidth = outline;
    }
}
