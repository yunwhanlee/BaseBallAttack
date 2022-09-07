using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class EffectManager : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Transform effectGroup;

    //* Hit Spark EF
    public GameObject normalHitSparkEF;
    public GameObject HomeRunHitSparkEF;

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
    public void createNormalHitSparkEF(Transform parentTf){
        var ins = Instantiate(normalHitSparkEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
    }

    public void createHomeRunHitSparkEF(Transform parentTf){
        var ins = Instantiate(HomeRunHitSparkEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
    }

    public void createBrokeBlockEF(Transform parentTf, Color color){
        var ins = Instantiate(brokeBlockEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ParticleSystem ps = ins.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(1,1,1,0.5f));
        Destroy(ins, main.duration);
    }
    public void createItemBlockExplosionEF(Transform parentTf){
        var ins = Instantiate(itemBlockExplosionEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1.5f);
    }
    public void createItemBlockDirLineTrailEF(Transform parentTf, Vector3 dir){
        var ins = Instantiate(itemBlockDirLineTrailEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        const int speed = 10;
        ins.GetComponent<Rigidbody>().AddForce(dir * speed, ForceMode.Impulse);
        Destroy(ins, 3f);
    }
    public void createDownWallHitEF(Vector3 parentPos){
        var ins = Instantiate(downWallHitEF, parentPos, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1);
    }

    public void createInstantKillTextEF(Transform parentTf){
        var ins = Instantiate(instantKillTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1.5f);
    }
    public GameObject createCritTxtEF(Vector3 parentPos, int damage){
        // var ins = Instantiate(criticalTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        var ins = ObjectPool.getObject("CritTxtEF", parentPos, Quaternion.identity);
        ins.GetComponentInChildren<Text>().text = damage.ToString();
        // Destroy(ins, 1.5f);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
        return ins;
    }
    public void createExplosionEF(Transform parentTf, float scale){
        Debug.Log("EFFECT:: Explosion:: scale=" + scale);
        var ins = Instantiate(explosionEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ins.transform.localScale = new Vector3(scale, scale, scale);
        Destroy(ins, 2);
    }
    //* -------------------------------------------------------------
    //* ActiveSkill Effect
    //* -------------------------------------------------------------
    public void createActiveSkillBatEF(int idx, Transform parentTf){
        Debug.LogFormat("createActiveSkillBatEF:: BatEfs[{0}].name={1}", idx, activeSkillBatEFs[idx].name);
        // foreach(Transform child in parentTf)  Destroy(child.gameObject); //init
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

    public void createAtvSkShotEF(int idx, Transform parentTf, Quaternion dir, bool isTrailEffect = false){
        Transform parent = (isTrailEffect)? parentTf : effectGroup;
        // var ins = Instantiate(activeSkillShotEFs[idx], parentTf.position, dir, parent) as GameObject;
        string key = "AtvSkShotEF" + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, dir, parent);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1));
    }
    public GameObject createAtvSkExplosionEF(int idx, Transform parentTf, int time = 2){
        var dir = Vector3.Normalize(parentTf.GetComponent<Rigidbody>().velocity);
        Debug.Log("createActiveSkillExplosionEF:: dir=" + dir);
        float dirZ = (dir.z < 0)? Mathf.Abs(dir.z) : -Mathf.Abs(dir.z);
        Quaternion rotate = Quaternion.LookRotation(new Vector3(dir.x, dir.y, dirZ));
        // var ins = Instantiate(activeSkillExplosionEFs[idx], parentTf.position, rotate, effectGroup) as GameObject;
        string key = "AtvSkExplosionEF" + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, rotate);
        StartCoroutine(ObjectPool.coDestroyObject(ins, time));
        return ins;
    }
    public void createAtvSkCastEF(int idx, Transform parentTf){
        if(!activeSkillCastEFs[idx]) return;
        Debug.Log("createActiveSkillCastEF:: name= " + activeSkillCastEFs[idx].name + ", parentTf= " + parentTf.name);
        var ins = Instantiate(activeSkillCastEFs[idx], parentTf.position, Quaternion.identity, parentTf) as GameObject;
        ins.transform.localRotation = Quaternion.Euler(0,0,0);
    }
    public void createAtvSkFireBallDotEF(Transform parentTf){
        var ins = Instantiate(fireBallDotEF, parentTf.position, Quaternion.identity, parentTf) as GameObject;
    }

    //* -------------------------------------------------------------
    //* Drop Items EF
    //* -------------------------------------------------------------
    public void createDropItemExpOrbEF(Transform parentTf){
        // var ins = Instantiate(dropItemExpOrbEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        var ins = ObjectPool.getObject("DropItemExpOrbEF", parentTf.position, Quaternion.identity);
        // Destroy(ins, 1.5f);
        StartCoroutine(ObjectPool.coDestroyObject(ins, 1.5f));
    }

    //* -------------------------------------------------------------
    //* UI EF
    //* -------------------------------------------------------------
    public void enableUIPerfectTxtEF(){
        StartCoroutine(coUnActivePerfectTxtEF());
    }
    IEnumerator coUnActivePerfectTxtEF(){
        perfectTxtEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        perfectTxtEF.SetActive(false);
    }
    public void enableUIHomeRunTxtEF(){
        StartCoroutine(coUnActiveHomeRunTxtEF());
    }
    IEnumerator coUnActiveHomeRunTxtEF(){
        homeRunTxtEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        homeRunTxtEF.SetActive(false);
    }
}
