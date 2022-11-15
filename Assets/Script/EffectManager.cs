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

    [Header("HIT SPARK EF")]
    public GameObject batHitSparkEF;
    public GameObject homeRunHitSparkEF;
    public GameObject stunEF;

    [Header("BLOCK EF")]
    public GameObject brokeBlockEF;
    public GameObject itemBlockExplosionEF;
    public GameObject itemBlockDirLineTrailEF;
    public GameObject downWallHitEF;
    public GameObject healTxtEF, heartEF;
    
    [Header("PSV SKILL EF")]
    public GameObject explosionEF;
    public GameObject criticalTxtEF;
    public GameObject instantKillTxtEF;
    public GameObject laserEF;
    public GameObject darkOrbHitEF;
    public GameObject eggPopEF;
    public GameObject godBlessEF;

    [Header("ATV SKILL EF")]
    [Tooltip("GAME SCENEがロードしたら、自動で読み込む。")]
    public GameObject[] activeSkillBatEFs;
    public GameObject[] activeSkillShotEFs;
    public GameObject[] activeSkillExplosionEFs;
    public GameObject[] activeSkillCastEFs;

    [Header("ATV HOMERUN BONUS EF")]
    public GameObject fireBallDotEF;
    public GameObject thunderStrikeEF;
    public GameObject colorBallStarExplosionEF;

    [Header("DROP ITEMS EF")]
    public GameObject dropItemExpOrbEF;

    [Header("UI EF")]
    public GameObject perfectTxtPopEF;
    public GameObject homeRunTxtPopEF;

    [Header("BOSS SKILL EF")]
    public GameObject bossHealSkillEF;
    public GameObject bossObstacleSpawnEF;
    public GameObject rockObstacleBrokenEF;
    public GameObject bossFireBallTrailEF;
    public GameObject bossFireBallExplosionEF;

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
    public void createRockObstacleBrokenEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.RockObstacleBrokenEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1));
    }
    public GameObject createBossFireBallTrailEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BossFireBallTrailEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.75f));
        return ins;
    }
    public GameObject createBossFireBallExplosionEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BossFireBallExplosionEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 2));
        return ins;
    }
    
    public void createBossObstacleSpawnEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BossObstacleSpawnEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 3));
    }
    public void createBossHealSkillEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BossHealSkillEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 3));
    }
    public void createBatHitSparkEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BatHitSparkEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1));
    }
    public void createHomeRunHitSparkEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.HomeRunHitSparkEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1));
    }
    public void createStunEF(Vector3 parentPos, float sec){
        var ins = ObjectPool.getObject(ObjectPool.DIC.StunEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, sec));
    }
    public void createBrokeBlockEF(Vector3 parentPos, Color color){
        var ins = ObjectPool.getObject(ObjectPool.DIC.BrokeBlockEF.ToString(), parentPos, QI, gm.effectGroup);
        ParticleSystem ps = ins.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(1,1,1,0.5f));
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, main.duration));
    }
    public void createItemBlockExplosionEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.ItemBlockExplosionEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
    }
    public void createItemBlockDirLineTrailEF(Vector3 parentPos, Vector3 dir){
        const int speed = 10;
        var ins = ObjectPool.getObject(ObjectPool.DIC.ItemBlockDirLineTrailEF.ToString(), parentPos, QI, gm.effectGroup);
        ins.GetComponent<Rigidbody>().AddForce(dir * speed, ForceMode.Impulse);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 3));
    }
    public void createDownWallHitEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.DownWallHitEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1));
    }
    public void createInstantKillTextEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.InstantKillTextEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
    }
    public GameObject createCritTxtEF(Vector3 parentPos, int damage){
        var ins = ObjectPool.getObject(ObjectPool.DIC.CritTxtEF.ToString(), parentPos, QI, gm.effectGroup);
        ins.GetComponentInChildren<Text>().text = damage.ToString();
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public GameObject createHealTxtEF(Vector3 parentPos, int heal){
        var ins = ObjectPool.getObject(ObjectPool.DIC.HealTxtEF.ToString(), parentPos, QI, gm.effectGroup);
        ins.GetComponentInChildren<Text>().text = "+" + heal.ToString();
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public GameObject createHeartEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.HeartEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public void createExplosionEF(Vector3 parentPos, float scale){
        // Debug.Log("EFFECT:: Explosion:: scale=" + scale);
        var ins = ObjectPool.getObject(ObjectPool.DIC.ExplosionEF.ToString(), parentPos, QI, gm.effectGroup);
        ins.transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
    }
    public GameObject createLaserEF(Vector3 parentPos, Vector3 direction){
        var ins = ObjectPool.getObject(ObjectPool.DIC.LaserEF.ToString(), parentPos, QI, gm.effectGroup);
        var rigid = ins.GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        var speed = 6000 * Time.deltaTime;
        var force = direction * speed;
        Debug.DrawRay(parentPos, direction * 100, Color.red, 3);
        rigid.AddForce(force, ForceMode.Impulse);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public GameObject createDarkOrbHitEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.DarkOrbHitEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public GameObject createEggPopEF(Vector3 parentPos){
        var ins = ObjectPool.getObject(ObjectPool.DIC.EggPopEF.ToString(), parentPos, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
        return ins;
    }
    public GameObject createGodBlessEF(Vector3 parentPos){
        Debug.Log("GOD BLESS YOU! EF");
        var ins = ObjectPool.getObject(ObjectPool.DIC.GodBlessEF.ToString(), parentPos, godBlessEF.transform.rotation, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 3f));
        return ins;
    }
    

//* -------------------------------------------------------------
//* ActiveSkill Effect
//* -------------------------------------------------------------
    public void createAtvSkShotEF(int idx, Transform parentTf, Quaternion dir, bool isTrailEffect = false){
        Transform parent = (isTrailEffect)? parentTf : gm.effectGroup;
        string key = ObjectPool.DIC.AtvSkShotEF.ToString() + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, dir, parent);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1));
    }
    public GameObject createAtvSkExplosionEF(int idx, Transform parentTf, int time = 2){
        //* Rotation
        //* (BUG) ICEWAVE方向が逆になること対応 -> ArrowとBallPreviewsのベクトルマイナス関係に設定。
        bool is2ndLineOff = (gm.pl.ballPreviewSphere[0].activeSelf)? true : false;
        Debug.Log("createAtvSkExplosionEF:: is2ndLineOff= " + is2ndLineOff);
        Vector3 arrowVec = gm.pl.arrowAxisAnchor.transform.position;
        Vector3[] ballPrevVecs = new Vector3[2]{
            gm.pl.ballPreviewSphere[0].transform.position,
            gm.pl.ballPreviewSphere[1].transform.position
        };
        
        Vector3 direction = (is2ndLineOff)? ballPrevVecs[0] - arrowVec : ballPrevVecs[1] - ballPrevVecs[0];
        Quaternion rotate = Quaternion.LookRotation(direction);

        //* Skill Idx
        string key = ObjectPool.DIC.AtvSkExplosionEF.ToString() + (gm.SelectAtvSkillBtnIdx == 0 ? "" : "2");
        var ins = ObjectPool.getObject(key, parentTf.position, rotate, gm.effectGroup);

        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, time));
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
    //* ATV HomeRun Bonus EF
    public GameObject directlyCreateFireBallDotEF(Transform parentTf){//* 直接生成し、Blockが消えたら一緒に消える。
        Quaternion rot = fireBallDotEF.transform.rotation;
        var ins = Instantiate(fireBallDotEF, parentTf.position, rot, parentTf) as GameObject;
        return ins;
    }
    public void createThunderStrikeEF(Vector3 parentTfPos){
        Quaternion rot = fireBallDotEF.transform.rotation;
        var ins = Instantiate(thunderStrikeEF, parentTfPos, rot, gm.effectGroup) as GameObject;
    }
    public void createColorBallStarExplosionEF(Vector3 parentTfPos){
        var ins = Instantiate(colorBallStarExplosionEF, parentTfPos, QI, gm.effectGroup) as GameObject;
    }
//* -------------------------------------------------------------
//* Drop Items EF
//* -------------------------------------------------------------
    public void createDropItemExpOrbEF(Transform parentTf){
        var ins = ObjectPool.getObject(ObjectPool.DIC.DropItemExpOrbEF.ToString(), parentTf.position, QI, gm.effectGroup);
        StartCoroutine(ObjectPool.coDestroyObject(ins, gm.effectGroup, 1.5f));
    }
//* -------------------------------------------------------------
//* UI EF
//* -------------------------------------------------------------
    public void enableUITxtEF(string name){
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
        perfectTxtPopEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        perfectTxtPopEF.SetActive(false);
    }
    IEnumerator coUnActiveHomeRunTxtEF(){
        homeRunTxtPopEF.SetActive(true);
        yield return new WaitForSeconds(3f);
        homeRunTxtPopEF.SetActive(false);
    }
}
