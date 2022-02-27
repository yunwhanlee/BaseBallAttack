using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player player;
    [SerializeField]private int speed = 10;
    private const int MAX_HIT_DEG = 45;

    Rigidbody rigid;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-this.transform.forward * speed, ForceMode.Impulse);

        Debug.Log("HitRange:: startPosZ=" + gm.hitRangeStartTf.position.z +  ", endPosZ="+ gm.hitRangeEndTf.position.z);
    }

    void Update(){
        //* Hit Range Area Ball Comeing Viewer GUI
        float startPosZ = gm.hitRangeStartTf.position.z;
        float endPosZ = gm.hitRangeEndTf.position.z;
        
        if(endPosZ <= this.transform.position.z && this.transform.position.z <= startPosZ){
            float offset = Mathf.Abs(startPosZ);
            float max = Mathf.Abs(endPosZ) - offset;
            float v = Mathf.Abs(this.transform.position.z) - offset;
            gm.hitBoxDegSlider.value = v / max;
        }
    }

    private void OnTriggerStay(Collider col) {
        if(player.doSwing){
            if(col.gameObject.tag == "HitRangeArea"){
                float v = gm.hitBoxDegSlider.value;
                float deg = (MAX_HIT_DEG * 2); //真ん中がMAXになり、始めと終わりは0になるため。
                if(v <= 0.5f){deg = -Mathf.Abs(MAX_HIT_DEG - (v * deg));}
                else{deg = Mathf.Abs(MAX_HIT_DEG - (v * deg));}

                if(player.doSwing){
                    Debug.Log("Player:: doSwing=" + player.doSwing);
                    Debug.Log("Ball_Prefab:: ■hitRangeSlider.v=" + v.ToString("N2") + ", ■deg=" + deg.ToString("N1"));
                    player.doSwing = false;

                    Vector3 dir = new Vector3(deg/MAX_HIT_DEG,0,1); // -45 ~ 45°
                    float degAbs = Mathf.Abs(deg);
                    float power = (35 < degAbs)? 1.5f : (25 < degAbs)? 2.25f : (15 < degAbs)? 3 : (7.5f < degAbs)? 4.5f : 6;
                    Debug.Log("Ball_Prefab:: deg=" + deg + ", power=" + power);
                    rigid.velocity = Vector3.zero;
                    rigid.AddForce((dir).normalized * speed * power, ForceMode.Impulse);
                }
            }
            else if(col.gameObject.tag == "Untagged"){
                player.doSwing = false;
            }
        }
    }

    public void setBallSpeed(int v){
        speed = v;
    }
}
