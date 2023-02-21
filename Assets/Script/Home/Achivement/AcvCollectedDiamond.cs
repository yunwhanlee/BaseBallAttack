using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCollectedDiamond : Achivement
{
    enum CLTDIA {
        _500, _1000, _2500, _5000, _10000, 
        _20000, _40000, _80000, _160000, _300000};
    // const int CLTDIA500 = 0, 
    //     CLTDIA1000 = 1, 
    //     CLTDIA2500 = 2, 
    //     CLTDIA5000 = 3, 
    //     CLTDIA10000 = 4;        

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.CollectedDiamond;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CollectedDiamondArr[(int)CLTDIA._160000].IsAccept)
            setNext(pDt, (int)CLTDIA._300000, _allClear: true);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._80000].IsAccept)
            setNext(pDt, (int)CLTDIA._160000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._40000].IsAccept)
            setNext(pDt, (int)CLTDIA._80000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._20000].IsAccept)
            setNext(pDt, (int)CLTDIA._40000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._10000].IsAccept)
            setNext(pDt, (int)CLTDIA._20000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._5000].IsAccept)
            setNext(pDt, (int)CLTDIA._10000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._2500].IsAccept)
            setNext(pDt, (int)CLTDIA._5000);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._1000].IsAccept)
            setNext(pDt, (int)CLTDIA._2500);
        else if(pDt.CollectedDiamondArr[(int)CLTDIA._500].IsAccept)
            setNext(pDt, (int)CLTDIA._1000);
        else if(!Array.Exists(pDt.CollectedDiamondArr, arr => arr.IsAccept))
            setNext(pDt, (int)CLTDIA._500);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickCalimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;
        var diaArr = pDt.CollectedDiamondArr;

        if(diaArr[(int)CLTDIA._500].IsComplete && !diaArr[(int)CLTDIA._500].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._500));
            setNext(pDt, (int)CLTDIA._1000);
        }
        else if(diaArr[(int)CLTDIA._1000].IsComplete && !diaArr[(int)CLTDIA._1000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._1000));
            setNext(pDt, (int)CLTDIA._2500);
        }
        else if(diaArr[(int)CLTDIA._2500].IsComplete && !diaArr[(int)CLTDIA._2500].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._2500));
            setNext(pDt, (int)CLTDIA._5000);
        }
        else if(diaArr[(int)CLTDIA._5000].IsComplete && !diaArr[(int)CLTDIA._5000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._5000));
            setNext(pDt, (int)CLTDIA._10000);
        }
        else if(diaArr[(int)CLTDIA._10000].IsComplete && !diaArr[(int)CLTDIA._10000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._10000));
            setNext(pDt, (int)CLTDIA._20000);
        }
        else if(diaArr[(int)CLTDIA._20000].IsComplete && !diaArr[(int)CLTDIA._20000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._20000));
            setNext(pDt, (int)CLTDIA._40000);
        }
        else if(diaArr[(int)CLTDIA._40000].IsComplete && !diaArr[(int)CLTDIA._40000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._40000));
            setNext(pDt, (int)CLTDIA._80000);
        }
        else if(diaArr[(int)CLTDIA._80000].IsComplete && !diaArr[(int)CLTDIA._80000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._80000));
            setNext(pDt, (int)CLTDIA._160000);
        }
        else if(diaArr[(int)CLTDIA._160000].IsComplete && !diaArr[(int)CLTDIA._160000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTDIA._160000));
            setNext(pDt, (int)CLTDIA._300000, _allClear: true);
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
