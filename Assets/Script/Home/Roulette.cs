using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roulette : MonoBehaviour
{
    [SerializeField] bool isSpin; public bool IsSpin { get => isSpin; set => isSpin = value;}
    public RectTransform spinBoard;
    public int spinPower = 2000;
    private int reduceCnt = 0;

    void Start()
    {
    }

    void Update()
    {  
        // float speed = Time.deltaTime * (spinPower - reduceCnt++);
        float speed = Time.deltaTime * spinPower;
        if(isSpin){
            if(speed > 0){
                /*
                *   ⓵ transform.rotation : 0~1単位 -> eulerAnglesに変換する必要ある。
                *   ⓶ eulerAnglesでは、範囲が0~360まで。
                *      しかし、InspectorViewでは、範囲が-180~180まで。
                *      ⇒ 合わせる必要がある！
                */
                float zRot = spinBoard.eulerAngles.z;
                float angle = zRot % 360; // 
                // Debug.Log("Roulette:: speed= " + speed + ", angle= " + angle);
                if(angle < 180){
                    Debug.Log("Roulette:: spinPower= " + speed + ", angle= " + angle + " A");
                }
                else{
                    Debug.Log("Roulette:: spinPower= " + speed + ", angle= " + angle + " B");
                }
                spinBoard.transform.Rotate(0, 0, speed);
            }
            else{
                init();
            }
        }
    }

    private void init(){
        isSpin = false;
        reduceCnt = 0;
    }
}
