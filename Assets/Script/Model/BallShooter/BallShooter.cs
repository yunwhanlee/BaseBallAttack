using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;
    [SerializeField] bool isBallExist;   public bool IsBallExist { get => isBallExist; set => isBallExist = value;}
    [SerializeField] bool isExclamationMarkOn;   public bool IsExclamationMarkOn { get => isExclamationMarkOn; set => isExclamationMarkOn = value;}
    [SerializeField] float time;
    [SerializeField] float throwBallSpeed;
    [SerializeField] float shootSpan = 3;
    [SerializeField] public GameObject ballPref;
    [SerializeField] public Transform entranceTf;
    [SerializeField] GameObject exclamationMarkObj;   public GameObject ExclamationMarkObj { get => exclamationMarkObj; set => exclamationMarkObj = value;}
    [SerializeField] GameObject bossFireBallMarkObj;    public GameObject BossFireBallMarkObj { get => bossFireBallMarkObj; set => bossFireBallMarkObj = value;}

    [SerializeField] bool isTwoCount;
    [SerializeField] bool isOneCount;
    [SerializeField] bool isZeroCount;

    void Start(){
        init();
        ExclamationMarkObj.SetActive(false);
        BossFireBallMarkObj.SetActive(false);
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }

    void FixedUpdate(){
        if(gm.State == GameManager.STATE.GAMEOVER) return;
        if(gm.State == GameManager.STATE.WAIT) return;

        //* 発射 前) ボールが存在しない (毎一回実行)
        if(!IsBallExist){
            Debug.Log("BallShooter:: FixedUpdate::");
            //* COUNTING
            time -= Time.deltaTime;

            gm.ShootCntTxt.text = time.ToString("N0");

            if(gm.ShootCntTxt.text == "3")
                gm.ShootCntTxt.text = "";

            if(gm.ShootCntTxt.text == "2" && !isTwoCount){
                isTwoCount = true;
                SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
            }
            else if(gm.ShootCntTxt.text == "1" && !isOneCount){
                isOneCount = true;
                SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
            }
            else if(gm.ShootCntTxt.text == "0" && !isZeroCount){
                isZeroCount = true;
                SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
            }


            gm.readyBtn.gameObject.SetActive(true);

            //* 「！」マークいきなりボール投げる。
            setSuddenlyThrowBall(LM._.SUDDENLY_THORW_PER);

            //* 発射
            if(time <= 0){
                Debug.Log("〇 BALL 発射！");
                SM.ins.sfxPlay(SM.SFX.CountDownShoot.ToString());
                IsBallExist = true;
                gm.throwScreenAnimSetTrigger("ThrowBall");

                gm.ShootCntTxt.text = "SHOOT";
                StartCoroutine(coClearShootCntTxt());
                Debug.Log("ballPreviewDirGoal="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
                Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
                GameObject ins = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir), gm.ballGroup);

                throwBall(ins, goalDir);
            }
        }
        else{//* ボールが存在し、飛んでいる。★
            gm.readyBtn.gameObject.SetActive(false);
            pl.previewBundle.SetActive(false);
            isTwoCount = false;
            isOneCount = false;
            isZeroCount = false;
        }
    }

    public void init() {
        time = shootSpan;
        isTwoCount = false;
        isOneCount = false;
        isZeroCount = false;
        IsExclamationMarkOn = false;
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }

    IEnumerator coClearShootCntTxt(){
        yield return Util.delay0_3;
        gm.ShootCntTxt.text = "";
    }

    private void setSuddenlyThrowBall(int per){
        if(time <= 1f && !IsExclamationMarkOn){
            IsExclamationMarkOn = true;
            int rand = Random.Range(0, 100);
            Debug.LogFormat("「！」マーク登場： per({0}) < rand({1})? -> {2} </color>", per, rand, (rand > per)? "<color=blue>TRUE" : "<color=red>FALSE");
            if(rand > per){
                throwBallSpeed *= 1.4f;
                StartCoroutine(coShowExclamationMark());
            }
        }
    }

    public void throwBall(GameObject ins, Vector3 goalDir){
        var ball = ins.GetComponent<Ball_Prefab>();
        int extra = Random.Range(0, 5);
        ball.Speed = (throwBallSpeed + extra) * Time.fixedDeltaTime;
        ball.rigid.AddForce(goalDir * ball.Speed, ForceMode.Impulse);
    }
    public IEnumerator coShowExclamationMark(){
        ExclamationMarkObj.SetActive(true);
        yield return Util.delay0_2;
        time = 0;
        ExclamationMarkObj.SetActive(false);
    }
}
