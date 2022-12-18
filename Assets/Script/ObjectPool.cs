using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    public enum DIC{
        //* EF
        AtvSkShotEF, AtvSkExplosionEF, AtvSkShotEF2, AtvSkExplosionEF2, 
        LvUpNovaEF,
        DropItemExpOrbEF, ShowExpUITxtEF, DownWallHitEF, BrokeBlockEF,
        ItemBlockDirLineTrailEF, ItemBlockExplosionEF,
        InstantKillTextEF, CritTxtEF, HealTxtEF, HeartEF, SnowExplosionEF, IcePropertyNovaFrost,
        BatHitSparkEF, HomeRunHitSparkEF, StunEF, 
        ExplosionEF, LaserEF, DarkOrbHitEF, EggPopEF, GodBlessEF, 
        BossHealSkillEF, BossObstacleSpawnEF, RockObstacleBrokenEF, BossFireBallExplosionEF, BossFireBallTrailEF, AimingEF, DefenceEF, 
        ThunderStrikeEF, ColorBallStarExplosionEF, //* ATV HomeRun Bonus

        //* OBJ
        DropItemExpOrbPf, DropItemRewardChest,
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
        em = gm.em;
        bm = gm.bm;
        init();
    }

    private void init(){
        //* Set Data 
        // EFFECT
        List<PoolData> poolDtList = new List<PoolData>(){};
        // 複数(配列)
        poolDtList.Add(new PoolData(DIC.AtvSkShotEF.ToString(), em.activeSkillShotEFs[DM.ins.personalData.SelectSkillIdx], 1, em.gm.effectGroup));
        if(em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkillIdx]) //* ThunderShotスキルはこれがないので条件文で対応。
            poolDtList.Add(new PoolData(DIC.AtvSkExplosionEF.ToString(), em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkillIdx], 1, em.gm.effectGroup));
        if(DM.ins.personalData.IsUnlock2ndSkill){
            //* (BUG) ATVスキルを登録する際に、em.EffectObjectが元々無いことがある。
            //! これが ObjectPoolで既に活性化する処理で「NULL Refereceエラー」になるため、em.ObjがNullなら、Addしないように対応。
            if(em.activeSkillShotEFs[DM.ins.personalData.SelectSkill2Idx])
                poolDtList.Add(new PoolData(DIC.AtvSkShotEF2.ToString(), em.activeSkillShotEFs[DM.ins.personalData.SelectSkill2Idx], 1, em.gm.effectGroup));
            if(em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkill2Idx])
                poolDtList.Add(new PoolData(DIC.AtvSkExplosionEF2.ToString(), em.activeSkillExplosionEFs[DM.ins.personalData.SelectSkill2Idx], 1, em.gm.effectGroup));
        }
        // 単一
        poolDtList.Add(new PoolData(DIC.LvUpNovaEF.ToString(), em.lvUpNovaEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.DropItemExpOrbEF.ToString(), em.dropItemExpOrbEF, 15, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ShowExpUITxtEF.ToString(), em.showExpUITxtEF, 15, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.InstantKillTextEF.ToString(), em.instantKillTxtEF, 5, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.CritTxtEF.ToString(), em.criticalTxtEF, 3, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.HealTxtEF.ToString(), em.healTxtEF, 10, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.HeartEF.ToString(), em.heartEF, 10, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.SnowExplosionEF.ToString(), em.snowExplosionEF, 10, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.IcePropertyNovaFrost.ToString(), em.icePropertyNovaFrost, 5, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.DownWallHitEF.ToString(), em.downWallHitEF, 10, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ItemBlockDirLineTrailEF.ToString(), em.itemBlockDirLineTrailEF, 3, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ItemBlockExplosionEF.ToString(), em.itemBlockExplosionEF, 3, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BrokeBlockEF.ToString(), em.brokeBlockEF, 60, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BatHitSparkEF.ToString(), em.batHitSparkEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.HomeRunHitSparkEF.ToString(), em.homeRunHitSparkEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.StunEF.ToString(), em.stunEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ExplosionEF.ToString(), em.explosionEF, 5, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.LaserEF.ToString(), em.laserEF, 3, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.DarkOrbHitEF.ToString(), em.darkOrbHitEF, 3, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.EggPopEF.ToString(), em.eggPopEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.GodBlessEF.ToString(), em.godBlessEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BossHealSkillEF.ToString(), em.bossHealSkillEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BossObstacleSpawnEF.ToString(), em.bossObstacleSpawnEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.RockObstacleBrokenEF.ToString(), em.rockObstacleBrokenEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BossFireBallTrailEF.ToString(), em.bossFireBallTrailEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.AimingEF.ToString(), em.aimingEF, 2, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.DefenceEF.ToString(), em.defenceEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.BossFireBallExplosionEF.ToString(), em.bossFireBallExplosionEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ThunderStrikeEF.ToString(), em.thunderStrikeEF, 1, em.gm.effectGroup));
        poolDtList.Add(new PoolData(DIC.ColorBallStarExplosionEF.ToString(), em.colorBallStarExplosionEF, 1, em.gm.effectGroup));

        // OBJ 
        poolDtList.Add(new PoolData(DIC.DropItemExpOrbPf.ToString(), bm.dropItemExpOrbPf, 50, gm.dropItemGroup));

        // poolDtList.ForEach(list => Debug.Log("NULL LIST CHECK ⇒ list.Key= " + list.Key + "list.Obj= " + (list.Obj? list.Obj.ToString() :"<color=red>"+list.Obj.ToString()+"</color>")));

        //* Enroll poolObjDtDict
        poolObjDtDict = new Dictionary<string, GameObject>();
        poolDtList.ForEach(dt => poolObjDtDict.Add(dt.Key, dt.Obj));

        //* Create Obj -> Inactive
        foreach(var obj in poolDtList){
            // Debug.LogFormat("poolObjDtDict[{0}]= {1}", dt.Key, dt.Obj);
            if(obj.Obj == null) continue; //* (BUG) nullなら、生成しなくて、次に飛ぶ。
            for(int i=0; i<obj.Cnt; i++)
                createNewObject(obj.Key, obj.GroupTf);
        }

        //* ★★★ 全て準備したOBJECTを活性化する ⇐ こうしないと、複数のEFが活性化したら、読込が重くなり、フリーズ掛ける。
        activateEveryObjects(poolDtList);
    }
    private void activateEveryObjects(List<PoolData> poolDtList){
        Debug.Log("LOADING:: OBJECT-POOL EFFECTを読み込む。");
        Vector3 activeEffectPos = new Vector3(0, 0, 15);
        foreach(var obj in poolDtList){
            Debug.Log($"key={obj.Key}, cnt={obj.Cnt}, groupTf={obj.GroupTf.name}");
            for(int i=0; i< obj.Cnt; i++){
                var ins = getObject(obj.Key, activeEffectPos, Quaternion.identity, obj.GroupTf);
                StartCoroutine(coDestroyObject(ins, obj.GroupTf, 1));
            }
        }
    }


    public static GameObject getObject(string key, Vector3 pos, Quaternion rot, Transform groupTf = null){
        List<GameObject> childList = new List<GameObject>();
        for(int i=0; i< groupTf.childCount; i++)
            childList.Add(groupTf.GetChild(i).gameObject);

        var obj = childList.Find(ch => ch.name == key && !ch.gameObject.activeSelf);
        if(obj){
            // Debug.Log("getObject():: Active obj= " + obj);
        }
        else{
            // Debug.Log("getObject():: New Create!");
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
        // Debug.LogFormat("createNewObject(string key):: key= {0}", key);
        var newObj = Instantiate(poolObjDtDict[key], this.transform);
        newObj.name = key.ToString();
        newObj.gameObject.SetActive(false);
        if(groupTf) newObj.transform.SetParent(groupTf);
        return newObj;
    }

    public static IEnumerator coDestroyObject(GameObject ins, Transform groupTf, float sec = 0){
        yield return new WaitForSeconds(sec);
        returnObject(ins, groupTf);
    }
    static void returnObject(GameObject ins, Transform groupTf){
        // Debug.Log("<color=red>ObjectPool:: returnObject() ins= " + ins.name + "</color>");
        if(!ins) return;
        ins.SetActive(false);
        ins.transform.SetParent(groupTf);
    }
}
