using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvStageClear : Achivement
{
    const int STG10 = 0, STG30 = 1, STG60 = 2, STG100 = 3, STG160 = 4; //* Stage Clear 

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.ClearStage;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.StageClearArr[STG160].IsAccept)
            setNext(pDt, STG160, _allClear: true);
        else if(pDt.StageClearArr[STG100].IsAccept)
            setNext(pDt, STG160);
        else if(pDt.StageClearArr[STG60].IsAccept)
            setNext(pDt, STG100);
        else if(pDt.StageClearArr[STG30].IsAccept)
            setNext(pDt, STG60);
        else if(pDt.StageClearArr[STG10].IsAccept)
            setNext(pDt, STG30);
        else if(!Array.Exists(pDt.StageClearArr, arr => arr.IsAccept))
            setNext(pDt, STG10);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;

        if(pDt.StageClearArr[STG10].IsComplete && !pDt.StageClearArr[STG10].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG10));
                    setNext(pDt, STG30);
                }
                else if(pDt.StageClearArr[STG30].IsComplete && !pDt.StageClearArr[STG30].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG30));
                    setNext(pDt, STG60);
                }
                else if(pDt.StageClearArr[STG60].IsComplete && !pDt.StageClearArr[STG60].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG60));
                    setNext(pDt, STG100);
                }
                else if(pDt.StageClearArr[STG100].IsComplete && !pDt.StageClearArr[STG100].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG100));
                    setNext(pDt, STG160);
                }
                else if(pDt.StageClearArr[STG160].IsComplete && !pDt.StageClearArr[STG160].IsAccept){
                    DM.ins.personalData.addDiamond(acceptReward(pDt, STG160));
                    setNext(pDt, STG160, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void setStageClear(int stage){
        //* Achivement (StageClear IsComplete)
        for(int i=0; i<DM.ins.personalData.StageClearArr.Length; i++){
            if(stage >= DM.ins.personalData.StageClearArr[i].Val)
                DM.ins.personalData.StageClearArr[i].IsComplete = true;
        }
    }
}
