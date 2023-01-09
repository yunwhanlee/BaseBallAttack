using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCollectedCoin : Achivement
{
    const int CLTCOIN10000 = 0, CLTCOIN25000 = 1, CLTCOIN50000 = 2, CLTCOIN100000 = 3, CLTCOIN1000000 = 4;

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.CollectedCoin;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CollectedCoinArr[CLTCOIN1000000].IsAccept)
            setNext(pDt, CLTCOIN1000000, _allClear: true);
        else if(pDt.CollectedCoinArr[CLTCOIN100000].IsAccept)
            setNext(pDt, CLTCOIN1000000);
        else if(pDt.CollectedCoinArr[CLTCOIN50000].IsAccept)
            setNext(pDt, CLTCOIN100000);
        else if(pDt.CollectedCoinArr[CLTCOIN25000].IsAccept)
            setNext(pDt, CLTCOIN50000);
        else if(pDt.CollectedCoinArr[CLTCOIN10000].IsAccept)
            setNext(pDt, CLTCOIN25000);
        else if(!Array.Exists(pDt.CollectedCoinArr, arr => arr.IsAccept))
            setNext(pDt, CLTCOIN10000);
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

        if(pDt.CollectedCoinArr[CLTCOIN10000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN10000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN10000));
            setNext(pDt, CLTCOIN25000);
        }
        else if(pDt.CollectedCoinArr[CLTCOIN25000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN25000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN25000));
            setNext(pDt, CLTCOIN50000);
        }
        else if(pDt.CollectedCoinArr[CLTCOIN50000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN50000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN50000));
            setNext(pDt, CLTCOIN100000);
        }
        else if(pDt.CollectedCoinArr[CLTCOIN100000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN100000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN100000));
            setNext(pDt, CLTCOIN1000000);
        }
        else if(pDt.CollectedCoinArr[CLTCOIN1000000].IsComplete && !pDt.CollectedCoinArr[CLTCOIN1000000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTCOIN1000000));
            setNext(pDt, CLTCOIN1000000, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectCoin(int amount){
        DM.ins.personalData.CollectedCoin += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedCoinArr.Length; i++){
            if(pDt.CollectedCoin >= pDt.CollectedCoinArr[i].Val)
                pDt.CollectedCoinArr[i].IsComplete = true;
        }
    }
}
