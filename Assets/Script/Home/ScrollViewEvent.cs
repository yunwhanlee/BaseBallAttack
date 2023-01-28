using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI.Extensions;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;

[System.Serializable]
public class ScrollView {
    //* Value
    [SerializeField] String type;  public String Type {get => type; set => type = value;} 
    [SerializeField] RectTransform scrollRect;  public RectTransform ScrollRect {get => scrollRect; set => scrollRect = value;}
    [SerializeField] RectTransform contentTf;  public RectTransform ContentTf {get => contentTf; set => contentTf = value;}
    [SerializeField] GameObject[] itemPrefs;  public GameObject[] ItemPrefs {get => itemPrefs; set => itemPrefs = value;}

    public ScrollView(RectTransform scrollRect, RectTransform contentTf, GameObject[] itemPrefs){
        this.type = scrollRect.gameObject.name.Split('_')[1];
        this.scrollRect = scrollRect;
        this.contentTf = contentTf;
        this.itemPrefs = itemPrefs;
    }

    public void createItem(RectTransform modelParentPref, RectTransform itemPassivePanel, RectTransform itemSkillBoxPref){
        Debug.LogFormat("createItem:: {0}, {1}, {2}, type= {3}",modelParentPref, itemPassivePanel, itemSkillBoxPref, this.type);
        //* Prefabs 生成
        var itemType = DM.ins.getCurPanelType2Enum(this.type);
        Array.ForEach(itemPrefs, itemPf=>{
            //* 生成
            RectTransform modelContentPf = null;
            RectTransform psvPanel = null;
            GameObject ins = null;

            switch(itemType){
                case DM.PANEL.Chara : 
                case DM.PANEL.Bat :
                    //* Rect Transform -> UI
                    modelContentPf = GameObject.Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf);
                    modelContentPf.anchoredPosition3D = Vector3.zero; //* 親(ModelContentPf)ずれること対応。

                    //* Transform -> GameObject
                    ins = GameObject.Instantiate(itemPf, Vector3.zero, Quaternion.identity, modelContentPf);
                    ins.transform.localPosition = Vector3.zero;

                    //* Item Passive UI Ready
                    psvPanel = GameObject.Instantiate(itemPassivePanel, itemPassivePanel.localPosition, itemPassivePanel.localRotation, modelContentPf);
                    psvPanel.anchoredPosition3D = new Vector3(0,-2,0);
                    //* 追加処理
                    ins.GetComponent<ItemInfo>().ItemPassive.setImgPrefs(DM.ins.personalData.ItemPassive);
                    break;
                case DM.PANEL.Skill :
                case DM.PANEL.CashShop :
                case DM.PANEL.PsvInfo :
                {
                    ins = GameObject.Instantiate(itemPf, Vector3.zero, Quaternion.identity, contentTf);
                    ins.transform.localPosition = Vector3.zero;
                    // Debug.Log("<color=yellow>【"+ this.type + "】</color> model.name= " + model.name + ", personalData.Lang= " + DM.ins.personalData.Lang);
                    break;
                }
                case DM.PANEL.Upgrade :{
                    ins = GameObject.Instantiate(itemPf, Vector3.zero, Quaternion.identity, contentTf);
                    ins.transform.localPosition = Vector3.zero;
                    //TODO 追加処理 APPLY LOAD DATA
                    var upgrade = DM.ins.personalData.Upgrade;
                    var idx = Array.FindIndex(itemPrefs, (pref) => pref == itemPf);
                    ins.GetComponent<ItemInfo>().UpgradeValueTxt.text = upgrade.Arr[idx].Lv.ToString();
                    break;
                }
            }

            if(!ins) return;
            
            var itemPassiveList = ins.GetComponent<ItemInfo>().ItemPassive.Arr;
            //* 調整
            switch(itemType){
                case DM.PANEL.Chara : 
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case DM.PANEL.Bat :
                    // Vector3 ZOOM_IN_POS = new Vector3(1.61f, 0.75f, 4.43f); //* ベットは小さいから少しズームイン。
                    // ins.transform.localPosition = ZOOM_IN_POS;
                    // ins.transform.localRotation = Quaternion.Euler(ins.transform.localRotation.x, ins.transform.localRotation.y, -45);
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case DM.PANEL.Skill : 
                case DM.PANEL.CashShop : 
                case DM.PANEL.Upgrade :
                    ins.transform.localPosition = new Vector3(0,0,0); //* posZがずれるから、調整
                    //! Buy AddEventListener !//
                    Debug.Log($"createItem:: <color=red>AddEventLister</color>(()=><color=yellow>onClickItem</color>({ins.name}))");
                    Button btn = ins.GetComponent<Button>();
                    var svEvent = ScrollRect.GetComponent<ScrollViewEvent>();
                    btn.onClick.AddListener(delegate{svEvent.onClickItem(ins);});
                    break;
        }
            // Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            ins.name = itemPf.name;//名前上書き：しないと後ろに(clone)が残る。
        });

