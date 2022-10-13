using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI.Extensions;

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
        Debug.LogFormat("createObject:: {0}, {1}, {2}, type= {3}",modelParentPref, itemPassivePanel, itemSkillBoxPref, this.type);
        //* Prefabs 生成
        var itemType = DM.ins.getCurItemType2Enum(this.type);
        Array.ForEach(itemPrefs, itemPf=>{
            //* 生成
            RectTransform modelContentPf = null;
            RectTransform psvPanel = null;
            GameObject model = null;

            switch(itemType){
                case DM.PANEL.Chara : 
                case DM.PANEL.Bat :
                    //* Rect Transform -> UI
                    modelContentPf = GameObject.Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf);
                    modelContentPf.anchoredPosition3D = Vector3.zero; //* 親(ModelContentPf)ずれること対応。

                    //* Transform -> GameObject
                    model = GameObject.Instantiate(itemPf, Vector3.zero, Quaternion.identity, modelContentPf);
                    model.transform.localPosition = Vector3.zero;

                    //* Item Passive UI Ready
                    psvPanel = GameObject.Instantiate(itemPassivePanel, itemPassivePanel.localPosition, itemPassivePanel.localRotation, modelContentPf);
                    psvPanel.anchoredPosition3D = new Vector3(0,-2,0);
                    model.GetComponent<ItemInfo>().ItemPassive.setImgPrefs(DM.ins.personalData.ItemPassive);
                    break;
                case DM.PANEL.Skill :
                case DM.PANEL.CashShop :
                case DM.PANEL.PsvInfo :
                {
                    model = GameObject.Instantiate(itemPf, Vector3.zero, Quaternion.identity, contentTf);
                    model.transform.localPosition = Vector3.zero;
                    // Debug.Log("<color=yellow>【"+ this.type + "】</color> model.name= " + model.name + ", personalData.Lang= " + DM.ins.personalData.Lang);
                    break;
                }
            }

            if(!model) return;
            
            var itemPassiveList = model.GetComponent<ItemInfo>().ItemPassive.Arr;
            //* 調整
            switch(itemType){
                case DM.PANEL.Chara : 
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case DM.PANEL.Bat :
                    Vector3 ZOOM_IN_POS = new Vector3(1.61f, 0.75f, 4.43f); //* ベットは小さいから少しズームイン。
                    model.transform.localPosition = ZOOM_IN_POS;
                    model.transform.localRotation = Quaternion.Euler(model.transform.localRotation.x, model.transform.localRotation.y, -45);
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case DM.PANEL.Skill : 
                case DM.PANEL.CashShop : 
                    model.transform.localPosition = new Vector3(0,0,0); //* posZがずれるから、調整
                    //! Add "OnClick()" EventListner
                    Button btn = model.GetComponent<Button>();
                    var svEvent = ScrollRect.GetComponent<ScrollViewEvent>();
                    btn.onClick.AddListener(delegate{svEvent.onClickSkillOrCashShopItemPanel(model);});
                    break;
        }
            // Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            model.name = itemPf.name;//名前上書き：しないと後ろに(clone)が残る。
        });

        pushItemLanguageList(itemType.ToString());
    }

    private void pushItemLanguageList(string itemType){
        //* ItemCreateが終わったら、一回活性化して、ItemInfoに書いた言語情報をLanguageのInfoリストへ入れる。
        var hm = GameObject.Find(DM.NAME.HomeManager.ToString()).GetComponent<HomeManager>();
        hm.onClickBtnGoToPanel(itemType);
    }
    public void setLanguage(){ //* SkillとCashShopのみ。
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
            }

            if(txtObjs == null && strList.Count == 0) return;

            //* Join Strings
            var tempStr = string.Join("_", strList);
            var languageList = LANG.getTxtList(tempStr);
            //* Set Language
            for(int j = 0; j < txtObjs.Length; j++) txtObjs[j].text = languageList[j];
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
        Debug.Log("--------------------------------------------");
    }
}

