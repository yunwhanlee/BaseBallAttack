using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class InAppManager : MonoBehaviour
{
    [SerializeField] HomeManager hm;

    // DM.NAME id;
    void Start(){
        Debug.Log("InAppManager::DM.ins.hm= " + DM.ins.hm);
        hm = DM.ins.hm;
        // checkBtn.GetComponent<IAPButton>().enabled = false;
    }

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
                hm.setCheckBtnUninteractable();
                break;
            case "diamond_small":{
                const int value = 1000;
                hm.displayShowRewardPanel(coin: 0, diamond: value);
                DM.ins.personalData.addDiamond(value);
                break;
            }
            case "diamond_medium":{
                const int value = 5000;
                hm.displayShowRewardPanel(coin: 0, diamond: value);
                DM.ins.personalData.addDiamond(value);
                break;
            }
            case "diamond_big":{
                const int value = 10000;
                hm.displayShowRewardPanel(coin: 0, diamond: value);
                DM.ins.personalData.addDiamond(value);
                break;
            }
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
        Util._.displayNoticeMsgDialog(LANG.getTxt(LANG.TXT.PurchaseFail.ToString()));
        // init();
    }

    // private void init(){
    //     iapBtn.enabled = false;
    //     iapBtn.productId = "NULL";
    // }
}
