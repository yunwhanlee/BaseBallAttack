using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

public class ItemInfo : MonoBehaviour
{
    [Header("言語 [0]:EN, [1]:JP, [2]:KR")]
    
    [FormerlySerializedAs("nameTxts")]
    [SerializeField] string[] nameTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [FormerlySerializedAs("explainTxts")]
    [SerializeField] string[] explainTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [FormerlySerializedAs("homeRunBonusTxts")] 
    [SerializeField] string[] homeRunBonusTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [Header("STATUS")]
    public Price price;
    [SerializeField] bool isLock = true;    public bool IsLock {get => isLock; set => isLock = value;}
    [SerializeField] bool isChecked = true;    public bool IsChecked {get => isChecked; set => isChecked = value;}
    [SerializeField] GameObject isCheckedImgObj;    public GameObject IsCheckedImgObj {get => isCheckedImgObj; set => isCheckedImgObj = value;}
    [SerializeField] List<MeshRenderer> modelMeshRdrList;   public List<MeshRenderer> ModelMeshRdrList {get => modelMeshRdrList; set => modelMeshRdrList = value;}
    [SerializeField] List<Material> originMtList;    public List<Material> OriginMtList {get => originMtList; set => originMtList = value;} //* MatalicとGrayが有った場合、黒くならないBUG対応。
    [SerializeField] Image grayPanel2D;   public Image GrayPanel2D {get => grayPanel2D; set => grayPanel2D = value;}
    [SerializeField] DM.RANK rank;     public DM.RANK Rank {get => rank; set => rank = value;}
    [SerializeField] Outline outline3D;    public Outline Outline3D{get => outline3D; set => outline3D = value;}
    [SerializeField] UnityEngine.UI.Extensions.NicerOutline outline2D;    public UnityEngine.UI.Extensions.NicerOutline Outline2D{get => outline2D; set => outline2D = value;}
    [SerializeField] Text cashShopPriceTxt;     public Text CashShopPriceTxt {get => cashShopPriceTxt; set => cashShopPriceTxt = value;}
    [SerializeField] ItemPsvList itemPassive;  public ItemPsvList ItemPassive {get => itemPassive; set=> itemPassive = value;}

    void Awake(){
        //* Push Language Txt Data
        if(this.name.Contains(DM.PANEL.Chara.ToString())){
            LANG.CharaList.Add(nameTxts);
        }
        else if(this.name.Contains(DM.PANEL.Bat.ToString())){
            LANG.BatList.Add(nameTxts);
        }
        else if(this.name.Contains(DM.PANEL.Skill.ToString())){
            LANG.SkillNameList.Add(nameTxts);
            LANG.SkillExplainList.Add(explainTxts);
            LANG.SkillHomeRunBonusList.Add(homeRunBonusTxts);
        }
        else if(this.name.Contains(DM.PANEL.CashShop.ToString())){
            LANG.CashShopNameList.Add(nameTxts);
            LANG.CashShopExplainList.Add(explainTxts);
        }
        else if(this.name.Contains(DM.PANEL.PsvInfo.ToString())){
            LANG.PsvInfoNameList.Add(nameTxts);
            LANG.PsvInfoExplainList.Add(explainTxts);
        }
    }

    void Start(){
        //* Set Panel Outline & CashShop Price Txt UI
        var itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        switch(itemType){
            //* 3D Model 形式
            case DM.PANEL.Chara :
            case DM.PANEL.Bat :{
                Outline3D = this.GetComponent<Outline>();

                var meshs = this.GetComponentsInChildren<MeshRenderer>();
                Array.ForEach(meshs, mesh => ModelMeshRdrList.Add(mesh));
                Array.ForEach(meshs, mesh => OriginMtList.Add(mesh.material));
                break;
            }
            //* 2D UI Sprite 形式
            case DM.PANEL.Skill :{
                Outline2D = this.GetComponent<UnityEngine.UI.Extensions.NicerOutline>();
                Debug.Log("Skill Outline2D=" + Outline2D);

                var imgs = this.GetComponentsInChildren<Image>();
                grayPanel2D = Array.FindLast(imgs, img => img.gameObject.name == "GrayPanel");
                break; 
            }
            default : { 
                if(CashShopPriceTxt){ //* CashShop
                    Debug.Log("CashShop:: this.name= " + this.name);
                    cashShopPriceTxt.text = price.getValue().ToString();
                }else{
                    Debug.Log("PsvInfo::");
                }

                break;
            }
        }

        //* Is Buy(UnLock)?
        setModelMesh();
        
        //* Set Price By Rank
        switch(rank){
            case DM.RANK.GENERAL : price.Coin = 100; break;
            case DM.RANK.RARE : price.Coin = 300; break;
            case DM.RANK.UNIQUE : price.Coin = 700; break;
            case DM.RANK.LEGEND : price.Coin = 1500; break;
            case DM.RANK.GOD : price.Coin = 4000; break;
        }
    }

    public void setModelMesh(){
        //TODO ゲームスタートして戻ったら、GRAY MATERAILが適用されていないBUG。

        int i = 0;
        //* ATVやPSVがLockの場合、2D黒パンネルを適用。
        if(GrayPanel2D) // CharaやBatはNone.
            GrayPanel2D.gameObject.SetActive(IsLock);

        //* Set Material
        if(IsLock)
            ModelMeshRdrList.ForEach((mesh) => mesh.materials = new Material[2] {DM.ins.grayItemLockMt, originMtList[i++]});
        else
            ModelMeshRdrList.ForEach(mesh => mesh.materials = new Material[] {originMtList[i++]});
    }
}
