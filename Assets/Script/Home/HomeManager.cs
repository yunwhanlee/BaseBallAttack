using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI.Extensions;
using UnityEngine.Serialization;
using UnityEngine.Rendering;

[System.Serializable] public class FrameUI {
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

[System.Serializable] 
public class StageSelect {
    [Header("お先に設定")]
    [SerializeField] bool isLocked;     public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SerializeField] Sprite sprite;     public Sprite Sprite { get => sprite; }
    [SerializeField] string titleName;     public string TitleName { get => titleName; }
    [SerializeField] int begin;     public int Begin { get => begin; set => begin = value;}
    [SerializeField] int end;     public int End { get => end; set => end = value;}
    [SerializeField] float bonusMultiCoin = 1;  public float BonusMultiCoin { get => bonusMultiCoin; set => bonusMultiCoin = value;}
    [SerializeField] float bonusMultiDiamond = 1;  public float BonusMultiDiamond { get => bonusMultiDiamond; set => bonusMultiDiamond = value;}
    [SerializeField] bool isComingSoon;     public bool IsComingSoon { get => isComingSoon; set => isComingSoon = value; }

    [Header("後で自動設定")]
    RectTransform rectTf;     public RectTransform RectTf { get => rectTf; set => rectTf = value; }
    Image img;     public Image Img { get => img; set => img = value; }
    Text title;     public Text Title { get => title; set => title = value; }
    Text range;     public Text Range { get => range; set => range = value; }
    Button imgBtn;     public Button ImgBtn { get => imgBtn; set => imgBtn = value; }
    RectTransform lockImgTf;    public RectTransform LockImgTf { get => lockImgTf; set => lockImgTf = value; }
    RectTransform bonusLabel;  public RectTransform BonusLabal { get => bonusLabel; set => bonusLabel = value; }
    Text comingSoonTxt;  public Text ComingSoonTxt { get => comingSoonTxt; set => comingSoonTxt = value; }
    Text bonusMultiCoinTxt;  public Text BonusMultiCoinTxt { get => bonusMultiCoinTxt; set => bonusMultiCoinTxt = value;}
    Text bonusMultiDiamondTxt;  public Text BonusMultiDiamondTxt { get => bonusMultiDiamondTxt; set => bonusMultiDiamondTxt = value;}
    
    public void setUIMember(int i){
        Debug.Log($"StageSelect:: setUIMember(i={i})::");

        //* Set Object
        img = Array.Find(rectTf.GetComponentsInChildren<Image>(), img => img.gameObject.name == "StageImg");
        title = Array.Find(rectTf.GetComponentsInChildren<Text>(), txt => txt.gameObject.name == "TitleTxt");
        range = Array.Find(rectTf.GetComponentsInChildren<Text>(), txt => txt.gameObject.name == "RangeTxt");
        comingSoonTxt = Array.Find(rectTf.GetComponentsInChildren<Text>(), obj => obj.name == "ComingSoonTxt");
        imgBtn = Array.Find(rectTf.GetComponentsInChildren<Button>(), btn => btn.gameObject.name == "ImgPanelBtn");

        lockImgTf = Array.Find(rectTf.GetComponentsInChildren<RectTransform>(), obj => obj.name == "LockImgTf");
        bonusLabel = Array.Find(rectTf.GetComponentsInChildren<RectTransform>(), obj => obj.name == "BonusLabel");
        bonusMultiCoinTxt = Array.Find(rectTf.GetComponentsInChildren<Text>(), obj => obj.name == "BonusMultiCoinTxt");
        bonusMultiDiamondTxt = Array.Find(rectTf.GetComponentsInChildren<Text>(), obj => obj.name == "BonusMultiDiamondTxt");

        //* UI
        img.sprite = Sprite;
        title.text = $"{LANG.getTxt(titleName)}";
        range.text = $"{LANG.getTxt(LANG.TXT.Stage.ToString())} {begin} ~ {end}";
        img.color = IsLocked? Color.gray : Color.white;
        lockImgTf.gameObject.SetActive(IsLocked);
        comingSoonTxt.text = (isComingSoon)? "Coming Soon.." : "";
        bonusMultiCoinTxt.text = $"x {bonusMultiCoin.ToString()}";
        bonusMultiDiamondTxt.text = $"x {bonusMultiDiamond.ToString()}";

        //* init
        bonusLabel.gameObject.SetActive((i == (int)DM.MODE.Normal)? false : true);
        imgBtn.GetComponent<Image>().color = (i == (int)DM.MODE.Normal)? Color.yellow : Color.white;
    }
}

public class HomeManager : MonoBehaviour
{
    //* OutSide
    public enum DlgBGColor {Chara, Bat, Skill, CashShop};
    public HomeEffectManager em;
    public AdmobManager am;
    [SerializeField] bool isMuteBtnSfx = true;

    [Header("SELECT PANEL COLOR")][Header("__________________________")]
    [SerializeField] Image selectPanelScrollBG;  public Image SelectPanelScrollBG {get => selectPanelScrollBG; set => selectPanelScrollBG = value;}
    [SerializeField] Color[] selectPanelColors;
    
    [Header("【 GUI 】")][Header("__________________________")]
    public Text versionTxt;
    public Text googlePlayLoginTxt;
    public Canvas mainCanvas;
    public FrameUI homePanel;
    public FrameUI selectPanel;
    public FrameUI unlock2ndSkillDialog;
    public Sprite unlock2ndSkillSprite;
    public Text unlock2ndSkillPriceTxt;
    public Text unlock2ndSkillOkTxt;
    public Text unlock2ndSkillCancelTxt;

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
    public Text roulettePanelTitleTxt;
    public Text rouletteIconCoolTimeTxt;
    public Text rouletteOnedayAdPlayCntTxt;

