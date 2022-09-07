using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    struct PoolData{
        string key;         public string Key {get => key; set => key = value;}
        GameObject obj;     public GameObject Obj {get => obj; set => obj = value;}
        int cnt;            public int Cnt {get => cnt; set => cnt = value;}

        public PoolData(string key, GameObject obj, int cnt = 1){
            this.key = key;
            this.obj = obj;
            this.cnt = cnt;
        }
    }

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
        //* Set Data 
        List<PoolData> poolDtList = new List<PoolData>(){};
        poolDtList.Add(new PoolData("AtvSkShotEF", em.activeSkillShotEFs[DM.ins.personalData.SelectSkillIdx]));
        poolDtList.Add(new PoolData("AtvSkExplosionEF", em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkillIdx]));
        if(DM.ins.personalData.IsUnlock2ndSkill){
            poolDtList.Add(new PoolData("AtvSkShotEF2", em.activeSkillShotEFs[DM.ins.personalData.SelectSkill2Idx]));
            poolDtList.Add(new PoolData("AtvSkExplosionEF2", em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkill2Idx]));
        }
        poolDtList.Add(new PoolData("DropItemExpOrbEF", em.dropItemExpOrbEF, 30));
        poolDtList.Add(new PoolData("InstantKillTextEF", em.instantKillTextEF, 5));
        poolDtList.Add(new PoolData("CritTxtEF", em.criticalTextEF, 20));
        
        poolObjDtDict = new Dictionary<string, GameObject>();

        //* Enroll Dic
        poolDtList.ForEach(dt => poolObjDtDict.Add(dt.Key, dt.Obj));
        
        //* Create Obj
        foreach(var dt in poolDtList){
            // Debug.LogFormat("poolObjDtDict[{0}]= {1}", dt.Key, dt.Obj);
            if(dt.Obj == null) continue; //* (BUG) nullなら、生成しなくて、次に飛ぶ。
            for(int i=0; i<dt.Cnt; i++)
                createNewObject(dt.Key);
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
        Debug.LogFormat("createNewObject(string key):: key= {0}", key);
        var newObj = Instantiate(poolObjDtDict[key], this.transform);
        newObj.name = key.ToString();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    static void returnObject(GameObject ins){
        ins.SetActive(false);
        ins.transform.SetParent(Ins.transform);


    }
    public static IEnumerator coDestroyObject(GameObject ins, float sec){
        yield return new WaitForSeconds(sec);
        returnObject(ins);
    }
}
