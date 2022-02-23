using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Prefab : MonoBehaviour
{
    [SerializeField]
    private int speed = 10;

    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(-this.transform.forward * speed, ForceMode.Impulse);
    }
}