    [Header("PREMIUM PACKAGE")][Header("__________________________")]
    public GameObject premiumPackPanel;
    public GameObject premiumPackFocusIcon;
    public Button premiumPackIconBtn;
    public Text premiumPackTitle;
    public Text[] premiumPackInfoTxtArr;
    public Text premiumPackPriceTxt;

    [Header("STAGE SELECT PANEL")][Header("__________________________")]
    public int stageIdx;
    public Color navyGray;
    public GameObject stageSelectPanel;
    public RectTransform stageSelectContent;    
    public GameObject stageSelectObjPf;
    public StageSelect[] stageSelects;
    public Text stageSelectPlayBtnTxt;
    public Text stageSelectNoticeTxt;
    public Text stageSelectBackBtnTxt;

    [Header("SHOW REWARD PANEL")][Header("__________________________")]
    public GameObject showRewardPanel;
    public Transform showRewardItemListGroup;
    public GameObject showRewardItemPf;

    [Header("ACHIVEMENT PANEL")][Header("__________________________")]
    public GameObject achivementPanel;
    public Text achivementPanelTitleTxt;
    public RectTransform achivementContent;
    public GameObject[] achivementPfs;
    public Sprite[] rewardIconSprs;
    public GameObject focusIconObj;

    [Header("DIALOG")][Header("__________________________")]
    //* ErrorNetwork
    public RectTransform errorNetworkDialog;
    public Text errorNetworkDialogTitleTxt;
    public Text errorNetworkDialogContentTxt;
    public Text errorNetworkDialogOkTxt;

    //* ShowAD
    public RectTransform showAdDialog;
    public Button showAdFreeBtn;
    public Text adDialogTitleTxt;
    public Text adDialogContentTxt;
    //* Rate
    public RectTransform rateDialog;
    public Text rateTitleTxt;
    public Text rateContentTxt1;
    public Text rateContentTxt2;
    public Text rateLaterBtnTxt;
    public Text rateOkBtnTxt;
    
    //* Setting
    public FrameUI settingDialog;
    public Image settingDialogCountryIconImg;
    public Sprite[] CountryIconSprArr;
    public Dropdown settingQualityDropdown;
    public RenderPipelineAsset[] qualityLevels;
    public Button settingDialogCancelBtn; // アプリ最初ダイアログでは、非表示。
    public Toggle expLogToggle;
    public Toggle bgmToggle;
    public Text settingNoticeTxt;
    public Text settingTitleTxt;
    public Text settingLanguageTxt;
    public Text settingQualityTxt;
    public Text settingExpLogTxt;
    public Text settingBGMTxt;
    public Text settingOkTxt;
    public Text settingCancelTxt;

    //* HardMode Enable Notice Dialog
    public RectTransform nextModeEnableNoticeDialog;
    public Image hardModeImg;
    public Image nightmareModeImg;
    public Text nextModeEnableNoticeTitleTxt;
    public Text nextModeEnableNoticeContent1Txt;
    public Text nextModeEnableNoticeOkBtnTxt;

    //* Update Dialog
    public RectTransform updateDialog;
    public Text updateDialogTitleTxt;
    public Text updateDialogContentTxt;
    public Text updateDialogOkBtnTxt;

    [Header("BUY OR CHECK BTN")][Header("__________________________")]
    public Button checkBtn;
    public Image checkMarkImg;
    public Text priceTxt;
    public Image priceTypeIconImg;

    [Header("PRICE TYPE ICON SPRITE")][Header("__________________________")]
    [FormerlySerializedAs("coinSprite")] [SerializeField] Sprite coinSpr;  public Sprite CoinSpr {get => coinSpr;}
    [FormerlySerializedAs("diamondSprite")] [SerializeField] Sprite diamondSpr;  public Sprite DiamondSpr {get => diamondSpr;}
    [FormerlySerializedAs("cashSprite")] [SerializeField] Sprite cashSpr;  public Sprite CashSpr {get => cashSpr;}

    [Header("MODEL")][Header("__________________________")]
    [SerializeField] Transform modelTf;   public Transform ModelTf {get => modelTf; set => modelTf = value;}

