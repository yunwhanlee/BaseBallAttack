using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;

    [SerializeField]private int ballSpeed;
    [SerializeField]private bool isBallExist;   public bool IsBallExist { get => isBallExist; set => isBallExist = value;}
    [SerializeField]private float time;
    [SerializeField]private float shootSpan = 4f;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;


    void Start(){
        resetCountingTime();
    }

    void Update(){
        if(gm.STATE == GameManager.State.GAMEOVER) return;
        if(gm.STATE == GameManager.State.WAIT) return;

        //* 発射 前) ボールが存在しない
        if(!IsBallExist){
            //* COUNTING
            time -= Time.deltaTime;
            gm.setShootCntText(time.ToString("N0"));
            gm.readyBtn.gameObject.SetActive(true);

            //TODO ボール投げる　レベルリング
            int rand = Random.Range(0, 100);
            if(time <= 1f && 10 < rand){
                time = 0;
            }

            //* 発射
            if(time <= 0){
                Debug.Log("🥎BALL 発射！");
                IsBallExist = true;
                gm.throwScreenAnimSetTrigger("ThrowBall");
                resetCountingTime();
                
                gm.setShootCntText("SHOOT");
                Debug.Log("ballPreviewDirGoalPos="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
                Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
                GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir), gm.ballGroup);

                instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
            }
        }
        else{//* ボールが存在し、飛んでいる。★
            gm.readyBtn.gameObject.SetActive(false);
            pl.previewBundle.SetActive(false);
        }
    }
    public void resetCountingTime() => time = shootSpan;
}
