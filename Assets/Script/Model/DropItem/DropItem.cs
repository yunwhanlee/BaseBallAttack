using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }
    public void moveToTarget(Transform target){
        Vector3 dir = target.position - this.rigid.transform.position;
        float power = 800 * Time.deltaTime;
        this.rigid.AddForce(dir * power, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("DropItem:: OnCollisionEnter:: col= " + col.gameObject.tag);
        if(col.gameObject.tag == "Player"){
            Debug.Log("Collect Orb");
            Destroy(this.gameObject);
        }
    }
}
