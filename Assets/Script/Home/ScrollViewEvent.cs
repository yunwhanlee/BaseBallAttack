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
    public SpriteRenderer boxSprRdr;
    public Text rankTxt;
    public Text nameTxt;

    [Header("--Select Btn Child--")]
    public Image checkMarkImg;
    public Text priceTxt;

    void Start(){
        scrollRect = GetComponent<ScrollRect>();
        rectWidth = DataManager.ins.ModelParentPref.rect.width;
    }

    //* Drag Event
    public void OnBeginDrag(PointerEventData eventData){
        // Debug.Log("<color=white> ScrollViewEvent:: Drag Begin </color>");
    }

    public void OnEndDrag(PointerEventData eventData){
        //* (BUG) スクロールが先端と末端だったら、イベント起動しない。
        // int posX = (int)DataManager.ins.ScrollContentTf.anchoredPosition.x;
        // Debug.Log("<color=white> ScrollViewEvent:: Drag End:: PosX=" + posX + ", CurIdx=" + CurIdx + "Charas.Len=" + (DataManager.ins.CharaPfs.Length - 1) + "</color>");

        // if(CurIdx == 0 || CurIdx == DataManager.ins.CharaPfs.Length-1){
        //     setScrollStopCharaInfo();
        // }
    }

    public void OnScrollViewPos(RectTransform pos){ //* －が右側
        float width = Mathf.Abs(rectWidth);
        float offset = -(width + width/2);
        float curPosX = pos.anchoredPosition.x - offset;
        var prefs = (DataManager.ins.SelectType == "Chara")? DataManager.ins.CharaPfs : DataManager.ins.BatPfs;
        float max = width * prefs.Length - width;
        CurIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        CurIdxBasePos = -((CurIdx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);

        Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", CurIdx=" + CurIdx + " (CurIdxBasePos=" + CurIdxBasePos + "), scrollSpeed=" + scrollSpeed);

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1){
            setScrollStopItemInfo();
        }

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    private void setScrollStopItemInfo(){
        string type = DataManager.ins.SelectType;
        var contentTf = (type == "Chara")? DataManager.ins.ContentCharaTf : DataManager.ins.ContentBatTf;
        //* Set PosX
        contentTf.anchoredPosition = new Vector2(CurIdxBasePos, -500);
        
        var curItem = getCurItem();
        Debug.Log("<color>CurItem= " + curItem.name + "</color>");

        //* Show Rank Text
        rankTxt.text = curItem.Rank.ToString();

        //* Is Buy(UnLock)? Set Price or CheckMark
        if(curItem.IsLock){
            setButtonUI();
        }else{
            setButtonUI();
            setCheckUIAndItemOutLine();
        }
        
        //* Set Name
        string name = curItem.name.Split('_')[1];
        nameTxt.text = name;

        //* Set Rank UI Color
        Color color = Color.white;
        var boxGlowEf = boxSprRdr.GetComponent<SpriteGlowEffect>();
        float brightness = 0;
        int outline = 2;
        switch(curItem.Rank){
            case DataManager.RANK.GENERAL : color = Color.white; brightness=1; outline=2;   break;
            case DataManager.RANK.RARE : color = Color.blue; brightness=8; outline=2;   break;
            case DataManager.RANK.UNIQUE : color = Color.red; brightness=8.5f; outline=5; break;
            case DataManager.RANK.LEGEND : color = Color.magenta; brightness=9; outline=8; break;
            case DataManager.RANK.GOD : color = Color.yellow; brightness=10; outline=5; break;
        }
        rankTxt.color = color;
        boxSprRdr.color = color;
        boxGlowEf.GlowColor = color;
        boxGlowEf.GlowBrightness = brightness;
        boxGlowEf.OutlineWidth = outline;
    }

    private ItemInfo getCurItem(){
        var contentTf = (DataManager.ins.SelectType == "Chara")? DataManager.ins.ContentCharaTf : DataManager.ins.ContentBatTf;
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        // foreach(var item in items){Debug.Log("getCurItem:: Item=" + item);}
        var curItem = items[CurIdx];
        return curItem;
    }

    public void setButtonUI(){
        var curItem = getCurItem();

        //処理
        if(curItem.IsLock){
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curItem.Price.ToString();
        }else{
            checkMarkImg.gameObject.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    public void setCheckUIAndItemOutLine(){
        string type = DataManager.ins.SelectType;

        RectTransform contentTf = (type == "Chara")? 
            DataManager.ins.ContentCharaTf : DataManager.ins.ContentBatTf;

        //* (BUG) CharaのRightArmにあるBatにも<ItemInfo>有ったので、重複される。CharaPrefのBatを全て非活性化してOK。
        var items = contentTf.GetComponentsInChildren<ItemInfo>();
        int i=0;
        Array.ForEach(items, item=> Debug.Log("ContentTf:: " + DataManager.ins.SelectType + " item["+(i++)+"]= " + item));
        var curItem = items[CurIdx];
        
        int selectItemIdx = (type == "Chara")? 
            DataManager.ins.personalData.SelectCharaIdx : DataManager.ins.personalData.SelectBatIdx;

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
    public void onClickBtnSelectItem(string type){
        if(DataManager.ins.SelectType != type) return;
        Debug.Log("onClickBtnSelectItem:: type= " + type + ", CurIdx= " + CurIdx);
        var curItem = getCurItem();
        //* (BUG) 買わないのにロードしたらChara選択されるバグ防止。
        int befIdx = (type == "Chara")? 
            DataManager.ins.personalData.SelectCharaIdx : DataManager.ins.personalData.SelectBatIdx; 
        
        if(type == "Chara")
            DataManager.ins.personalData.SelectCharaIdx = CurIdx;
        else 
            DataManager.ins.personalData.SelectBatIdx = CurIdx;

        //* Is Lock?
        // Debug.Log("onClickBtnSelectCharactor:: isLock= " + curChara.IsLock +  ", coin= " + DataManager.ins.personalData.Coin + ", curChara.Price= " + curChara.Price);
        if(curItem.IsLock){
            if(DataManager.ins.personalData.Coin >= curItem.Price){
                //* Buy
                em.createItemBuyEF();
                curItem.IsLock = false;
                
                curItem.setMeterialIsLock();//curChara.MeshRdrList.ForEach(meshRdr=> meshRdr.materials = new Material[] {meshRdr.material});
                DataManager.ins.personalData.Coin -= curItem.Price;

                setButtonUI();
            }
            else{
                //TODO No Coin
                if(type == "Chara")
                    DataManager.ins.personalData.SelectCharaIdx = befIdx;
                else 
                    DataManager.ins.personalData.SelectBatIdx = befIdx;
            }
        }
    }
}
