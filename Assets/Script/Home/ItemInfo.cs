using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] bool isLock = true;    public bool IsLock {get => isLock; set => isLock = value;}
    [SerializeField] List<MeshRenderer> meshRdrList;   public List<MeshRenderer> MeshRdrList {get => meshRdrList; set => meshRdrList = value;}
    [SerializeField] DM.RANK rank;     public DM.RANK Rank {get => rank; set => rank = value;}
    Outline outline3D;    public Outline Outline3D{get => outline3D; set => outline3D = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] int psvSkillAbility;     public int PsvSkillAbility {get => psvSkillAbility;}
    void Start(){
        outline3D = this.GetComponent<Outline>();

        //* Set MeshRenderer ChildList
        
        var childs = this.GetComponentsInChildren<MeshRenderer>();
        Array.ForEach(childs, chd => MeshRdrList.Add(chd));

        //* Is Buy(UnLock)?
        setMeterialIsLock();
        
        //* Set Price By Rank
        switch(rank){
            case DM.RANK.GENERAL : Price = 100; break;
            case DM.RANK.RARE : Price = 300; break;
            case DM.RANK.UNIQUE : Price = 700; break;
            case DM.RANK.LEGEND : Price = 1500; break;
            case DM.RANK.GOD : Price = 4000; break;
        }
    }

    public void setMeterialIsLock(){
        if(IsLock){
            var grayBlackMt = DM.ins.grayBlackNoBuyMt;
            MeshRdrList.ForEach(meshRdr=>{
                //* grayBlack Material 追加
                meshRdr.materials = new Material[] {meshRdr.material, grayBlackMt}; //meshRdr.materialsが配列だから、再代入する
            });
        }
        else{
            MeshRdrList.ForEach(meshRdr=> meshRdr.materials = new Material[] {meshRdr.material});
        }
    }
}
