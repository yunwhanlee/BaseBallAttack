using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;

    const string TWO = "2", ONE = "1", ZERO = "0", SHOOT = "SHOOT", NULL = "";

    [SerializeField] bool isReadyShoot;   public bool IsReadyShoot { get => isReadyShoot; set => isReadyShoot = value;}
    [SerializeField] bool isExclamationMarkOn;   public bool IsExclamationMarkOn { get => isExclamationMarkOn; set => isExclamationMarkOn = value;}
    [SerializeField] float throwBallSpeed;
    [SerializeField] public GameObject mainBall;
    [SerializeField] public GameObject subBall;

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

        //* Ball Shoot (毎一回実行)
        if(!isReadyShoot){
            isReadyShoot = true;
            coShootCountID = StartCoroutine(coShootCount()); //* ID登録
        }
    }
/// -----------------------------------------------------------------
/// 関数
/// -----------------------------------------------------------------
    public void init() {
        IsExclamationMarkOn = false;
        throwBallSpeed = LM._.THROW_BALL_SPEED;
    }

    public IEnumerator coShootCount(){
        gm.readyBtn.gameObject.SetActive(true);

        //* Shoot Cnt
        SM.ins.sfxPlay(SM.SFX.CountDown.ToString()); 
        gm.ShootCntTxt.text = TWO;
        yield return Util.delay1;

        SM.ins.sfxPlay(SM.SFX.CountDown.ToString()); 
        gm.ShootCntTxt.text = ONE;

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
            gm.ShootCntTxt.text = ZERO;
            yield return Util.delay1;

            SM.ins.sfxPlay(SM.SFX.CountDown.ToString());
        }

        //* Shoot Ball
        Debug.Log("BALL 発射！");
        SM.ins.sfxPlay(SM.SFX.CountDownShoot.ToString());
        gm.throwScreenAnimSetTrigger(DM.ANIM.ThrowBall.ToString());

        gm.ShootCntTxt.text = SHOOT;
        yield return Util.delay0_3;
        
        gm.ShootCntTxt.text = NULL;

        Debug.Log("ballPreviewDirGoal="+gm.ballPreviewDirGoal.transform.position+", entranceTfPos="+entranceTf.position);
        Vector3 goalDir = (gm.ballPreviewDirGoal.transform.position - entranceTf.position).normalized;
        
        // GameObject ins = ObjectPool.getObject(ObjectPool.DIC.MainBall.ToString(), entranceTf.position, Quaternion.LookRotation(goalDir), gm.ballStorage);
        // ins.name = DM.NAME.MainBall.ToString();
        // ins.transform.SetParent(gm.ballGroup);
        GameObject ins = setBallObject(DM.NAME.MainBall.ToString(), entranceTf, Quaternion.LookRotation(goalDir));

        //* (BUG-55) もしエラーで、DownWallコライダーのIsTriggerがFalseの場合、
        //* 投げるボールが壁にぶつかってプレイヤーへ届かないので、Trueに戻す処理。
        // if(!gm.downWallCollider.isTrigger){
        //     gm.downWallCollider.isTrigger = true;
        // }

        throwBall(ins, goalDir);
    }
    public GameObject setBallObject(string name, Transform tf, Quaternion rotation){
        GameObject ins = ObjectPool.getObject(name, tf.position, rotation, gm.ballStorage);
        if(ins.name != name)    ins.name = name;
        ins.transform.SetParent(gm.ballGroup);

        return ins;
    }
    public void stopCoShootCount(){
        if(coShootCountID != null){
            StopCoroutine(coShootCountID);
            coShootCountID = null;
            isReadyShoot = false;
        }
    }
    public void throwBall(GameObject ins, Vector3 goalDir){
        gm.readyBtn.gameObject.SetActive(false);
        var ball = ins.GetComponent<Ball_Prefab>();
        int extra = Random.Range(0, 5);
        ball.Speed = (throwBallSpeed + extra) * Time.fixedDeltaTime;
        ball.myRigid.AddForce(goalDir * ball.Speed, ForceMode.Impulse);
    }
}
