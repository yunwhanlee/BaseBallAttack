using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBallTrailEF : MonoBehaviour
{
    GameManager gm;
    public Vector3 target;

    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, 50 * Time.deltaTime);
    }
}
