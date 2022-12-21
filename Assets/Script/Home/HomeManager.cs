using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI.Extensions;
using UnityEngine.Serialization;

[System.Serializable]   public class FrameUI{
    //* Value
    [FormerlySerializedAs("panel")] [SerializeField] GameObject panel;   public GameObject Panel {get => panel; set => panel = value;}
    [FormerlySerializedAs("goBtn")] [SerializeField] Button goBtn;     public Button GoBtn {get => goBtn; set => goBtn = value;}
    [FormerlySerializedAs("titleTxt")] [SerializeField] Text titleTxt;     public Text TitleTxt {get => titleTxt; set => titleTxt = value;}
    [FormerlySerializedAs("infoTxt")] [SerializeField] Text infoTxt;     public Text InfoTxt {get => infoTxt; set => infoTxt = value;}
    //* Constructor
    FrameUI(GameObject panel, Button goBtn, Text titleTxt, Text infoTxt){
        this.panel = panel;
        this.goBtn = goBtn;
        this.titleTxt = titleTxt;
        this.infoTxt = infoTxt;
    }
    //* Method
}

public class HomeManager : MonoBehaviour
{
    //* OutSide
    public enum DlgBGColor {Chara, Bat, Skill, CashShop};

    [Header("SELECT PANEL COLOR")][Header("__________________________")]
    [SerializeField] Image selectPanelScrollBG;  public Image SelectPanelScrollBG {get => selectPanelScrollBG; set => selectPanelScrollBG = value;}
    [SerializeField] Color[] selectPanelColors;

    [Header("PRICE TYPE ICON SPRITE")][Header("__________________________")]
    [SerializeField] Sprite coinIconSprite;  public Sprite CoinIconSprite {get => coinIconSprite;}
    [SerializeField] Sprite diamondIconSprite;  public Sprite DiamondIconSprite {get => diamondIconSprite;}
    
    [Header("【 GUI 】")][Header("__________________________")]
    public FrameUI homePanel;
    public FrameUI selectPanel;
    public FrameUI unlock2ndSkillDialog;
    public FrameUI settingDialog;
    [Space(10)]
    public Text startBtnTxt;
    public Dropdown languageOptDropDown;    public Dropdown LanguageOptDropDown {get => languageOptDropDown; set => languageOptDropDown = value;}
    public int selectedSkillBtnIdx;
    public Text selectedSkillBtnIdxTxt;
    public Button[] skillBtns;
    public Button startGameBtn;
    public Button ItemPsvInfoBtn;
    
    [SerializeField] Image selectSkillImg;  public Image SelectSkillImg {get => selectSkillImg; set => selectSkillImg = value;}

    [Header("ROULETTE EVENT")][Header("__________________________")]
    public Roulette roulette;
    public GameObject roulettePanel;
    public Button rouletteIconBtn;
    public Text rouletteIconCoolTimeTxt;

    [Header("DIALOG")][Header("__________________________")]
    public RectTransform showAdDialog;
    public Text adDialogTitleTxt;
    public Text adDialogContentTxt;

    [Header("BUY OR CHECK BTN")][Header("__________________________")]
    public Button checkBtn;
    public Image checkMarkImg;
    public Text priceTxt;
    public Image priceTypeIconImg;

    [Header("MODEL")][Header("__________________________")]
    [SerializeField] Transform modelTf;   public Transform ModelTf {get => modelTf; set => modelTf = value;}

    // [Header("NOTICE MESSAGE")]
    // public Text noticeMessageTxtPref;
    // public Transform mainPanelTf;

    //TODO public DialogUI cashShop;

