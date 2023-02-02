using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class onClickStayCheckBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //* 連続アップグレード可能に : Skill & Upgrade。
    [SerializeField] float time;
    [SerializeField] float span;
    [SerializeField] bool isTap;
    [SerializeField] bool isRepeatAutoPurchase;

    const float WAIT_SEC = 1, DELAY = 0.2f;
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown::");
        isTap = true;

        InvokeRepeating("autoPurchaseItem", WAIT_SEC, DELAY);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp::");
        isTap = false;
        isRepeatAutoPurchase = false;
        time = 0;
        CancelInvoke();
    }

    void Update(){
        if(!isTap) return;

        time += Time.deltaTime;
        if(time > span){
            isRepeatAutoPurchase = true;
        }
    }

    void autoPurchaseItem(){
        if(!isRepeatAutoPurchase) return;
        var type = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        Debug.Log($"PurchaseUpgrade(type={type}):: onClickCheckBtn");
        ScrollView scrollview = null;
        ScrollViewEvent scrollviewEvent = null;
        switch(type){
            case DM.PANEL.Skill:
                scrollview = DM.ins.scrollviews[(int)DM.PANEL.Skill];
                scrollviewEvent = scrollview.ScrollRect.GetComponent<ScrollViewEvent>();
                scrollviewEvent.onClickCheckBtn(type.ToString());
                break;
            case DM.PANEL.Upgrade:
                scrollview = DM.ins.scrollviews[(int)DM.PANEL.Upgrade];
                scrollviewEvent = scrollview.ScrollRect.GetComponent<ScrollViewEvent>();
                scrollviewEvent.onClickCheckBtn(type.ToString());
                break;
        }
    }
}
