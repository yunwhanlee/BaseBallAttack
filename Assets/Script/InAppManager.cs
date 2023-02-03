using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
    [SerializeField] HomeManager hm;

    DM.NAME id;
    void Start(){
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
    }

    public void requestID(DM.NAME reqID){
        id = reqID;

    }

    public void OnPurchaseCompleteIAP() {
        switch(id){
            case DM.NAME.RemoveAD:
                hm.displayShowRewardPanel(coin: 0, diamond: 0, rouletteTicket: 0, removeAD: true);
                DM.ins.personalData.IsRemoveAD = true;
                DM.ins.setUIRemoveAD();
                break;
        }
    }   

    public void OnPurchaseFailIAP() {

    }
}
