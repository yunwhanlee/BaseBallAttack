using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;
    [SerializeField]private bool isBallExist;   public bool IsBallExist { get => isBallExist; set => isBallExist = value;}
    [SerializeField]private bool isExclamationMarkOn;   public bool IsExclamationMarkOn { get => isExclamationMarkOn; set => isExclamationMarkOn = value;}
    [SerializeField]private float time;
    [SerializeField]private float throwBallSpeed = LM._.THROW_BALL_SPEED;
    [SerializeField]private float shootSpan = 2;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;
    [SerializeField]private GameObject exclamationMarkObj;   public GameObject ExclamationMarkObj { get => exclamationMarkObj; set => exclamationMarkObj = value;}
    [SerializeField]private GameObject bossFireBallMarkObj;   public GameObject BossFireBallMarkObj { get => bossFireBallMarkObj; set => bossFireBallMarkObj = value;}


    void Start(){
        init();
        ExclamationMarkObj.SetActive(false);
        BossFireBallMarkObj.SetActive(false);
    }

    void FixedUpdate(){
        if(gm.State == GameManager.STATE.GAMEOVER) return;
        if(gm.State == GameManager.STATE.WAIT) return;

        //* 発射 前) ボールが存在しない
        if(!IsBallExist){
            //* COUNTING
            time -= Time.deltaTime;
            gm.ShootCntTxt.text = time.ToString("N0");
            gm.readyBtn.gameObject.SetActive(true);

            //* 「！」マークいきなりボール投げる。
            setSuddenlyThrowBall(LM._.SUDDENLY_THORW_PER);

            //* 発射
            if(time <= 0){
                Debug.Log("〇 BALL 発射！");
                IsBallExist = true;
                gm.throwScreenAnimSetTrigger("ThrowBall");

                gm.ShootCntTxt.text = "SHOOT";
                Debug.Log("ballPreviewDirGoal="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
                Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
                GameObject ins = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir), gm.ballGroup);

                throwBall(ins, goalDir);
            }
        }
        else{//* ボールが存在し、飛んでいる。★
            gm.readyBtn.gameObject.SetActive(false);
            pl.previewBundle.SetActive(false);
        }
    }
    public void init() {
        time = shootSpan;
        IsExclamationMarkOn = false;
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }

    public void throwBall(GameObject ins, Vector3 goalDir){
        var ball = ins.GetComponent<Ball_Prefab>();
        int extra = Random.Range(0, 5);
        ball.Speed = (throwBallSpeed + extra) * Time.fixedDeltaTime;
        ball.rigid.AddForce(goalDir * ball.Speed, ForceMode.Impulse);
    }

    private void setSuddenlyThrowBall(int per){
        if(time <= 1f && !IsExclamationMarkOn){
            IsExclamationMarkOn = true;
            int rand = Random.Range(0, 100);
            Debug.LogFormat("「！」マーク登場： per({0}) < rand({1})? -> {2} </color>", per, rand, (rand > per)? "<color=blue>TRUE" : "<color=red>FALSE");
            if(rand > per){
                throwBallSpeed *= 1.4f;
                StartCoroutine(coShowExclamationMark(0.2f));
            }
        }
    }
    public IEnumerator coShowExclamationMark(float sec){
        Debug.Log($"coShowExclamationMark(sec={sec})");
        ExclamationMarkObj.SetActive(true);
        yield return new WaitForSeconds(sec);
        time = 0;
        ExclamationMarkObj.SetActive(false);
    }
}
