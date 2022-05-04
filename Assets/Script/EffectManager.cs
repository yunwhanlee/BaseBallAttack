using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    //* OutSide
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
    public GameObject[] activeSkillBatEFs = new GameObject[2];
    public GameObject[] activeSkillShotEFs = new GameObject[2];
    public GameObject[] activeSkillExplosionEFs = new GameObject[2];
    public GameObject[] activeSkillCastEFs = new GameObject[2];

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

    public void createActiveSkillBatEF(int idx, Transform parentTf){
        // foreach(Transform child in parentTf)  Destroy(child.gameObject); //init
        var ins = Instantiate(activeSkillBatEFs[idx], parentTf.position, parentTf.rotation, parentTf) as GameObject;
    }
    public void createActiveSkillShotEF(int idx, Transform parentTf, Quaternion dir, bool isTrailEffect = false){
        Transform parent = (isTrailEffect)? parentTf : effectGroup;
        var ins = Instantiate(activeSkillShotEFs[idx], parentTf.position, dir, parent) as GameObject;
        Destroy(ins, 1);
    }
    public void createActiveSkillExplosionEF(int idx, Transform parentTf){
        var ins = Instantiate(activeSkillExplosionEFs[idx], parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 2);
    }

    public void createActiveSkillCastEF(int idx, Transform parentTf){
        var ins = Instantiate(activeSkillCastEFs[idx], parentTf.position, parentTf.rotation, parentTf) as GameObject;
    }
}
