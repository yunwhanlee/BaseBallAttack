using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;



public class EffectManager : MonoBehaviour
{
    Quaternion QI = Quaternion.identity;

    //* OutSide
    public GameManager gm;
    public Transform effectGroup;

    //* Hit Spark EF
    public GameObject batHitSparkEF;
    public GameObject homeRunHitSparkEF;

    //* Block EF
    public GameObject brokeBlockEF;
    public GameObject itemBlockExplosionEF;
    public GameObject itemBlockDirLineTrailEF;
    public GameObject downWallHitEF;
    
    //* Passive Skill EF
    public GameObject explosionEF;
    public GameObject criticalTextEF;
    public GameObject instantKillTextEF;

    //* Active Skill EF
    public GameObject[] activeSkillBatEFs;
    public GameObject[] activeSkillShotEFs;
    public GameObject[] activeSkillExplosionEFs;
    public GameObject[] activeSkillCastEFs;

    //* Active HomeRun Bonus
    public GameObject fireBallDotEF;

    //* Drop Items EF
    public GameObject dropItemExpOrbEF;

    //* UI EF
    public GameObject perfectTxtEF;
    public GameObject homeRunTxtEF;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        Debug.Log("EffectManager:: gm.activeSkillDataBase.Length=" + gm.activeSkillDataBase.Length);
        int cnt = gm.activeSkillDataBase.Length;
        activeSkillBatEFs = new GameObject[cnt];
        activeSkillShotEFs = new GameObject[cnt];
        activeSkillExplosionEFs = new GameObject[cnt];
        activeSkillCastEFs = new GameObject[cnt];
    }
//*---------------------------------------
//* 関数
//*---------------------------------------
    public void createBatHitSparkEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BatHitSparkEF.ToString(), parentPos, QI, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1));
    }

    public void createHomeRunHitSparkEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.HomeRunHitSparkEF.ToString(), parentPos, QI, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1));
    }

    public void createBrokeBlockEF(Vector3 parentPos, Color color){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BrokeBlockEF.ToString(), parentPos, QI, effectGroup);
        ParticleSystem ps = ins.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(1,1,1,0.5f));
        StartCoroutine(ObjectPool.coDestroyObject(ins, main.duration));
    }
    public void createItemBlockExplosionEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.ItemBlockExplosionEF.ToString(), parentPos, QI, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
    }
    public void createItemBlockDirLineTrailEF(Vector3 parentPos, Vector3 dir){
        const int speed = 10;
        var ins = ObjectPool.getObject(ObjectPool.DIC.ItemBlockDirLineTrailEF.ToString(), parentPos, QI, effectGroup);
        ins.GetComponent<Rigidbody>().AddForce(dir * speed, ForceMode.Impulse);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 3));
    }
    public void createDownWallHitEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.DownWallHitEF.ToString(), parentPos, QI, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1));
    }
    public void createInstantKillTextEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.InstantKillTextEF.ToString(), parentPos, QI, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
    }
    public GameObject createCritTxtEF(Vector3 parentPos, int damage){
        var ins = ObjectPool.getObject(ObjectPool.DIC.CritTxtEF.ToString(), parentPos, QI, effectGroup);
        ins.GetComponentInChildren<Text>().text = damage.ToString();
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
        return ins;
    }
    public void createExplosionEF(Vector3 parentPos, float scale){
        // Debug.Log("EFFECT:: Explosion:: scale=" + scale);
        var ins = ObjectPool.getObject(ObjectPool.DIC.ExplosionEF.ToString(), parentPos, QI, effectGroup);
        ins.transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
    }
//* -------------------------------------------------------------
//* ActiveSkill Effect
//* -------------------------------------------------------------
    public void createAtvSkShotEF(int idx, Transform parentTf, Quaternion dir, bool isTrailEffect = false){
        Transform parent = (isTrailEffect)? parentTf : effectGroup;
        string key = ObjectPool.DIC.AtvSkShotEF.ToString() + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, dir, parent);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1));
    }
    public GameObject createAtvSkExplosionEF(int idx, Transform parentTf, int time = 2){
        var dir = Vector3.Normalize(parentTf.GetComponent<Rigidbody>().velocity);
        float dirZ = (dir.z < 0)? Mathf.Abs(dir.z) : -Mathf.Abs(dir.z);
        Quaternion rotate = Quaternion.LookRotation(new Vector3(dir.x, dir.y, dirZ));
        string key = ObjectPool.DIC.AtvSkExplosionEF.ToString() + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, rotate, effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, time));
        return ins;
    }
    public void directlyCreateActiveSkillBatEF(int idx, Transform parentTf){ //* １個だけ、直接生成してON/OFFで調整。
        // Debug.LogFormat("createActiveSkillBatEF:: BatEfs[{0}].name={1}", idx, activeSkillBatEFs[idx].name);
        var ins = Instantiate(activeSkillBatEFs[idx], parentTf.position, parentTf.rotation, parentTf) as GameObject;
    }
    public void enableSelectedActiveSkillBatEF(int skillIdx, Transform batEfs){
        foreach(Transform batEf in batEfs){
            int childIdx = batEf.GetSiblingIndex();
            if(skillIdx == childIdx)
                batEf.gameObject.SetActive(true);
            else
                batEf.gameObject.SetActive(false);
        }
    }
    public void directlyCreateAtvSkCastEF(int idx, Transform parentTf){ //* 直接生成してON/OFFで調整。
        if(!activeSkillCastEFs[idx]) return;
        // Debug.Log("createActiveSkillCastEF:: name= " + activeSkillCastEFs[idx].name + ", parentTf= " + parentTf.name);
        var ins = Instantiate(activeSkillCastEFs[idx], parentTf.position, QI, parentTf) as GameObject;
        ins.transform.localRotation = Quaternion.Euler(0,0,0);
    }
    public void directlyCreateFireBallDotEF(Transform parentTf){//* 直接生成し、Blockが消えたら一緒に消える。
        var ins = Instantiate(fireBallDotEF, parentTf.position, QI, parentTf) as GameObject;
    }
//* -------------------------------------------------------------
//* Drop Items EF
//* -------------------------------------------------------------
    public void createDropItemExpOrbEF(Transform parentTf){
        var ins = ObjectPool.getObject(ObjectPool.DIC.DropItemExpOrbEF.ToString(), parentTf.position, QI);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
    }
//* -------------------------------------------------------------
//* UI EF
//* -------------------------------------------------------------
    public void enableUIStageTxtEF(string name){
        switch(name){
            case "Perfect":
                StartCoroutine(coUnActivePerfectTxtEF());
                break;
            case "HomeRun":
                StartCoroutine(coUnActiveHomeRunTxtEF());
                break;
        }
    }
    IEnumerator coUnActivePerfectTxtEF(){
        perfectTxtEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        perfectTxtEF.SetActive(false);
    }
    IEnumerator coUnActiveHomeRunTxtEF(){
        homeRunTxtEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        homeRunTxtEF.SetActive(false);
    }
}
