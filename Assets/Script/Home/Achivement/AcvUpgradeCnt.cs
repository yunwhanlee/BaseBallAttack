using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvUpgradeCnt : Achivement
{
    const int UPG20PER = 0, UPG40PER = 1, UPG60PER = 2, UPG80PER = 3, UPGALL = 4;

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.UpgradeCnt;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.UpgradeCntArr[UPGALL].IsAccept)
            setNext(pDt, UPGALL, _allClear: true);
        else if(pDt.UpgradeCntArr[UPG80PER].IsAccept)
            setNext(pDt, UPGALL);
        else if(pDt.UpgradeCntArr[UPG60PER].IsAccept)
            setNext(pDt, UPG80PER);
        else if(pDt.UpgradeCntArr[UPG40PER].IsAccept)
            setNext(pDt, UPG60PER);
        else if(pDt.UpgradeCntArr[UPG20PER].IsAccept)
            setNext(pDt, UPG40PER);
        else if(!Array.Exists(pDt.UpgradeCntArr, arr => arr.IsAccept))
            setNext(pDt, UPG20PER);
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickCalimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.UpgradeCntArr[UPG20PER].IsComplete && !pDt.UpgradeCntArr[UPG20PER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, UPG20PER));
            setNext(pDt, UPG40PER);
        }
        else if(pDt.UpgradeCntArr[UPG40PER].IsComplete && !pDt.UpgradeCntArr[UPG40PER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, UPG40PER));
            setNext(pDt, UPG60PER);
        }
        else if(pDt.UpgradeCntArr[UPG60PER].IsComplete && !pDt.UpgradeCntArr[UPG60PER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, UPG60PER));
            setNext(pDt, UPG80PER);
        }
        else if(pDt.UpgradeCntArr[UPG80PER].IsComplete && !pDt.UpgradeCntArr[UPG80PER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, UPG80PER));
            setNext(pDt, UPGALL);
        }
        else if(pDt.UpgradeCntArr[UPGALL].IsComplete && !pDt.UpgradeCntArr[UPGALL].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, UPGALL));
            setNext(pDt, UPGALL, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void addUpgradeCnt() {
        DM.ins.personalData.UpgradeCnt++;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.UpgradeCntArr.Length; i++){
            if(pDt.UpgradeCnt >= pDt.UpgradeCntArr[i].Val)
                pDt.UpgradeCntArr[i].IsComplete = true;
        }
    }
}
