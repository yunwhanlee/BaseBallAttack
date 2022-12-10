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
        float speed = Time.deltaTime * (spinPower - reduceCnt++);
        if(isSpin){
            if(speed > 0){
                Debug.Log("Roulette:: spinPower= " + speed);
                spinBoard.transform.Rotate(0, 0, -speed);
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
