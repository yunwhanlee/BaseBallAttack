using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCollectedCoin : Achivement
{
    enum CLTCOIN {
        _10000, _25000, _50000, _100000, _200000, _400000, _800000, _1000000
    };

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.CollectedCoin;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CollectedCoinArr[(int)CLTCOIN._800000].IsAccept)
            setNext(pDt, (int)CLTCOIN._1000000, _allClear: true);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._400000].IsAccept)
            setNext(pDt, (int)CLTCOIN._800000);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._200000].IsAccept)
            setNext(pDt, (int)CLTCOIN._400000);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._100000].IsAccept)
            setNext(pDt, (int)CLTCOIN._200000);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._50000].IsAccept)
            setNext(pDt, (int)CLTCOIN._100000);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._25000].IsAccept)
            setNext(pDt, (int)CLTCOIN._50000);
        else if(pDt.CollectedCoinArr[(int)CLTCOIN._10000].IsAccept)
            setNext(pDt, (int)CLTCOIN._25000);
        else if(!Array.Exists(pDt.CollectedCoinArr, arr => arr.IsAccept))
            setNext(pDt, (int)CLTCOIN._10000);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        Debug.Log("Achivement::onClickCalimBtn:: " + this.name);
        var pDt = DM.ins.personalData;
        var coinArr = pDt.CollectedCoinArr;

        if(coinArr[(int)CLTCOIN._10000].IsComplete && !coinArr[(int)CLTCOIN._10000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._10000));
            setNext(pDt, (int)CLTCOIN._25000);
        }
        else if(coinArr[(int)CLTCOIN._25000].IsComplete && !coinArr[(int)CLTCOIN._25000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._25000));
            setNext(pDt, (int)CLTCOIN._50000);
        }
        else if(coinArr[(int)CLTCOIN._50000].IsComplete && !coinArr[(int)CLTCOIN._50000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._50000));
            setNext(pDt, (int)CLTCOIN._100000);
        }
        else if(coinArr[(int)CLTCOIN._100000].IsComplete && !coinArr[(int)CLTCOIN._100000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._100000));
            setNext(pDt, (int)CLTCOIN._200000);
        }
        else if(coinArr[(int)CLTCOIN._200000].IsComplete && !coinArr[(int)CLTCOIN._200000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._200000));
            setNext(pDt, (int)CLTCOIN._400000);
        }
        else if(coinArr[(int)CLTCOIN._400000].IsComplete && !coinArr[(int)CLTCOIN._400000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._400000));
            setNext(pDt, (int)CLTCOIN._800000);
        }
        else if(coinArr[(int)CLTCOIN._800000].IsComplete && !coinArr[(int)CLTCOIN._800000].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, (int)CLTCOIN._800000));
            setNext(pDt, (int)CLTCOIN._1000000, _allClear: true);
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