//* -----------------------------------------------------------------------------------
//* スクロールビュー イベント
//* -----------------------------------------------------------------------------------
public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //* OutSide
    public HomeEffectManager em;
    public ScrollRect scrollRect;
    public HomeManager hm;

    // float rectWidth;
    [SerializeField] float curIdxBasePosX;    public float CurIdxBasePosX {get => curIdxBasePosX; set => curIdxBasePosX = value;}
    [SerializeField] int curIdx;     public int CurIdx {get => curIdx; set => curIdx = value;}

    [Header("SCROLL SPEED")]
    [SerializeField] float scrollStopSpeed;
    float scrollSpeed;
    float scrollBefFramePosX;

    [Header("【 UI 】")]
    public RectTransform uiGroup;   public RectTransform UIGroup {get => uiGroup;}
    public SpriteRenderer boxSprRdr;    public SpriteRenderer BoxSprRdr {get => boxSprRdr; set => boxSprRdr = value;}
    public Text rankTxt;    public Text RankTxt {get => rankTxt; set => rankTxt = value;}
    public Text nameTxt;    public Text NameTxt {get => nameTxt; set => nameTxt = value;}
    public Image downArrowImg;  public Image DownArrowImg {get => downArrowImg; set => downArrowImg = value;}

    [Header("BUY OR CHECK BTN")]
    public Image checkMarkImg;
    public Text priceTxt;
    public Image priceTypeIconImg;

    void Start(){
        scrollRect = GetComponent<ScrollRect>();
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        
        if(this.gameObject.name != $"ScrollView_{DM.PANEL.Skill.ToString()}" 
            && this.gameObject.name != $"ScrollView_{DM.PANEL.CashShop.ToString()}"
            && this.gameObject.name != $"ScrollView_{DM.PANEL.PsvInfo.ToString()}"){
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
            :0;

        // Set Scroll PosX
        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        CurIdxBasePosX = -((CurIdx+1) * width);

        // Set Rank Color & Text & OutLine
        updateItemInfo();
    }

    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData){
        // Debug.Log("<color=white> ScrollViewEvent:: Drag Begin </color>");
    }

    public void OnEndDrag(PointerEventData eventData){
        //* (BUG) スクロールが先端と末端だったら、イベント起動しない。
        // int posX = (int)dm.ScrollContentTf.anchoredPosition.x;
        // Debug.Log("<color=white> ScrollViewEvent:: Drag End:: PosX=" + posX + ", CurIdx=" + CurIdx + "Charas.Len=" + (dm.CharaPfs.Length - 1) + "</color>");

        // if(CurIdx == 0 || CurIdx == dm.CharaPfs.Length-1){
        //     setScrollStopCharaInfo();
        // }
    }

    //* Update()
    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        if(gameObject.name == "ItemPassivePanel(Clone)") return;

        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        var prefs = (DM.ins.SelectItemType == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ItemPrefs : DM.ins.scrollviews[(int)DM.PANEL.Bat].ItemPrefs;
        float max = width * prefs.Length - width;
        CurIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        CurIdxBasePosX = -((CurIdx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);
        // Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", CurIdx=" + CurIdx + " (CurIdxBasePos=" + CurIdxBasePos + "), scrollSpeed=" + scrollSpeed);

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1)
            updateItemInfo();

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    public void updateItemInfo(){
        if(BoxSprRdr == null) return; // Error) Object Null

        string type = DM.ins.SelectItemType;
        var contentTf = (type == DM.PANEL.Chara.ToString())?
            DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf
            : DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf;
        //* Set PosX
        contentTf.anchoredPosition = new Vector2(CurIdxBasePosX, -500);
        
        var curItem = getCurItem();
        Debug.Log("<color>CurItem= " + curItem.name + "</color>");

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
            NameTxt.text = LANG.getTxtList(curItem.name)[LANG.NAME];

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

    public void drawCheckBtnUI(){
        var curItem = getCurItem();
        Debug.Log("drawChoiceBtnUI():: curItem.IsLock= " + curItem.IsLock);

        //* Set PriceType Icon Img Sprite
        if(curItem.price.Type == Price.TP.COIN){
            priceTypeIconImg.sprite = hm.CoinIconSprite;
        }else{
            priceTypeIconImg.sprite = hm.DiamondIconSprite;
        }

        if(DM.ins.SelectItemType == DM.PANEL.CashShop.ToString()){
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curItem.price.getValue().ToString();
            return;
        }

        if(curItem.IsLock){//* 💲表示
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curItem.price.getValue().ToString();
        }else{//* ✅表示
            checkMarkImg.gameObject.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    public void setCurSelectedItem(int typeIdx){
        switch(typeIdx){
            case (int)DM.PANEL.Chara: 
            case (int)DM.PANEL.Bat: 
                setScrollViewAnchoredPosX(typeIdx);
                break;
            case (int)DM.PANEL.Skill: 
                drawSkillPanelOutline();
                break;
            case (int)DM.PANEL.CashShop: 
                checkMarkImg.gameObject.SetActive(true);
                priceTxt.gameObject.SetActive(false);
                break;
        }
    }

    private void setScrollViewAnchoredPosX(int typeIdx){
        int index = (typeIdx==(int)DM.PANEL.Chara)? DM.ins.personalData.SelectCharaIdx : DM.ins.personalData.SelectBatIdx;
        float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
        float saveModelPosX = -Mathf.Abs(Mathf.FloorToInt(width * (index+1)));
        DM.ins.scrollviews[typeIdx].ContentTf.anchoredPosition = new Vector2(saveModelPosX, -500);
    }

    private void drawSkillPanelOutline(){
            //* 初期化
            var content = DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf;
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
        if(DM.ins.SelectItemType != type) return;
        var curItem = getCurItem();
        Debug.LogFormat("onClickCheckB  tn:: type= {0}, CurIdx= {1}, curItem= {2}, IsLock= {3}",type, CurIdx, curItem, curItem.IsLock);

        if(type == DM.PANEL.Skill.ToString() && curItem.IsChecked){
            hm.displayMessageDialog("This Skill is Already Registed");
            return;
        }
        
        //* (BUG) 買わないのにロードしたらChara選択されるバグ防止。
        int befIdx = DM.ins.personalData.getSelectIdx(type);
        DM.ins.personalData.setSelectIdx(CurIdx);

        //* Check
        if(curItem.IsLock){
            if(curItem.price.Type == Price.TP.COIN){ //DM.ins.personalData.Coin : DM.ins.personalData.Diamond;
                //* Buy
                if(DM.ins.personalData.Coin >= curItem.price.getValue()){
                    DM.ins.personalData.Coin -= curItem.price.getValue();
                    em.createItemBuyEF();
                    curItem.IsLock = false;
                    curItem.setGrayMtIsLock();
                    DM.ins.personalData.setUnLockCurList(CurIdx);
                    drawCheckBtnUI();
                }
                else{//TODO Audio
                    DM.ins.personalData.setSelectIdx(befIdx);
                }
            }
            else if(curItem.price.Type == Price.TP.DIAMOND){
                //* Buy
                if(DM.ins.personalData.Diamond >= curItem.price.getValue()){
                    DM.ins.personalData.Diamond -= curItem.price.getValue();
                    em.createItemBuyEF();
                    curItem.IsLock = false;
                    curItem.setGrayMtIsLock();
                    DM.ins.personalData.setUnLockCurList(CurIdx);
                    drawCheckBtnUI();
                }
                else{//TODO Audio
                    DM.ins.personalData.setSelectIdx(befIdx);
                }
            }
        }

        //* Skill Panel UI Extra Update
        if(DM.ins.SelectItemType == DM.PANEL.Skill.ToString()){
            onClickSkillOrCashShopItemPanel(curItem.gameObject);
        }
        //* CashShop
        else if(DM.ins.SelectItemType == DM.PANEL.CashShop.ToString()){
            onClickSkillOrCashShopItemPanel(curItem.gameObject);
        }
    }

    public void onClickSkillOrCashShopItemPanel(GameObject ins){
        DM.PANEL type = ins.name.Contains(DM.PANEL.Skill.ToString())? DM.PANEL.Skill
            : ins.name.Contains(DM.PANEL.CashShop.ToString())? DM.PANEL.CashShop
            : DM.PANEL.NULL;

        if(type == DM.PANEL.NULL) return;

        //* Current Index
        var btns = DM.ins.scrollviews[(int)type].ContentTf.GetComponentsInChildren<Button>();
        CurIdx = Array.FindIndex(btns, btn => btn.name == ins.name);

        drawCheckBtnUI();
        setCheckIconColorAndOutline();

        var sprite = btns[CurIdx].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        Debug.Log("onClickSkillPanel():: CurIdx= " + CurIdx + ", ins.name= " + ins.name + ", sprite= " + sprite);
    }

    // public void onClickCashShopList(ItemInfo selectItemInfo){
    //     var btns = DM.ins.scrollviews[(int)DM.PANEL.CashShop].ContentTf.GetComponentsInChildren<Button>();
    //     CurIdx = Array.FindIndex(btns, btn => btn.name == selectItemInfo.name);
    //     Debug.Log("onClickCashShopList():: price= " + selectItemInfo.Price + ", CurIdx= " + CurIdx);

    //     //TODO PROCESS 
    // }

    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private ItemInfo getCurItem(){
        var contents = getItemArr();
        int lastIdx = contents.Length - 1;
        //* (BUG) スクロールアイテムを最後を超えると、Out of Indexエラー発生。Clampで最大値LastIndexに固定。
        return contents[Mathf.Clamp(CurIdx, 0, lastIdx)];
    }
    private ItemInfo[] getItemArr(){
        var contentTf = (DM.ins.SelectItemType == DM.PANEL.Chara.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.Bat.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Bat].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.Skill.ToString())? DM.ins.scrollviews[(int)DM.PANEL.Skill].ContentTf
        : (DM.ins.SelectItemType == DM.PANEL.CashShop.ToString())? DM.ins.scrollviews[(int)DM.PANEL.CashShop].ContentTf
        : DM.ins.scrollviews[(int)DM.PANEL.Chara].ContentTf;
        Debug.Log("getItemArr():: contentTf= " + contentTf);
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        return items;
    }
    private void setCheckIconColorAndOutline(){// (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        string type = DM.ins.SelectItemType;
        var items = getItemArr();
        var curItem = getCurItem();

        int selectItemIdx = (type == DM.PANEL.Chara.ToString())? DM.ins.personalData.SelectCharaIdx
            :(type == DM.PANEL.Bat.ToString())? DM.ins.personalData.SelectBatIdx
            :(type == DM.PANEL.Skill.ToString())?
                (hm.selectedSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx
                    :DM.ins.personalData.SelectSkill2Idx : -1;

        if(selectItemIdx==-1) return;
        
        if(selectItemIdx == CurIdx){
            Debug.LogFormat("setCheckIconColorAndOutline():: selectItemIdx({0}) == CurIdx({1}) ? -> {2}, curItem= {3}", selectItemIdx, CurIdx, selectItemIdx == CurIdx, curItem);
            checkMarkImg.color = Color.green;
            Array.ForEach(items, item => activeOutline(item, false));
            activeOutline(curItem, true);
            // setSelectSkillImgAtHome();
        }
        else{
            checkMarkImg.color = Color.gray;
            activeOutline(curItem, false);
        }
    }

    private void activeOutline(ItemInfo item, bool isActive){
        // if(item.Outline3D == null) return;
        // Debug.LogFormat("activeOutline():: itemType= {0}, isActive= {1}", itemType, isActive);
        var itemType = DM.ins.getCurItemType2Enum(DM.ins.SelectItemType);
        switch(itemType){
            case DM.PANEL.Chara:
            case DM.PANEL.Bat:
                if(item.Outline3D) item.Outline3D.enabled = isActive; 
                break;
            case DM.PANEL.Skill:
                if(item.Outline2D) item.Outline2D.enabled = isActive;
                break;
        }
    }
}
