using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achivement : MonoBehaviour
{   
    //* Outside
    HomeManager hm;

    //* Value
    enum REWARD_TYPE { COIN, DIAMOND, ROULETTE_TICKET };
    [SerializeField] REWARD_TYPE rewardType = REWARD_TYPE.DIAMOND;
    [SerializeField] int step = 1, maxStep;
    [SerializeField] int cnt, max;
    [SerializeField] int rewardValue;

    //* Obj
    Image panelImg, rewardIconImg;
    Text infoTxt, valueTxt, rewardTxt;
    Button claimBtn;

    void Start()
    {
        //* Assign Object
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();

        panelImg = this.GetComponent<Image>();
        infoTxt = this.transform.Find("InfoTxt").GetComponent<Text>();
        valueTxt = this.transform.Find("ValueTxt").GetComponent<Text>();
        rewardTxt = this.transform.Find("RewardTxt").GetComponent<Text>();
        rewardIconImg = this.transform.Find("RewardIcon").GetComponent<Image>();
        claimBtn = this.transform.Find("ClaimBtn").GetComponent<Button>();

        //* Set Icon
        rewardIconImg.sprite = (rewardType == REWARD_TYPE.COIN)? hm.rewardIconSprs[(int)REWARD_TYPE.COIN]
            : (rewardType == REWARD_TYPE.DIAMOND)? hm.rewardIconSprs[(int)REWARD_TYPE.DIAMOND]
            : (rewardType == REWARD_TYPE.ROULETTE_TICKET)? hm.rewardIconSprs[(int)REWARD_TYPE.ROULETTE_TICKET] : null;

        //* Set Value
        switch(this.name){
            case "StageClear" : {
                cnt = DM.ins.personalData.LastStage;
                max = 10;
                if(DM.ins.personalData.Stage10Clear){
                    max = 30;
                }
                else if(DM.ins.personalData.Stage30Clear){
                    max = 60;
                }
                else if(DM.ins.personalData.Stage60Clear){
                    max = 100;
                }
                else if(DM.ins.personalData.Stage100Clear){
                    max = 160;
                }
                else if(DM.ins.personalData.Stage160Clear){
                    
                }
            }
                break;
            case "AtvSkillCollector" : {
                List<bool> lockList = DM.ins.personalData.SkillLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "BatCollector" : {
                List<bool> lockList = DM.ins.personalData.BatLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "CharactorCollector" : {
                List<bool> lockList = DM.ins.personalData.CharaLockList;
                max = lockList.Count;
                cnt = lockList.FindAll(list => list == false).Count;
                }
                break;
            case "DestroyBlocks" :
                break;
            case "CoinRich" :
                break;
            case "DiamondRich" :
                break;
            case "NormalModeClear" :
                break;
            case "HardModeClear" :
                break;
            case "KillBoss" :
                break;
            case "RouletteTicketCollector" :
                break;
            case "UpgradeMaster" :
                break;
        }

        //* Set Color
        panelImg.color = (cnt == max)? Color.white : Color.gray;
        claimBtn.GetComponent<Image>().color = (cnt == max)? Color.white : Color.gray;

        //* Set Text
        valueTxt.text = cnt + "/" + max;
    }
}
