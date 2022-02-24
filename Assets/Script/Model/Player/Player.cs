using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void setAnimTrigger(string name){
        anim.SetTrigger(name);
    }
}
