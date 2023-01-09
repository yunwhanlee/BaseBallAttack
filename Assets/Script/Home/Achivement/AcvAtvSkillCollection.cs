using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvAtvSkillCollection : Achivement
{
    const int CLTSKILLHALF = 0, CLTSKILLALL = 1;

    void Start(){
        var pDt = DM.ins.personalData;

        List<bool> lockList = DM.ins.personalData.SkillLockList;
        cnt = lockList.FindAll(list => list == false).Count;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.AtvSkillCollectionArr[CLTSKILLALL].IsAccept)
            setNext(pDt, CLTSKILLALL, _allClear: true);
        else if(pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsAccept)
            setNext(pDt, CLTSKILLALL);
        else if(!Array.Exists(pDt.AtvSkillCollectionArr, arr => arr.IsAccept))
            setNext(pDt, CLTSKILLHALF);
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

        if(pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsComplete && !pDt.AtvSkillCollectionArr[CLTSKILLHALF].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTSKILLHALF));
            setNext(pDt, CLTSKILLALL);
        }
        else if(pDt.AtvSkillCollectionArr[CLTSKILLALL].IsComplete && !pDt.AtvSkillCollectionArr[CLTSKILLALL].IsAccept){
            DM.ins.personalData.addDiamond(acceptReward(pDt, CLTSKILLALL));
            setNext(pDt, CLTSKILLALL, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectAtvSkill() {
        DM.ins.personalData.CollectedAtvSkill++;
        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.AtvSkillCollectionArr.Length; i++){
            if(pDt.CollectedAtvSkill >= pDt.AtvSkillCollectionArr[i].Val)
                pDt.AtvSkillCollectionArr[i].IsComplete = true;
        }
    }
}
