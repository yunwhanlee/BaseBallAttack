using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject hitAxisArrow;

    Animator anim;
    public bool doSwing = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void setAnimTrigger(string name){
        //Debug.Log("Player:: Swing");
        anim.SetTrigger(name);
        doSwing = true;
    }
}
