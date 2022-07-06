using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    //* OutSide
    GameManager gm;

    //* Value
    int expVal; public int ExpVal{ get => expVal; set => expVal = value;}
    bool isMoveToPlayer = false; public bool IsMoveToPlayer{ get => isMoveToPlayer; set => isMoveToPlayer = value;}
    float moveSpeed = 0.02f;

    Rigidbody rigid;
    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody>();
        spawnPopUp();
    }

    void Update(){
        if(isMoveToPlayer){
            transform.position = Vector3.Lerp(transform.position, gm.pl.transform.position, moveSpeed);
        }
    }

    public void moveToTarget(Transform target){
        isMoveToPlayer = true;
        // Vector3 dir = target.position - this.rigid.transform.position;
        // float force = 800 * Time.deltaTime;
        // this.rigid.AddForce(dir * force, ForceMode.Impulse);
    }

    public void spawnPopUp(){
        float v = 0.3f;
        float randX = Random.Range(-v, v);
        float randZ = Random.Range(-v, v);
        Debug.LogFormat("DropItem:: shootPopUp():: randX= {0}, randZ= {1}", randX, randZ);
        Vector3 dir = new Vector3(randX, 1, randZ);
        float force = 350 * Time.deltaTime;
        this.rigid.AddForce(dir * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Player"){
            gm.pl.addExp(ExpVal); //* (BUG) GAMEOVER後、再スタート場合、EXPが増えないように。
            gm.em.createDropItemExpOrbEF(this.transform);
            Destroy(this.gameObject);
        }
    }
}
