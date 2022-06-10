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
        if(scrollSpeed < 1){
            updateItemInfo();
        }

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
        setChoiceBtnUI();
        if(!curItem.IsLock){
            setCheckUIAndItemOutLine();
        }
        
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

    private ItemInfo getCurItem(){
        var contentTf = (DM.ins.SelectType == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf
            : (DM.ins.SelectType == "Bat")? DM.ins.scrollviews[(int)DM.ITEM.Bat].ContentTf
            : (DM.ins.SelectType == "Skill")? DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf
            : null;
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        // foreach(var item in items){Debug.Log("getCurItem:: Item=" + item);}
        var curItem = items[CurIdx];
        return curItem;
    }

    public void setChoiceBtnUI(){
        var curItem = getCurItem();
        //処理
        if(curItem.IsLock){//* 💲表示
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curItem.Price.ToString();
        }else{//* ✅表示
            checkMarkImg.gameObject.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    public void setCheckUIAndItemOutLine(){
        string type = DM.ins.SelectType;

        RectTransform contentTf = (type == "Chara")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf
            : (type == "Bat")? DM.ins.scrollviews[(int)DM.ITEM.Chara].ContentTf
            : (type == "Skill")? DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf : null;

        //* (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        int i=0;
        Array.ForEach(items, item=> Debug.Log("ContentTf:: " + DM.ins.SelectType + " item["+(i++)+"]= " + item));
        var curItem = items[CurIdx];
        
        int selectItemIdx = (type == "Chara")? 
            DM.ins.personalData.SelectCharaIdx : DM.ins.personalData.SelectBatIdx;

        if(selectItemIdx == CurIdx){
            checkMarkImg.color = Color.green;
            Array.ForEach(items, item => item.Outline3D.enabled = false);
            curItem.Outline3D.enabled = true;
        }
        else{
            checkMarkImg.color = Color.gray;
            curItem.Outline3D.enabled = false;
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickCheckBtn(string type){
        if(DM.ins.SelectType != type) return;
        Debug.Log("onClickBtnSelectItem:: type= " + type + ", CurIdx= " + CurIdx);
        var curItem = getCurItem();
        //* (BUG) 買わないのにロードしたらChara選択されるバグ防止。
        int befIdx = (type == "Chara")? DM.ins.personalData.SelectCharaIdx
            :(type == "Bat")? DM.ins.personalData.SelectBatIdx
            :(type == "Skill")? DM.ins.personalData.SelectSkillIdx : -1;
        
        if(type == "Chara") DM.ins.personalData.SelectCharaIdx = CurIdx;
        else if(type == "Bat") DM.ins.personalData.SelectBatIdx = CurIdx;
        else if(type == "Skill") DM.ins.personalData.SelectSkillIdx = CurIdx;

        //* Is Lock?
        // Debug.Log("onClickBtnSelectCharactor:: isLock= " + curChara.IsLock +  ", coin= " + DataManager.ins.personalData.Coin + ", curChara.Price= " + curChara.Price);
        if(curItem.IsLock){
            if(DM.ins.personalData.Coin >= curItem.Price){
                //* Buy
                em.createItemBuyEF();
                curItem.IsLock = false;
                if(type == "Chara") DM.ins.personalData.CharaLockList[CurIdx] = false;
                else if(type == "Bat") DM.ins.personalData.BatLockList[CurIdx] = false;
                else if(type == "Skill") DM.ins.personalData.SkillLockList[CurIdx] = false;
                
                curItem.setGrayMtIsLock();
                DM.ins.personalData.Coin -= curItem.Price;

                setChoiceBtnUI();
            }
            else{
                //TODO No Coin
                if(type == "Chara") DM.ins.personalData.SelectCharaIdx = befIdx;
                else if(type == "Bat") DM.ins.personalData.SelectBatIdx = befIdx;
                else if(type == "Skill") DM.ins.personalData.SelectBatIdx = befIdx;
            }
        }
    }

    public void onClickBtnSelectSkill(GameObject ins){
        //* Color
        var btns = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<Button>();
        // Array.ForEach(btns, btn=>{
        //     ColorBlock cb = btn.colors;
        //     cb.normalColor = new Color(1,1,1,0.5f);
        //     //* Select
        //     if(ins.name == btn.name)
        //         cb.normalColor = Color.blue;
            
        //     btn.colors = cb;
        // });
        
        //* Current Index
        CurIdx = Array.FindIndex(btns, btn => btn.name == ins.name);
        Debug.Log("SelectSkill():: CurIdx= " + CurIdx + ", ins.name= " + ins.name);

        setChoiceBtnUI();

        var items = DM.ins.scrollviews[(int)DM.ITEM.Skill].ContentTf.GetComponentsInChildren<ItemInfo>();
        var curItem = items[CurIdx];

        int selectItemIdx = DM.ins.personalData.SelectSkillIdx;
        if(selectItemIdx == CurIdx){
            checkMarkImg.color = Color.green;
            Array.ForEach(items, item => item.Outline2D.enabled = false);
            curItem.Outline2D.enabled = true;
        }
        else{
            checkMarkImg.color = Color.gray;
            curItem.Outline2D.enabled = false;
        }
    }
}