    void Awake(){
        DM.ins.hm = this;
    }
    void Start(){
        // onClickResetBtn();
        if(DM.ins.hm == null) return;

        //* (BUG-77) シーンをロードするときに、ボタンの音がじゃなになること対応。
        StartCoroutine(coWaitInitTriggerBtnSfxOff());

        versionTxt.text = $"ver{Version.MAJOR}.{Version.MINOR}.{Version.REVISION}";

        unlock2ndSkillPriceTxt.text = LM._.UNLOCK_2ND_ATVSKILL_PRICE.ToString();

        errorNetworkDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ErrorNetworkDialog_Title.ToString());
        errorNetworkDialogContentTxt.text = LANG.getTxt(LANG.TXT.ErrorNetworkDialog_Content.ToString());
        errorNetworkDialogOkTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());

        /* Setting Dialog */
        //* Exp Log
        expLogToggle.isOn = DM.ins.personalData.IsActiveExpLog;
        bgmToggle.isOn = DM.ins.personalData.IsActiveBGM;

        //* Set Quality
        QualitySettings.SetQualityLevel(DM.ins.personalData.Quality);
        settingQualityDropdown.value = QualitySettings.GetQualityLevel();

        firstPlayAppSettingDialog();

        onClickBtnGoToPanel(DM.SCENE.Home.ToString());

        setSelectSkillImg(true);

        setStageSelectUIList();

        checkPremiumPackPurchaseStatus();

        achivementPanelTitleTxt.text = LANG.getTxt(LANG.TXT.Achivement.ToString());
        Array.ForEach(achivementPfs, pref => {
            var ins = Instantiate(pref, achivementContent.transform);
            ins.name = ins.name.Split('(')[0]; //* 名前 + (Clone)削除。
        });

        Debug.Log("LanguageOptDropDown.value= " + LanguageOptDropDown.value);
        Debug.Log("DM.ins.personalData.Lang= " + (int)DM.ins.personalData.Lang);
        LanguageOptDropDown.value = (int)DM.ins.personalData.Lang; //* Loadデータで初期化

        rouletteIconBtn.GetComponent<Image>().color = Color.grey;
        roulettePanelTitleTxt.text = LANG.getTxt(LANG.TXT.Roulette.ToString());
        startBtnTxt.text = LANG.getTxt(LANG.TXT.Start.ToString());

        adDialogTitleTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRouletteTicket_Title.ToString());
        adDialogContentTxt.text = LANG.getTxt(LANG.TXT.ShowAdDialogRouletteTicket_Content.ToString());

        selectedSkillBtnIdxTxt.text = LANG.getTxt(LANG.TXT.FirstSkill.ToString());

        //* display Rate Dialog with playTime
        if(DM.ins.personalData.PlayTime == LM._.DISPLAY_RATE_DIALOG_PLAYTIME){
            DM.ins.personalData.PlayTime++; //* Only一回のみ
            rateDialog.gameObject.SetActive(true);
            rateTitleTxt.text = LANG.getTxt(LANG.TXT.Rate.ToString());
            rateContentTxt1.text = LANG.getTxt(LANG.TXT.RateDialog_Content1.ToString());
            rateContentTxt2.text = LANG.getTxt(LANG.TXT.RateDialog_Content2.ToString());
            rateLaterBtnTxt.text = LANG.getTxt(LANG.TXT.Later.ToString());
            rateOkBtnTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());
        }
        
        //* Setting Panel Country Icon
        settingNoticeTxt.text = LANG.getTxt(LANG.TXT.SettingNotice.ToString());
        settingTitleTxt.text = LANG.getTxt(LANG.TXT.Setting.ToString());
        settingLanguageTxt.text = LANG.getTxt(LANG.TXT.Language.ToString());
        settingQualityTxt.text = LANG.getTxt(LANG.TXT.Quality.ToString());
        settingExpLogTxt.text = LANG.getTxt(LANG.TXT.ExpLog.ToString());
        settingBGMTxt.text = LANG.getTxt(LANG.TXT.BGM.ToString());
        settingOkTxt.text =  LANG.getTxt(LANG.TXT.Ok.ToString());
        settingCancelTxt.text =  LANG.getTxt(LANG.TXT.No.ToString());

        settingDialogCountryIconImg.sprite = 
            DM.ins.personalData.Lang == LANG.TP.EN? CountryIconSprArr[(int)LANG.TP.EN]
            : DM.ins.personalData.Lang == LANG.TP.JP? CountryIconSprArr[(int)LANG.TP.JP]
            : CountryIconSprArr[(int)LANG.TP.KR];

        //* HardMode Enable Notice Dialog (Only OneTime)
        if(DM.ins.personalData.IsNormalModeClear && !DM.ins.personalData.IsHardmodeEnableNotice){
            DM.ins.personalData.IsHardmodeEnableNotice = true;
            setNextModeEnableDialogUI(DM.MODE.Hard.ToString());
        }
        if(DM.ins.personalData.IsHardModeClear && !DM.ins.personalData.IsNightMaremodeEnableNotice){
            DM.ins.personalData.IsNightMaremodeEnableNotice = true;
            setNextModeEnableDialogUI(DM.MODE.Nightmare.ToString());
        }
        if(DM.ins.personalData.IsNightmareModeClear && !DM.ins.personalData.IsUpdateNotice){
            DM.ins.personalData.IsUpdateNotice = true;
            updateDialog.gameObject.SetActive(true);
        }

        //* StageSelect Panel
        stageSelectPlayBtnTxt.text = LANG.getTxt(LANG.TXT.Play.ToString());
        stageSelectNoticeTxt.text = LANG.getTxt(LANG.TXT.StageSelectNotice.ToString());
        stageSelectBackBtnTxt.text = LANG.getTxt(LANG.TXT.Back.ToString());

        //* UpdateDialog
        updateDialogTitleTxt.text = LANG.getTxt(LANG.TXT.UpdateDialog_Title.ToString());
        updateDialogContentTxt.text = LANG.getTxt(LANG.TXT.UpdateDialog_Content.ToString());
        updateDialogOkBtnTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());
    }

    void Update(){
        if(DM.ins.personalData.RouletteTicket > 0){
            rouletteIconBtn.GetComponent<Image>().color = Color.white; // def
            rouletteIconCoolTimeTxt.text = "";
        }
        else{
            //* Reset CoolTime
            if(rouletteIconBtn.GetComponent<Image>().color != Color.grey){
                DM.ins.personalData.RouletteTicketCoolTime = DateTime.Now.ToString();
                Debug.Log($"Reset CoolTime:: DM.RouletteTicketCoolTime= {DM.ins.personalData.RouletteTicketCoolTime}");
            }

            rouletteIconBtn.GetComponent<Image>().color = Color.grey;
            rouletteIconCoolTimeTxt.text = updateADCoolTime(); //* Coolタイム 表示!
        }

        //* Achivement redDot表示。(受け取るものがあればお知らせ用。)
        focusIconObj.SetActive(DM.ins.personalData.checkAcceptableAchivement());
    }

