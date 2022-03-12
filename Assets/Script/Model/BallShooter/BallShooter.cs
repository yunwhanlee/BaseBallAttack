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
    [SerializeField]public float strikeCnt = 0;
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
                Debug.Log("ballPreviewDirGoalPos="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
                Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
                GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir));
                
                instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
            }
        }
    }
    public void setIsBallExist(bool boolen){
        isBallExist = boolen;
    }
}
