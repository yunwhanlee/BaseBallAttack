using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("【HIT 角度範囲】")]
    public GameObject hitAxisArrow;
    public Image swingArcArea;
    private float swingArcRange;
    public int MAX_HIT_DEG = 90; //左右ある方向の最大角度
    public int offsetHitDeg = 45; // Startが０度(←方向)から、↑ →向きに回る。

    [Header("Status")]
    public bool doSwing = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        swingArcRange = MAX_HIT_DEG * 2;//左右合わせるから * 2
        swingArcArea.fillAmount = swingArcRange / 360;
        swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,offsetHitDeg);
        hitAxisArrow.transform.rotation = Quaternion.Euler(0,-offsetHitDeg,0);
        Debug.Log("Start:: swingArcArea表示角度= " + swingArcRange + ", 角度をfillAmount値(0~1)に変換=" + swingArcRange / 360);
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
            swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,offsetHitDeg);
        }
        else{
            rect.localScale = new Vector3(-Mathf.Abs(rect.localScale.x), rect.localScale.y, rect.localScale.z);
            swingArcArea.fillOrigin = Left;
            swingArcArea.rectTransform.localRotation = Quaternion.Euler(0,0,-offsetHitDeg);
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
