using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]   public class DialogUI{
    //* Value
    [SerializeField] GameObject panel;   public GameObject Panel {get => panel; set => panel = value;}
    [SerializeField] Button goBtn;     public Button GoBtn {get => goBtn; set => goBtn = value;}

    //* Constructor
    DialogUI(GameObject panel, Button goBtn){
        this.panel = panel;
        this.goBtn = goBtn;
    }

    //* Method
}

public class HomeManager : MonoBehaviour
{
    //* OutSide

    [Header("<--Select Panel Color-->")]
    [SerializeField] Image selectPanelScrollBG;  public Image SelectPanelScrollBG {get => selectPanelScrollBG; set => selectPanelScrollBG = value;}
    public enum DlgBGColor {Chara, Bat, Skill, CashShop}
    [SerializeField] Color[] selectPanelColors;
    
    [Header("<-- UI -->")]
    public DialogUI homeDialog;
    public DialogUI selectDialog;
    public Button startGameBtn;
    [SerializeField] Image selectSkillImg;  public Image SelectSkillImg {get => selectSkillImg; set => selectSkillImg = value;}

    [Header("<-- Model -->")]
    [SerializeField] Transform modelTf;   public Transform ModelTf {get => modelTf; set => modelTf = value;}

    //TODO public DialogUI cashShop;

    void Start()
    {
        onClickBtnGoToDialog("Home");
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnGoToDialog(string name){
        //* Current Model Data & ParentTf
        DM.ins.SelectType = name;
        var curChara = DM.ins.scrollviews[(int)DM.ITEM.Chara].Prefs[DM.ins.personalData.SelectCharaIdx];
        var curBat = DM.ins.scrollviews[(int)DM.ITEM.Bat].Prefs[DM.ins.personalData.SelectBatIdx];
        
        switch(name){
            case "Home" : 
                createCurModel(curChara, curBat, modelTf);
                setSelectSkillImg();
                setGUI(name);
                break;
            case "Chara" : 
                setGUI(name);
                break;
            case "Bat" : 
                setGUI(name);
                break;
            case "Skill" : 
                setGUI(name);
                break;
            case "CashShop":
                //TODO
                break;
        }
    }

    public void onClickStartGameBtn(){
        //* Model Copy
        var playerModel = modelTf.GetChild(0);
        playerModel.GetComponent<Animator>().SetBool("IsIdle", false); //Ready Pose
        playerModel.SetParent(DM.ins.transform);
        // var copyModel = Instantiate(playerModel, Vector3.zero, Quaternion.identity, DM.ins.transform);

        //* Set Item Passive Data
        int[] lvArrTemp = getItemPsvLvArr(playerModel);
        DM.ins.personalData.ItemPassive.setLvArr(lvArrTemp);

        SceneManager.LoadScene("Play");
    }

    public void onClickResetBtn(){
        DM.ins.personalData.reset();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    //* ----------------------------------------------------------------
    //* Private Function
    //* ----------------------------------------------------------------
    private int[] getItemPsvLvArr(Transform playerModel){
        // Debug.Log("getItemPsvLvArr:: playerModel= " + playerModel);
        ItemInfo[] childs = playerModel.GetComponentsInChildren<ItemInfo>();
        int len = childs[0].ItemPassive.Arr.Length;
        Debug.Log("lenght= " + len);
        int[] lvArrTemp = new int[len];
        Array.ForEach(childs, child=>{
            for(int i=0; i<len; i++){
                lvArrTemp[i] += child.ItemPassive.Arr[i].lv;
                // Debug.LogFormat("lvArrTemp[{0}]= {1}", i,lvArrTemp[i]);
            }
        });
        return lvArrTemp;
    }

    private void createCurModel(GameObject chara, GameObject bat, Transform modelTf){
        var childs = modelTf.GetComponentsInChildren<Transform>();
        // Chara
        var charaIns = Instantiate(chara, Vector3.zero, Quaternion.identity, modelTf) as GameObject;
        // Bat
        Transform rightArmTf = charaIns.transform.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find("RightArm");
        Instantiate(bat, bat.transform.position, bat.transform.rotation, rightArmTf);
        
        //* Home Model Delete
        if(0 < modelTf.childCount){
            for(int i=0;i<childs.Length;i++){
                if(i != 0) //* (BUG) [0]は親だから処理しない。
                    Destroy(childs[i].gameObject);
            }
        }
    }
    private void setGUI(string type){
        //* Set Type Hash Index
        int selectType = (type == DM.ITEM.Chara.ToString())? (int)DM.ITEM.Chara
            : (type == DM.ITEM.Bat.ToString())? (int)DM.ITEM.Bat
            : (type == DM.ITEM.Skill.ToString())? (int)DM.ITEM.Skill
            : -9999;

        //* Active GUI
        switch(type){
            case "Home":
                homeDialog.Panel.gameObject.SetActive(true);
                homeDialog.GoBtn.gameObject.SetActive(false);
                selectDialog.Panel.gameObject.SetActive(false);
                Array.ForEach(DM.ins.scrollviews, sv => sv.ScrollRect.gameObject.SetActive(false));
                
                break;
            default : 
                homeDialog.Panel.gameObject.SetActive(false);
                homeDialog.GoBtn.gameObject.SetActive(true);
                selectDialog.Panel.gameObject.SetActive(true);
                SelectPanelScrollBG.color = selectPanelColors[selectType];
                DM.ins.scrollviews[selectType].ScrollRect.gameObject.SetActive(true);

                float width = Mathf.Abs(DM.ins.ModelContentPref.rect.width);
                int index = (type == DM.ITEM.Chara.ToString())? DM.ins.personalData.SelectCharaIdx : DM.ins.personalData.SelectBatIdx;
                float saveModelPosX = -Mathf.Abs(Mathf.FloorToInt(width * (index+1)));
                DM.ins.scrollviews[selectType].ContentTf.anchoredPosition = new Vector2(saveModelPosX, -500);
                break;
        }
    }

    public void setSelectSkillImg(){
        Debug.Log("setSelectSkillImgAtHome():: DM.ins.personalData.SelectSkillIdx= " + DM.ins.personalData.SelectSkillIdx);
        var prefs = DM.ins.scrollviews[(int)DM.ITEM.Skill].Prefs;
        int curIdx = DM.ins.personalData.SelectSkillIdx;
        var sprite = prefs[curIdx].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        SelectSkillImg.sprite = sprite;
    }
}
