using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    [SerializeField]private int ballSpeed;

    [SerializeField]private float time;
    [SerializeField]private float span = 3;
    [SerializeField]public GameObject ballPref;
    [SerializeField]public Transform entranceTf;

    void Update()
    {
        time += Time.deltaTime;
        
        //* 発射
        if(time > span){
            time = 0;
            //Debug.Log("🥎BALL 発射！");
            GameObject instance = Instantiate(ballPref, entranceTf.position, Quaternion.identity);
            instance.GetComponent<Ball_Prefab>().setBallSpeed(ballSpeed);
            Destroy(instance, 5f);
        }
    }
}
