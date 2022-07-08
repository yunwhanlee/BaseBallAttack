using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Transform effectGroup;

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

    //* Drop Items EF
    public GameObject dropItemExpOrbEF;

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
    public void createCriticalTextEF(Transform parentTf, int damage){
        var ins = Instantiate(criticalTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ins.GetComponentInChildren<Text>().text = damage.ToString();
        Destroy(ins, 1.5f);
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
    public void enableSelectedActiveSkillBatEF(Transform batEfs){
        foreach(Transform batEf in batEfs){
            int childIdx = batEf.GetSiblingIndex();
            if(DM.ins.personalData.SelectSkillIdx == childIdx)
                batEf.gameObject.SetActive(true);
            else
                batEf.gameObject.SetActive(false);
        }
    }

    public void createActiveSkillShotEF(int idx, Transform parentTf, Quaternion dir, bool isTrailEffect = false){
        Transform parent = (isTrailEffect)? parentTf : effectGroup;
        var ins = Instantiate(activeSkillShotEFs[idx], parentTf.position, dir, parent) as GameObject;
        Destroy(ins, 1);
    }
    public void createActiveSkillExplosionEF(int idx, Transform parentTf, int time = 2){
        var ins = Instantiate(activeSkillExplosionEFs[idx], parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, time);
    }

    public void createActiveSkillCastEF(int idx, Transform parentTf){
        if(!activeSkillCastEFs[idx]) return;
        var ins = Instantiate(activeSkillCastEFs[idx], parentTf.position, parentTf.rotation, parentTf) as GameObject;
    }

    //* -------------------------------------------------------------
    //* Drop Items EF
    //* -------------------------------------------------------------
    public void createDropItemExpOrbEF(Transform parentTf){
        var ins = Instantiate(dropItemExpOrbEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1.5f);
    }
}
