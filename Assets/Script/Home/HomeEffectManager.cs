using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HomeEffectManager : MonoBehaviour
{
    public GameObject itemBuyEF;

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
}