    void Start(){
        Debug.Log("Math:: -------------------------------");        
        int i=0;
        const int OFFSET = 100;
        List<int> hpList = Util._.calcArithmeticProgressionList(start: OFFSET, max: 99, d: OFFSET, gradualUpValue: 0.01f);
        hpList.ForEach(hp => Debug.Log($"Math:: blockHpList[{i}]= " + hpList[i++] / OFFSET));

        onClickBtnGoToPanel(DM.SCENE.Home.ToString());
        setSelectSkillImg(true);
        LanguageOptDropDown.value = (int)DM.ins.personalData.Lang; //* Loadデータで初期化

        // if(DM.ins.personalData.RouletteTicketCoolTime == null){
        //     DM.ins.personalData.RouletteTicketCoolTime = DateTime.Now.ToString();
        // }

        rouletteIconBtn.GetComponent<Image>().color = Color.grey;
        startBtnTxt.text = LANG.getTxt(LANG.TXT.Start.ToString());

        adDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRouletteTicket_Title.ToString());
        adDialogContentTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRouletteTicket_Content.ToString());

        selectedSkillBtnIdxTxt.text = LANG.getTxt(LANG.TXT.FirstSkill.ToString());
    }

    void Update(){
        if(DM.ins.personalData.RouletteTicket > 0){
            rouletteIconBtn.GetComponent<Image>().color = Color.white; // def
            rouletteIconCoolTimeTxt.text = "";
        }
        else{
            //* Reset CoolTime
            if(rouletteIconBtn.GetComponent<Image>().color != Color.grey)
                DM.ins.personalData.RouletteTicketCoolTime = DateTime.Now.ToString();

            rouletteIconBtn.GetComponent<Image>().color = Color.grey;
            rouletteIconCoolTimeTxt.text = updateADCoolTime(); //* Coolタイム 表示!
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnQuestionMarkIcon(){
        DM.ins.displayTutorialUI();
    }

    public void onClickBtnGoToPanel(string name){
        //* Current Model Data & ParentTf
        DM.ins.SelectItemType = name;
        var curChara = DM.ins.scrollviews[(int)DM.PANEL.Chara].ItemPrefs[DM.ins.personalData.SelectCharaIdx];
        var curBat = DM.ins.scrollviews[(int)DM.PANEL.Bat].ItemPrefs[DM.ins.personalData.SelectBatIdx];
        
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);

        setGUI();
        
        switch(itemType){
            case DM.PANEL.Chara :
            case DM.PANEL.Bat :
            case DM.PANEL.Skill :
            case DM.PANEL.CashShop :
            case DM.PANEL.Upgrade :
                // なし
                break;
            case DM.PANEL.PsvInfo :
                checkBtn.gameObject.SetActive(false);
                break;
            default: //* Home
                createCurModel(curChara, curBat, modelTf);
                setSelectSkillImg();
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
        selectedSkillBtnIdxTxt.text = LANG.getTxt((idx == 0)? LANG.TXT.FirstSkill.ToString() : LANG.TXT.SecondSkill.ToString());
        skillBtns[idx].GetComponent<NicerOutline>().enabled = true;
    }
    public void onClickRouletteIconBtn(){
        Debug.Log("onClickRouletteIconBtn:: RouletteTicket= " + DM.ins.personalData.RouletteTicket);
        if(DM.ins.personalData.RouletteTicket <= 0){
            showAdDialog.gameObject.SetActive(true);
        }
        else{
            roulettePanel.gameObject.SetActive(true);
            homePanel.Panel.gameObject.SetActive(false);
            homePanel.GoBtn.gameObject.SetActive(false);
            ItemPsvInfoBtn.gameObject.SetActive(false);
        }
    }
    public void onClickShowADButton(){
        //TODO AD広告全部見たら、
        DM.ins.personalData.RouletteTicket++;
        showAdDialog.gameObject.SetActive(false);
        roulettePanel.gameObject.SetActive(true);
        homePanel.Panel.gameObject.SetActive(false);
        homePanel.GoBtn.gameObject.SetActive(false);
        ItemPsvInfoBtn.gameObject.SetActive(false);
    }
    public void onClickSettingBtn(){
        settingDialog.Panel.SetActive(true);
    }
    public void onClickSettingOkBtn(bool isOk){
        settingDialog.Panel.SetActive(false);
        DM.ins.personalData.save();
        if(isOk) SceneManager.LoadScene(DM.SCENE.Home.ToString()); // Re-load
    }

    public void onDropDownLanguageOpt(){
        int idx = LanguageOptDropDown.value;
        Debug.Log("onDropDownSettingOpt():: idx= " + idx);
        switch(idx){
            case 0: DM.ins.personalData.Lang = LANG.TP.EN;   break;
            case 1: DM.ins.personalData.Lang = LANG.TP.JP;   break;
            case 2: DM.ins.personalData.Lang = LANG.TP.KR;   break;
        }
    }

    public void onClickBtnUnlock2ndSkillDialog(bool isActive){
        if(DM.ins.personalData.IsUnlock2ndSkill) return;
        unlock2ndSkillDialog.Panel.SetActive(isActive);
        unlock2ndSkillDialog.TitleTxt.text = LANG.getTxt(LANG.TXT.DialogUnlock2ndSkill_Title.ToString());
        unlock2ndSkillDialog.InfoTxt.text = LANG.getTxt(LANG.TXT.DialogUnlock2ndSkill_Info.ToString());
    }

    public void onclickBtnBuyUnlock2ndSkill(){
        int price = 1000;
        var unlockSkillList = DM.ins.personalData.SkillLockList.FindAll(list => list == false);
        int unlockSkillListCnt = unlockSkillList.Count;
        Debug.Log("unlockSkillListCnt= " + unlockSkillListCnt);
        if(DM.ins.personalData.Coin < price) {
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.MsgNoMoney.ToString()));
            return;
        }
        else if(unlockSkillListCnt < 2){
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.MsgNoSkill.ToString()));
            return;
        }
        
        DM.ins.personalData.IsUnlock2ndSkill = true;
        DM.ins.personalData.Coin -= price;
        unlock2ndSkillDialog.Panel.SetActive(false);
        drawGrayPanel(false);

        //* Set SkillImg without overlap
        var prefs = DM.ins.scrollviews[(int)DM.PANEL.Skill].ItemPrefs;
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

    public void onClickStartBtn(){
        //* Model Copy
        var playerModel = modelTf.GetChild(0);
        playerModel.GetComponent<Animator>().SetBool(DM.ANIM.IsIdle.ToString(), false); //Ready Pose
        playerModel.SetParent(DM.ins.transform);
        // var copyModel = Instantiate(playerModel, Vector3.zero, Quaternion.identity, DM.ins.transform);

        //* Set Item Passive Data
        int[] lvArrTemp = getItemPsvLvArr(playerModel);
        DM.ins.personalData.ItemPassive.setLvArr(lvArrTemp);

        SceneManager.LoadScene(DM.SCENE.Loading.ToString());
    }

    public void onClickResetBtn(){
        DM.ins.personalData.reset();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void onClickItemPsvInfoBtn(){
        //* Chara Or BatのPSV表示。
        bool isOpen = !DM.ins.scrollviews[(int)DM.PANEL.PsvInfo].ScrollRect.gameObject.activeSelf;
        int currentType = (int)DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        
        //* PsvInfo Panel表示。(全てのPSVが出る)
        DM.ins.scrollviews[(int)DM.PANEL.PsvInfo].ScrollRect.gameObject.SetActive(isOpen);

        //* Set UI
        DM.ins.scrollviews[currentType].ScrollRect.gameObject.SetActive(!isOpen);
        checkBtn.gameObject.SetActive(!isOpen);
        homePanel.GoBtn.gameObject.SetActive(!isOpen);

        var ctt = DM.ins.scrollviews[(int)DM.PANEL.PsvInfo].ContentTf;
        //* 閉じる
        if(!isOpen){
            //* 初期化（全て表示）
            for(int i=0; i< ctt.childCount; i++){
                ctt.GetChild(i).gameObject.SetActive(true);
            }
        }
        //* 開く
        else{
            //* 現在モデルにアクセス。
            var scrollViewEvent = DM.ins.scrollviews[currentType].ScrollRect.GetComponent<ScrollViewEvent>();
            ItemInfo curModel = scrollViewEvent.getCurItem();
            ItemPsvDt[] psvDtArr = curModel.ItemPassive.Arr;
            
            //* Lv0以上スキル Filter。
            var filterPsvArr = Array.FindAll(psvDtArr, psv => psv.lv > 0);
            Debug.Log("onClickItemPsvInfoBtn:: curModel= " + curModel);
            Array.ForEach(filterPsvArr, psv => Debug.Log("onClickItemPsvInfoBtn:: psv.name= " + psv.name));

            //* 「PsvInfoPanel.ContentTf」と「filterPsvArr」と名前が同じ物だけ表示。
            for(int i=0; i< ctt.childCount; i++){
                Debug.Log($"onClickItemPsvInfoBtn:: ctt.GetChild({i}).name= {ctt.GetChild(i).name}");
                string cttPsvName = ctt.GetChild(i).name.Split('_')[1];
                if(Array.Exists(filterPsvArr, psv => cttPsvName == psv.name)){
                    ctt.GetChild(i).gameObject.SetActive(true);
                }
                else{
                    ctt.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void setSelectSkillImg(bool isInit = false){
        Debug.LogFormat("------setSelectSkillImg():: selectedSkillBtnIdx({0}) SelectSkillIdx({1}), SelectSkill2Idx({2})------", selectedSkillBtnIdx, DM.ins.personalData.SelectSkillIdx, DM.ins.personalData.SelectSkill2Idx);
        var ctt = DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf;
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
        Transform tf = Util._.getCharaRightArmPath(charaIns.transform);
        Instantiate(bat, bat.transform.position, bat.transform.rotation, tf);
        
        //* Home Model Delete
        if(0 < modelTf.childCount){
            for(int i=0;i<childs.Length;i++){
                if(i != 0) //* (BUG) [0]は親だから処理しない。
                    Destroy(childs[i].gameObject);
            }
        }
    }
    private void setGUI(){
        //* Set CheckBtn Image
        checkBtn.gameObject.SetActive(true);
        checkMarkImg.gameObject.SetActive(true);
        priceTxt.gameObject.SetActive(false);

        setActiveDialogGUI(DM.ins.SelectItemType);
        switch(DM.ins.SelectItemType){
            case "Home":
                Array.ForEach(DM.ins.scrollviews, sv => 
                    sv.ScrollRect.gameObject.SetActive(false));
                break;
            default : 
                int typeIdx = (int)DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
                SelectPanelScrollBG.color = selectPanelColors[typeIdx];
                DM.ins.scrollviews[typeIdx].ScrollRect.gameObject.SetActive(true);

                var scrollViewEvent = DM.ins.scrollviews[typeIdx].ScrollRect.GetComponent<ScrollViewEvent>();
                scrollViewEvent.setCurSelectedItem(typeIdx);
                scrollViewEvent.updateModelTypeItemInfo();
                scrollViewEvent.exceptAlreadySelectedAnotherSkill(selectedSkillBtnIdx, skillBtns);
                break;
        }
    }
    private void setActiveDialogGUI(string type){
        bool isHome = (type == DM.SCENE.Home.ToString())? true : false;
        homePanel.Panel.gameObject.SetActive(isHome);
        homePanel.GoBtn.gameObject.SetActive(!isHome);
        selectPanel.Panel.gameObject.SetActive(!isHome);

        ItemPsvInfoBtn.gameObject.SetActive((type == DM.PANEL.Chara.ToString() || type == DM.PANEL.Bat.ToString())? true : false);
    }
    private void drawGrayPanel(bool isActive){
        var child = unlock2ndSkillDialog.GoBtn.transform.GetChild(1);
        Debug.Log("drawGrayPanel:: child.name= " + child.name);
        if(child.name == DM.NAME.GrayPanel.ToString()) child.gameObject.SetActive(isActive);
    }

    private string updateADCoolTime(){
        DateTime finishTime = DateTime.Parse(DM.ins.personalData.RouletteTicketCoolTime).AddMinutes(LM._.ROULETTE_TICKET_COOLTIME_MINUTE);
        TimeSpan coolTime = finishTime - DateTime.Now;

        //* Coolタイムが終わったら
        if(coolTime.Ticks < 0){ 
            DM.ins.personalData.RouletteTicket++;
            DM.ins.personalData.RouletteTicketCoolTime = null;
        }

        string coolTimeStr = coolTime.Minutes.ToString("00") + ":" + coolTime.Seconds.ToString("00");
        Debug.Log("coolTime= " + coolTime.Ticks + ", coolTimeStr= " + coolTimeStr);

        return coolTimeStr;
    }
}
