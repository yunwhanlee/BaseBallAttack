using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Ins;
    public GameManager gm;
    private EffectManager em;

    [SerializeField] Dictionary<string, GameObject> poolObjDtDict;

    private void Awake() {
        Ins = this;
    }
    private void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        init();
    }

    private void init(){
        poolObjDtDict = new Dictionary<string, GameObject>();

        //* Add Dic
        poolObjDtDict.Add("AtvSkShotEF", em.activeSkillShotEFs[DM.ins.personalData.SelectSkillIdx]);
        poolObjDtDict.Add("AtvSkExplosionEF", em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkillIdx]);
        if(DM.ins.personalData.IsUnlock2ndSkill){
            poolObjDtDict.Add("AtvSkShotEF2", em.activeSkillShotEFs[DM.ins.personalData.SelectSkill2Idx]);
            poolObjDtDict.Add("AtvSkExplosionEF2", em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkill2Idx]);
        }
        poolObjDtDict.Add("DropItemExpOrbEF", em.dropItemExpOrbEF);
        poolObjDtDict.Add("InstantKillTextEF", em.instantKillTextEF);
        poolObjDtDict.Add("CritTxtEF", em.criticalTextEF);
        
        //* Create Obj Into Transform
        foreach(var dic in poolObjDtDict){
            Debug.LogFormat("poolObjDtDict[{0}]= {1}", dic.Key, dic.Value);
            if(dic.Value == null) continue; //* (BUG) nullなら、生成しなくて、次に飛ぶ。
            createNewObject(dic.Key);
        }
    }

    public static GameObject getObject(string key, Vector3 pos, Quaternion rot, Transform parent = null){
        List<GameObject> childList = new List<GameObject>();
        for(int i=0; i< Ins.transform.childCount; i++)
            childList.Add(Ins.transform.GetChild(i).gameObject);

        var obj = childList.Find(ch => ch.name == key && !ch.gameObject.activeSelf);
        if(obj){
            Debug.Log("getObject():: Active obj= " + obj);
        }
        else{
            Debug.Log("getObject():: New Create!");
            obj = Ins.createNewObject(key);
        }
        
        //* 属性
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.localRotation = rot;
        if(parent != null)  obj.transform.SetParent(parent);

        return obj;
    }

    private GameObject createNewObject(string key){
        var newObj = Instantiate(poolObjDtDict[key], this.transform);
        newObj.name = key.ToString();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    static void returnObject(GameObject ins){
        ins.SetActive(false);
        ins.transform.SetParent(Ins.transform);
        // Ins.poolObjDict.Remove(key);
    }
    public static IEnumerator coDestroyObject(GameObject ins, float sec){
        yield return new WaitForSeconds(sec);
        returnObject(ins);
    }
}
