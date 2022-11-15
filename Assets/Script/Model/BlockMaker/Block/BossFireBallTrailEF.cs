using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBallTrailEF : MonoBehaviour
{
    GameManager gm;
    [SerializeField] Vector3 target;    public Vector3 Target {get=>target; set=>target=value;}

    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update(){
        transform.position = Vector3.MoveTowards(transform.position, target, 50 * Time.deltaTime);
    }
}
