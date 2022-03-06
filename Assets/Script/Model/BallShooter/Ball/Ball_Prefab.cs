using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;
    public BallShooter ballShooter;
    public int aliveTime;

    //* Value
    private bool isHited = false;
    private int speed;

    Rigidbody rigid;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();
        ballShooter = GameObject.Find("BallShooter").GetComponent<BallShooter>();

        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-this.transform.forward * speed, ForceMode.Impulse);

        //Invoke("onDestroyMe",aliveTime);

        //Debug.Log("HitRange:: startPosZ=" + gm.hitRangeStartTf.position.z +  ", endPosZ="+ gm.hitRangeEndTf.position.z);
    }
    void Update(){
            //* Hit Range Area Ball Comeing Viewer
            float startPosZ = gm.hitRangeStartTf.position.z;
            float endPosZ = gm.hitRangeEndTf.position.z;
            
            if(!isHited && endPosZ <= this.transform.position.z && this.transform.position.z <= startPosZ){
                //HitRange Slider UI
                float offset = Mathf.Abs(startPosZ);
                float max = Mathf.Abs(endPosZ) - offset;
                float v = Mathf.Abs(this.transform.position.z) - offset;
                gm.hitRangeDegSlider.value = v / max;

                float deg = calcHitRangeToDegree();
                //Debug.Log("Ball_Prefab:: deg=" + deg);
                pl.hitAxisArrow.transform.rotation = Quaternion.AngleAxis(deg, Vector3.up);
            }
    }

    //** Control
    private void OnTriggerStay(Collider col) {
        //* Batting
        if(col.gameObject.tag == "HitRangeArea"){
            pl.setSwingArcColor("red");
            float deg = calcHitRangeToDegree();
            if(pl.doSwing){
                // Debug.Log("Player:: doSwing=" + pl.doSwing);
                // Debug.Log("Ball_Prefab:: hitRangeSlider.value=" + gm.hitRangeDegSlider.value.ToString("N2") + ", deg=" + deg.ToString("N1"));
                isHited = true;
                pl.doSwing = false;

                //offset Axis
                const int leftSide = -1, rightSide = 1;
                int sign = pl.transform.localScale.x < 0 ? leftSide : rightSide;
                int offsetAxis = (sign < 0) ? (pl.MAX_HIT_DEG/2) * leftSide : (pl.MAX_HIT_DEG/2) * rightSide;
                // Debug.Log("Ball_Prefab:: ■offsetAxis=" + offsetAxis);

                //Vector3 dir = new Vector3((deg / pl.MAX_HIT_DEG) * sign,0,1); // -45 ~ 45°
                Vector3 dir = Vector3.forward;
                dir = Quaternion.AngleAxis(deg, Vector3.up) * dir;
                
                float degAbs = Mathf.Abs(deg);
                float power = (35 < degAbs)? 2f : (25 < degAbs)? 2.25f : (15 < degAbs)? 2.5f : (7.5f < degAbs)? 2.75f : 3f;
                // Debug.Log("Ball_Prefab:: deg=" + deg + ", power=" + power);
                rigid.velocity = Vector3.zero;
                rigid.AddForce((dir).normalized * speed * power, ForceMode.Impulse);
            }
        }
        else if(col.gameObject.tag == "Untagged"){
            pl.doSwing = false;
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "HitRangeArea"){
            pl.setSwingArcColor("yellow");
        }
        //* Ball(自分)を削除
        else if(col.gameObject.tag == "DestroyBallLine"){
            onDestroyMe();
        }
    }

    //** Obstacle
    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "NormalBlock"){
            Destroy(col.gameObject);
        }
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void onDestroyMe(){
        Destroy(this.gameObject);
    }
    public void setBallSpeed(int v){
        speed = v;
    }
    //----------------------------------------
    private float calcHitRangeToDegree(){
        //Degree
        float v = gm.hitRangeDegSlider.value;//0 ~ 1値
        float deg = (pl.MAX_HIT_DEG * 2); //真ん中がMAXになり、始めと終わりは0になるため。
        if(v <= 0.5f){deg = -Mathf.Abs(pl.MAX_HIT_DEG - (v * deg));}
        else{deg = Mathf.Abs(pl.MAX_HIT_DEG - (v * deg));}
        //SetHitAxisDir
        const int leftSide = -1, rightSide = 1;
        int sign = pl.transform.localScale.x < 0 ? leftSide : rightSide;
        //Start Degree Offset
        int offset = (sign < 0) ? (pl.offsetHitDeg) * leftSide : (pl.offsetHitDeg) * rightSide;

        return deg * sign + offset;
    }
}
