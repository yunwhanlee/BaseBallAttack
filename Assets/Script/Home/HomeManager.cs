using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI.Extensions;

[System.Serializable]   public class DialogUI{
    //* Value
    [SerializeField] GameObject panel;   public GameObject Panel {get => panel; set => panel = value;}
    [SerializeField] Button goBtn;     public Button GoBtn {get => goBtn; set => goBtn = value;}

    //* Constructor
    DialogUI(GameObject panel, Button goBtn){
        this.panel = panel;
        this.goBtn = goBtn;
    }

    //* Method
}

public class HomeManager : MonoBehaviour
{
    //* OutSide
    [Header("<--Select Panel Color-->")]
    [SerializeField] Image selectPanelScrollBG;  public Image SelectPanelScrollBG {get => selectPanelScrollBG; set => selectPanelScrollBG = value;}
    public enum DlgBGColor {Chara, Bat, Skill, CashShop}
    [SerializeField] Color[] selectPanelColors;
    
    [Header("<-- UI -->")]
    public DialogUI homeDialog;
    public DialogUI selectDialog;
    public DialogUI unlock2ndSkillDialog;
    public DialogUI settingDialog;
    public Dropdown languageOptDropDown;
    public int selectedSkillBtnIdx;
    public Text selectedSkillBtnIdxTxt;
    public Button[] skillBtns;
    public Button startGameBtn;
    
    [SerializeField] Image selectSkillImg;  public Image SelectSkillImg {get => selectSkillImg; set => selectSkillImg = value;}


    [Header("<-- Model -->")]
    [SerializeField] Transform modelTf;   public Transform ModelTf {get => modelTf; set => modelTf = value;}

    [Header("<-- Notice Message Txt -->")]
    public Text noticeMessageTxtPref;
    public Transform mainPanelTf;

    //TODO public DialogUI cashShop;

