using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCollectedDiamond : Achivement
{
    const int CLTDIA500 = 0, CLTDIA1000 = 1, CLTDIA2000 = 2, CLTDIA5000 = 3, CLTDIA10000 = 4;

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.CollectedDiamond;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CollectedDiamondArr[CLTDIA10000].IsAccept)
            setNext(pDt, CLTDIA10000, _allClear: true);
        else if(pDt.CollectedDiamondArr[CLTDIA5000].IsAccept)
            setNext(pDt, CLTDIA10000);
        else if(pDt.CollectedDiamondArr[CLTDIA2000].IsAccept)
            setNext(pDt, CLTDIA5000);
        else if(pDt.CollectedDiamondArr[CLTDIA1000].IsAccept)
            setNext(pDt, CLTDIA2000);
        else if(pDt.CollectedDiamondArr[CLTDIA500].IsAccept)
            setNext(pDt, CLTDIA1000);
        else if(!Array.Exists(pDt.CollectedDiamondArr, arr => arr.IsAccept))
            setNext(pDt, CLTDIA500);
    }

    // Update is called once per frame
    void Update(){
        base.Update();
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickCalimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;

        if(pDt.CollectedDiamondArr[CLTDIA500].IsComplete && !pDt.CollectedDiamondArr[CLTDIA500].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA500));
            setNext(pDt, CLTDIA1000);
        }
        else if(pDt.CollectedDiamondArr[CLTDIA1000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA1000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA1000));
            setNext(pDt, CLTDIA2000);
        }
        else if(pDt.CollectedDiamondArr[CLTDIA2000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA2000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA2000));
            setNext(pDt, CLTDIA5000);
        }
        else if(pDt.CollectedDiamondArr[CLTDIA5000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA5000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA5000));
            setNext(pDt, CLTDIA10000);
        }
        else if(pDt.CollectedDiamondArr[CLTDIA10000].IsComplete && !pDt.CollectedDiamondArr[CLTDIA10000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTDIA10000));
            setNext(pDt, CLTDIA10000, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectDiamond(int amount){
        DM.ins.personalData.CollectedDiamond += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedDiamondArr.Length; i++){
            if(pDt.CollectedDiamond >= pDt.CollectedDiamondArr[i].Val)
                pDt.CollectedDiamondArr[i].IsComplete = true;
        }
    }
}
