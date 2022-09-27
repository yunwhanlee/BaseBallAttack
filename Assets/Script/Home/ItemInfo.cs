using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] bool isLock = true;    public bool IsLock {get => isLock; set => isLock = value;}
    [SerializeField] bool isChecked = true;    public bool IsChecked {get => isChecked; set => isChecked = value;}
    [SerializeField] GameObject isCheckedImgObj;    public GameObject IsCheckedImgObj {get => isCheckedImgObj; set => isCheckedImgObj = value;}
    [SerializeField] List<MeshRenderer> meshRdrList;   public List<MeshRenderer> MeshRdrList {get => meshRdrList; set => meshRdrList = value;}
    [SerializeField] List<Material> originMtList;    public List<Material> OriginMtList {get => originMtList; set => originMtList = value;} //* MatalicとGrayが有った場合、黒くならないBUG対応。
    [SerializeField] Image grayPanel;   public Image GrayPanel {get => grayPanel; set => grayPanel = value;}
    [SerializeField] DM.RANK rank;     public DM.RANK Rank {get => rank; set => rank = value;}
    [SerializeField] Outline outline3D;    public Outline Outline3D{get => outline3D; set => outline3D = value;}
    [SerializeField] UnityEngine.UI.Extensions.NicerOutline outline2D;    public UnityEngine.UI.Extensions.NicerOutline Outline2D{get => outline2D; set => outline2D = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] Text cashShopPriceTxt;     public Text CashShopPriceTxt {get => cashShopPriceTxt; set => cashShopPriceTxt = value;}
    [SerializeField] ItemPsvList itemPassive;  public ItemPsvList ItemPassive {get => itemPassive; set=> itemPassive = value;}


    void Start(){
        var type = DM.ins.getCurItemType2Idx();

        switch(type){
            //* 3D Model 形式
            case (int)DM.ITEM.Chara :
            case (int)DM.ITEM.Bat :{
                Outline3D = this.GetComponent<Outline>();

                var childs = this.GetComponentsInChildren<MeshRenderer>();
                Array.ForEach(childs, chd => MeshRdrList.Add(chd));
                Array.ForEach(childs, chd => OriginMtList.Add(chd.material));
                break;
            }
            //* 2D UI Sprite 形式
            case (int)DM.ITEM.Skill :{
                Outline2D = this.GetComponent<UnityEngine.UI.Extensions.NicerOutline>();
                Debug.Log("Skill Outline2D=" + Outline2D);

                var imgs = this.GetComponentsInChildren<Image>();
                grayPanel = Array.FindLast(imgs, img => img.gameObject.name == "GrayPanel");
                break; 
            }
            default : { 
                if(cashShopPriceTxt){ //* CashShop
                    Debug.Log("CashShop:: this.name= " + this.name);
                    cashShopPriceTxt.text = price.ToString();
                }

                break;
            }
        }


        //* Is Buy(UnLock)?
        setGrayMtIsLock();
        
        //* Set Price By Rank
        switch(rank){
            case DM.RANK.GENERAL : Price = 100; break;
            case DM.RANK.RARE : Price = 300; break;
            case DM.RANK.UNIQUE : Price = 700; break;
            case DM.RANK.LEGEND : Price = 1500; break;
            case DM.RANK.GOD : Price = 4000; break;
        }
    }

    void Update(){
    }

    public void setGrayMtIsLock(){
        if(IsLock){//* gray Material 追加
            if(GrayPanel)   GrayPanel.gameObject.SetActive(true);
            else{
                MeshRdrList.ForEach(mesh=> mesh.materials = new Material[] {DM.ins.grayItemLockMt});
            }
        }
        else{
            if(GrayPanel)   GrayPanel.gameObject.SetActive(false);
            else{
                int i = 0;
                //* MatalicとGrayが有った場合、黒くならないBUG対応。
                MeshRdrList.ForEach(mesh=> mesh.materials = new Material[] {originMtList[i++]});
            }
        }
    }
}
