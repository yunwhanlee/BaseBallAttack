using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchSlideControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{   //*2D Screen *//
    //* OutSide
    public GameManager gm;
    public Player pl;

    public RectTransform pad;
    public RectTransform stick;
    private Vector3 dir;

    public void OnDrag(PointerEventData eventData){
        if(gm.state != GameManager.State.WAIT) return;
        stick.position = eventData.position;

        //Stick動き制限
        stick.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.25f);

        //Stick角度
        Vector2 dir = (stick.position - pad.gameObject.transform.position).normalized;
        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //Player矢印角度に適用
        const int offsetDeg2DTo3D = 90;
        pl.hitAxisArrow.transform.rotation = Quaternion.Euler(0,offsetDeg2DTo3D -deg,0);

        //Debug.Log("OnDrag:: Stick Move Deg="+deg);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pad.position = eventData.position;
        pad.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector2.zero;
        pad.gameObject.SetActive(false);
        pl.setAnimTrigger("Swing");
    }
}
