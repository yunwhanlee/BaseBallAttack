using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Roulette : MonoBehaviour
{
    //* OutSide
    HomeManager hm;

    enum ITEM {coin, gem, RouletteTicket, bomb};
    const float rotOffset = 22.5f;
    const int itemCnt = 8;
    const int SPEED = 1000;
    const int REWARD_IMG = 1;
    const int MAX_CHALLENGE_CNT = 3;
    [SerializeField] GameObject[] itemObjArr;
    [SerializeField] List<Sprite> originItemSprList;
    [SerializeField] List<string> originItemPriceStrList;
    [SerializeField] bool isSpin;
    [SerializeField] bool isChallengeTrigger;
    [SerializeField] int challengeCnt;

    public RectTransform spinBoard;
    public RectTransform centerTf;
    [SerializeField] Image centerImg;
    [SerializeField] Text centerTxt;
    public Text ticketCntTxt;
    public Button spinBtn;
    public Image spinBtnIconImg;
    public Text spinBtnTxt;
    public Button challengeBtn;
    public GameObject ContgraturationBlastEF;
    public Button exitBtn;
    public Sprite rewardIcon;
    public Sprite iconClover;
    public Sprite iconBomb;
    private int rewardPrice;

    [SerializeField] float curSpeed;

    void OnEnable() {
        isSpin = true;
        challengeCnt = 0;
        isChallengeTrigger = false;

        if(originItemSprList.Count > 0)
            initItemList();
    }

    void Start(){
        Debug.Log("Roulette::Start():: DM.ins.hm= " + DM.ins.hm);
        hm = DM.ins.hm;
        
        //* Set Origin Item List
        Array.ForEach(itemObjArr, arr => {
            var img = arr.GetComponentsInChildren<Image>()[0];
            var txt = arr.GetComponentsInChildren<Text>()[0];
            
            originItemSprList.Add(img.sprite);
            originItemPriceStrList.Add(txt.text);
        });

        centerImg = centerTf.GetComponentsInChildren<Image>()[REWARD_IMG];
        centerTxt = centerTf.GetComponentInChildren<Text>();

        curSpeed = 0;
        isChallengeTrigger = false;
        challengeBtn.gameObject.SetActive(false);

        //* Lang
        spinBtnTxt.text = LANG.getTxt(LANG.TXT.Ready.ToString());

        setCenterTfUI(isInit: true);
    }

    void Update(){
        ticketCntTxt.text = "x " + DM.ins.personalData.RouletteTicket.ToString();
        //* ROTATE
        if(curSpeed > 0){
            if(isSpin){
                spinBoard.transform.Rotate(0, 0, curSpeed * Time.deltaTime);
                // spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteStop.ToString());
                setCenterTfUI(isSpin);
            }
            else{
                const float t = 0.9f; //* AとBの返し値。
                curSpeed = (curSpeed <= 2)? 0 : Mathf.Lerp(0, curSpeed, t);

                if(curSpeed == 0){
                    spinBtn.interactable = true;
                    spinBtnTxt.text = LANG.getTxt(LANG.TXT.Get.ToString());
                    
                    //* 角度による、Index値
                    /*
                    *   ⓵ transform.rotation : 0~1単位 -> eulerAnglesに変換する必要ある。
                    *   ⓶ eulerAnglesでは、範囲が0~360まで。
                    *      しかし、InspectorViewでは、範囲が-180~180まで。
                    */
                    float zRot = spinBoard.eulerAngles.z;
                    float ang = zRot % 360; //* one lap(１回り): 360°
                    int devide = (360 / itemCnt);
                    int idx =  (int)((ang + rotOffset) / devide);
                    idx = (idx == itemCnt)? 0 : idx; //* (BUG-75) RouletteのResult角度が少しずれているバグ対応。
                    Debug.Log("Roulette::Update:: speed= " + curSpeed + ", angle= " + ang + ", devide= " + devide +", index=" + idx);

                    //* Reward
                    setRewardData(idx);
                    setCenterTfUI(isSpin);
                }
                spinBoard.transform.Rotate(0, 0, curSpeed * Time.deltaTime);
            }
        }
    }

    private void initItemList(){
        challengeCnt = 0;

        int i=0;
        Array.ForEach(itemObjArr, arr => {
            var img = arr.GetComponentsInChildren<Image>()[0];
            var txt = arr.GetComponentsInChildren<Text>()[0];

            img.sprite = originItemSprList[i];
            txt.text = originItemPriceStrList[i];

            i++;
        });
    }
    public void onClickRouletteSpinBtn(){ //* -> OK Buttonにも使える！
        if(DM.ins.personalData.RouletteTicket <= 0) return;

        //* GET RESULT
        if(rewardIcon != null){
            if(!isChallengeTrigger){
                Debug.Log("onClickRouletespinBtn:: GET RESULT");
                DM.ins.personalData.RouletteTicket--;
                SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());

                spinBoard.rotation = Quaternion.identity;
                spinBtnTxt.text = LANG.getTxt(LANG.TXT.Ready.ToString());
                challengeBtn.gameObject.SetActive(false);

                setRewardResult();
                setCenterTfUI(isInit: true);
                setRewardData();

                if(DM.ins.personalData.RouletteTicket <= 0)
                    onClickRouletteExitBtn();
            }else{
                // isChallengeTrigger = false;
                Debug.Log("onClickRouletespinBtn:: CHALLENGE DOUBLE");
                SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());

                spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteSpin.ToString());
                challengeBtn.gameObject.SetActive(false);

                setSpin();
            }
            return;
        }
        setSpin();
    }

    private void setSpin(){
        if(!isSpin){ 
            Debug.Log("onClickRouletespinBtn:: SET STOP");
            isSpin = true;
            curSpeed = SPEED;
            spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteStop.ToString());
        }
        else{
            Debug.Log($"onClickRouletespinBtn:: SET SPIN:: speed= {curSpeed}");
            isSpin = false;
            spinBtn.interactable = (curSpeed > 0)? false : true;
            spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteSpin.ToString());
            isChallengeTrigger = false;
        }
    }

    public void onClickRouletteExitBtn(){
        if(isSpin && curSpeed != 0) return;
        this.gameObject.SetActive(false);
        hm.homePanel.Panel.gameObject.SetActive(true);
    }

    public void onClickChallengeBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        isChallengeTrigger = true;
        challengeCnt++;
        centerTf.transform.localScale = Vector3.one;
        challengeBtn.gameObject.SetActive(false);
        spinBtnTxt.text = LANG.getTxt(LANG.TXT.Ready.ToString());

        int i = 0;
        Array.ForEach(itemObjArr, list => {
            var img = list.GetComponentsInChildren<Image>()[0];
            var txt = list.GetComponentsInChildren<Text>()[0];
            int v = (challengeCnt == 1)? 2 : (challengeCnt == 2)? 4 : 8;

            if(i % v == 0) {
                img.sprite = rewardIcon;
                txt.text = (rewardPrice * 2).ToString();
            }
            else {
                img.sprite = iconBomb;
                txt.text = (0).ToString();
            }
            i++;
        });

    }

    private void setCenterTfUI(bool isInit){
        if(isInit){
            exitBtn.gameObject.SetActive(true);
            spinBtnIconImg.gameObject.SetActive(true);
            ContgraturationBlastEF.SetActive(false);
            
            centerTf.transform.localScale = Vector3.one;
            centerImg.enabled = false;
            centerImg.sprite = null;
            centerTxt.text = "";
        }
        else{
            exitBtn.gameObject.SetActive(false);
            spinBtnIconImg.gameObject.SetActive(false);

            if(rewardIcon.name.Contains(ITEM.bomb.ToString())){
                SM.ins.sfxPlay(SM.SFX.FireBallExplosion.ToString());
                challengeBtn.gameObject.SetActive(false);
                spinBtnTxt.text = LANG.getTxt(LANG.TXT.Back.ToString()); 
            }
            else{
                SM.ins.sfxPlay(SM.SFX.RouletteReward.ToString());
                ContgraturationBlastEF.SetActive(true);
                if(challengeCnt < MAX_CHALLENGE_CNT){
                    challengeBtn.gameObject.SetActive(true);
                    int multiplyBonus = challengeCnt == 0? 2 : challengeCnt == 1? 4 : 8;
                    challengeBtn.GetComponentInChildren<Text>().text = 
                        $"x{multiplyBonus.ToString()} {LANG.getTxt(LANG.TXT.Challenge.ToString())} ";
                }
            }

            
            centerTf.transform.localScale = Vector3.one * 5.5f;
            centerImg.enabled = true;
            centerImg.sprite = rewardIcon;
            centerTxt.text = rewardPrice.ToString();
        }
    }

    private void setRewardData(int index = -1){
        rewardIcon = (index == -1)? null : itemObjArr[index].GetComponentInChildren<Image>().sprite;
        rewardPrice = (index == -1)? 0 : int.Parse(itemObjArr[index].GetComponentInChildren<Text>().text);
    }

    private void setRewardResult(){
        Debug.Log("onClickRouletteSpinBtn::setRewardResult:: rewardIcon= " + rewardIcon.name + ", rewardPrice= " + rewardPrice);
        if(rewardIcon.name.Contains(ITEM.coin.ToString())){
            SM.ins.sfxPlay(SM.SFX.DropBoxCoinPick.ToString());
            DM.ins.personalData.addCoin(rewardPrice); // DM.ins.personalData.Coin += rewardPrice;
        }
        else if(rewardIcon.name.Contains(ITEM.gem.ToString())){
            SM.ins.sfxPlay(SM.SFX.DropBoxCoinPick.ToString());
            DM.ins.personalData.addDiamond(rewardPrice);
        }
        else if(rewardIcon.name.Contains(ITEM.RouletteTicket.ToString())){
            SM.ins.sfxPlay(SM.SFX.DropBoxCoinPick.ToString());
            DM.ins.personalData.addRouletteTicket(rewardPrice);
        }
        else if(rewardIcon.name.Contains(ITEM.bomb.ToString())) {}

        initItemList();
    }
}
