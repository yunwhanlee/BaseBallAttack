using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HomeEffectManager : MonoBehaviour
{
    //* Outside
    public HomeManager hm;

    void Start() {
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
    }

    public GameObject itemBuyEF;
    public GameObject congratuBlastRainbowEF;
    public GameObject upgradeItemEF;

    public void createItemBuyEF(GameObject SkillImgObj, string type){
        var ins = Instantiate(itemBuyEF, itemBuyEF.transform.position, Quaternion.identity) as GameObject;
        
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
