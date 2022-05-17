using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Begin");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag End");
    }

    public void getScrollViewPos(RectTransform pos){ //* －が右側
        // Debug.Log("Scroll View Pos =" + pos.anchoredPosition.x + ", " + pos.anchoredPosition.y); // 0 ~ 1
        float width = Mathf.Abs(540);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        float max = width * DataManager.ins.charaPfs.Length - width;
        // Debug.Log("curPosX=" + (curPosX) + " / " + max + ", idx=" + Mathf.Abs(Mathf.FloorToInt((curPosX) / width)));
    }
}
