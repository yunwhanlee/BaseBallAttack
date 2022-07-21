using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] bool isLock = true;    public bool IsLock {get => isLock; set => isLock = value;}
    [SerializeField] List<MeshRenderer> meshRdrList;   public List<MeshRenderer> MeshRdrList {get => meshRdrList; set => meshRdrList = value;}
    [SerializeField] Image grayPanel;   public Image GrayPanel {get => grayPanel; set => grayPanel = value;}
    [SerializeField] DM.RANK rank;     public DM.RANK Rank {get => rank; set => rank = value;}
    [SerializeField] Outline outline3D;    public Outline Outline3D{get => outline3D; set => outline3D = value;}
    [SerializeField] UnityEngine.UI.Extensions.NicerOutline outline2D;    public UnityEngine.UI.Extensions.NicerOutline Outline2D{get => outline2D; set => outline2D = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] ItemPassiveList itemPassive;  public ItemPassiveList ItemPassive {get => itemPassive; set=> itemPassive = value;}


    void Start(){
        switch(DM.ins.SelectItemType){
            //* 3D Model 形式
            case "Chara" :
            case "Bat" :{
                Outline3D = this.GetComponent<Outline>();

                var childs = this.GetComponentsInChildren<MeshRenderer>();
                Array.ForEach(childs, chd => MeshRdrList.Add(chd));
                break;
            }
            //* 2D UI Sprite 形式
            case "Skill" :{
                Outline2D = this.GetComponent<UnityEngine.UI.Extensions.NicerOutline>();
                // Debug.Log("Skill Outline2D=" + Outline2D);

                var imgs = this.GetComponentsInChildren<Image>();
                grayPanel = Array.FindLast(imgs, img => img.gameObject.name == "GrayPanel");
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

    public void setGrayMtIsLock(){
        if(IsLock){//* gray Material 追加
            if(GrayPanel)   GrayPanel.gameObject.SetActive(true);
            else    MeshRdrList.ForEach(mesh=>mesh.materials = new Material[] {mesh.material, DM.ins.grayItemLock});
        }
        else{
            if(GrayPanel)   GrayPanel.gameObject.SetActive(false);
            else    MeshRdrList.ForEach(mesh=> mesh.materials = new Material[] {mesh.material});
        }
    }
}
