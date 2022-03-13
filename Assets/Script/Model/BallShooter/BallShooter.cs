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
    [SerializeField]public int strikeCnt = 0;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;


    void Start(){
        resetCountingTime();
    }

    void Update(){
        if(gm.state == GameManager.State.WAIT) return;

        //* ボールが存在しない、発射前
        if(!isBallExist){
            //* COUNTING
            time -= Time.deltaTime;
            gm.setShootCntText(time.ToString("N0"));
            gm.readyBtn.gameObject.SetActive(true);

            //* 発射
            if(time <= 0){
                Debug.Log("🥎BALL 発射！");
                isBallExist = true;
                strikeCnt = 0;
                foreach(var img in gm.strikeBallImgs) img.gameObject.SetActive(false); //GUI非表示 初期化
                resetCountingTime();
                gm.setShootCntText("SHOOT");
                Debug.Log("ballPreviewDirGoalPos="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
                Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
                GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir));

                instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
            }
        }
        else{//* ボールが存在し、飛んでいる。
            gm.readyBtn.gameObject.SetActive(false);
        }
    }

    public void setIsBallExist(bool boolen) => isBallExist = boolen;
    public void resetCountingTime() => time = shootSpan;
}
