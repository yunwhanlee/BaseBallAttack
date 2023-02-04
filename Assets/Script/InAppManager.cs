using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class InAppManager : MonoBehaviour
{
    [SerializeField] HomeManager hm;
    // [SerializeField] Button checkBtn;
    // IAPButton iapBtn;


    // DM.NAME id;
    void Start(){
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        // checkBtn.GetComponent<IAPButton>().enabled = false;
    }

    // public void requestID(DM.NAME reqID){
    //     iapBtn = checkBtn.GetComponent<IAPButton>();
    //     Debug.Log("requestID:: reqID= " + reqID + "productID= " + iapBtn.productId);
    //     // id = reqID;
    //     iapBtn.enabled = true;
    //     switch(reqID){
    //         case DM.NAME.RemoveAD:
    //             iapBtn.productId = "remove_ad";
    //             break;
    //     }
    // }

/// ------------------------------------------------------------------
/// EVENT
/// ------------------------------------------------------------------
    public void onPurchaseCompleteIAP(string ip) {
        // Debug.Log($"OnPurchaseCompleteIAP:: productId= {iapBtn.productId}");
        switch(ip){
            case "remove_ad":
                hm.displayShowRewardPanel(coin: 0, diamond: 0, rouletteTicket: 0, removeAD: true);
                DM.ins.personalData.IsRemoveAD = true;
                DM.ins.setUIRemoveAD();
                break;
            case "diamond_small":
                hm.displayShowRewardPanel(coin: 0, diamond: 10000);
                DM.ins.personalData.addDiamond(10000);
                break;
            case "diamond_medium":
                hm.displayShowRewardPanel(coin: 0, diamond: 50000);
                DM.ins.personalData.addDiamond(50000);
                break;
            case "diamond_big":
                hm.displayShowRewardPanel(coin: 0, diamond: 100000);
                DM.ins.personalData.addDiamond(100000);
                break;
            case "premiumpackage":
                SM.ins.sfxPlay(SM.SFX.PurchaseSuccess.ToString());
                DM.ins.personalData.IsPurchasePremiumPack = true;

                // Set Data
                DM.ins.personalData.addRouletteTicket(LM._.PREM_PACK_ROULETTE_TICKET);
                DM.ins.personalData.addCoin(LM._.PREM_PACK_COIN); // DM.ins.personalData.Coin += LM._.PREM_PACK_COIN;
                DM.ins.personalData.addDiamond(LM._.PREM_PACK_DIAMOND);
                DM.ins.personalData.IsRemoveAD = true;

                // UI
                DM.ins.setUIRemoveAD();
                hm.checkPremiumPackPurchaseStatus();

                hm.displayShowRewardPanel(
                    coin: LM._.PREM_PACK_COIN,
                    diamond: LM._.PREM_PACK_DIAMOND,
                    rouletteTicket: LM._.PREM_PACK_ROULETTE_TICKET,
                    removeAD: true
                );
                break;
        }
    }   

    public void onPurchaseFailIAP() {
        SM.ins.sfxPlay(SM.SFX.PurchaseFail.ToString());
        Util._.displayNoticeMsgDialog("Purchase Fail");
        // init();
    }

    // private void init(){
    //     iapBtn.enabled = false;
    //     iapBtn.productId = "NULL";
    // }
}
