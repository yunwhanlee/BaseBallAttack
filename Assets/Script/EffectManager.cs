using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //* OutSide
    public Transform effectGroup;

    public GameObject brokeBlockEF;
    public GameObject explosionEF;
    public GameObject thunderShotEF;
    public GameObject criticalTextEF;

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

    public void createEffectThunderShot(Transform parentTf, Quaternion dir){
        var ins = Instantiate(thunderShotEF, parentTf.position, dir, effectGroup) as GameObject;
        Destroy(ins, 1);
    }

    public void createEffectCriticalText(Transform parentTf){
        var ins = Instantiate(criticalTextEF, parentTf.position, Quaternion.identity, effectGroup) as GameObject;
        Destroy(ins, 1.5f);
    }
}