//* ----------------------------------------------------------------
//* Button
//* ----------------------------------------------------------------
#region Button
    public void onClickGooglePlayLeaderBoard() {
        Debug.Log("onClickGooglePlayLeaderBoard()::");
        //* GooglePlayログイン。
        GPGSBinder.Inst.Login((success, user) => {
            googlePlayLoginTxt.text = 
            $"{success}, {user.userName}, {user.id}, {user.state}, {user.underage}";
            if(success == true){
                // DM.ins.IsGPGSLogin = true;
                GPGSBinder.Inst.ShowAllLeaderboardUI();
            }
            else{
                errorNetworkDialog.gameObject.SetActive(true);
            }
        });
        //* LeaderBoard表示。
        StartCoroutine(coDisplayLoaderBoard());
    }
    IEnumerator coDisplayLoaderBoard() {
        Debug.Log("coDisplayLoaderBoard::");
        yield return new WaitUntil(() => DM.ins.IsGPGSLogin);
        Debug.Log("coDisplayLoaderBoard:: Display LoaderBoard");
        GPGSBinder.Inst.ShowAllLeaderboardUI();
    }
    public void onClickBtnQuestionMarkIcon() {
        DM.ins.displayTutorialUI();
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
    }

    public void onClickPremiumPackIconBtn() { 
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        premiumPackPanel.SetActive(true); 
    }

    public void onClickAchivementIconBtn() { achivementPanel.SetActive(true); SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());}

    public void onCLickRewardPanelOkBtn() {
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        showRewardItemListGroup.transform.DetachChildren();
        showRewardPanel.SetActive(false);
    }

    public void onClickBtnGoToPanel(string name){
        Debug.Log($"onClickBtnGoToPanel(name= {name}):: DM.ins.hm= {DM.ins.hm}, checkBtn= {checkBtn}");
        if(!isMuteBtnSfx) SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        //* Current Model Data & ParentTf
        DM.ins.SelectItemType = name;
        var curChara = DM.ins.scrollviews[(int)DM.PANEL.Chara].ItemPrefs[DM.ins.personalData.SelectCharaIdx];
        var curBat = DM.ins.scrollviews[(int)DM.PANEL.Bat].ItemPrefs[DM.ins.personalData.SelectBatIdx];
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);

        setGUI();
        
        switch(itemType){
            case DM.PANEL.Chara :
                // DM.ins.scrollviews[(int)DM.PANEL.Chara].ScrollRect.GetComponent<ScrollViewEvent>().BefIdx = DM.ins.personalData.SelectCharaIdx;
                // DM.ins.scrollviews[(int)DM.PANEL.Chara].ScrollRect.GetComponent<ScrollViewEvent>().CurIdx = DM.ins.personalData.SelectCharaIdx;
                // DM.ins.scrollviews[(int)DM.PANEL.Chara].ScrollRect.GetComponent<ScrollViewEvent>().updateModelTypeItemInfo();

                break;
            case DM.PANEL.Bat :
                // DM.ins.scrollviews[(int)DM.PANEL.Bat].ScrollRect.GetComponent<ScrollViewEvent>().BefIdx = DM.ins.personalData.SelectCharaIdx;
                // DM.ins.scrollviews[(int)DM.PANEL.Bat].ScrollRect.GetComponent<ScrollViewEvent>().CurIdx = DM.ins.personalData.SelectCharaIdx;
                // DM.ins.scrollviews[(int)DM.PANEL.Bat].ScrollRect.GetComponent<ScrollViewEvent>().updateModelTypeItemInfo();
                break;
            case DM.PANEL.Skill :
            case DM.PANEL.CashShop :
            case DM.PANEL.Upgrade :
                // なし
                break;
            case DM.PANEL.PsvInfo :
                // if(checkBtn)
                    checkBtn.gameObject.SetActive(false);
                break;
            default: //* Home
                createCurModel(curChara, curBat, modelTf);
                setSelectSkillImg();
                break;
        }
    }

    public void onClickBtnSelectSkillImg(int idx){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
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
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        if(DM.ins.personalData.RouletteTicket <= 0){ //* AD Show Dialog 表示

            //* 一日が過ぎたら、OneDay Ad Play Cnt 初期化
            TimeSpan spanTime = DateTime.Now - DateTime.Parse(DM.ins.personalData.RouletteTicketOneDayAdCntCoolTime); //* TEST NEXT DAY => .AddHours(25);
            if(Math.Abs(spanTime.Days) >= 1){
                Debug.Log($"onClickRouletteIconBtn:: spanTime= {spanTime}, spanTime.Days= {Math.Abs(spanTime.Days)}");
                DM.ins.personalData.RouletteTicketOneDayAdPlayCnt = 0;
                DM.ins.personalData.RouletteTicketOneDayAdCntCoolTime = DateTime.Now.ToString();
            }

            //* 一日、AD利用回数をチェック
            if(DM.ins.personalData.RouletteTicketOneDayAdPlayCnt >= LM._.ROULETTE_TICKET_ONEDAY_AD_PLAY_MAX_CNT){
                Debug.Log("<color=red>onClickRouletteIconBtn:: RouletteTicket OneDay ShowAd Max Cnt OVER!!</color>");
                Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.OneDayUsingMaxCntOver.ToString()));
                return;
            }
            showAdDialog.gameObject.SetActive(true);

            //* Oneday AD PlayCnt Text
            rouletteOnedayAdPlayCntTxt.text = $"{DM.ins.personalData.RouletteTicketOneDayAdPlayCnt} / {LM._.ROULETTE_TICKET_ONEDAY_AD_PLAY_MAX_CNT}";
        }
        else{ //* Rolette Panel 表示
            Debug.Log("onClickRouletteIconBtn:: Dialog表示");

            roulettePanel.gameObject.SetActive(true);
            homePanel.Panel.gameObject.SetActive(false);
            homePanel.GoBtn.gameObject.SetActive(false);
            ItemPsvInfoBtn.gameObject.SetActive(false);
        }
    }
    public void onClickRateOkBtn(){
        Debug.Log("<color=yellow>TODO</color> onClickRateBtn():: Googleストアーへ移動!");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        Application.OpenURL("https://play.google.com/store/games");
    }
    public void onClickShowADButton(){
        //* 広告要請
        am.showRewardAd(DM.REWARD.ROULETTE_TICKET);
    }

