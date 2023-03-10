using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcvCollectedRouletteTicket : Achivement
{
    const int CLTTICKET10 = 0, CLTTICKET50 = 1, CLTTICKET100 = 2, CLTTICKET150 = 3, CLTTICKET300 = 4;

    void Start(){
        var pDt = DM.ins.personalData;

        cnt = pDt.CollectedRouletteTicket;
        Debug.Log($"Achivement:: {this.name}:: cnt= {cnt}");
        //* Init
        if(pDt.CollectedRouletteTicketArr[CLTTICKET300].IsAccept)
            setNext(pDt, CLTTICKET300, _allClear: true);
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET150].IsAccept)
            setNext(pDt, CLTTICKET300);
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET100].IsAccept)
            setNext(pDt, CLTTICKET150);
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET50].IsAccept)
            setNext(pDt, CLTTICKET100);
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET10].IsAccept)
            setNext(pDt, CLTTICKET50);
        else if(!Array.Exists(pDt.CollectedRouletteTicketArr, arr => arr.IsAccept))
            setNext(pDt, CLTTICKET10);
    }


//*---------------------------------------
//*  Button Event Function
//*---------------------------------------
    public override void onClickClaimBtn(){
        var pDt = DM.ins.personalData;

        if(pDt.CollectedRouletteTicketArr[CLTTICKET10].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET10].IsAccept){
            DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET10));
            setNext(pDt, CLTTICKET50);
        }
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET50].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET50].IsAccept){
            DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET50));
            setNext(pDt, CLTTICKET100);
        }
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET100].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET100].IsAccept){
            DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET100));
            setNext(pDt, CLTTICKET150);
        }
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET150].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET150].IsAccept){
            DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET150));
            setNext(pDt, CLTTICKET300);
        }
        else if(pDt.CollectedRouletteTicketArr[CLTTICKET300].IsComplete && !pDt.CollectedRouletteTicketArr[CLTTICKET300].IsAccept){
            DM.ins.personalData.addRouletteTicket(acceptReward(pDt, CLTTICKET300));
            setNext(pDt, CLTTICKET300, _allClear: true);
        }
    }
//*---------------------------------------
//*  Function
//*---------------------------------------
    static public void collectRouletteTicket(int amount){
        DM.ins.personalData.CollectedRouletteTicket += amount;

        var pDt = DM.ins.personalData;
        for(int i=0; i<pDt.CollectedRouletteTicketArr.Length; i++){
            if(pDt.CollectedRouletteTicket >= pDt.CollectedRouletteTicketArr[i].Val)
                pDt.CollectedRouletteTicketArr[i].IsComplete = true;
        }
    }
}
