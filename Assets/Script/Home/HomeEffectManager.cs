using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HomeEffectManager : MonoBehaviour
{
    //* Outside
    public HomeManager hm;
    public Transform mainCanvasTf; 

    void Start() {
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        mainCanvasTf = GameObject.Find("MainCanvas").transform;
    }

    public GameObject itemBuyEF;
    public GameObject congratuBlastRainbowEF;
    public GameObject upgradeItemEF;

    public void createItemBuyEF(ScrollViewEvent scrollView, GameObject SkillImgObj, string type){
        //* (BUG-26) HomeEFManager::createItemBuyEF:: parent -> MainCanvasにして、後ろのGUIボタンが押されるバグを対応。
        var ins = Instantiate(itemBuyEF, mainCanvasTf);

        //* Title Lang
        Text title = Array.Find(ins.GetComponentsInChildren<Text>(), (txtObj) => txtObj.name == "TitleTxt");
        title.text = LANG.getTxt(LANG.TXT.Purchase_Complete.ToString());

        Debug.Log("!!!! scrollView.name -> " + scrollView.name);
        
        //* (BUG-7) ItemBuyEF-UIがスキルの場合は、似合わないこと対応：該当なスキルイメージ生成、メダルの真ん中に位置調整。
        if(type == DM.PANEL.Skill.ToString()){
            var canvasTf = ins.GetComponentInChildren<Canvas>().transform;
            var imgIns = Instantiate(SkillImgObj, Vector3.zero, Quaternion.identity, canvasTf);
            //* RectTransform 調整
            imgIns.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 100, 0);
            imgIns.GetComponent<RectTransform>().localScale = Vector3.one * 2;
        }
        Destroy(ins, 2);
    }

    public void createCongratuBlastRainbowEF(Transform parentTf){
        var ins = Instantiate(congratuBlastRainbowEF, parentTf, instantiateInWorldSpace: false);
        Destroy(ins, 3);
    }

    public void createUpgradeItemEF(Transform parentTf){
        hm.mainCanvas.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
        var ins = Instantiate(upgradeItemEF, parentTf, instantiateInWorldSpace: false);
        Destroy(ins, 1);
    }
}