    void Start(){
        onClickBtnGoToPanel("Home");
        setSelectSkillImg(true);
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnGoToPanel(string name){
        //* Current Model Data & ParentTf
        DM.ins.SelectItemType = name;
        var curChara = DM.ins.scrollviews[(int)DM.ITEM.Chara].ItemPrefs[DM.ins.personalData.SelectCharaIdx];
        var curBat = DM.ins.scrollviews[(int)DM.ITEM.Bat].ItemPrefs[DM.ins.personalData.SelectBatIdx];
        
        switch(DM.ins.SelectItemType){
            case "Home" :
                createCurModel(curChara, curBat, modelTf);
                setGUI();
                setSelectSkillImg();
                break;
            case "Chara" :
            case "Bat" :
            case "Skill" :
            case "CashShop" :
                setGUI();
                break;
        }
    }

    public void onClickBtnSelectSkillImg(int idx){
        Array.ForEach(skillBtns, skillBtn => {
            var outline = skillBtn.GetComponent<NicerOutline>();
            outline.enabled = false;
        });
        //* 2ndSkillがUnLockまだされた状態。
        if(idx == 1 && !DM.ins.personalData.IsUnlock2ndSkill) {
            skillBtns[0].GetComponent<NicerOutline>().enabled = true;
            return;
        }
        selectedSkillBtnIdx = idx;
        skillBtns[idx].GetComponent<NicerOutline>().enabled = true;
        selectedSkillBtnIdxTxt.text = (idx == 0)? "1st Skill" : "2nd Skill";
    }

    public void onClickSettingBtn(){
        settingDialog.Panel.SetActive(true);
    }
    public void onClickSettingRequestBtn(bool isOk){
        settingDialog.Panel.SetActive(false);
        if(isOk) SceneManager.LoadScene("Home"); // Re-load
    }


    public void onDropDownLanguageOpt(){
        string opt = languageOptDropDown.options[languageOptDropDown.value].text;
        Debug.Log("onDropDownSettingOpt():: opt.txt= " + opt);
        if(opt == "English")    DM.ins.Language = LANG.TP.EN;
        if(opt == "日本語")    DM.ins.Language = LANG.TP.JP;
        if(opt == "한글")    DM.ins.Language = LANG.TP.KR;
    }

    public void onClickBtnUnlock2ndSkillDialog(bool isActive){
        if(DM.ins.personalData.IsUnlock2ndSkill) return;
        unlock2ndSkillDialog.Panel.SetActive(isActive);
    }

    public void onclickBtnBuyUnlock2ndSkill(){
        int price = 1000;
        var unlockSkillList = DM.ins.personalData.SkillLockList.FindAll(list => list == false);
        int unlockSkillListCnt = unlockSkillList.Count;
        Debug.Log("unlockSkillListCnt= " + unlockSkillListCnt);
        if(DM.ins.personalData.Coin < price) {
            displayMessageDialog("NO MONEY!");
            return;
        }
        else if(unlockSkillListCnt < 2){
            displayMessageDialog("NO SKILL!");
            return;
        }
        
        DM.ins.personalData.IsUnlock2ndSkill = true;
        DM.ins.personalData.Coin -= price;
        unlock2ndSkillDialog.Panel.SetActive(false);
        drawGrayPanel(false);

        //* Set SkillImg without overlap
        var prefs = DM.ins.scrollviews[(int)DM.ITEM.Skill].ItemPrefs;
        List<int> remainedSkillIdxList = new List<int>();
        for(int i = 0; i < prefs.Length; i++){
            if(DM.ins.personalData.SelectSkillIdx != i)
                remainedSkillIdxList.Add(i);
        }
        DM.ins.personalData.SelectSkill2Idx = remainedSkillIdxList[0];
        var sprite = prefs[remainedSkillIdxList[0]].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        var secondSkillImg = skillBtns[1].transform.GetChild(0).GetComponent<Image>();

        secondSkillImg.sprite = sprite;        
    }
    public void displayMessageDialog(string msg){
        noticeMessageTxtPref.text = msg;
        var ins = Instantiate(noticeMessageTxtPref, mainPanelTf.transform.position, Quaternion.identity, mainPanelTf);
        ins.rectTransform.localPosition = new Vector3(0,-900,-400);
        Destroy(ins.gameObject,2);
    }

    public void onClickStartGameBtn(){
        //* Model Copy
        var playerModel = modelTf.GetChild(0);
        playerModel.GetComponent<Animator>().SetBool("IsIdle", false); //Ready Pose
        playerModel.SetParent(DM.ins.transform);
        // var copyModel = Instantiate(playerModel, Vector3.zero, Quaternion.identity, DM.ins.transform);

        //* Set Item Passive Data
        int[] lvArrTemp = getItemPsvLvArr(playerModel);
        DM.ins.personalData.ItemPassive.setLvArr(lvArrTemp);

        SceneManager.LoadScene("Play");
    }

    public void onClickResetBtn(){
        DM.ins.personalData.reset();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void setSelectSkillImg(bool isInit = false){
        Debug.LogFormat("------setSelectSkillImg():: selectedSkillBtnIdx({0}) SelectSkillIdx({1}), SelectSkill2Idx({2})------", selectedSkillBtnIdx, DM.ins.personalData.SelectSkillIdx, DM.ins.personalData.SelectSkill2Idx);
        var ctt = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf;
        if(isInit){
            setSelectSkillSprite(0, ctt, DM.ins.personalData.SelectSkillIdx);
            check2ndSkillSprite(ctt, DM.ins.personalData.SelectSkill2Idx);
            return;
        }

        if(selectedSkillBtnIdx == 0){
            setSelectSkillSprite(0, ctt, DM.ins.personalData.SelectSkillIdx);
        }else{
            check2ndSkillSprite(ctt, DM.ins.personalData.SelectSkill2Idx);
        }
    }

    private void setSelectSkillSprite(int btnIdx, RectTransform content, int idx){
        Sprite spr = content.GetChild(idx).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        Debug.Log("spr.name=" + spr.name);
        var skillImg = skillBtns[btnIdx].transform.GetChild(0).GetComponent<Image>();
        skillImg.sprite = spr;
    }
    private void check2ndSkillSprite(RectTransform ctt, int skillIdx){
        if(DM.ins.personalData.IsUnlock2ndSkill){
            drawGrayPanel(false);
            setSelectSkillSprite(1, ctt, skillIdx);
        }
    }

    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private int[] getItemPsvLvArr(Transform playerModel){
        // Debug.Log("getItemPsvLvArr:: playerModel= " + playerModel);
        ItemInfo[] childs = playerModel.GetComponentsInChildren<ItemInfo>();
        int len = childs[0].ItemPassive.Arr.Length;
        Debug.Log("lenght= " + len);
        int[] lvArrTemp = new int[len];
        Array.ForEach(childs, child=>{
            for(int i=0; i<len; i++){
                lvArrTemp[i] += child.ItemPassive.Arr[i].lv;
                // Debug.LogFormat("lvArrTemp[{0}]= {1}", i,lvArrTemp[i]);
            }
        });
        return lvArrTemp;
    }

    private void createCurModel(GameObject chara, GameObject bat, Transform modelTf){
        var childs = modelTf.GetComponentsInChildren<Transform>();
        // Chara
        var charaIns = Instantiate(chara, Vector3.zero, Quaternion.identity, modelTf) as GameObject;
        // Bat
        Transform rightArmTf = charaIns.transform.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find("RightArm");
        Instantiate(bat, bat.transform.position, bat.transform.rotation, rightArmTf);
        
        //* Home Model Delete
        if(0 < modelTf.childCount){
            for(int i=0;i<childs.Length;i++){
                if(i != 0) //* (BUG) [0]は親だから処理しない。
                    Destroy(childs[i].gameObject);
            }
        }
    }
    private void setGUI(){
        setActiveDialogGUI(DM.ins.SelectItemType);
        switch(DM.ins.SelectItemType){
            case "Home":
                Array.ForEach(DM.ins.scrollviews, sv => 
                    sv.ScrollRect.gameObject.SetActive(false));
                break;
            default : 
                int typeIdx = DM.ins.getCurItemType2Idx();
                SelectPanelScrollBG.color = selectPanelColors[typeIdx];
                DM.ins.scrollviews[typeIdx].ScrollRect.gameObject.SetActive(true);

                var scrollViewEvent = DM.ins.scrollviews[typeIdx].ScrollRect.GetComponent<ScrollViewEvent>();
                scrollViewEvent.setCurSelectedItem(typeIdx);
                scrollViewEvent.updateItemInfo();
                scrollViewEvent.exceptAlreadySelectedAnotherSkill(selectedSkillBtnIdx, skillBtns);
                break;
        }
    }
    private void setActiveDialogGUI(string type){
        bool isHome = (type == "Home")? true : false;
        homeDialog.Panel.gameObject.SetActive(isHome);
        homeDialog.GoBtn.gameObject.SetActive(!isHome);
        selectDialog.Panel.gameObject.SetActive(!isHome);
    }
    private void drawGrayPanel(bool isActive){
        var child = unlock2ndSkillDialog.GoBtn.transform.GetChild(1);
        Debug.Log("drawGrayPanel:: child.name= " + child.name);
        if(child.name == "GrayPanel") child.gameObject.SetActive(isActive);
    }
}
