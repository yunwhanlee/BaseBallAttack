using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    //* OutSide
    public Transform effectGroup;

    //* Obstacle EF
    public GameObject brokeBlockEF;

    //* Passive Skill EF
    public GameObject explosionEF;
    public GameObject criticalTextEF;
    public GameObject instantKillTextEF;

    //* Active Skill EF
    public GameObject activeSkill1_BatEF;
    public GameObject activeSkill1_ShotEF;
    public GameObject activeSkill1_ExplosionEF;

    //*---------------------------------------
    //* 関数
    //*---------------------------------------
    public void createEffectBrokeBlock(Transform parentTf, Color color){
        var ins = Instantiate(brokeBlockEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ParticleSystem ps = ins.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(1,1,1,0.5f));
        Destroy(ins, main.duration);
    }


    public void createEffectExplosion(Transform parentTf, float scale){
        Debug.Log("EFFECT:: Explosion:: scale=" + scale);
        var ins = Instantiate(explosionEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ins.transform.localScale = new Vector3(scale, scale, scale);
        Destroy(ins, 2);
    }
    public void createEffectCriticalText(Transform parentTf, int damage){
        var ins = Instantiate(criticalTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        ins.GetComponentInChildren<Text>().text = damage.ToString();
        Destroy(ins, 1.5f);
    }
        public void createEffectInstantKillText(Transform parentTf){
        var ins = Instantiate(instantKillTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1.5f);
    }

    public void createActiveSkillBatEF(Transform parentTf){
        //init
        foreach(Transform child in parentTf)  Destroy(child.gameObject);
        var ins = Instantiate(activeSkill1_BatEF, parentTf.position, parentTf.rotation, parentTf) as GameObject;
    }
    public void createActiveSkillShotEF(Transform parentTf, Quaternion dir){
        var ins = Instantiate(activeSkill1_ShotEF, parentTf.position, dir, effectGroup) as GameObject;
        Destroy(ins, 1);
    }
    public void createActiveSkillExplosionEF(Transform parentTf){
        var ins = Instantiate(activeSkill1_ExplosionEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 2);
    }
}
