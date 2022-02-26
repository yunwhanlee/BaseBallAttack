using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    //[SerializeField]int hitAngleMin = 45;
    //[SerializeField]int hitAngleMax = -45;

    [SerializeField]private int speed = 10;

    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-this.transform.forward * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "HitBox"){
            StartCoroutine(coHitBall(col));
        }
        else if(col.gameObject.tag == "Wall"){
        }
    }

    IEnumerator coHitBall(Collision col){
        //Debug.Log("Hit! HitBox inActive for one hit");
        col.gameObject.GetComponent<BoxCollider>().enabled = false;
        GameObject hitBoxObj = col.gameObject;
        Vector3 dir = hitBoxObj.transform.forward; //打った 方向
        float degY = hitBoxObj.transform.rotation.eulerAngles.y;
        degY = (degY < 180) ? degY : 360 - degY; //(BUG)-角度でも、0から360範囲で返す値を正しく変換する。
        float power = (35 < degY)? 1.5f
            : (25 < degY)? 2.25f
            : (15 < degY)? 3
            : (7.5f < degY)? 4.5f : 6;
        Debug.Log("coHitBall:: degY=" + degY + ", power=" + power);

        rigid.velocity = Vector3.zero;
        rigid.AddForce((dir).normalized * speed * power, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Back HitBox active");
        hitBoxObj.GetComponent<BoxCollider>().enabled = true;
    }

    public void setBallSpeed(int v){
        speed = v;
    }
}