#region SETTING
    private void firstPlayAppSettingDialog(){
        Debug.Log("HM::firstPlayAppSettingDialog():: ");
        if(!DM.ins.personalData.IsChoiceLang){
            //! (BUG-47) モバイルビルドしたら、最初Loadデータがなくなるバグ。*原因：最初言語設定でonClickSettingOkBtnからsaveしますが、collectionなどのデータがないまましちゃう。
            DM.ins.personalData.reset(); //* (BUG-47)対応:: resetして、saveする前にTemplateを用意する。
            DM.ins.personalData.IsChoiceLang = true;
            settingDialog.Panel.SetActive(true);
            settingDialogCancelBtn.gameObject.SetActive(false);
        }
    }
    public void onClickSettingBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        settingDialog.Panel.SetActive(true);
    }
    public void onDropDownLanguageOpt(){
        int idx = LanguageOptDropDown.value;
        Debug.Log("onDropDownSettingOpt():: idx= " + idx);
        switch(idx){
            case 0: DM.ins.personalData.Lang = LANG.TP.EN;
                settingDialogCountryIconImg.sprite = CountryIconSprArr[0];
                break;
            case 1: DM.ins.personalData.Lang = LANG.TP.JP;
                settingDialogCountryIconImg.sprite = CountryIconSprArr[1];
                break;
            case 2: DM.ins.personalData.Lang = LANG.TP.KR;
                settingDialogCountryIconImg.sprite = CountryIconSprArr[2];
                break;
        }
    }
    public void onDropDownChangeQualityOpt(int value){
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
        DM.ins.personalData.Quality = value; // Save
    }
    public void onChangeExpLogToggle(){
        Debug.Log("onChangeExpLogToggle:: " + expLogToggle.isOn);
        DM.ins.personalData.IsActiveExpLog = expLogToggle.isOn;
    }
    public void onChangeBGMToggle(){
        Debug.Log("onChangeBGMToggle:: " + bgmToggle.isOn);
        DM.ins.personalData.IsActiveBGM = bgmToggle.isOn;
    }
    public void onClickSettingOkBtn(bool isOk){
        Debug.Log($"HM::onClickSettingOkBtn({isOk})");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        settingDialog.Panel.SetActive(false);
        // expLogToggle.isOn = DM.ins.personalData.IsActiveExpLog;
        DM.ins.personalData.save();
        if(isOk) SceneManager.LoadScene(DM.SCENE.Home.ToString()); //* Re-load
    }
#endregion

