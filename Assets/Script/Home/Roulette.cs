using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roulette : MonoBehaviour
{
    //* OutSide
    HomeManager hm;

    enum ITEM_OBJ_NAME {coin, gem, RouletteTicket, poop};
    const float rotOffset = -22.5f;
    const int itemCnt = 8;
    [SerializeField] GameObject[] itemObjArr;  public GameObject[] ItemObjArr { get => itemObjArr; set => itemObjArr = value;}
    [SerializeField] bool isSpin; public bool IsSpin { get => isSpin; set => isSpin = value;}
    public RectTransform spinBoard;
    public RectTransform centerTf;
    public Text ticketCntTxt;
    public Image spinBtnIconImg;
    public Text spinBtnTxt;
    public Slider SpinPowerSlider;
    
    private Sprite rewardIcon;
    private int rewardPrice;

    const int SPIN_POWER_MAX = 1200;
    [Range(500, 2000)]  public int power = 1500;
    public float spinGauge = 0;

    [SerializeField] int reduceCnt = 0;
    [SerializeField] bool isRight;
    const int REWARD_IMG = 1;

    void Start()
    {
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();

        //* Lang
        spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteSpin.ToString());

        setCenterTfUI(isInit: true);
    }

    void Update(){

        ticketCntTxt.text = "x " + DM.ins.personalData.RouletteTicket.ToString();
        if(isSpin){
            float speed = Time.deltaTime * (spinGauge - reduceCnt++);
            if(speed > 0){
                setCenterTfUI(isSpin);
                spinBoard.transform.Rotate(0, 0, speed);
            }
            else{
                init();
                /*
                *   ⓵ transform.rotation : 0~1単位 -> eulerAnglesに変換する必要ある。
                *   ⓶ eulerAnglesでは、範囲が0~360まで。
                *      しかし、InspectorViewでは、範囲が-180~180まで。
                */
                float zRot = spinBoard.eulerAngles.z;
                float angle = zRot % (360 + rotOffset); //* one lap(１回り): 360°
                int index = (int)(angle / ((360 + rotOffset) / itemCnt));
                Debug.Log("Roulette::Update:: speed= " + speed + ", angle= " + angle + ", index=" + index);

                setRewardData(index);
                setCenterTfUI(isSpin);
            }
        }
        else{
            //* Power Gauge Slider
            if(rewardIcon == null){
                if(spinGauge <= 0)  isRight = true;
                else if(spinGauge > SPIN_POWER_MAX)  isRight = false;
                spinGauge += Time.deltaTime * power * (isRight? +1 : -1);
                SpinPowerSlider.value = spinGauge / SPIN_POWER_MAX;
            }
        }
    }

    public void onClickRouletteSpinBtn(){ //* -> OK Buttonにも使える！
        if(isSpin) return;
        else{
            if(rewardIcon != null){
                setRewardResult();

                setCenterTfUI(isInit: true);
                setRewardData();

                if(DM.ins.personalData.RouletteTicket <= 0)
                    onClickRouletteExitBtn();
                return;
            }
        }
        if(DM.ins.personalData.RouletteTicket <= 0) return;
        Debug.Log("onClickRouletteSpinBtn:: Roulette Spin!!");
        isSpin = true;
        DM.ins.personalData.RouletteTicket--;
    }

    public void onClickRouletteExitBtn(){
        if(isSpin) return;
        this.gameObject.SetActive(false);
        hm.homePanel.Panel.gameObject.SetActive(true);
    }

    private void init(){
        isSpin = false;
        reduceCnt = 0;
    }

    private void setCenterTfUI(bool isInit){
        centerTf.transform.localScale = Vector3.one * (isInit? 1 : 6.5f);
        centerTf.GetComponentsInChildren<Image>()[REWARD_IMG].enabled = (isInit? false : true);
        centerTf.GetComponentsInChildren<Image>()[REWARD_IMG].sprite = (isInit? null : rewardIcon);
        centerTf.GetComponentInChildren<Text>().text = (isInit? "" : rewardPrice.ToString());

        spinBtnIconImg.gameObject.SetActive(isInit? true : false);
        spinBtnTxt.text = (isInit? spinBtnTxt.text = LANG.getTxt(LANG.TXT.RouletteSpin.ToString())
            : LANG.getTxt(LANG.TXT.Get.ToString()));
    }

    private void setRewardData(int index = -1){
        rewardIcon = (index == -1)? null : itemObjArr[index].GetComponentInChildren<Image>().sprite;
        rewardPrice = (index == -1)? 0 : int.Parse(itemObjArr[index].GetComponentInChildren<Text>().text);
    }

    private void setRewardResult(){
        Debug.Log("onClickRouletteSpinBtn::setRewardResult:: rewardIcon= " + rewardIcon.name + ", rewardPrice= " + rewardPrice);
        if(rewardIcon.name.Contains(ITEM_OBJ_NAME.coin.ToString()))
            DM.ins.personalData.Coin += rewardPrice;
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.gem.ToString()))
            DM.ins.personalData.Diamond += rewardPrice;
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.RouletteTicket.ToString()))
            DM.ins.personalData.RouletteTicket += rewardPrice;
        else if(rewardIcon.name.Contains(ITEM_OBJ_NAME.poop.ToString()));
            //なし
    }
}
