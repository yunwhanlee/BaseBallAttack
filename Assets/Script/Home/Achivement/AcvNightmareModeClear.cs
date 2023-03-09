using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvNightmareModeClear : Achivement
{
    void Start(){
        var pDt = DM.ins.personalData;

        if(pDt.NightmareModeClear[0].IsComplete) cnt = 1;
        setNext(pDt, idx: 0, pDt.NightmareModeClear[0].IsAccept);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickCalimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.NightmareModeClear[0].IsComplete && !pDt.NightmareModeClear[0].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, idx: 0));
            setNext(pDt, idx: 0, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void setNightmareModeClear(){
        DM.ins.personalData.NightmareModeClear[0].IsComplete = true;
    }
}
