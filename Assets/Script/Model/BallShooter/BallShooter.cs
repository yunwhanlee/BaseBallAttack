using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    public GameManager gm;
    [SerializeField]private int ballSpeed;

    [SerializeField]private float time;
    [SerializeField]private float shootSpan = 3;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;

    void Start(){
        time = shootSpan;
    }

    void Update()
    {
        if(gm.state == GameManager.State.WAIT) return;

        gm.shootCntTxt.text = time.ToString("N0");
        time -= Time.deltaTime;
    
        //* 発射
        if(time <= 0){
            time = shootSpan;
            gm.shootCntTxt.text = "";
            //Debug.Log("🥎BALL 発射！");
            GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.identity);
            instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
        }
    }
}
