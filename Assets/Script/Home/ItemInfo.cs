using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

public class ItemInfo : MonoBehaviour
{
    [Header("言語 [0]:EN, [1]:JP, [2]:KR")][Header("__________________________")]
    
    [FormerlySerializedAs("nameTxts")]
    [SerializeField] string[] nameTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [FormerlySerializedAs("explainTxts")]
    [SerializeField] string[] explainTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [FormerlySerializedAs("homeRunBonusTxts")] 
    [SerializeField] string[] homeRunBonusTxts = new string[System.Enum.GetValues(typeof(LANG.TP)).Length];

    [Header("STATUS")][Header("__________________________")]
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
    [FormerlySerializedAs("upgradeValueTxt")] [SerializeField] Text upgradeValueTxt;     public Text UpgradeValueTxt {get => upgradeValueTxt; set => upgradeValueTxt = value;}
    [FormerlySerializedAs("lvTxt")] [SerializeField] Text lvTxt;     public Text LvTxt {get => lvTxt; set => lvTxt = value;}
    [FormerlySerializedAs("cashShopPriceTxt")] [SerializeField] Text cashShopPriceTxt;     public Text CashShopPriceTxt {get => cashShopPriceTxt; set => cashShopPriceTxt = value;}

    [SerializeField] ItemPsvList itemPassive;  public ItemPsvList ItemPassive {get => itemPassive; set=> itemPassive = value;}
    [SerializeField] GameObject rankAuraEF;  public GameObject RankAuraEF {get => rankAuraEF; set=> rankAuraEF = value;}

    void OnEnable(){
        Debug.Log($"ItemInfo::OnEnable:: this.name= {this.name}");
        DM.PANEL itemType = DM.ins.getCurPanelType2Enum(DM.ins.SelectItemType);
        arrangeItem(itemType);
    }

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
        else if(this.name.Contains(DM.PANEL.Upgrade.ToString())){
            LANG.UpgradeNameList.Add(nameTxts);
            LANG.UpgradeExplainList.Add(explainTxts);
        }
    }

    void Start(){
        if(this.name.Contains(DM.PANEL.Chara.ToString())
            || this.name.Contains(DM.PANEL.Bat.ToString())
            || this.name.Contains(DM.PANEL.Skill.ToString())){
            checkLockedModel();
            //* Set Price By Rank
            switch(rank){
                case DM.RANK.GENERAL : 
                    price.Val = 100;
                    break;
                case DM.RANK.RARE : 
                    price.Val = 300; 
                    break;
                case DM.RANK.UNIQUE : 
                    price.Val = 700; 
                    break;
                case DM.RANK.LEGEND : 
                    price.Val = 1500; 
                    break;
                case DM.RANK.GOD : 
                    price.Val = 4000; 
                    break;
            }
            Debug.Log("IteonInfo::Start:: Set Price By Rank= price.Val= " + price.Val);
        }
    }

    private void arrangeItem(DM.PANEL type){
        switch(type){
            //* 3D Model 形式
            case DM.PANEL.Chara :
            case DM.PANEL.Bat : {
                Outline3D = this.GetComponent<Outline>();

                var meshs = this.GetComponentsInChildren<MeshRenderer>();
                Array.ForEach(meshs, mesh => ModelMeshRdrList.Add(mesh));
                Array.ForEach(meshs, mesh => OriginMtList.Add(mesh.material));
                break;
            }
            //* 2D UI Sprite 形式
            case DM.PANEL.Skill : {
                Outline2D = this.GetComponent<UnityEngine.UI.Extensions.NicerOutline>();

                var imgs = this.GetComponentsInChildren<Image>();
                grayPanel2D = Array.FindLast(imgs, img => img.gameObject.name == "GrayPanel");
                break; 
            }
            case DM.PANEL.CashShop : { //* 追加的な特別変数へ代入。
                try{
                    CashShopPriceTxt.text = price.getValue().ToString();
                }
                catch(Exception err){
                    Debug.LogError("ItemInfo:: rsc/home/selectItemPanel/Content/<b>CashShop</b>のPrefabのInspectorビューへ、CashShopPriceTxtがNullです。\n◆ERROR: " + err);
                }
                break;
            }
            case DM.PANEL.Upgrade : { //* 追加的な特別変数へ代入。
                break;
            }
        }
    }

    public void checkLockedModel(){
        Debug.Log($"ItemInfo::checkLockedModel:: this.name={this.name}, IsLock={IsLock}, ModelMeshRdrList.Count= {ModelMeshRdrList.Count}, OriginMtList.Count= {OriginMtList.Count}");
        //TODO ゲームスタートして戻ったら、GRAY MATERAILが適用されていないBUG。

        int i = 0;
        //* 2D
        if(GrayPanel2D) // CharaやBatはNone.
            GrayPanel2D.gameObject.SetActive(IsLock);

        //* 3D & 2D
        if(IsLock)
            ModelMeshRdrList.ForEach((mesh) => mesh.materials = new Material[2] {OriginMtList[i++], DM.ins.GrayItemLockMt});
        else{
            //* (BUG) GAMESTARTしてからHOMEに戻ったら、CharaとBatのロックしたモデルが購入しても、そのまま黒色。
            // ModelMeshRdrList.ForEach(mesh => mesh.material = new Material[] {OriginMtList[i++]}); ➡ 原因はこうしても、マテリアル配列数は初期化されないから
            ModelMeshRdrList.ForEach(mesh => {
                Material[] tempArr = mesh.materials;
                Array.Resize(ref tempArr, 1);
                mesh.materials = tempArr;
            });
        }
    }

    public void setUpgradeGUI(UpgradeDt item){
        Debug.Log($"ItenInfo::setUpgradeGUI(UpgradeDt item={item})::");
        this.UpgradeValueTxt.text = item.getVal2Str();
        this.LvTxt.text = $"{item.lv}/{item.maxLv}";
    }
}
