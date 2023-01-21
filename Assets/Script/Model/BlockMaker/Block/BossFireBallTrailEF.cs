using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBallTrailEF : MonoBehaviour
{
    // GameManager gm;
    [SerializeField] Vector3 targetPos;    public Vector3 TargetPos {get=>targetPos; set=>targetPos=value;}

    void Update(){
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 50 * Time.deltaTime);
    }
}
