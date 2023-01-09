using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCharactorCollection : Achivement
{
    const int CLTCHARAQUATER = 0, CLTCHARAHALF = 1, CLTCHARAALL = 2;

    void Start(){
        var pDt = DM.ins.personalData;

        List<bool> lockList = DM.ins.personalData.CharaLockList;
        cnt = lockList.FindAll(list => list == false).Count;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CharaCollectionArr[CLTCHARAALL].IsAccept)
            setNext(pDt, CLTCHARAALL, _allClear: true);
        else if(pDt.CharaCollectionArr[CLTCHARAHALF].IsAccept)
            setNext(pDt, CLTCHARAALL);
        else if(pDt.CharaCollectionArr[CLTCHARAQUATER].IsAccept)
            setNext(pDt, CLTCHARAHALF);
        else if(!Array.Exists(pDt.CharaCollectionArr, arr => arr.IsAccept))
            setNext(pDt, CLTCHARAQUATER);
    }

    // Update is called once per frame
    void Update(){
        base.Update();
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickCalimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.CharaCollectionArr[CLTCHARAQUATER].IsComplete && !pDt.CharaCollectionArr[CLTCHARAQUATER].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAQUATER));
            setNext(pDt, CLTCHARAHALF);
        }
        else if(pDt.CharaCollectionArr[CLTCHARAHALF].IsComplete && !pDt.CharaCollectionArr[CLTCHARAHALF].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAHALF));
            setNext(pDt, CLTCHARAALL);
        }
        else if(pDt.CharaCollectionArr[CLTCHARAALL].IsComplete && !pDt.CharaCollectionArr[CLTCHARAALL].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCHARAALL));
            setNext(pDt, CLTCHARAALL, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectChara() {
        DM.ins.personalData.CollectedChara++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CharaCollectionArr.Length; i++){
            if(pDt.CollectedChara >= pDt.CharaCollectionArr[i].Val)
                pDt.CharaCollectionArr[i].IsComplete = true;
        }
    }
}
