using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileBallGizmos : MonoBehaviour
{
    //* OutSide
    [SerializeField] GameManager gm;
    [SerializeField] float width;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.pl.FireBallCastWidth = width;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, width);
    }
}
