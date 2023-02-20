using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roulette : MonoBehaviour
{
    //* OutSide
    HomeManager hm;

    enum ITEM_OBJ_NAME {coin, gem, RouletteTicket, poop};
    const float rotOffset = 22.5f;
    const int itemCnt = 8;
    const int REWARD_IMG = 1;
    const int SPEED = 1000;
    [SerializeField] GameObject[] itemObjArr;  public GameObject[] ItemObjArr { get => itemObjArr; set => itemObjArr = value;}
    [SerializeField] bool isSpin;
    public RectTransform spinBoard;
    public RectTransform centerTf;
    public Text ticketCntTxt;
    public Button spinBtn;
    public Image spinBtnIconImg;
    public Text spinBtnTxt;
    public Button challengeBtn;
    public GameObject ContgraturationBlastEF;
    public Button exitBtn;
    
    
    public Sprite rewardIcon;
    private int rewardPrice;

    [SerializeField] float curSpeed;

    void OnEnable() {
        isSpin = true;
    }

    void Start(){
        Debug.Log("Roulette::Start():: DM.ins.hm= " + DM.ins.hm);
        hm = DM.ins.hm;

        curSpeed = 0;
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
                    float zRot = spinBoard.eulerAngles.z;
                    float ang = zRot % 360; //* one lap(１回り): 360°
                    int devide = (360 / itemCnt);
                    int idx =  (int)((ang + rotOffset) / devide);
                    idx = (idx == itemCnt)? 0 : idx; //* (BUG-75) RouletteのResult角度が少しずれているバグ対応。
                    Debug.Log("Roulette::Update:: speed= " + curSpeed + ", angle= " + ang + ", devide= " + devide +", index=" + idx);

                    SM.ins.sfxPlay(SM.SFX.RouletteReward.ToString());
                    setRewardData(idx);
                    setCenterTfUI(isSpin);
                    challengeBtn.gameObject.SetActive(true);
                }
                spinBoard.transform.Rotate(0, 0, curSpeed * Time.deltaTime);
            }
        }
        // float _zRot = spinBoard.eulerAngles.z;
        // float _angle = _zRot % 360; //* one lap(１回り): 360°
        // int devideVal = (360 / itemCnt);
        // int _idx = (int)((_angle + rotOffset) / devideVal);
        // _idx = _idx == itemCnt? 0 : _idx;
        // // Debug.Log("Roulette::Update:: speed= " + speed + ", angle= " + _angle + ", index=" + _index);
        // Debug.Log("_zRot= " + _zRot + " index= " + _idx + " devideVal= " + devideVal);
    }

    public void onClickRouletteSpinBtn(){ //* -> OK Buttonにも使える！
        if(DM.ins.personalData.RouletteTicket <= 0) return;

        //* GET RESULT
        if(rewardIcon != null){
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

            return;
        }
        
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
            /*
            *   ⓵ transform.rotation : 0~1単位 -> eulerAnglesに変換する必要ある。
            *   ⓶ eulerAnglesでは、範囲が0~360まで。
            *      しかし、InspectorViewでは、範囲が-180~180まで。
            */
            // float zRot = spinBoard.eulerAngles.z;
            // float angle = zRot % (360 + rotOffset); //* one lap(１回り): 360°
            // int index = (int)(angle / ((360 + rotOffset) / itemCnt));
            // Debug.Log("Roulette::Update:: speed= " + speed + ", angle= " + angle + ", index=" + index);
            // SM.ins.sfxPlay(SM.SFX.RouletteReward.ToString());
            // setRewardData(index);
            // setCenterTfUI(isSpin);
        }
    }

    public void onClickRouletteExitBtn(){
        if(isSpin && curSpeed != 0) return;
        this.gameObject.SetActive(false);
        hm.homePanel.Panel.gameObject.SetActive(true);
    }

    private void setCenterTfUI(bool isInit){
        exitBtn.gameObject.SetActive(isInit? true : false); //* 取得Btnがある場合は、Exitでそのまま出るBUG対応。
        ContgraturationBlastEF.SetActive(isInit? false : true); //* Effect追加。
        centerTf.transform.localScale = Vector3.one * (isInit? 1 : 5.5f);
        centerTf.GetComponentsInChildren<Image>()[REWARD_IMG].enabled = (isInit? false : true);
        centerTf.GetComponentsInChildren<Image>()[REWARD_IMG].sprite = (isInit? null : rewardIcon);
        centerTf.GetComponentInChildren<Text>().text = (isInit? "" : rewardPrice.ToString());

        spinBtnIconImg.gameObject.SetActive(isInit? true : false);
        // spinBtnTxt.text = (isInit? spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteSpin.ToString())
        //     : LANG.getTxt(LANG.TXT.Get.ToString()));
    }

    private void setRewardData(int index = -1){
        rewardIcon = (index == -1)? null : itemObjArr[index].GetComponentInChildren<Image>().sprite;
        rewardPrice = (index == -1)? 0 : int.Parse(itemObjArr[index].GetComponentInChildren<Text>().text);
    }

    private void setRewardResult(){
        Debug.Log("onClickRouletteSpinBtn::setRewardResult:: rewardIcon= " + rewardIcon.name + ", rewardPrice= " + rewardPrice);
        if(rewardIcon.name.Contains(ITEM_OBJ_NAME.coin.ToString()))
            DM.ins.personalData.addCoin(rewardPrice); // DM.ins.personalData.Coin += rewardPrice;
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.gem.ToString()))
            DM.ins.personalData.addDiamond(rewardPrice);
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.RouletteTicket.ToString()))
            DM.ins.personalData.addRouletteTicket(rewardPrice);
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.poop.ToString())){}
            //なし
    }
}
