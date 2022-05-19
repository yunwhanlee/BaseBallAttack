using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharactorInfo : MonoBehaviour
{
    [SerializeField] bool isLock = true;    public bool IsLock {get => isLock; set => isLock = value;}
    [SerializeField] List<MeshRenderer> meshRdrList;   public List<MeshRenderer> MeshRdrs {get => meshRdrList; set => meshRdrList = value;}
    [SerializeField] DataManager.RANK rank;     public DataManager.RANK Rank {get => rank; set => rank = value;}
    Outline outline3D;    public Outline Outline3D{get => outline3D; set => outline3D = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] int psvSkillAbility;     public int PsvSkillAbility {get => psvSkillAbility;}
    void Start(){
        outline3D = this.GetComponent<Outline>();

        //* Set MeshRenderer ChildList
        var childs = this.GetComponentsInChildren<MeshRenderer>();
        Array.ForEach(childs, chd => meshRdrList.Add(chd));

        //* Is Buy(UnLock)?
        if(isLock){
            var grayBlackMt = DataManager.ins.grayBlackNoBuyMt;
            meshRdrList.ForEach(meshRdr=>{
                //* grayBlack Material 追加
                meshRdr.materials = new Material[] {meshRdr.material, grayBlackMt}; //meshRdr.materialsが配列だから、再代入する
            });
        }
        
        //* Set Price By Rank
        switch(rank){
            case DataManager.RANK.GENERAL : Price = 100; break;
            case DataManager.RANK.RARE : Price = 300; break;
            case DataManager.RANK.UNIQUE : Price = 700; break;
            case DataManager.RANK.LEGEND : Price = 1500; break;
            case DataManager.RANK.GOD : Price = 4000; break;
        }
    }

    void Update()
    {
        
    }
}