        pushItemLanguageList(itemType.ToString());
    }

    private void pushItemLanguageList(string itemType){
        //* ItemCreateが終わったら、一回活性化して、ItemInfoに書いた言語情報をLanguageのInfoリストへ入れる。
        var hm = GameObject.Find(DM.NAME.HomeManager.ToString()).GetComponent<HomeManager>();
        hm.onClickBtnGoToPanel(itemType);
    }
    public void setLanguage(){
        // for(int i=0; i<ContentTf.childCount; i++) Debug.Log($"ScrollViewEvent::setLanguage():: ContentTf={ContentTf.GetChild(i)}")

        //* タイトル。
        var title = scrollRect.GetComponent<ScrollViewEvent>().TitleTxt;
        if(this.type == DM.PANEL.CashShop.ToString()) title.text = LANG.getTxt(LANG.TXT.CashShop.ToString());
        else if(this.type == DM.PANEL.PsvInfo.ToString()) title.text = LANG.getTxt(LANG.TXT.PsvSkillInfo.ToString());
        else if(this.type == DM.PANEL.Upgrade.ToString()) title.text = LANG.getTxt(LANG.TXT.Upgrade.ToString());

        //* コンテンツ。
        for(int i = 0; i < ContentTf.childCount; i++){
            Text[] txtObjs = null;
            List<string> strList = new List<string>();
            
            var childs = ContentTf.GetChild(i).GetComponentsInChildren<Text>();
            
            if(this.type == DM.PANEL.Skill.ToString()){
                txtObjs = Array.FindAll(childs, chd => chd.name == LANG.OBJNAME.NameTxt.ToString() || chd.name == LANG.OBJNAME.ExplainTxt.ToString() || chd.name == LANG.OBJNAME.HomeRunBonusTxt.ToString());
                strList.Add(DM.PANEL.Skill.ToString());
                strList.Add(txtObjs[LANG.NAME].text);
                strList.Add(txtObjs[LANG.EXPLAIN].text);
                strList.Add(txtObjs[LANG.HOMERUNBONUS].text);
            }
            else if(this.type == DM.PANEL.CashShop.ToString()){
                txtObjs = Array.FindAll(childs, chd => chd.name == LANG.OBJNAME.NameTxt.ToString() || chd.name == LANG.OBJNAME.ExplainTxt.ToString());
                strList.Add(DM.PANEL.CashShop.ToString());
                strList.Add(txtObjs[LANG.NAME].text);
                strList.Add(txtObjs[LANG.EXPLAIN].text);
            }
            else if(this.type == DM.PANEL.PsvInfo.ToString()){
                txtObjs = Array.FindAll(childs, chd => chd.name == LANG.OBJNAME.NameTxt.ToString() || chd.name == LANG.OBJNAME.ExplainTxt.ToString());
                // Array.ForEach(txtObjs, txtObj => Debug.Log($"{txtObj.name}= {txtObj.text}"));
                strList.Add(DM.PANEL.PsvInfo.ToString());
                strList.Add(txtObjs[LANG.NAME].text);
                strList.Add(txtObjs[LANG.EXPLAIN].text);
                // Debug.Log($"Replace Num:: DM.ins.personalData.Upgrade.Arr[{i}].unit->{DM.ins.personalData.Upgrade.Arr[i].unit}");
            }
            else if(this.type == DM.PANEL.Upgrade.ToString()){
                txtObjs = Array.FindAll(childs, chd => chd.name == LANG.OBJNAME.NameTxt.ToString() || chd.name == LANG.OBJNAME.ExplainTxt.ToString());
                Array.ForEach(txtObjs, txtObj => Debug.Log($"{this.type}::{txtObj.name}= {txtObj.text}"));
                strList.Add(DM.PANEL.Upgrade.ToString());
                strList.Add(txtObjs[LANG.NAME].text);
                strList.Add(txtObjs[LANG.EXPLAIN].text); //* Replace InspectorView Setting Number (ExplainText)
            }
            if(txtObjs == null && strList.Count == 0) return;
            //* Join Strings
            var tempStr = string.Join("_", strList);
            var languageList = LANG.getTxtList(tempStr);
            //* Set Language
            for(int j = 0; j < txtObjs.Length; j++) txtObjs[j].text = languageList[j];
        }
    }

    public void initUpgradeDt(ItemInfo[] upgrades){
        for(int i=0; i<upgrades.Length; i++){
            UpgradeDt upgradeDt = DM.ins.personalData.Upgrade.Arr[i];
            upgrades[i].setUI(upgradeDt);
            //* Set Price
            List<int> priceList = ScrollViewEvent.setUpgradePriceCalc(upgradeDt);
            upgrades[i].price.setValue(priceList[upgradeDt.Lv]);
        }
    }
    public void initAtvSkillUpgradeDt(ItemInfo[] skills){
        for(int i=0; i<skills.Length; i++){
            UpgradeDt atvSkUpgDt = DM.ins.personalData.AtvSkillUpgrade.Arr[i];
            skills[i].setUI(atvSkUpgDt);
            //* Set Price
            List<int> priceList = ScrollViewEvent.setAtvSkillUpgradePriceCalc(atvSkUpgDt);
            skills[i].price.setValue(priceList[atvSkUpgDt.Lv]);
        }
    }

    private void displayItemPassiveUI(string type, ItemPsvDt[] itemPassiveList, RectTransform itemSkillBoxPref, RectTransform psvPanel){
        Array.ForEach(itemPassiveList, list=>{
            if(list.lv > 0){
                // Debug.Log(list.imgPref.name + "= " + list.lv);
                var boxPref = GameObject.Instantiate(itemSkillBoxPref, itemSkillBoxPref.localPosition, itemSkillBoxPref.localRotation, psvPanel);
                var imgPref = GameObject.Instantiate(list.imgPref, Vector3.zero, Quaternion.identity, boxPref).transform;
                boxPref.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
                // switch(type){
                //     case "Chara" : break;
                //!     case "Bat": psvPanel.GetComponent<RectTransform>().localPosition = new Vector3(2.4f, -2, 6.5f); break;
                // }
                boxPref.GetComponentInChildren<Text>().text = list.lv.ToString();
                boxPref.GetComponentInChildren<Text>().transform.SetAsLastSibling();
            }
        });
    }
}

