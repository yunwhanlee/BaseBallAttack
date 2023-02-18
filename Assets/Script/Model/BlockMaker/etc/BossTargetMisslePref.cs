using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargetMisslePref : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] new SphereCollider collider;

    GameManager gm;

    [SerializeField] Transform target;
    [SerializeField] float maxSpeed;
    [SerializeField] float curSpeed;
    [SerializeField] int popPower;

    const float DEC_DMG_PER = 0.5f; //* このミサイルは、あくまでも補助用だから、ダメージを減る。

    void Awake(){
        gm = DM.ins.gm;
        rigid = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<SphereCollider>();
    }

    void OnEnable() {
        init();
        /*
        * (BUG-19) BossTargetMissilePrefが再度登場したら、init()で初期化したのに、速度とかCoroutine待機時間などが可笑しい。
        *  原因：init()関数内で、AddForce()とStartCoroutine(coDelay())を入れ、それが初期化より早く実行される。
        */ 
        rigid.AddForce(Vector3.up * popPower * Time.deltaTime, ForceMode.Impulse);
        StartCoroutine(coDelay());
    }

    void Update(){
        if(target){
            //* ボースが死んだら、追いかけることやめて破壊。
            if(target.GetComponent<BossBlock>().Hp <= 0){
                StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
                return;
            }
                

            if(curSpeed <= maxSpeed)
                curSpeed += maxSpeed * Time.deltaTime;

            transform.position += transform.up * curSpeed * Time.deltaTime;

            Vector3 dir = (target.transform.position - this.transform.position).normalized;
            transform.up = Vector3.Lerp(transform.up, dir, 15 * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.transform.CompareTag(DM.TAG.BossBlock.ToString())){
            SM.ins.sfxPlay(SM.SFX.BossTargetMissle.ToString());
            
            //* ダメージ計算。
            const float offset100Percent = 1;
            float upgradeBossDmgPer = offset100Percent + DM.ins.personalData.Upgrade.Arr[(int)DM.UPGRADE.BossDamage].getValue();
            int dmg = (int)((gm.pl.dmg.Val * upgradeBossDmgPer) * DEC_DMG_PER);
            Debug.Log($"BossTargetMisslePerf:: dmg= {dmg} ({gm.pl.dmg.Val} * {upgradeBossDmgPer} * {DEC_DMG_PER})");
            col.gameObject.GetComponent<BossBlock>().decreaseHp((dmg <= 0)? 1 : dmg); //* Dmgが０なら、１にする。

            gm.em.createBossTargetMissileEF(this.transform.position);
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }

/// -------------------------------------------------------------------------------------
/// 関数
/// -------------------------------------------------------------------------------------
    void init(){
        target = null;
        curSpeed = 0;
        StopAllCoroutines();
        collider.enabled = false;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    IEnumerator coDelay(){
        yield return new WaitUntil(()=> rigid.velocity.y < 0);
        yield return Util.delay0_2;
        searchBoss();
    }

    public void searchBoss(){
        BossBlock boss = gm.bm.getBoss();
        if(boss){
            target = gm.bossGroup.GetChild(0);
            collider.enabled = true;
        }
    }


}
