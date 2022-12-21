using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialPanel : MonoBehaviour
{
    int lastIdx;
    [SerializeField] GameObject[] contentArr;
    [SerializeField] int pageIdx = 0;

    [Header("PAGE ARROW BUTTON")][Header("__________________________")]
    [SerializeField] Button prevArrowBtn;
    [SerializeField] Button nextArrowBtn;

    [Header("PAGE NAVI ICON")][Header("__________________________")]
    [SerializeField] RectTransform pageNaviGroup;
    [SerializeField] GameObject pageNaviIconPf;
    [SerializeField] GameObject[] pageNaviFocusIconArr;

    void Start(){
        lastIdx = contentArr.Length - 1;
        contentArr[0].SetActive(true);
        prevArrowBtn.interactable = false;
        nextArrowBtn.interactable = true;
        createFocusIconArr();
    }

    void Update(){
        //* Contents
        if(!contentArr[pageIdx].activeSelf){
            Array.ForEach(contentArr, ctt => ctt.SetActive(false));
            contentArr[pageIdx].SetActive(true);
        }

        //* ArrowBtn
        prevArrowBtn.interactable = (pageIdx <= 0)? false : true;
        nextArrowBtn.interactable = (pageIdx >= lastIdx)? false : true;
    }

    /* 
    *   Function
    */
    private void setActiveFocusIcon(){
        for(int i = 0; i <= lastIdx; i++)
            pageNaviFocusIconArr[i].SetActive((i == pageIdx)? true : false);
    }
    private void createFocusIconArr(){
        pageNaviFocusIconArr = new GameObject[contentArr.Length];
        for(int i = 0; i <= lastIdx; i++){
            var ins = Instantiate(pageNaviIconPf, pageNaviGroup, false);
            pageNaviFocusIconArr[i] = ins.transform.GetChild(0).gameObject;
        }
        setActiveFocusIcon();
    }

    /* 
    *   Button Event
    */
    public void onClickExitBtn(){
        Destroy(this.gameObject);
    }

    public void onClickLeftArrow(){
        if(pageIdx < 0) return;
        pageIdx--;
        setActiveFocusIcon();
    }
    public void onClickRightArrow(){
        if(pageIdx >= lastIdx) return;
        pageIdx++;
        setActiveFocusIcon();
    }
}
