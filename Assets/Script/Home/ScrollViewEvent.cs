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

    float rectWidth;
    float curIdxBasePos;    public float CurIdxBasePos {get => curIdxBasePos; set => curIdxBasePos = value;}
    int curIdx;     public int CurIdx {get => curIdx; set => curIdx = value;}

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
        rectWidth = DataManager.ins.CharaParentTf.rect.width;
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
        float max = width * DataManager.ins.CharaPfs.Length - width;
        CurIdx = Mathf.Abs(Mathf.FloorToInt((curPosX) / width));
        CurIdxBasePos = -((CurIdx+1) * width);

        //* Scroll Speed
        scrollSpeed = Mathf.Abs(scrollBefFramePosX - pos.anchoredPosition.x);
        // Debug.Log(scrollBefFramePosX + " - " + pos.anchoredPosition.x + " = " + scrollSpeed);

        Debug.Log("getScrollViewPos:: Stop Scrolling:: curPosX=" + (curPosX) + " / " + max + ", idx=" + CurIdx + " (CurIdxBasePos=" + CurIdxBasePos + "), scrollSpeed=" + scrollSpeed);

        //* Stop Scrolling Near Index Chara
        if(scrollSpeed < 1){
            setScrollStopCharaInfo();
        }

        //* update Before Frame PosX
        scrollBefFramePosX = pos.anchoredPosition.x;
    }

    private void setScrollStopCharaInfo(){
        //* Set PosX
        DataManager.ins.ContentTf.anchoredPosition = new Vector2(CurIdxBasePos, -500);
        
        var curChara = getCurChara();

        //* Show Rank Text
        rankTxt.text = curChara.Rank.ToString();

        //* Is Buy(UnLock)? Set Price or CheckMark
        if(curChara.IsLock){
            setButtonUI();
        }else{
            setButtonUI();
            setCheckUIAndCharaOutLine();
        }
        
        //* Set Name
        string name = curChara.name.Split('_')[1];
        nameTxt.text = name;

        //* Set Rank UI Color
        Color color = Color.white;
        var boxGlowEf = boxSprRdr.GetComponent<SpriteGlowEffect>();
        float brightness = 0;
        int outline = 2;
        switch(curChara.Rank){
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

    private CharactorInfo getCurChara(){
        var charaPrefs = DataManager.ins.ContentTf.GetComponentsInChildren<CharactorInfo>();
        var curChara = charaPrefs[CurIdx];
        return curChara;
    }

    public void setButtonUI(){
        var curChara = getCurChara();

        //処理
        if(curChara.IsLock){
            checkMarkImg.gameObject.SetActive(false);
            priceTxt.gameObject.SetActive(true);
            priceTxt.text = curChara.Price.ToString();
        }else{
            checkMarkImg.gameObject.SetActive(true);
            priceTxt.gameObject.SetActive(false);
        }
    }

    public void setCheckUIAndCharaOutLine(){
        var charaPrefs = DataManager.ins.ContentTf.GetComponentsInChildren<CharactorInfo>();
        var curChara = charaPrefs[CurIdx];

        //処理
        if(DataManager.ins.personalData.SelectCharaIdx == CurIdx){
            checkMarkImg.color = Color.green;
            Array.ForEach(charaPrefs, chara => chara.Outline3D.enabled = false);
            curChara.Outline3D.enabled = true;
        }
        else{
            checkMarkImg.color = Color.gray;
            curChara.Outline3D.enabled = false;
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnSelectCharactor(){
        var curChara = getCurChara();
        int befIdx = DataManager.ins.personalData.SelectCharaIdx; //* (BUG) 買わないのにロードしたら選択されるバグ防止。
        DataManager.ins.personalData.SelectCharaIdx = CurIdx;
        //* Is Lock?
        // Debug.Log("onClickBtnSelectCharactor:: isLock= " + curChara.IsLock +  ", coin= " + DataManager.ins.personalData.Coin + ", curChara.Price= " + curChara.Price);
        if(curChara.IsLock){
            if(DataManager.ins.personalData.Coin >= curChara.Price){
                //* Buy
                em.createItemBuyEF();
                curChara.IsLock = false;
                
                curChara.setMeterialIsLock();//curChara.MeshRdrList.ForEach(meshRdr=> meshRdr.materials = new Material[] {meshRdr.material});
                DataManager.ins.personalData.Coin -= curChara.Price;

                setButtonUI();
            }
            else{
                //TODO No Coin
                DataManager.ins.personalData.SelectCharaIdx = befIdx;
            }
        }
    }
}
