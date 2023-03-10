using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvDestroyBlocks : Achivement
{
    const int DTR100 = 0, DTR200 = 1, DTR400 = 2, DTR700 = 3, DTR1000 = 4; //* Destroy Block Cnt
    
    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.DestroyBlockCnt;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.DestroyBlockArr[DTR1000].IsAccept)
            setNext(pDt, DTR1000, _allClear: true);
        else if(pDt.DestroyBlockArr[DTR700].IsAccept)
            setNext(pDt, DTR1000);
        else if(pDt.DestroyBlockArr[DTR400].IsAccept)
            setNext(pDt, DTR700);
        else if(pDt.DestroyBlockArr[DTR200].IsAccept)
            setNext(pDt, DTR400);
        else if(pDt.DestroyBlockArr[DTR100].IsAccept)
            setNext(pDt, DTR200);
        else if(!Array.Exists(pDt.DestroyBlockArr, arr => arr.IsAccept))
            setNext(pDt, DTR100);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;
        if(pDt.DestroyBlockArr[DTR100].IsComplete && !pDt.DestroyBlockArr[DTR100].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, DTR100));
            setNext(pDt, DTR200);
        }
        else if(pDt.DestroyBlockArr[DTR200].IsComplete && !pDt.DestroyBlockArr[DTR200].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, DTR200));
            setNext(pDt, DTR400);
        }
        else if(pDt.DestroyBlockArr[DTR400].IsComplete && !pDt.DestroyBlockArr[DTR400].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, DTR400));
            setNext(pDt, DTR700);
        }
        else if(pDt.DestroyBlockArr[DTR700].IsComplete && !pDt.DestroyBlockArr[DTR700].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, DTR700));
            setNext(pDt, DTR1000);
        }
        else if(pDt.DestroyBlockArr[DTR1000].IsComplete && !pDt.DestroyBlockArr[DTR1000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, DTR1000));
            setNext(pDt, DTR1000, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void addDestroyBlockCnt(){
        DM.ins.personalData.DestroyBlockCnt++;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.DestroyBlockArr.Length; i++){
            if(pDt.DestroyBlockCnt >= pDt.DestroyBlockArr[i].Val)
                pDt.DestroyBlockArr[i].IsComplete = true;
        }
    }
}
