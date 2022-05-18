using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        int posX = (int)DataManager.ins.ScrollContentTf.anchoredPosition.x;
        Debug.Log("<color=white> ScrollViewEvent:: Drag End:: PosX=" + posX + ", curIdx=" + curIdx + "Charas.Len=" + (DataManager.ins.CharaPfs.Length - 1) + "</color>");

        if(curIdx == 0 || curIdx == DataManager.ins.CharaPfs.Length-1){
            setScrollStopCharaInfo();
        }
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

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1){
            setScrollStopCharaInfo();
            // Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", idx=" + curIdx + " (curIdxBasePos=" + curIdxBasePos + "), scrollSpeed=" + scrollSpeed);
        }

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    private void setScrollStopCharaInfo(){
        //* Set PosX
        DataManager.ins.ScrollContentTf.anchoredPosition = new Vector2(curIdxBasePos, -500);
        
        //* Set Name
        string objName = DataManager.ins.CharaPfs[curIdx].name;
        string charaName = objName.Split('_')[1];
        nameTxt.text = charaName;

        //* Set Price
        
    }
}
