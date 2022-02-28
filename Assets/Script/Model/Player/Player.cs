using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject hitAxisArrow;
    public Image swingArcArea;
    public int MAX_HIT_DEG = 45;

    Animator anim;
    public bool doSwing = false;

    void Start()
    {
        Debug.Log("swingArcArea.fillOrigin=" + swingArcArea.fillOrigin);
        anim = GetComponentInChildren<Animator>();
    }

    void Update(){
        setSwingArcPos();
    }

    private void setSwingArcPos(){
        RectTransform rect = swingArcArea.rectTransform;
        const int Top = 2, Left = 3;
        if(this.transform.position.x < 0){
            rect.localScale = new Vector3(Mathf.Abs(rect.localScale.x), rect.localScale.y, rect.localScale.z);
            swingArcArea.fillOrigin = Top;
        }
        else{
            rect.localScale = new Vector3(-Mathf.Abs(rect.localScale.x), rect.localScale.y, rect.localScale.z);
            swingArcArea.fillOrigin = Left;
        }
    }

    public void setAnimTrigger(string name){
        //Debug.Log("Player:: Swing");
        anim.SetTrigger(name);
        doSwing = true;
    }
    public void setSwingArcColor(string color){
        swingArcArea.color = color == "red" ? new Color(1,0,0,0.4f) : new Color(1,1,0,0.4f); //yellow
    }
}
