using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        shootPopUp();
    }

    public void moveToTarget(Transform target){
        Vector3 dir = target.position - this.rigid.transform.position;
        float force = 800 * Time.deltaTime;
        this.rigid.AddForce(dir * force, ForceMode.Impulse);
    }

    public void shootPopUp(){
        float v = 0.3f;
        float randX = Random.Range(-v, v);
        float randZ = Random.Range(-v, v);
        Debug.LogFormat("DropItem:: shootPopUp():: randX= {0}, randZ= {1}", randX, randZ);
        Vector3 dir = new Vector3(randX, 1, randZ);
        float force = 350 * Time.deltaTime;
        this.rigid.AddForce(dir * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player"){
            Destroy(this.gameObject);
        }
    }
}
