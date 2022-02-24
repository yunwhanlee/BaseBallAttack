using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    [SerializeField]int hitAngleMin = 60;
    [SerializeField]int hitAngleMax = -60;

    [SerializeField]private int speed = 10;
    

    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-this.transform.forward * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("HitBox")){
            StartCoroutine(coHitBall(col));
        }
    }

    IEnumerator coHitBall(Collision col){
        //Debug.Log("Hit! HitBox inActive for one hit");
        col.gameObject.GetComponent<BoxCollider>().enabled = false;
        GameObject hitBoxObj = col.gameObject;
        Vector3 dir = hitBoxObj.transform.forward; //打った 方向
        float degY = Mathf.Abs(hitBoxObj.transform.rotation.eulerAngles.y);
        float power = (45 < degY && degY <= 60)? 1
            : (30 < degY && degY <= 45)? 1.45f
            : (20 < degY && degY <= 30)? 2
            : (10 < degY && degY <= 20)? 2.75f
            : ( 5 < degY && degY <= 10)? 3.8f : 5;
        Debug.Log("HitBox:: angleY=" + degY + ", power=" + power);

        rigid.velocity = Vector3.zero;
        rigid.AddForce((dir).normalized * speed * power, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Back HitBox active");
        hitBoxObj.GetComponent<BoxCollider>().enabled = true;
    }
}
