using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvBatCollection : Achivement
{
    const int CLTBATQUATER = 0, CLTBATHALF = 1, CLTBATALL = 2;

    void Start(){
        var pDt = DM.ins.personalData;

        List<bool> lockList = DM.ins.personalData.BatLockList;
        cnt = lockList.FindAll(list => list == false).Count;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.BatCollectionArr[CLTBATALL].IsAccept)
            setNext(pDt, CLTBATALL, _allClear: true);
        else if(pDt.BatCollectionArr[CLTBATHALF].IsAccept)
            setNext(pDt, CLTBATALL);
        else if(pDt.BatCollectionArr[CLTBATQUATER].IsAccept)
            setNext(pDt, CLTBATHALF);
        else if(!Array.Exists(pDt.BatCollectionArr, arr => arr.IsAccept))
            setNext(pDt, CLTBATQUATER);
    }
//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.BatCollectionArr[CLTBATQUATER].IsComplete && !pDt.BatCollectionArr[CLTBATQUATER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATQUATER));
            setNext(pDt, CLTBATHALF);
        }
        else if(pDt.BatCollectionArr[CLTBATHALF].IsComplete && !pDt.BatCollectionArr[CLTBATHALF].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATHALF));
            setNext(pDt, CLTBATALL);
        }
        else if(pDt.BatCollectionArr[CLTBATALL].IsComplete && !pDt.BatCollectionArr[CLTBATALL].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBATALL));
            setNext(pDt, CLTBATALL, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectBat() {
        DM.ins.personalData.CollectedBat++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.BatCollectionArr.Length; i++){
            if(pDt.CollectedBat >= pDt.BatCollectionArr[i].Val)
                pDt.BatCollectionArr[i].IsComplete = true;
        }
    }
}
