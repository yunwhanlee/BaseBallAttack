using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    public GameManager gm;
    [SerializeField]private int ballSpeed;

    [SerializeField]private bool isBallExist;
    [SerializeField]private float time;
    [SerializeField]private float shootSpan = 4f;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;


    void Start(){
        time = shootSpan;
    }

    void Update(){
        if(gm.state == GameManager.State.WAIT) return;

        //* ボールが存在しないときのみ
        if(!isBallExist){
            //* 発射前 COUNTING
            gm.setShootCntText(time.ToString("N0"));
            time -= Time.deltaTime;

            //* 発射
            if(time <= 0){
                Debug.Log("🥎BALL 発射！");
                isBallExist = true;
                time = shootSpan;
                GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.identity);
                instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
            }
        }
    }
    public void setIsBallExist(bool boolen){
        isBallExist = boolen;
    }
}
