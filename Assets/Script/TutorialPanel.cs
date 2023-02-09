using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]  string[] titleObjNameArr;    public string[] TitleObjNameArr { get => titleObjNameArr; set => titleObjNameArr = value;}
    [SerializeField]  string[] contentObjNameArr;  public string[] ContentObjNameArr { get => contentObjNameArr; set => contentObjNameArr = value;}
    int lastIdx;
    [SerializeField] GameObject[] contentArr;   public GameObject[] ContentArr {get => contentArr; set => contentArr = value;}
    [SerializeField] int pageIdx;   public int PageIdx { get => pageIdx; set => pageIdx = value;}

    [Header("PAGE ARROW BUTTON")][Header("__________________________")]
    [SerializeField] Button prevArrowBtn;
    [SerializeField] Button nextArrowBtn;

    [Header("PAGE NAVI ICON")][Header("__________________________")]
    [SerializeField] RectTransform pageNaviGroup;
    [SerializeField] GameObject pageNaviIconPf;
    [SerializeField] GameObject[] pageNaviFocusIconArr;
    [Header("Text")][Header("__________________________")]
    [SerializeField] Text SkipTxt;


    void Start(){
        lastIdx = contentArr.Length - 1;
        // contentArr[0].SetActive(true);
        prevArrowBtn.interactable = false;
        nextArrowBtn.interactable = true;
        createFocusIconArr();
        setLanguage();
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

/// --------------------------------------------------------------------------
/// Function
/// --------------------------------------------------------------------------
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
    public void setLanguage(){
        //* Set NameArr (チュートリアル追加さら、こちらへ追加。)
        titleObjNameArr = new string[] { LANG.TXT.TutorialA_Title.ToString(), LANG.TXT.TutorialB_Title.ToString(), LANG.TXT.TutorialC_Title.ToString(), LANG.TXT.TutorialD_Title.ToString(), LANG.TXT.TutorialE_Title.ToString(), LANG.TXT.TutorialF_Title.ToString(), LANG.TXT.TutorialG_Title.ToString()};
        contentObjNameArr = new string[] { LANG.TXT.TutorialA_Content.ToString(), LANG.TXT.TutorialB_Content.ToString(), LANG.TXT.TutorialC_Content.ToString(), LANG.TXT.TutorialD_Content.ToString(), LANG.TXT.TutorialE_Content.ToString(), LANG.TXT.TutorialF_Content.ToString(), LANG.TXT.TutorialG_Content.ToString()};
        //* Process
        for(int i = 0; i <= lastIdx; i++){
            Text titleTxt = contentArr[i].transform.Find(DM.NAME.TitleTxt.ToString()).GetComponent<Text>();
            Text contentTxt = contentArr[i].transform.Find(DM.NAME.ContentTxt.ToString()).GetComponent<Text>();
            titleTxt.text = LANG.getTxt(titleObjNameArr[i]);
            contentTxt.text = LANG.getTxt(contentObjNameArr[i]);
        }
    }

/// --------------------------------------------------------------------------
/// Btn Event
/// --------------------------------------------------------------------------
    public void onClickExitBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        Destroy(this.gameObject);
    }

    public void onClickLeftArrow(){
        if(pageIdx < 0) return;
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        pageIdx--;
        setActiveFocusIcon();
    }
    public void onClickRightArrow(){
        if(pageIdx >= lastIdx) return;
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        pageIdx++;
        setActiveFocusIcon();
    }
}
