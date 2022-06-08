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
    DataManager dm;

    [Header("<--Select Panel Color-->")]
    [SerializeField] Image selectPanelScrollBG;  public Image SelectPanelScrollBG {get => selectPanelScrollBG; set => selectPanelScrollBG = value;}
    public enum DlgBGColor {Chara, Bat, Skill, CashShop}
    [SerializeField] Color[] selectPanelColors;
    
    [Header("<-- UI -->")]
    public DialogUI homeDialog;
    public DialogUI selectDialog;
    public Button   startGameBtn;

    [Header("<-- Model -->")]
    [SerializeField] Transform modelTf;   public Transform ModelTf {get => modelTf; set => modelTf = value;}



    //TODO public DialogUI selectBat;
    //TODO public DialogUI cashShop;

    void Start()
    {
        dm = DataManager.ins;
        onClickBtnGoToDialog("Home");
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnGoToDialog(string name){
        //* Current Model Data & ParentTf
        dm.SelectType = name;
        var curChara = dm.scrollviews[0].Prefs[dm.personalData.SelectCharaIdx];
        var curBat = dm.scrollviews[1].Prefs[dm.personalData.SelectBatIdx];
        // var modelTf = homeDialog.Panel.transform.Find("BackGroundGroup").transform.Find("ModelTf");
        
        switch(name){
            case "Home" : 
                createCurModel(curChara, curBat, modelTf);
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
        DlgBGColor dlgBgClr = (type == "Chara")? DlgBGColor.Chara
        : (type == "Bat")? DlgBGColor.Bat
        : (type == "Skill")? DlgBGColor.Skill
        : DlgBGColor.CashShop;

        //* Active GUI
        switch(type){
            case "Home":
                homeDialog.Panel.gameObject.SetActive(true);
                homeDialog.GoBtn.gameObject.SetActive(false);
                selectDialog.Panel.gameObject.SetActive(false);
                Array.ForEach(dm.scrollviews, sv => sv.ScrollRect.gameObject.SetActive(false));
                break;
            default : 
                homeDialog.Panel.gameObject.SetActive(false);
                homeDialog.GoBtn.gameObject.SetActive(true);
                selectDialog.Panel.gameObject.SetActive(true);
                SelectPanelScrollBG.color = selectPanelColors[(int)dlgBgClr];
                dm.scrollviews[(int)dlgBgClr].ScrollRect.gameObject.SetActive(true);
                break;
        }
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickStartGameBtn(){
        //* Model Copy
        var playerModel = modelTf.GetChild(0);
        playerModel.GetComponent<Animator>().SetBool("IsIdle", false); //Ready Pose
        Instantiate(playerModel, Vector3.zero, Quaternion.identity, DataManager.ins.transform);

        SceneManager.LoadScene("Play");
    }

    public void onClickResetBtn(){
        DataManager.ins.personalData.reset();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
