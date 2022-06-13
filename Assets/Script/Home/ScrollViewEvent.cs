using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;
using UnityEngine.EventSystems;
using System;

public class ScrollViewEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //* OutSide
    public HomeEffectManager em;
    public ScrollRect scrollRect;

    float rectWidth;
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
        rectWidth = DM.ins.ModelParentPref.rect.width;

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
        float width = Mathf.Abs(rectWidth);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        var prefs = (DM.ins.SelectType == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].Prefs : DM.ins.scrollviews[(int)DM.ITEM.Bat].Prefs;
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
        string type = DM.ins.SelectType;
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


    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private ItemInfo[] getItemArr(){
        var contentTf = (DM.ins.SelectType == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf
        : (DM.ins.SelectType == "Bat")? DM.ins.scrollviews[(int)DM.ITEM.Bat].ContentTf
        : (DM.ins.SelectType == "Skill")? DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf
        : null;
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        return items;
    }

    private ItemInfo getCurItem(){
        var items = getItemArr();        
        var curItem = items[CurIdx];
        return curItem;
    }

    private void setCheckIconColorAndOutline(){// (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        string type = DM.ins.SelectType;
        var items = getItemArr();
        var curItem = getCurItem();

        int selectItemIdx = (type == DM.ITEM.Chara.ToString())? DM.ins.personalData.SelectCharaIdx
            :(type == DM.ITEM.Bat.ToString())? DM.ins.personalData.SelectBatIdx
            :(type == DM.ITEM.Skill.ToString())? DM.ins.personalData.SelectSkillIdx : -1;

        if(selectItemIdx==-1) return;
        
        if(selectItemIdx == CurIdx){
            checkMarkImg.color = Color.green;

            //* OutLine
            Array.ForEach(items, item => {
                switch(type){
                    case "Chara": case "Bat":   item.Outline3D.enabled = false; break;
                    case "Skill":               item.Outline2D.enabled = false; break;
                }
            });
            switch(type){
                case "Chara": case "Bat":   curItem.Outline3D.enabled = true; break;
                case "Skill":               curItem.Outline2D.enabled = true; break;
            }
        }
        else{
            checkMarkImg.color = Color.gray;
            switch(type){
                case "Chara": case "Bat":   curItem.Outline3D.enabled = false; break;
                case "Skill":               curItem.Outline2D.enabled = false; break;
            }
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickCheckBtn(string type){
        if(DM.ins.SelectType != type) return;
        Debug.Log("onClickCheckBtn:: type= " + type + ", CurIdx= " + CurIdx);
        var curItem = getCurItem();

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

        //* Skill Panel UI Update
        if(DM.ins.SelectType == DM.ITEM.Skill.ToString()){
            onClickSkillPanel(curItem.gameObject);
        }
    }

    public void onClickSkillPanel(GameObject ins){
        //* Current Index
        var btns = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<Button>();
        CurIdx = Array.FindIndex(btns, btn => btn.name == ins.name);
        Debug.Log("onClickSkillPanelBtn():: CurIdx= " + CurIdx + ", ins.name= " + ins.name);

        drawChoiceBtnUI();
        setCheckIconColorAndOutline();
    }
}
