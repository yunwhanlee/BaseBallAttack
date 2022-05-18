using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    float rectWidth;
    float curIdxBasePos;

    [Header("--Scroll Speed--")]
    [SerializeField] float scrollStopSpeed;
    float scrollSpeed;
    float scrollBefFramePosX;

    

    void Start(){
        rectWidth = DataManager.ins.CharaParentTf.rect.width;
    }

    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData){
        // Debug.Log("<color=white> ScrollViewEvent:: Drag Begin </color>");
    }

    public void OnEndDrag(PointerEventData eventData){
        // int posX = (int)DataManager.ins.ScrollContentTf.anchoredPosition.x;
        // Debug.Log("<color=white> ScrollViewEvent:: Drag End:: curIdxPos=" + curIdxBasePos + ", ScrollContentTf.PosX=" + posX + "</color>");
    }

    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        float width = Mathf.Abs(rectWidth);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        float max = width * DataManager.ins.CharaPfs.Length - width;
        int idx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        curIdxBasePos = -((idx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);

        //* Stop Scrolling To Near Chara PosX
        if(scrollSpeed < 1){
            DataManager.ins.ScrollContentTf.anchoredPosition = new Vector2(curIdxBasePos, -500);
        }

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;

        // Debug.Log("getScrollViewPos:: curPosX=" + (curPosX) + " / " + max + ", idx=" + idx + " (curIdxBasePos=" + curIdxBasePos + "), scrollSpeed=" + scrollSpeed);
    }

}