//* -----------------------------------------------------------------------------------
//* スクロールビュー イベント
//* -----------------------------------------------------------------------------------
public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler//, IEndDragHandler
{
    //* OutSide
    public HomeEffectManager em;
    public ScrollRect scrollRect;
    public HomeManager hm;
    public Transform mainCanvasTf;

    // float rectWidth;
    [SerializeField] float curIdxBasePosX;    public float CurIdxBasePosX {get => curIdxBasePosX; set => curIdxBasePosX = value;}
    [SerializeField] int befIdx;
    [SerializeField] int curIdx;     public int CurIdx {get => curIdx; set => curIdx = value;}

    [Header("SCROLL SPEED")][Header("__________________________")]
    [SerializeField] float scrollStopSpeed;
    float scrollSpeed;
    [SerializeField] float scrollBefFramePosX;

    [Header("【 UI 】")][Header("__________________________")]
    public RectTransform uiGroup;   public RectTransform UIGroup {get => uiGroup;}
    public SpriteRenderer boxSprRdr;    public SpriteRenderer BoxSprRdr {get => boxSprRdr; set => boxSprRdr = value;}
    [FormerlySerializedAs("titleTxt")] public Text titleTxt;   public Text TitleTxt {get => titleTxt; set => titleTxt = value;}
    public Text rankTxt;    public Text RankTxt {get => rankTxt; set => rankTxt = value;}
    public Text nameTxt;    public Text NameTxt {get => nameTxt; set => nameTxt = value;}
    public Image downArrowImg;  public Image DownArrowImg {get => downArrowImg; set => downArrowImg = value;}
    public bool isScrolling; 

    void Start(){
        scrollRect = GetComponent<ScrollRect>();
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        mainCanvasTf = hm.mainCanvas.transform;
        
        if(this.gameObject.name != $"ScrollView_{DM.PANEL.Skill.ToString()}" 
            && this.gameObject.name != $"ScrollView_{DM.PANEL.CashShop.ToString()}"
            && this.gameObject.name != $"ScrollView_{DM.PANEL.PsvInfo.ToString()}"
            && this.gameObject.name != $"ScrollView_{DM.PANEL.Upgrade.ToString()}"){
            BoxSprRdr = UIGroup.GetChild(0).GetComponent<SpriteRenderer>();
            RankTxt = UIGroup.GetChild(1).GetComponent<Text>();
            NameTxt = UIGroup.GetChild(2).GetComponent<Text>();
            DownArrowImg = UIGroup.GetChild(3).GetComponent<Image>();
        }

        //* 初期化
        // Set Current Index
        CurIdx = (this.gameObject.name.Contains(DM.PANEL.Chara.ToString()))? DM.ins.personalData.SelectCharaIdx
            :(this.gameObject.name.Contains(DM.PANEL.Bat.ToString()))? DM.ins.personalData.SelectBatIdx
            :(this.gameObject.name.Contains(DM.PANEL.Skill.ToString()))? DM.ins.personalData.SelectSkillIdx
            :(this.gameObject.name.Contains(DM.PANEL.CashShop.ToString()))? DM.ins.personalData.SelectSkillIdx
            :(this.gameObject.name.Contains(DM.PANEL.Upgrade.ToString()))? DM.ins.personalData.SelectSkillIdx
            :0;

        // Set Scroll PosX
        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        CurIdxBasePosX = -((CurIdx+1) * width);

        // Set Rank Color & Text & OutLine
        updateModelTypeItemInfo();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isScrolling = true;
    }

    //* Update()
    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        if(gameObject.name == "ItemPassivePanel(Clone)") return;
        // if(!isScrolling) return;
        // if(scrollSpeed > 4) isScrolling = true;

        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        float offset = -(width + width/2);

        float curPosX = pos.anchoredPosition.x - offset;

        var prefs = (DM.ins.SelectItemType == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ItemPrefs : DM.ins.scrollviews[(int)DM.PANEL.Bat].ItemPrefs;
        float max = width * prefs.Length - width;
        CurIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        CurIdxBasePosX = -((CurIdx+1) * width);

        string type = DM.ins.SelectItemType;
        RectTransform contentTf = (type == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf: DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf;

        //* Scroll Speed
        scrollSpeed = (int)Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", CurIdx=" + CurIdx + ", scrollSpeed=" + scrollSpeed);
        Debug.Log($"getScrollViewPos:: CurIdxBasePosX={CurIdxBasePosX}, contentTf.anchoredPosition.x= {contentTf.anchoredPosition.x}");


        //* 目に見えるObjectのみ表示。(最適化)
        if(befIdx !=curIdx){
            Debug.Log($"befIdx({befIdx}) != curIdx({curIdx})");
            befIdx = curIdx;

            //* Get ContentTf;

            //* 全てのアイテム 非表示
            // displayAll3DItems(contentTf, false);
            //* 現在 アイテムと回りのみ表示
            // displayVisible3DItems(contentTf);
        }

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 2){// && isScrolling){
            // scrollSpeed = 0;
            // isScrolling = false;
            updateModelTypeItemInfo();
            //* Set PosX
            contentTf.anchoredPosition = new Vector2(CurIdxBasePosX, -500);
        }

        
        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    public void updateModelTypeItemInfo(){ //* CharaとBat専用。
        //* Get ContentTf;
        string type = DM.ins.SelectItemType;
        var contentTf = (type == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf : DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf;

        if(BoxSprRdr == null) return; // Error) Object Null
        if(type != DM.PANEL.Chara.ToString() && type != DM.PANEL.Bat.ToString()) return; //* (BUG-1) Upgrade-PANELも入ってきて、再ロードした場合 Out Of Indexバグの発生すること対応。

        //* Set PosX
        // contentTf.anchoredPosition = new Vector2(CurIdxBasePosX, -500);

        //* 全てのアイテム 表示
        // displayAll3DItems(contentTf, true);
        
        ItemInfo curItem = getCurItem();
        Debug.Log($"<color=yellow>updateItemInfo:: type={type}, curItem= {curItem.name}</color>");
        
        //* Show Rank Text
        RankTxt.text = curItem.Rank.ToString();

        //* Is Buy(UnLock)? Set Price or CheckMark
        drawCheckBtnUI();
        if(!curItem.IsLock)
            setCheckIconColorAndOutline();
        
        //* Set Name
        // string name = curItem.name.Split('_')[1];
        // int idx = LANG.CharaList.FindIndex(list => name == list[(int)LANG.TP.EN]);
        // NameTxt.text = LANG.CharaList[idx][(int)DM.ins.Language]; //name;
        Debug.Log("KOKOKO?!");
        
        NameTxt.text = LANG.getTxtList(curItem.name)[LANG.NAME]; //* (BUG-1) 対応

        //* Set Rank UI Box Color
        Color color = Color.white;
        var boxGlowEf = BoxSprRdr.GetComponent<SpriteGlowEffect>();
        float brightness = 0;
        int outline = 2;
        switch(curItem.Rank){
            case DM.RANK.GENERAL : color = Color.white; brightness=1; outline=2;   break;
            case DM.RANK.RARE : color = Color.blue; brightness=8; outline=2;   break;
            case DM.RANK.UNIQUE : color = Color.red; brightness=8.5f; outline=5; break;
            case DM.RANK.LEGEND : color = Color.magenta; brightness=9; outline=8; break;
            case DM.RANK.GOD : color = Color.yellow; brightness=10; outline=5; break;
        }
        DownArrowImg.color = color;
        rankTxt.color = color;
        BoxSprRdr.color = color;
        boxGlowEf.GlowColor = color;
        boxGlowEf.GlowBrightness = brightness;
        boxGlowEf.OutlineWidth = outline;
    }
    private void displayAll3DItems(RectTransform contentTf, bool isActive){
        for(int i=0; i<contentTf.childCount; i++){
            Transform modelParentTf = contentTf.GetChild(i);
            // Debug.Log("modelParentTf=>" + modelParentTf.GetChild(0).name + ", " + modelParentTf.GetChild(1).name);
            modelParentTf.GetChild(0).gameObject.SetActive(isActive);
            modelParentTf.GetChild(1).gameObject.SetActive(isActive);
        }
    }
    private void displayVisible3DItems(RectTransform contentTf, int range = 1){
        for(int i=0; i<contentTf.childCount; i++){
            Transform modelParentTf = contentTf.GetChild(i);
            for(int j = -range; j <= range; j++){
                if(i == curIdx + j){
                    // Debug.Log($"contentTf.GetChild({i}).name= {contentTf.GetChild(i).name}");
                    modelParentTf.GetChild(0).gameObject.SetActive(true);
                    modelParentTf.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }

    public void drawCheckBtnUI(){
        var curItem = getCurItem();
        Debug.Log($"drawChoiceBtnUI():: curItem.name= {curItem.name}, .IsLock= {curItem.IsLock}, .price= {curItem.price.getValue()}");

        //* Set PriceType Icon Sprite
        switch(curItem.price.Type){
            case Price.TP.COIN: hm.priceTypeIconImg.sprite = hm.CoinSpr; break;
            case Price.TP.DIAMOND: hm.priceTypeIconImg.sprite = hm.DiamondSpr; break;
            case Price.TP.CASH: hm.priceTypeIconImg.sprite = hm.CashSpr; break;
        }

        //* CashShop & Upgrade Panel
        if(DM.ins.SelectItemType == DM.PANEL.CashShop.ToString()){
            setPriceUI(curItem.price.getValue().ToString());

            //* 一回限り商品
            if(curItem.name.Contains(DM.NAME.RemoveAD.ToString()) && curItem.IsLock){
                hm.checkBtn.interactable = false;
                hm.priceTxt.text = "done";
            }
            return;
        }
        else if(DM.ins.SelectItemType == DM.PANEL.Upgrade.ToString()){
            UpgradeDt upgradeDt = DM.ins.personalData.Upgrade.Arr[CurIdx];
            string priceTxt = (upgradeDt.Lv == upgradeDt.MaxLv)? "MAX" : curItem.price.getValue().ToString();
            setPriceUI(priceTxt);
            return;
        }
        else if(DM.ins.SelectItemType == DM.PANEL.Skill.ToString()){
            UpgradeDt atvSkillUpgradeDt = DM.ins.personalData.AtvSkillUpgrade.Arr[CurIdx];
            string priceTxt = (atvSkillUpgradeDt.Lv == atvSkillUpgradeDt.MaxLv)? "MAX" : (curItem.price.getValue()).ToString();

            int skillIdx = (hm.selectedSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx : DM.ins.personalData.SelectSkill2Idx;
            int curItemIdx = Array.IndexOf(DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf.GetComponentsInChildren<ItemInfo>(), curItem);
            if(skillIdx == curItemIdx){
                setPriceUI(priceTxt);
            }else{
                //* Model Pattern Panel
                if(curItem.IsLock){//* 💲表示
                    setPriceUI(curItem.price.getValue().ToString());
                }else{//* ✅表示
                    hm.checkMarkImg.gameObject.SetActive(true);
                    hm.priceTxt.gameObject.SetActive(false);
                    hm.priceTypeIconImg.gameObject.SetActive(false);
                }
            }
            
            return;
        }

        //* Model Pattern Panel
        if(curItem.IsLock){//* 💲表示
            setPriceUI(curItem.price.getValue().ToString());
        }else{//* ✅表示
            hm.checkMarkImg.gameObject.SetActive(true);
            hm.priceTxt.gameObject.SetActive(false);
            hm.priceTypeIconImg.gameObject.SetActive(false);
        }
    }
    private void setPriceUI(string price){
        hm.checkBtn.interactable = true;
        hm.checkMarkImg.gameObject.SetActive(false);
        hm.priceTxt.gameObject.SetActive(true);
        hm.priceTypeIconImg.gameObject.SetActive((price == "MAX")? false : true);
        hm.priceTxt.text = price; // -> curItem.price.getValue().ToString();
    }

    public void setCurSelectedItem(int typeIdx){
        switch(typeIdx){
            case (int)DM.PANEL.Chara: 
            case (int)DM.PANEL.Bat: 
                setScrollViewAnchoredPosX(typeIdx);
                break;
            case (int)DM.PANEL.Skill: 
                drawSkillPanelOutline(typeIdx);
                break;
            case (int)DM.PANEL.CashShop: 
                hm.checkMarkImg.gameObject.SetActive(true);
                hm.priceTxt.gameObject.SetActive(false);
                hm.priceTypeIconImg.gameObject.SetActive(false);
                break;
        }
    }

    private void setScrollViewAnchoredPosX(int typeIdx){
        int index = (typeIdx==(int)DM.PANEL.Chara)? DM.ins.personalData.SelectCharaIdx : DM.ins.personalData.SelectBatIdx;
        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        float saveModelPosX = -Mathf.Abs(Mathf.FloorToInt(width * (index+1)));
        DM.ins.scrollviews[typeIdx].ContentTf.anchoredPosition = new Vector2(saveModelPosX, -500);
    }

    private void drawSkillPanelOutline(int typeIdx){
            //* 初期化
            var content = DM.ins.scrollviews[typeIdx].ContentTf;
            for(int i=0; i<content.childCount; i++){
                content.GetChild(i).GetComponent<NicerOutline>().enabled = false;
            }

            //* Set Outline
            int skillldx = (hm.selectedSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx : DM.ins.personalData.SelectSkill2Idx;
            var selectedSkillTf = content.GetChild(skillldx);
            Debug.Log("setCurSelectedItem:: selectedSkillTf= " + selectedSkillTf);
            selectedSkillTf.GetComponent<NicerOutline>().enabled = true;
    }

    public void exceptAlreadySelectedAnotherSkill(int selectedSkillBtnIdx, Button[] skillBtns){
        //* Skill2 Unlock?
        if(DM.ins.personalData.IsUnlock2ndSkill && DM.ins.SelectItemType == DM.PANEL.Skill.ToString()){
            //* 除外するスキル名
            int anotherIdx = (selectedSkillBtnIdx == 0)? 1 : 0;
            string exceptSkillName = skillBtns[anotherIdx].transform.GetChild(0).GetComponent<Image>().sprite.name;

            //* 初期化：スクロールにあるスキル目録
            var contentTf = DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf;
            for(int i=0; i<contentTf.childCount; i++){
                Debug.Log("exceptAlreadySelectedAnotherSkill():: 初期化 " + contentTf.GetChild(i).name);
                contentTf.GetChild(i).GetComponent<ItemInfo>().IsChecked = false;//gameObject.SetActive(true);
                contentTf.GetChild(i).GetComponent<ItemInfo>().IsCheckedImgObj.SetActive(false);
            }

            //* 除外するスキルObj 表示
            var childs = contentTf.GetComponentsInChildren<Transform>();
            var skillTfs = Array.FindAll(childs, tf => tf.name.Contains("Skill_"));
            Array.ForEach(skillTfs, tf => {
                string objName = tf.name.Split('_')[1];
                Debug.LogFormat("{0}.Contain({1}) => {2}", exceptSkillName, objName, exceptSkillName.Contains(objName));
                if(exceptSkillName.Contains(objName)){
                    tf.GetComponent<ItemInfo>().IsChecked = true;
                    tf.GetComponent<ItemInfo>().IsCheckedImgObj.SetActive(true);
                    var txt = tf.GetComponent<ItemInfo>().IsCheckedImgObj.transform.GetChild(0).GetComponent<Text>();
                    txt.text = (hm.selectedSkillBtnIdx == 0)? "2nd Skill" : "1st Skill";
                }
            });
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickCheckBtn(string type){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        if(DM.ins.SelectItemType != type) return;
        
        Debug.Log($"<b>onClickCheckBtn(type={type}):: CurIdx= {CurIdx}</b>");
        var curItem = getCurItem();

        if(type == DM.PANEL.Skill.ToString() && curItem.IsChecked){
            Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.MsgAlreadyRegistedSkill.ToString()));
            return;
        }
        
        //* (BUG) 買わないのにロードしたらChara選択されるバグ防止。
        int befIdx = DM.ins.personalData.getSelectIdx(type);
        DM.ins.personalData.setSelectIdx(CurIdx);

        #region PARCHASE
        Debug.Log("PARCHASE==> curItem= " + curItem.name + ", IsLock=> " + curItem.IsLock);
        DM.PANEL enumType = DM.ins.getCurPanelType2Enum(type);
        switch(enumType){
            case DM.PANEL.Chara :
            case DM.PANEL.Bat :
            case DM.PANEL.CashShop :
            case DM.PANEL.Upgrade :
                if(curItem.price.Type == Price.TP.COIN){
                    DM.ins.personalData.Coin = purchaseItem(DM.ins.personalData.Coin, curItem, befIdx);
                }
                else if(curItem.price.Type == Price.TP.DIAMOND){
                    DM.ins.personalData.Diamond = purchaseItem(DM.ins.personalData.Diamond, curItem, befIdx);
                }
                else if(curItem.price.Type == Price.TP.CASH){
                    const int justEnter = 9999999;
                    purchaseItem(justEnter, curItem, befIdx);
                }
                break;
            case DM.PANEL.Skill : //* (BUG-33) onClickCheckBtn::checkAtvSkillItemSelectionIsFirstTime CHARAとBATの場合購入したら出るバグがあるため、caseによって処理を分ける。
                if(curItem.price.Type == Price.TP.COIN){
                    if(checkAtvSkillItemSelectionIsFirstTime(curItem))
                        DM.ins.personalData.Coin = purchaseItem(DM.ins.personalData.Coin, curItem, befIdx);
                }
                else if(curItem.price.Type == Price.TP.DIAMOND){
                    if(checkAtvSkillItemSelectionIsFirstTime(curItem))
                        DM.ins.personalData.Diamond = purchaseItem(DM.ins.personalData.Diamond, curItem, befIdx);
                }
                break;
        }

        //* 購入可能なPANELのUI処理。
        if(DM.ins.SelectItemType == DM.PANEL.Skill.ToString()
            || DM.ins.SelectItemType == DM.PANEL.CashShop.ToString()
            || DM.ins.SelectItemType == DM.PANEL.Upgrade.ToString()){
            onClickItem(curItem.gameObject);
        }
        #endregion
    }

    private bool checkAtvSkillItemSelectionIsFirstTime(ItemInfo curItem){
        //* 始めてスキル選択が切り替えたら、購入処理はしない。
        Debug.Log(" outline= " + curItem.GetComponent<NicerOutline>().enabled);
        return (curItem.GetComponent<NicerOutline>().enabled);
    }

    public void onClickItem(GameObject ins){
        DM.PANEL type = ins.name.Contains(DM.PANEL.Skill.ToString())? DM.PANEL.Skill
            : ins.name.Contains(DM.PANEL.CashShop.ToString())? DM.PANEL.CashShop
            : ins.name.Contains(DM.PANEL.Upgrade.ToString())? DM.PANEL.Upgrade
            : DM.PANEL.NULL;

        if(type == DM.PANEL.NULL) return;

        //* Current Index
        var btns = DM.ins.scrollviews[(int)type].ContentTf.GetComponentsInChildren<Button>();
        CurIdx = Array.FindIndex(btns, btn => btn.name == ins.name);

        drawCheckBtnUI();
        setCheckIconColorAndOutline();

        var sprite = btns[CurIdx].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        Debug.Log($"<color=yellow>onClickItem()</color>:: CurIdx= {CurIdx}, ins.name= {ins.name}, sprite= {sprite.name}");
    }
    public ItemInfo getCurItem(){
        var contents = getItemArr();
        int lastIdx = contents.Length - 1;
        //* (BUG) スクロールアイテムを最後を超えると、Out of Indexエラー発生。Clampで最大値LastIndexに固定。
        return contents[Mathf.Clamp(CurIdx, 0, lastIdx)];
    }

    static public List<int> setUpgradePriceCalc(UpgradeDt upgradeDt){
        List<int> priceList = null;
        var name = DM.ins.convertUpgradeStr2Enum(upgradeDt.name);
            switch(name){
                case DM.UPGRADE.Dmg:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_DMG.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_DMG.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_DMG.GradualUpValue);
                    break;
                case DM.UPGRADE.BallSpeed:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_BALLSPD.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_BALLSPD.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_BALLSPD.GradualUpValue);
                    break;
                case DM.UPGRADE.BossDamage:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_BOSSDMG.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_BOSSDMG.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_BOSSDMG.GradualUpValue);
                    break;
                case DM.UPGRADE.CoinBonus:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_COINBONUS.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_COINBONUS.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_COINBONUS.GradualUpValue);
                    break;
                case DM.UPGRADE.Critical:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_CRIT.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_CRIT.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_CRIT.GradualUpValue);
                    break;
                case DM.UPGRADE.CriticalDamage:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_CRITDMG.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_CRITDMG.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_CRITDMG.GradualUpValue);
                    break;
                case DM.UPGRADE.Defence:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_DEFENCE.Start, 
                        upgradeDt.MaxLv, 
                        d: LM._.UPGRADE_DEFENCE.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_DEFENCE.GradualUpValue);
                    break;
            }
            return priceList;
    }
    static public List<int> setAtvSkillUpgradePriceCalc(UpgradeDt atvSkUpgDt){
        List<int> priceList = null;
            DM.ATV name = DM.ins.convertAtvSkillStr2Enum(atvSkUpgDt.name);
            switch(name){
                case DM.ATV.ThunderShot:
                case DM.ATV.FireBall:
                case DM.ATV.ColorBall:
                case DM.ATV.PoisonSmoke:
                case DM.ATV.IceWave:
                    priceList = Util._.calcArithmeticProgressionList(
                        start: LM._.UPGRADE_ATVSKILL.Start, 
                        atvSkUpgDt.MaxLv, 
                        d: LM._.UPGRADE_ATVSKILL.CommonDiffrence, 
                        gradualUpValue: LM._.UPGRADE_ATVSKILL.GradualUpValue);
                    break;
            }
            priceList.ForEach(price => Debug.Log("setAtvSkillUpgradePriceCalc:: priceList:: price= " + price));
            
            return priceList;
    }
    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private int purchaseItem(int goods, ItemInfo curItem, int befIdx){
        Debug.Log($"<color=yellow>ScrollViewEvent::purchaseItem():: curItem= {curItem.name}, CurIdx= {CurIdx}</color>");
        Price.TP goodsType = curItem.price.Type;
        int price = curItem.price.getValue();
        if(goods >= price){
            if(curItem.IsLock){
                SM.ins.sfxPlay(SM.SFX.PurchaseSuccess.ToString());
                Debug.Log("purchaseItem:: curItem.transform.GetChild(0)=" + curItem.transform.GetChild(0).name);
                goods -= price;

                GameObject imgObj = null;
                switch(DM.ins.SelectItemType){
                    case "Skill":
                        //* (BUG-8) Home:: Bat.getChild(0).getChild(0)-> Null ---> getChild(0)が正しい。
                        imgObj = curItem.transform.GetChild(0).gameObject;
                        break;
                    case "Bat":
                        imgObj = curItem.transform.GetChild(curItem.transform.childCount - 1).gameObject;
                        break;
                }
                em.createItemBuyEF(this, imgObj, DM.ins.SelectItemType);
                em.createCongratuBlastRainbowEF(mainCanvasTf);
                curItem.IsLock = false; //* 解禁
                curItem.checkLockedModel();
                DM.ins.personalData.setUnLockCurList(CurIdx);
                drawCheckBtnUI();
            }
            else{
                if(DM.ins.SelectItemType == DM.PANEL.Upgrade.ToString()){
                    UpgradeDt upgradeDt = DM.ins.personalData.Upgrade.Arr[CurIdx];
                    Debug.Log("purchaseItem:: upgradeDt.name= " + upgradeDt.name);
                    if(upgradeDt.Lv < upgradeDt.MaxLv){
                        goods -= price;

                        //* Set Lv
                        upgradeDt.setLvUp();
                        curItem.setUI(upgradeDt);

                        //* Sound
                        if(upgradeDt.Lv < upgradeDt.MaxLv)
                            SM.ins.sfxPlay(SM.SFX.Upgrade.ToString());

                        //* Set Price
                        List<int> priceList = setUpgradePriceCalc(upgradeDt);
                        curItem.price.setValue(priceList[upgradeDt.Lv]);

                        em.createUpgradeItemEF(curItem.UpgradeValueTxt.transform);

                        //* Achivement
                        AcvUpgradeCnt.addUpgradeCnt();
                    }
                }
                else if(DM.ins.SelectItemType == DM.PANEL.Skill.ToString()){
                    UpgradeDt atvSkUpgDt = DM.ins.personalData.AtvSkillUpgrade.Arr[CurIdx];
                    Debug.Log("purchaseItem:: activeSkillUpgradeDt.name= " + atvSkUpgDt.name);
                    if(atvSkUpgDt.Lv < atvSkUpgDt.MaxLv){
                        goods -= price;

                        //* Set Lv
                        atvSkUpgDt.setLvUp();
                        curItem.setUI(atvSkUpgDt);

                        //* Sound
                        if(atvSkUpgDt.Lv < atvSkUpgDt.MaxLv)
                            SM.ins.sfxPlay(SM.SFX.Upgrade.ToString());

                        //* Set Price
                        List<int> priceList = setAtvSkillUpgradePriceCalc(atvSkUpgDt);
                        curItem.price.setValue(priceList[atvSkUpgDt.Lv]);

                        em.createUpgradeItemEF(curItem.UpgradeValueTxt.transform);
                    }
                }
                else if(DM.ins.SelectItemType == DM.PANEL.CashShop.ToString()){
                    goods -= price;
                    string itemName = curItem.name.Split('_')[1];
                    Debug.Log($"ScrollViewEvent::purchaseItem():: itemName= {itemName}");
                    if(itemName.Contains(DM.NAME.Coin.ToString())){
                        int reward = int.Parse(itemName.Split('n')[1]);
                        hm.displayShowRewardPanel(coin: reward);
                        DM.ins.personalData.addCoin(reward); // DM.ins.personalData.Coin += reward;
                    }
                    else if(itemName.Contains(DM.NAME.Diamond.ToString())){
                        bool success = DM.ins.reqAppPayment();
                        if(success){
                            int reward = int.Parse(itemName.Split('d')[1]);
                            hm.displayShowRewardPanel(coin: 0, diamond: reward);
                            DM.ins.personalData.addDiamond(reward);
                        }
                    }
                    else if(itemName.Contains(DM.NAME.RemoveAD.ToString())) {
                        bool success = DM.ins.reqAppPayment();
                        if(success){
                            hm.displayShowRewardPanel(coin: 0, diamond: 0, rouletteTicket: 0, removeAD: true);
                            DM.ins.personalData.IsRemoveAD = true;
                            DM.ins.setUIRemoveAD();
                        }
                    }
                }
                drawCheckBtnUI();
            }
        }
        else {
            SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
            //* (BUG-20)MAX-LVなら、「財貨 不足」ダイアログ表示しない。
            if(DM.ins.SelectItemType == DM.PANEL.Upgrade.ToString()){
                UpgradeDt upgradeDt = DM.ins.personalData.Upgrade.Arr[CurIdx];
                if(upgradeDt.Lv == upgradeDt.MaxLv) return goods;
            }
            //* 「財貨 不足」ダイアログ表示。
            DM.ins.personalData.setSelectIdx(befIdx);
            string goodsTypeStr = (goodsType == Price.TP.COIN)? LANG.getTxt(LANG.TXT.Coin.ToString()) : LANG.getTxt(LANG.TXT.Diamond.ToString());
            Util._.displayNoticeMsgDialog(goodsTypeStr + " " + LANG.getTxt(LANG.TXT.NotEnough.ToString()));
        }
        return goods;
    }
    private ItemInfo[] getItemArr(){
        var contentTf = (DM.ins.SelectItemType == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.Bat.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.Skill.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.CashShop.ToString())? DM.ins.scrollviews[(int)DM.PANEL.CashShop].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.Upgrade.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Upgrade].ContentTf
        : null;
        Debug.Log("getItemArr():: contentTf= " + contentTf);
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        return items;
    }
    private void setCheckIconColorAndOutline(){// (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        string type = DM.ins.SelectItemType;
        var items = getItemArr();
        var curItem = getCurItem();

        int skillIdx = (type == DM.PANEL.Chara.ToString())? DM.ins.personalData.SelectCharaIdx
            :(type == DM.PANEL.Bat.ToString())? DM.ins.personalData.SelectBatIdx
            :(type == DM.PANEL.Skill.ToString())?
                (hm.selectedSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx
                    :DM.ins.personalData.SelectSkill2Idx : -1;

        if(skillIdx==-1) return;
        
        if(skillIdx == CurIdx){
            Debug.LogFormat("setCheckIconColorAndOutline():: skillIdx({0}) == CurIdx({1}) ? -> {2}, curItem= {3}", skillIdx, CurIdx, skillIdx == CurIdx, curItem);
            hm.checkMarkImg.color = Color.green;
            Array.ForEach(items, item => activeOutline(item, false));
            activeOutline(curItem, true);
            // setSelectSkillImgAtHome();
        }
        else{
            hm.checkMarkImg.color = Color.gray;
            activeOutline(curItem, false);
        }
    }

    private void activeOutline(ItemInfo item, bool isActive){
        // if(item.Outline3D == null) return;
        // Debug.LogFormat("activeOutline():: itemType= {0}, isActive= {1}", itemType, isActive);
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        switch(itemType){
            case DM.PANEL.Chara:
            case DM.PANEL.Bat:
                if(item.Outline3D) {
                    item.Outline3D.enabled = isActive; 
                    item.RankAuraEF.SetActive(!isActive);
                    
                    //* Rank AuraEF 色 設定。
                    var particle = item.RankAuraEF.GetComponent<ParticleSystem>().main;
                    switch(item.Rank){
                        case DM.RANK.GENERAL: particle.startColor = DM.ins.rankGeneralClr; break;
                        case DM.RANK.RARE: particle.startColor = DM.ins.rankRareClr; break;
                        case DM.RANK.UNIQUE: particle.startColor = DM.ins.rankUniqueClr; break;
                        case DM.RANK.LEGEND: particle.startColor = DM.ins.rankLegendClr; break;
                        case DM.RANK.GOD: particle.startColor = DM.ins.rankGodClr; break;
                    }
                }
                break;
            case DM.PANEL.Skill:
                if(item.Outline2D) item.Outline2D.enabled = isActive;
                break;
        }
    }
}
