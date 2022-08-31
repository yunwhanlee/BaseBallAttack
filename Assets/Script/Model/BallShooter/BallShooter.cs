using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public Player pl;

    [SerializeField]private int ballSpeed;
    [SerializeField]private bool isBallExist;   public bool IsBallExist { get => isBallExist; set => isBallExist = value;}
    [SerializeField]private bool isExclamationMarkOn;   public bool IsExclamationMarkOn { get => isExclamationMarkOn; set => isExclamationMarkOn = value;}
    [SerializeField]private float time;
    [SerializeField]private float shootSpan = 2;
    [SerializeField]private int exclamationThrowPer = 50;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;
    [SerializeField]private GameObject exclamationMarkObj;   public GameObject ExclamationMarkObj { get => exclamationMarkObj; set => exclamationMarkObj = value;}


    void Start(){
        init();
        ExclamationMarkObj.SetActive(false);
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

            //* 「！」マークいきなりボール投げる。
            suddenlyThrowBall(exclamationThrowPer);

            //* 発射
            if(time <= 0){
                Debug.Log("〇 BALL 発射！");
                IsBallExist = true;
                gm.throwScreenAnimSetTrigger("ThrowBall");

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
    public void init() {
        time = shootSpan;
        IsExclamationMarkOn = false;
    }

    private void suddenlyThrowBall(int per){
        if(time <= 1f && !IsExclamationMarkOn){
            IsExclamationMarkOn = true;
            int rand = Random.Range(0, 100);
            Debug.LogFormat("「！」マーク登場： per({0}) < rand({1})? -> {2} </color>", per, rand, (rand > per)? "<color=blue>TRUE" : "<color=red>FALSE");
            if(rand > per)
                StartCoroutine(coShowExclamationMark());
        }
    }

    IEnumerator coShowExclamationMark(){
        ExclamationMarkObj.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        time = 0;
        ExclamationMarkObj.SetActive(false);
    }
}
