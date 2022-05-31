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
    [Header("<-- UI Dialog -->")]
    public DialogUI homeDialog;
    public DialogUI selectCharaDialog;

    //TODO public DialogUI selectBat;
    //TODO public DialogUI cashShop;
    
    
    

    void Start()
    {
        onClickBtnGoToDialog("Home");
    }

    void Update()
    {
        
    }

    //* ----------------------------------------------------------------
    //*   UI Button
    //* ----------------------------------------------------------------
    public void onClickBtnGoToDialog(string name){
        //* Current Model Data & ParentTf
        DataManager.ins.SelectType = name;

        var curChara = DataManager.ins.CharaPfs[DataManager.ins.personalData.SelectCharaIdx];
        //TODO
        var curBat = DataManager.ins.BatPfs[DataManager.ins.personalData.SelectBatIdx];//[DataManager.ins.personalData.SelectCharaIdx];

        var parentTf = homeDialog.Panel.transform.Find("BackGroundGroup").transform.Find("ModelTf");
        var childs = parentTf.GetComponentsInChildren<Transform>();

        
        
        switch(name){
            case "Home" : 
                //* Model Create
                Instantiate(curChara, Vector3.zero, Quaternion.identity, parentTf);

                //* Home Model Delete
                if(0 < parentTf.childCount){
                    int i = 0;
                    Array.ForEach(childs, child => {
                        if(i != 0){ //* (BUG) [0]は親だから処理しない。
                            Destroy(child.gameObject);
                        }
                        i++;
                    });
                }

                //* UI
                homeDialog.Panel.gameObject.SetActive(true);
                homeDialog.GoBtn.gameObject.SetActive(false);
                selectCharaDialog.Panel.gameObject.SetActive(false);
                DataManager.ins.ScrollViewChara.gameObject.SetActive(false);
                DataManager.ins.ScrollViewBat.gameObject.SetActive(false);

                break;

            case "Chara" : 
                DataManager.ins.ScrollViewChara.gameObject.SetActive(true);

                //* UI
                homeDialog.Panel.gameObject.SetActive(false);
                homeDialog.GoBtn.gameObject.SetActive(true);
                selectCharaDialog.Panel.gameObject.SetActive(true);
                break;
                
            case "Bat" : 
                DataManager.ins.ScrollViewBat.gameObject.SetActive(true);

                //* UI
                homeDialog.Panel.gameObject.SetActive(false);
                homeDialog.GoBtn.gameObject.SetActive(true);
                selectCharaDialog.Panel.gameObject.SetActive(true);
                break;
        }
    }
    public void onClickResetBtn(){
        // ItemInfo[] items = DataManager.ins.ContentTf.GetComponentsInChildren<ItemInfo>();
        DataManager.ins.personalData.reset();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
