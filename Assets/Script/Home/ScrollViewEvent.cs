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
        Debug.LogFormat("createObject:: {0}, {1}, {2}",modelParentPref, itemPassivePanel, itemSkillBoxPref);
        //* Prefabs 生成
        Array.ForEach(itemPrefs, obj=>{
            //* 生成
            Transform parentTf = null;
            GameObject model = null;
            Transform psvPanel = null;
            switch(this.type){
                case "Skill" :
                    model = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, contentTf);
                    break;
                case "Chara" : 
                case "Bat" :
                    parentTf = GameObject.Instantiate(modelParentPref, modelParentPref.localPosition, modelParentPref.localRotation, contentTf).transform;
                    model = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity, parentTf);

                    //* Item Passive UI Ready
                    psvPanel = GameObject.Instantiate(itemPassivePanel, itemPassivePanel.localPosition, itemPassivePanel.localRotation, parentTf).transform;
                    model.GetComponent<ItemInfo>().ItemPassive.setImgPrefs(DM.ins.personalData.ItemPassive);
                    break;
            }

            if(!model) return;
            var itemPassiveList = model.GetComponent<ItemInfo>().ItemPassive.Arr;
            //* 調整
            switch(this.type){
                case "Chara" : 
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case "Bat" :
                    parentTf.GetComponent<RectTransform>().localPosition = new Vector3(0,200,800); //* xとyは自動調整される。
                    model.transform.localPosition = new Vector3(model.transform.localPosition.x, 0.75f, model.transform.localPosition.z);
                    model.transform.localRotation = Quaternion.Euler(model.transform.localRotation.x, model.transform.localRotation.y, -45);
                    displayItemPassiveUI(type, itemPassiveList, itemSkillBoxPref, psvPanel);
                    break;
                case "Skill" : 
                    model.transform.localPosition = new Vector3(0,0,0); //* posZがずれるから、調整
                    //* Add "OnClick()" EventListner
                    Button btn = model.GetComponent<Button>();
                    var svEvent = ScrollRect.GetComponent<ScrollViewEvent>();
                    btn.onClick.AddListener(delegate{svEvent.onClickSkillPanel(model);});
                    break;
        }
            Debug.Log("modelParentTf.pos=" + modelParentPref.position + ", modelParentTf.localPos=" + modelParentPref.localPosition);
            model.name = obj.name;//名前上書き：しないと後ろに(clone)が残る。
        });
    }

    private void displayItemPassiveUI(string type, ItemPsvDt[] itemPassiveList, RectTransform itemSkillBoxPref, Transform psvPanel){
        Array.ForEach(itemPassiveList, list=>{
            if(list.lv > 0){
                Debug.Log(list.imgPref.name + "= " + list.lv);
                var boxPref = GameObject.Instantiate(itemSkillBoxPref, itemSkillBoxPref.localPosition, itemSkillBoxPref.localRotation, psvPanel);
                var imgPref = GameObject.Instantiate(list.imgPref, Vector3.zero, Quaternion.identity, boxPref).transform;
                boxPref.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
                switch(type){
                    case "Chara" : break;
                    case "Bat": psvPanel.GetComponent<RectTransform>().localPosition = new Vector3(2.4f, -2, 6.5f); break;
                }
                boxPref.GetComponentInChildren<Text>().text = list.lv.ToString();
                boxPref.GetComponentInChildren<Text>().transform.SetAsLastSibling();
            }
        });
        Debug.Log("--------------------------------------------");
    }
}

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //* OutSide
    public HomeEffectManager em;
    public ScrollRect scrollRect;
    public HomeManager hm;

    // float rectWidth;
    float curIdxBasePos;    public float CurIdxBasePos {get => curIdxBasePos; set => curIdxBasePos = value;}
    [SerializeField] int curIdx;     public int CurIdx {get => curIdx; set => curIdx = value;}

    [Header("--Scroll Speed--")]
    [SerializeField] float scrollStopSpeed;
    float scrollSpeed;
    float scrollBefFramePosX;

    [Header("<--UI-->")]
    public RectTransform uiGroup;   public RectTransform UIGroup {get => uiGroup;}
    public SpriteRenderer boxSprRdr;    public SpriteRenderer BoxSprRdr {get => boxSprRdr; set => boxSprRdr = value;}
    public Text rankTxt;    public Text RankTxt {get => rankTxt; set => rankTxt = value;}
    public Text nameTxt;    public Text NameTxt {get => nameTxt; set => nameTxt = value;}

    [Header("--Select Btn Child--")]
    public Image checkMarkImg;
    public Text priceTxt;

    void Start(){
        scrollRect = GetComponent<ScrollRect>();
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        
        if(this.gameObject.name != "ScrollView_Skill"){
            BoxSprRdr = UIGroup.GetChild(0).GetComponent<SpriteRenderer>();
            RankTxt = UIGroup.GetChild(1).GetComponent<Text>();
            NameTxt = UIGroup.GetChild(2).GetComponent<Text>();
        }
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
        var prefs = (DM.ins.SelectItemType == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ItemPrefs : DM.ins.scrollviews[(int)DM.ITEM.Bat].ItemPrefs;
        float max = width * prefs.Length - width;
        CurIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        CurIdxBasePos = -((CurIdx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);
        Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", CurIdx=" + CurIdx + " (CurIdxBasePos=" + CurIdxBasePos + "), scrollSpeed=" + scrollSpeed);

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1)
            updateItemInfo();

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    public void updateItemInfo(){
        string type = DM.ins.SelectItemType;
        var contentTf = (type == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf : DM.ins.scrollviews[(int)DM.ITEM.Bat].ContentTf;
        //* Set PosX
        contentTf.anchoredPosition = new Vector2(CurIdxBasePos, -500);
        
        var curItem = getCurItem();
        Debug.Log("<color>CurItem= " + curItem.name + "</color>");

        //* Show Rank Text
        RankTxt.text = curItem.Rank.ToString();

        //* Is Buy(UnLock)? Set Price or CheckMark
        drawChoiceBtnUI();
        if(!curItem.IsLock)
            setCheckIconColorAndOutline();
        
        //* Set Name
        string name = curItem.name.Split('_')[1];
        NameTxt.text = name;

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
        rankTxt.color = color;
        BoxSprRdr.color = color;
        boxGlowEf.GlowColor = color;
        boxGlowEf.GlowBrightness = brightness;
        boxGlowEf.OutlineWidth = outline;
    }

    public void drawChoiceBtnUI(){
        var curItem = getCurItem();
        if(curItem.IsLock){//* 💲表示
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curItem.Price.ToString();
        }else{//* ✅表示
            checkMarkImg.gameObject.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    public void setCurSelectedItem(int typeIdx){
        string type = DM.ins.SelectItemType;
        if(type == DM.ITEM.Chara.ToString() || type == DM.ITEM.Bat.ToString()){
            float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
            int index = (type == DM.ITEM.Chara.ToString())? DM.ins.personalData.SelectCharaIdx : DM.ins.personalData.SelectBatIdx;
            float saveModelPosX = -Mathf.Abs(Mathf.FloorToInt(width * (index+1)));
            DM.ins.scrollviews[typeIdx].ContentTf.anchoredPosition = new Vector2(saveModelPosX, -500);
        }
        else{ //* DM.ITEM.Skill
            // Outline
            var saveSkillTf = DM.ins.scrollviews[typeIdx].ContentTf.GetChild(DM.ins.personalData.SelectSkillIdx);
            if(!saveSkillTf.GetComponent<ItemInfo>().IsChecked){
                saveSkillTf.GetComponent<NicerOutline>().enabled = true;
            }
        }
    }

    public void exceptAlreadySelectedAnotherSkill(int selectedSkillBtnIdx, Button[] skillBtns){
        //* Skill2 Unlock?
        if(DM.ins.personalData.IsUnlock2ndSkill && DM.ins.SelectItemType == DM.ITEM.Skill.ToString()){
            //* 除外するスキル名
            int anotherIdx = (selectedSkillBtnIdx == 0)? 1 : 0;
            string exceptSkillName = skillBtns[anotherIdx].transform.GetChild(0).GetComponent<Image>().sprite.name;

            //* 初期化：スクロールにあるスキル目録
            var contentTf = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf;
            for(int i=0; i<contentTf.childCount; i++){
                Debug.Log("exceptAlreadySelectedAnotherSkill()::初期化 " + contentTf.GetChild(i).name);
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
                    tf.GetComponent<ItemInfo>().IsChecked = true;//gameObject.SetActive(false);
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
        Debug.Log("onClickCheckBtn:: type= " + type + ", CurIdx= " + CurIdx);
        var curItem = getCurItem();

        if(type == DM.ITEM.Skill.ToString() && curItem.IsChecked){
            hm.displayMessageDialog("This Skill is Already Registed");
            return;
        }
        

        //* (BUG) 買わないのにロードしたらChara選択されるバグ防止。
        int befIdx = DM.ins.personalData.getSelectIdx(type);
        DM.ins.personalData.setSelectIdx(type, CurIdx);

        //* Check
        if(curItem.IsLock){
            if(DM.ins.personalData.Coin >= curItem.Price){
                //* Buy
                em.createItemBuyEF();
                curItem.IsLock = false;
                curItem.setGrayMtIsLock();
                DM.ins.personalData.setUnLockCurList(type, CurIdx);
                DM.ins.personalData.Coin -= curItem.Price;
                drawChoiceBtnUI();
            }
            else{
                //TODO Audio
                DM.ins.personalData.setSelectIdx(type, befIdx);
            }
        }

        //* Skill Panel UI Extra Update
        if(DM.ins.SelectItemType == DM.ITEM.Skill.ToString()){
            onClickSkillPanel(curItem.gameObject);
        }
    }

    public void onClickSkillPanel(GameObject ins){
        //* Current Index
        var btns = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<Button>();
        CurIdx = Array.FindIndex(btns, btn => btn.name == ins.name);

        drawChoiceBtnUI();
        setCheckIconColorAndOutline();

        var sprite = btns[CurIdx].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        Debug.Log("onClickSkillPanel():: CurIdx= " + CurIdx + ", ins.name= " + ins.name + ", sprite= " + sprite);
    }

    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private ItemInfo getCurItem(){      
        return getItemArr()[CurIdx];
    }
    private ItemInfo[] getItemArr(){
        var contentTf = (DM.ins.SelectItemType == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf
        : (DM.ins.SelectItemType == "Bat")? DM.ins.scrollviews[(int)DM.ITEM.Bat].ContentTf
        : (DM.ins.SelectItemType == "Skill")? DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf
        : null;
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        return items;
    }
    private void setCheckIconColorAndOutline(){// (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        string type = DM.ins.SelectItemType;
        var items = getItemArr();
        var curItem = getCurItem();

        int selectItemIdx = (type == DM.ITEM.Chara.ToString())? DM.ins.personalData.SelectCharaIdx
            :(type == DM.ITEM.Bat.ToString())? DM.ins.personalData.SelectBatIdx
            :(type == DM.ITEM.Skill.ToString())?
                (hm.selectedSkillBtnIdx == 0)? DM.ins.personalData.SelectSkillIdx
                    :DM.ins.personalData.SelectSkill2Idx : -1;

        if(selectItemIdx==-1) return;
        
        if(selectItemIdx == CurIdx){
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
        string type = DM.ins.SelectItemType;
        switch(type){
            case "Chara": case "Bat":   
                item.Outline3D.enabled = isActive; 
                break;
            case "Skill":               
                item.Outline2D.enabled = isActive;
                break;
        }
    }
}