#region UNLOCK 2ND ATV SKILL DIALOG
    public void onClickBtnUnlock2ndSkillDialog(bool isActive){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        if(DM.ins.personalData.IsUnlock2ndSkill) return;
        unlock2ndSkillDialog.Panel.SetActive(isActive);
        unlock2ndSkillDialog.TitleTxt.text = LANG.getTxt(LANG.TXT.DialogUnlock2ndSkill_Title.ToString());
        unlock2ndSkillDialog.InfoTxt.text = LANG.getTxt(LANG.TXT.DialogUnlock2ndSkill_Info.ToString());
        unlock2ndSkillOkTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());
        unlock2ndSkillCancelTxt.text = LANG.getTxt(LANG.TXT.No.ToString());
    }

    public void onclickBtnBuyUnlock2ndSkill(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        int price = LM._.UNLOCK_2ND_ATVSKILL_PRICE;
        var unlockSkillList = DM.ins.personalData.AtvSkillLockList.FindAll(list => list == false);
        int unlockSkillListCnt = unlockSkillList.Count;
        Debug.Log("unlockSkillListCnt= " + unlockSkillListCnt);
        if(DM.ins.personalData.Diamond < price) {
            Util._.displayNoticeMsgDialog(
                LANG.getTxt(LANG.TXT.Diamond.ToString()) + LANG.getTxt(LANG.TXT.NotEnough.ToString()));
            return;
        }
        else if(unlockSkillListCnt < 2){
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.MsgNoSkill.ToString()));
            return;
        }

        //* Purchase Success
        displayShowRewardPanelUnLock2ndAtvSkill();
        
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
#endregion

    public void onClickStartBtn() => setStageSelectPanel(true);
    public void onClickStageSelectExitBtn() => setStageSelectPanel(false);
    
    private void setStageSelectPanel(bool isOn){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        stageSelectPanel.SetActive(isOn); //* Open StageSelectPanel
        modelTf.gameObject.SetActive(!isOn); //* 3DモデルがUIパンネルを突き抜けるので、非表示
    }

    public void onClickStageSelectImgBtn(int idxNum){
        Debug.Log($"onClickStagePictureBtn:: idxNum= {idxNum}, isLocked= {stageSelects[idxNum].IsLocked}, WIDTH= {stageSelects[idxNum].RectTf.rect.width}, SPACING= {stageSelectContent.GetComponent<HorizontalLayoutGroup>().spacing}");
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        float WIDTH = stageSelects[idxNum].RectTf.rect.width;
        float SPACING = stageSelectContent.GetComponent<HorizontalLayoutGroup>().spacing;

        int i=0;
        Array.ForEach(stageSelects, stageSelect => {
            if(i == idxNum) stageIdx = i;
            stageSelect.ImgBtn.GetComponent<Image>().color = (i == idxNum)? Color.yellow : navyGray;
            i++;
        });
        
        stageSelectContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(WIDTH * idxNum + SPACING * idxNum), 0);
    }

    public void onClickPlayBtn(){
        //* Fail
        if(stageSelects[stageIdx].IsLocked){
            SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.MsgHardmodeLocked.ToString()));
            return;
        }
    
        //* Set Stage & Mode
        DM.ins.StageNum = stageSelects[stageIdx].Begin;
        DM.ins.Mode = (stageIdx == 0)? DM.MODE.Normal : (stageIdx == 1)? DM.MODE.Hard : DM.MODE.Nightmare;

        var playerModel = modelTf.GetChild(0);
        playerModel.GetComponent<Animator>().SetBool(DM.ANIM.IsIdle.ToString(), false); //Ready Pose
        playerModel.SetParent(DM.ins.transform);
        // var copyModel = Instantiate(playerModel, Vector3.zero, Quaternion.identity, DM.ins.transform);

        //* Set Item Passive Data
        int[] lvArrTemp = getItemPsvLvArr(playerModel);
        DM.ins.personalData.ItemPassive.setLvArr(lvArrTemp);

        //* (BUG-57) 時々スタートしたとき、BatEffect Transformが活性化していることを対応。
        var bat = Array.Find(playerModel.GetComponentsInChildren<ItemInfo>(), tf => tf.name.Contains("Bat"));
        for(int i=0; i<bat.transform.childCount; i++){
            if(bat.transform.GetChild(i).name == "BatEffectTf" && bat.transform.GetChild(i).gameObject.activeSelf){
                Debug.Log($"{i}: {bat.transform.GetChild(i).name}, active? {bat.transform.GetChild(i).gameObject.activeSelf} -> OFF");
                bat.transform.GetChild(i).gameObject.SetActive(false); 
                break; //* (BUG-87) for文のif文に該当するなら、処理してfor文を抜ける処理をしたくてreturnをしましたが、これは関数()を抜ける処理でした。breakに切り替え修正。
            }
        }

        //* Set Sky X Value
        Debug.Log("onClickPlayBtn:: curStageSelectIndex= " + stageIdx);
        float x = (DM.ins.Mode == DM.MODE.Normal)? LM._.SKY_MT_X_MORNING : (DM.ins.Mode == DM.MODE.Hard)? LM._.SKY_MT_X_DINNER : LM._.SKY_MT_X_NIGHT;
        DM.ins.simpleSkyMt.SetTextureOffset("_MainTex", new Vector2(x, 0));

        //* シーン 読込み。
        SM.ins.sfxPlay(SM.SFX.PlayBtn.ToString());
        SceneManager.LoadScene(DM.SCENE.Loading.ToString());
    }

    public void onClickResetBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        DM.ins.personalData.reset();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }

    public void onClickItemPsvInfoBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
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
    public void onClickPremiumPackPurchaseBtn(){
        //* InAppManager.csへ宣言。
    }
    public void displayShowRewardPanel(int coin = 0, int diamond = 0, int rouletteTicket = 0, bool removeAD = false){
        //* Sound
        SM.ins.sfxPlay(SM.SFX.PurchaseSuccess.ToString());
        //* Effect
        em.createCongratuBlastRainbowEF(mainCanvas.transform);
        //* Open Show Reward Panel
        showRewardPanel.SetActive(true);
        //* Set Showreward ItemPfs
        const int COIN = 0, DIAMOND = 1, ROULETTE_TICKET = 2, REMOVE_AD = 3;
        if(coin > 0)    createShowRewardItemPf(COIN, $"{coin}");
        if(diamond > 0)    createShowRewardItemPf(DIAMOND, $"{diamond}");
        if(rouletteTicket > 0)    createShowRewardItemPf(ROULETTE_TICKET, $"{rouletteTicket}");
        if(removeAD)    createShowRewardItemPf(REMOVE_AD, "SKIP");
    }
    public void displayShowRewardPanelUnLock2ndAtvSkill(){
        //* Sound
        SM.ins.sfxPlay(SM.SFX.PurchaseSuccess.ToString());
        //* Effect
        em.createCongratuBlastRainbowEF(mainCanvas.transform);
        //* Open Show Reward Panel
        showRewardPanel.SetActive(true);
        //* Set Showreward ItemPfs
        const int ICON = 0, TEXT = 1;
        var itemPf = Instantiate(showRewardItemPf, showRewardItemListGroup, false);
        itemPf.transform.GetChild(ICON).GetComponent<Image>().sprite = unlock2ndSkillSprite;
        itemPf.transform.GetChild(TEXT).GetComponent<Text>().text = "";
    }
    public void setCheckBtnUninteractable(){
        checkBtn.interactable = false;
        priceTxt.text = "done";
    }
