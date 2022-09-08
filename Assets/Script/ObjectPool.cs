using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    public enum DIC{
        //* EF
        AtvSkShotEF, AtvSkExplosionEF, AtvSkShotEF2, AtvSkExplosionEF2, 
        DropItemExpOrbEF, DownWallHitEF, BrokeBlockEF,
        ItemBlockDirLineTrailEF, ItemBlockExplosionEF,
        InstantKillTextEF, CritTxtEF,
        BatHitSparkEF, HomeRunHitSparkEF, 
        ExplosionEF,

        //* OBJ
        DropItemExpOrbPf
    }

    struct PoolData{
        string key;         public string Key {get => key; set => key = value;}
        GameObject obj;     public GameObject Obj {get => obj; set => obj = value;}
        int cnt;            public int Cnt {get => cnt; set => cnt = value;}
        Transform groupTf;  public Transform GroupTf {get => groupTf; set => groupTf = value;}

        public PoolData(string key, GameObject obj, int cnt = 1, Transform groupTf = null){
            this.key = key; 
            this.obj = obj; 
            this.cnt = cnt;
            this.groupTf = groupTf;
        }
    }

    public static ObjectPool Ins;
    private GameManager gm;
    private EffectManager em;
    private BlockMaker bm; 

    [SerializeField] Dictionary<string, GameObject> poolObjDtDict;

    private void Awake() {
        Ins = this;
    }
    private void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        bm = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();
        init();
    }

    private void init(){
        //* Set Data 
        // EF
        List<PoolData> poolDtList = new List<PoolData>(){};
        poolDtList.Add(new PoolData(DIC.AtvSkShotEF.ToString(), em.activeSkillShotEFs[DM.ins.personalData.SelectSkillIdx]));
        poolDtList.Add(new PoolData(DIC.AtvSkExplosionEF.ToString(), em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkillIdx]));
        if(DM.ins.personalData.IsUnlock2ndSkill){
            poolDtList.Add(new PoolData(DIC.AtvSkShotEF2.ToString(), em.activeSkillShotEFs[DM.ins.personalData.SelectSkill2Idx]));
            poolDtList.Add(new PoolData(DIC.AtvSkExplosionEF2.ToString(), em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkill2Idx]));
        }
        poolDtList.Add(new PoolData(DIC.DropItemExpOrbEF.ToString(), em.dropItemExpOrbEF, 30));
        poolDtList.Add(new PoolData(DIC.InstantKillTextEF.ToString(), em.instantKillTextEF, 5));
        poolDtList.Add(new PoolData(DIC.CritTxtEF.ToString(), em.criticalTextEF, 20));
        poolDtList.Add(new PoolData(DIC.DownWallHitEF.ToString(), em.downWallHitEF, 10));
        poolDtList.Add(new PoolData(DIC.ItemBlockDirLineTrailEF.ToString(), em.itemBlockDirLineTrailEF, 3));
        poolDtList.Add(new PoolData(DIC.ItemBlockExplosionEF.ToString(), em.itemBlockExplosionEF, 3));
        poolDtList.Add(new PoolData(DIC.BrokeBlockEF.ToString(), em.brokeBlockEF, 15));
        poolDtList.Add(new PoolData(DIC.BatHitSparkEF.ToString(), em.batHitSparkEF));
        poolDtList.Add(new PoolData(DIC.HomeRunHitSparkEF.ToString(), em.homeRunHitSparkEF));
        poolDtList.Add(new PoolData(DIC.ExplosionEF.ToString(), em.explosionEF, 5));

        // OBJ 
        poolDtList.Add(new PoolData(DIC.DropItemExpOrbPf.ToString(), bm.dropItemExpOrbPf, 50, bm.dropItemGroup));

        //* Enroll Dic
        poolObjDtDict = new Dictionary<string, GameObject>();
        poolDtList.ForEach(dt => poolObjDtDict.Add(dt.Key, dt.Obj));
        
        //* Create Obj
        foreach(var dt in poolDtList){
            // Debug.LogFormat("poolObjDtDict[{0}]= {1}", dt.Key, dt.Obj);
            if(dt.Obj == null) continue; //* (BUG) nullなら、生成しなくて、次に飛ぶ。
            for(int i=0; i<dt.Cnt; i++)
                createNewObject(dt.Key, dt.GroupTf);
        }
    }

    public static GameObject getObject(string key, Vector3 pos, Quaternion rot, Transform groupTf = null){
        List<GameObject> childList = new List<GameObject>();
        for(int i=0; i< Ins.transform.childCount; i++)
            childList.Add(Ins.transform.GetChild(i).gameObject);

        var obj = childList.Find(ch => ch.name == key && !ch.gameObject.activeSelf);
        if(obj){
            Debug.Log("getObject():: Active obj= " + obj);
        }
        else{
            Debug.Log("getObject():: New Create!");
            obj = Ins.createNewObject(key, groupTf);
        }
        
        //* 属性
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.localRotation = rot;
        if(groupTf != null)  obj.transform.SetParent(groupTf);

        return obj;
    }

    private GameObject createNewObject(string key, Transform groupTf = null){
        Debug.LogFormat("createNewObject(string key):: key= {0}", key);
        var newObj = Instantiate(poolObjDtDict[key], this.transform);
        newObj.name = key.ToString();
        newObj.gameObject.SetActive(false);
        if(groupTf) newObj.transform.SetParent(groupTf);
        return newObj;
    }

    static void returnObject(GameObject ins){
        ins.SetActive(false);
        ins.transform.SetParent(Ins.transform);
    }
    public static IEnumerator coDestroyObject(GameObject ins, float sec = 0.1f){
        yield return new WaitForSeconds(sec);
        returnObject(ins);
    }
}
