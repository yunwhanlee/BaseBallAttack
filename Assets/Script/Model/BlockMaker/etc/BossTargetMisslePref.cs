using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargetMisslePref : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] SphereCollider collider;

    GameManager gm;

    [SerializeField] Transform target;
    [SerializeField] float maxSpeed;
    [SerializeField] float curSpeed;
    [SerializeField] int popPower;

    void Awake(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        yield return new WaitForSeconds(0.3f);
        searchBoss();
    }

    public void searchBoss(){
        BossBlock boss = gm.bm.getBoss();
        if(boss){
            target = gm.bossGroup.GetChild(0);
            collider.enabled = true;
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.transform.CompareTag(DM.TAG.BossBlock.ToString())){
            int dmg = gm.stage / 10 + 1;
            col.gameObject.GetComponent<BossBlock>().decreaseHp(dmg);
            gm.em.createBossTargetMissileEF(this.transform.position);
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }
}