#endregion
//* ----------------------------------------------------------------
//* Function
//* ----------------------------------------------------------------
#region Function
    private void setNextModeEnableDialogUI(string mode){
        //* 表示
        nextModeEnableNoticeDialog.gameObject.SetActive(true);
        //* Lang
        if(mode == DM.MODE.Hard.ToString()){
            hardModeImg.gameObject.SetActive(true);
            nextModeEnableNoticeTitleTxt.text = LANG.getTxt(LANG.TXT.HardMode.ToString());
        }else{
            nightmareModeImg.gameObject.SetActive(true);
            nextModeEnableNoticeTitleTxt.text = LANG.getTxt(LANG.TXT.NightmareMode.ToString());
        }
        nextModeEnableNoticeContent1Txt.text = LANG.getTxt(LANG.TXT.NextMode_Content.ToString());
        nextModeEnableNoticeOkBtnTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());
    }

    IEnumerator coWaitInitTriggerBtnSfxOff(){
        yield return Util.delay1;
        isMuteBtnSfx = false;
    }
    public void showRoulettePanel(){
        roulettePanel.gameObject.SetActive(true);
        showAdDialog.gameObject.SetActive(false);
        homePanel.Panel.gameObject.SetActive(false);
        homePanel.GoBtn.gameObject.SetActive(false);
        ItemPsvInfoBtn.gameObject.SetActive(false);
    }
    public void setRateDialog(bool isActive){
        updateDialog.gameObject.SetActive(false);

        rateDialog.gameObject.SetActive(isActive);
        rateTitleTxt.text = LANG.getTxt(LANG.TXT.Rate.ToString());
        rateContentTxt1.text = LANG.getTxt(LANG.TXT.RateDialog_Content1.ToString());
        rateContentTxt2.text = LANG.getTxt(LANG.TXT.RateDialog_Content2.ToString());
        rateLaterBtnTxt.text = LANG.getTxt(LANG.TXT.Later.ToString());
        rateOkBtnTxt.text = LANG.getTxt(LANG.TXT.Ok.ToString());
    }

    private void setStageSelectUIList(){
        //* Set Stage Range (Beginと.End)
        int stageCnt = LM._.BOSS_STAGE_SPAN * LM._.VICTORY_BOSSKILL_CNT;
        stageSelects[(int)DM.MODE.Normal].Begin = 1;
        stageSelects[(int)DM.MODE.Normal].End = stageCnt;

        stageSelects[(int)DM.MODE.Hard].Begin = 1 + stageCnt;
        stageSelects[(int)DM.MODE.Hard].End = stageCnt * 2;
        
        stageSelects[(int)DM.MODE.Nightmare].Begin = 1 + (stageCnt * 2);
        stageSelects[(int)DM.MODE.Nightmare].End = stageCnt * 3;

        //* Check Mode Data
        if(DM.ins.personalData.IsNormalModeClear)
            stageSelects[(int)DM.MODE.Hard].IsLocked = false;
        if(DM.ins.personalData.IsHardModeClear)
            stageSelects[(int)DM.MODE.Nightmare].IsLocked = false;

        //* Set StageSelectList
        for(int i=0; i<stageSelects.Length; i++){
            //* Set member Value
            stageSelects[i].RectTf = Instantiate(stageSelectObjPf, stageSelectContent, false).GetComponent<RectTransform>();
            stageSelects[i].setUIMember(i);

            //* AddEventListener('onClick')
            int copy = i; //! (BUG-22) For分内にonClick.AddListener(int i)すると、Indexが全てLast+1になるバグ対応。
            stageSelects[i].ImgBtn.onClick.AddListener(() => onClickStageSelectImgBtn(copy));
        }
    }

    private void setSelectSkillImg(bool isInit = false){
        Debug.Log($"HM::setSelectSkillImg({isInit}):: selectedSkillBtnIdx= {selectedSkillBtnIdx}, DM.ins.SelectSkillIdx= {DM.ins.personalData.SelectSkillIdx}, DM.ins.SelectSkill2Idx= {DM.ins.personalData.SelectSkill2Idx}");        
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
        Debug.Log($"HM::setSelectSkillSprite({btnIdx}, {content}, {idx})::");
        
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
        Debug.Log("createCurModel:: ");
        var childs = modelTf.GetComponentsInChildren<Transform>();
        // Chara
        var charaIns = Instantiate(chara, Vector3.zero, Quaternion.identity, modelTf) as GameObject;
        // Bat
        Transform tf = Util._.getCharaRightArmPath(charaIns.transform);
        var batIns = Instantiate(bat, bat.transform.position, bat.transform.rotation, tf);
        Debug.Log($"createCurModel:: batIns= {batIns.name}");

        //* Chara RankAura Off
        charaIns.GetComponent<ItemInfo>().RankAuraEF.SetActive(false);

        //* Bat Imageなど要らない部分を非表示して、3Dモデルのみ表示。
        for(int i=0; i < batIns.transform.childCount; i++){
            if(i==0) {
                batIns.transform.GetChild(0).gameObject.SetActive(true);
                batIns.GetComponentInParent<MeshRenderer>().enabled = true;
            }
            else  batIns.transform.GetChild(i).gameObject.SetActive(false);
        }
        
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
        Debug.Log("onClickBtnGoToPanel::setGUI:: checkBtn= " + checkBtn);
        if(!checkBtn) return;
        checkBtn.gameObject.SetActive(true);
        checkMarkImg.gameObject.SetActive(true);
        priceTxt.gameObject.SetActive(false);
        priceTypeIconImg.gameObject.SetActive(false);

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
        TimeSpan coolTime;
        DateTime finishTime = DateTime.MinValue;
        try{
            //! (BUG-40) ROULETTEチケットが０になったとき、string not valid エラー発生。
            //? DateTime.ParseExactを使ったことが原因。しかし、不安定でDataTime.Parseで同じエラー出る可能性あるかも。
            // finishTime = DateTime.ParseExact(DM.ins.personalData.RouletteTicketCoolTime, "yyyy/MM/dd HH:mm:ss", null).AddMinutes(LM._.ROULETTE_TICKET_COOLTIME_MINUTE);
            finishTime = DateTime.Parse(DM.ins.personalData.RouletteTicketCoolTime).AddMinutes(LM._.ROULETTE_TICKET_COOLTIME_MINUTE);
            coolTime = finishTime - DateTime.Now;

            //* CoolTimeが終わったら、チケットを一つ得る。
            if(coolTime.Ticks < 0){
                Debug.Log("Coolタイムが終わったら");
                DM.ins.personalData.RouletteTicket++;
                DM.ins.personalData.RouletteTicketCoolTime = null;
            }

            //* TEXT-UIへ表示するため、String化。
            string coolTimeStr = coolTime.Minutes.ToString("00") + ":" + coolTime.Seconds.ToString("00");
            return coolTimeStr;
        }
        catch(Exception err){
            Debug.LogError("ERROR::updateADCoolTime():: " 
                + "\n coolTime= " + coolTime
                + "\n finishTime= " + finishTime
                + "\n DM.RouletteTicketCoolTime= "+ DM.ins.personalData.RouletteTicketCoolTime
                + "\n DateTime.Now= " + DateTime.Now
                + "\n msg= " + err
            );
            DM.ins.personalData.addRouletteTicket(1);
            return "";
        }
    }
    public void checkPremiumPackPurchaseStatus(){
        const int COIN = 0, DIAMOND = 1, ROULETTE_TICKET = 2, REMOVE_AD = 3;
        if(!DM.ins.personalData.IsPurchasePremiumPack){
            //* Set Language Premium Pack
            premiumPackTitle.text = LANG.getTxt(LANG.TXT.PremiumPack.ToString());
            premiumPackInfoTxtArr[COIN].text = $"{LM._.PREM_PACK_COIN} {LANG.getTxt(LANG.TXT.Diamond.ToString())}";
            premiumPackInfoTxtArr[DIAMOND].text = $"{LM._.PREM_PACK_DIAMOND} {LANG.getTxt(LANG.TXT.Coin.ToString())}";
            premiumPackInfoTxtArr[ROULETTE_TICKET].text = $"x {LM._.PREM_PACK_ROULETTE_TICKET}";
            premiumPackInfoTxtArr[REMOVE_AD].text = $"{LANG.getTxt(LANG.TXT.RemoveAllADs.ToString())}";
            premiumPackPriceTxt.text = $"￥{ LM._.PREM_PACK_PRICE}";
        }
        else{
            //* Close PremiumPackPanel
            StartCoroutine(coWaitClosePremiumPackPanel());

        }
    }
    IEnumerator coWaitClosePremiumPackPanel(){
        //* (BUG-58) IAP Button PremiumPackageで購入したら親パンネルがすぐ閉じちゃうとエラー。
        yield return Util.delay0_1;
        premiumPackPanel.SetActive(false);
        premiumPackFocusIcon.SetActive(false);
        premiumPackIconBtn.interactable = false;
    }
    private void createShowRewardItemPf(int idx, string valueStr){
        const int ICON = 0, TEXT = 1;
        var itemPf = Instantiate(showRewardItemPf, showRewardItemListGroup, false);
        Sprite IconSpr = premiumPackInfoTxtArr[idx].transform.parent.GetChild(ICON).GetComponent<Image>().sprite;
        string TextVal = valueStr;

        itemPf.transform.GetChild(ICON).GetComponent<Image>().sprite = IconSpr;
        itemPf.transform.GetChild(TEXT).GetComponent<Text>().text = TextVal;
    }
    public void onClickBtnTestPruchasePremiumPackage(){
        SM.ins.sfxPlay(SM.SFX.PurchaseSuccess.ToString());
        DM.ins.personalData.IsPurchasePremiumPack = true;

        // Set Data
        DM.ins.personalData.addRouletteTicket(LM._.PREM_PACK_ROULETTE_TICKET);
        DM.ins.personalData.addCoin(LM._.PREM_PACK_COIN); // DM.ins.personalData.Coin += LM._.PREM_PACK_COIN;
        DM.ins.personalData.addDiamond(LM._.PREM_PACK_DIAMOND);
        DM.ins.personalData.IsRemoveAD = true;

        // UI
        DM.ins.setUIRemoveAD();
        checkPremiumPackPurchaseStatus();

        displayShowRewardPanel(
            coin: LM._.PREM_PACK_COIN,
            diamond: LM._.PREM_PACK_DIAMOND,
            rouletteTicket: LM._.PREM_PACK_ROULETTE_TICKET,
            removeAD: true
        );
    }
#endregion
}
