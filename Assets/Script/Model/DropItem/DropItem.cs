using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    //* OutSide
    GameManager gm;

    //* Value
    int expVal; public int ExpVal{ get => expVal; set => expVal = value;}
    [SerializeField] bool isMoveToPlayer; public bool IsMoveToPlayer{ get => isMoveToPlayer; set => isMoveToPlayer = value;}
    [SerializeField] float moveSpeed = 5f;
    public float cnt = 0;
    [SerializeField] Rigidbody rigid;

    void Awake() {
        Debug.Log("<color=yellow>DropItem Awake() </color>");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        Debug.Log("<color=yellow>DropItem OnEnable() </color>");
        spawnPopUp();
    }

    void OnDisable() { //* (BUG) 再活性化するときに、動きOFF。
        IsMoveToPlayer = false;
    }

    void Update(){
        if(IsMoveToPlayer){
            //* 放物線
            transform.position = Vector3.Slerp(transform.position, gm.pl.transform.position, moveSpeed * Time.deltaTime); // transform.position = Vector3.Lerp(transform.position, gm.pl.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    public void moveToTarget(Transform target) => IsMoveToPlayer = true;

    public void spawnPopUp(){
        float v = 0.3f;
        float randX = Random.Range(-v, v);
        float randZ = Random.Range(-v, v);
        // Debug.LogFormat("DropItem:: shootPopUp():: randX= {0}, randZ= {1}", randX, randZ);
        Vector3 dir = new Vector3(randX, 1, randZ);
        float force = 350 * Time.deltaTime;
        this.rigid.AddForce(dir * force, ForceMode.Impulse);
    }

    public static IEnumerator coWaitPlayerCollectOrb(GameManager gm){
        gm.bm.IsCreateBlock = true;
        if(gm.bm.transform.childCount == 0){//* Remove All Blocks Perfect Bonus!
            Debug.Log("PERFECT!");
            gm.perfectTxt.GetComponent<Animator>().SetTrigger("doSpawn");
            gm.em.enableUIStageTxtEF("Perfect");
            yield return new WaitForSeconds(1);
            //* STAGE % 5 == 0だったら、LONGブロックが続けて生成するBUG対応。
            ++gm.stage;
            gm.bm.IsCreateBlock = true;
        }
        float sec = 0.8f;
        yield return new WaitForSeconds(sec);

        Debug.LogFormat("<color=white>coWaitCollectOrb:: checkLevelUp() wait: {0}sec</color>",sec);
        gm.checkLevelUp();
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Player"){
            gm.pl.addExp(ExpVal); //* (BUG) GAMEOVER後、再スタート場合、EXPが増えないように。
            gm.em.createDropItemExpOrbEF(this.transform);
            // Destroy(this.gameObject);
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }
}
