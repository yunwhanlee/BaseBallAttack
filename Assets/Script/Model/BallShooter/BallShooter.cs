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
    [SerializeField] float throwBallSpeed;
    [SerializeField] public GameObject ballPref;
    [SerializeField] public Transform entranceTf;
    [SerializeField] GameObject exclamationMarkObj;   public GameObject ExclamationMarkObj { get => exclamationMarkObj; set => exclamationMarkObj = value;}
    [SerializeField] GameObject bossFireBallMarkObj;    public GameObject BossFireBallMarkObj { get => bossFireBallMarkObj; set => bossFireBallMarkObj = value;}
    public Coroutine coShootCountID;

    void Start(){
        init();
        ExclamationMarkObj.SetActive(false);
        BossFireBallMarkObj.SetActive(false);
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }

    void Update(){
        if(gm.State == GameManager.STATE.GAMEOVER) return;
        if(gm.State == GameManager.STATE.WAIT) return;

        //* 発射 前) ボールが存在しない (毎一回実行)
        if(!isBallExist){
            Debug.Log("BallShooter::");
            isBallExist = true;
            coShootCountID = StartCoroutine(coShootCount());
        }
        else{//* ボールが存在し、飛んでいる。★
        }
    }
    public void init() {
        IsExclamationMarkOn = false;
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }
    public IEnumerator coShootCount(){
        gm.readyBtn.gameObject.SetActive(true);

        //* Shoot Cnt
        SM.ins.sfxPlay(SM.SFX.CountDown.ToString()); 
        gm.ShootCntTxt.text = "2";
        yield return Util.delay1;

        SM.ins.sfxPlay(SM.SFX.CountDown.ToString()); 
        gm.ShootCntTxt.text = "1";

        //* 「!」Mark
        int rand = Random.Range(0, 100);
        if(rand > LM._.SUDDENLY_THORW_PER){
            yield return Util.delay0_3;

            throwBallSpeed *= 1.4f;
            ExclamationMarkObj.SetActive(true);
            yield return Util.delay0_2;

            ExclamationMarkObj.SetActive(false);
        }
        else{
            yield return Util.delay1;

            SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
            gm.ShootCntTxt.text = "0";
            yield return Util.delay1;

            SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
        }

        //* Shoot Ball
        Debug.Log("BALL 発射！");
        SM.ins.sfxPlay(SM.SFX.CountDownShoot.ToString());
        gm.throwScreenAnimSetTrigger("ThrowBall");

        gm.ShootCntTxt.text = "SHOOT";
        yield return Util.delay0_3;

        gm.ShootCntTxt.text = "";

        Debug.Log("ballPreviewDirGoal="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
        Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
        GameObject ins = Instantiate(ballPref, entranceTf.position, Quaternion.LookRotation(goalDir), gm.ballGroup);

        //* (BUG-55) もしエラーで、DownWallコライダーのIsTriggerがFalseの場合、
        //* 投げるボールが壁にぶつかってプレイヤーへ届かないので、Trueに戻す処理。
        if(!gm.downWallCollider.isTrigger) 
            gm.downWallCollider.isTrigger = true;

        throwBall(ins, goalDir);
    }

    public void stopCoStop(){
        if(coShootCountID != null){
            StopCoroutine(coShootCountID);
            isBallExist = false;
        }
    }

    public void throwBall(GameObject ins, Vector3 goalDir){
        gm.readyBtn.gameObject.SetActive(false);

        var ball = ins.GetComponent<Ball_Prefab>();
        int extra = Random.Range(0, 5);
        ball.Speed = (throwBallSpeed + extra) * Time.fixedDeltaTime;
        ball.rigid.AddForce(goalDir * ball.Speed, ForceMode.Impulse);
    }
}
