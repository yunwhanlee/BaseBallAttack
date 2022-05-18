using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private float rectWidth;
    private float curIdxBasePos;

    void Start(){
        rectWidth = DataManager.ins.charaParentTf.rect.width;
        Debug.Log("ScrollViewEvent:: rectWidth= " + rectWidth);
    }

    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData){
        Debug.Log("<color=white> ScrollViewEvent:: Drag Begin </color>");
    }

    public void OnEndDrag(PointerEventData eventData){
        int posX = (int)DataManager.ins.ScrollContentTf.anchoredPosition.x;
        Debug.Log("<color=white> ScrollViewEvent:: Drag End:: curIdxPos=" + curIdxBasePos + ", ScrollContentTf.PosX=" + posX + "</color>");
        DataManager.ins.ScrollContentTf.anchoredPosition = new Vector2(curIdxBasePos, -500);
    }

    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        // Debug.Log("Scroll View Pos =" + pos.anchoredPosition.x + ", " + pos.anchoredPosition.y); // 0 ~ 1
        float width = Mathf.Abs(rectWidth);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        float max = width * DataManager.ins.charaPfs.Length - width;
        int idx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        curIdxBasePos = -((idx+1) * width);
        // Debug.Log("getScrollViewPos:: curPosX=" + (curPosX) + " / " + max + ", idx=" + idx + " (curIdxPos=" + curIdxPos + ")");
        
    }
}
