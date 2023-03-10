using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvBosskillCollection : Achivement
{
    const int CLTBOSSKILL1 = 0, CLTBOSSKILL2 = 1, CLTBOSSKILL3 = 2, CLTBOSSKILL4 = 3
        ,CLTBOSSKILL5 = 4, CLTBOSSKILL6 = 5, CLTBOSSKILL7 = 6, CLTBOSSKILL8 = 7;

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.UpgradeCnt;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.BosskillCollectionArr[CLTBOSSKILL8].IsAccept)
            setNext(pDt, CLTBOSSKILL8, _allClear: true);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL7].IsAccept)
            setNext(pDt, CLTBOSSKILL8);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL6].IsAccept)
            setNext(pDt, CLTBOSSKILL7);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL5].IsAccept)
            setNext(pDt, CLTBOSSKILL6);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL4].IsAccept)
            setNext(pDt, CLTBOSSKILL5);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL3].IsAccept)
            setNext(pDt, CLTBOSSKILL4);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL2].IsAccept)
            setNext(pDt, CLTBOSSKILL3);
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL1].IsAccept)
            setNext(pDt, CLTBOSSKILL2);
        else if(!Array.Exists(pDt.BosskillCollectionArr, arr => arr.IsAccept))
            setNext(pDt, CLTBOSSKILL1);
    }

//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.BosskillCollectionArr[CLTBOSSKILL1].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL1].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL1));
            setNext(pDt, CLTBOSSKILL2);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL2].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL2].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL2));
            setNext(pDt, CLTBOSSKILL3);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL3].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL3].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL3));
            setNext(pDt, CLTBOSSKILL4);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL4].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL4].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL4));
            setNext(pDt, CLTBOSSKILL5);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL5].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL5].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL5));
            setNext(pDt, CLTBOSSKILL6);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL6].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL6].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL6));
            setNext(pDt, CLTBOSSKILL7);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL7].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL7].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL7));
            setNext(pDt, CLTBOSSKILL8);
        }
        else if(pDt.BosskillCollectionArr[CLTBOSSKILL8].IsComplete && !pDt.BosskillCollectionArr[CLTBOSSKILL8].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTBOSSKILL8));
            setNext(pDt, CLTBOSSKILL8, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectBossKill(string name) {
        Debug.Log("Achivement::AcvBosskillCollection::collectBossKill():: DM.ins.gm.bm= " + DM.ins.gm.bm);
        //* Check Boss Name
        int bossCnt = DM.ins.gm.bm.bossPrefs.Length + 1;
        Debug.Log($"Achivement::collectBossKill:: bossCnt= {bossCnt}");
        int idx = -1;
        for(int i=1; i<=bossCnt; i++){
            if(name.Contains($"Boss{i}")) idx = i;
        }
        if(idx == -1) {
            Debug.LogError("ボース名に会うテキストがないです。");
            return;
        }

        DM.ins.personalData.CollectedBossKill = idx;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.BosskillCollectionArr.Length; i++){
            if(pDt.CollectedBossKill >= pDt.BosskillCollectionArr[i].Val)
                pDt.BosskillCollectionArr[i].IsComplete = true;
        }
    }
}
