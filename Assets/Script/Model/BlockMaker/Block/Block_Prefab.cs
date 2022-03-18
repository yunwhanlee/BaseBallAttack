using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block_Prefab : MonoBehaviour
{
    public GameManager gm;
    public int hp;
    public Text hpTxt;

    void Start() {
        //TODO Set Hp by Stage Number
        hpTxt.text = hp.ToString();
    }

    void Update(){
        if(hp <= 0) Destroy(this.gameObject);
        hpTxt.text = hp.ToString();
    }

    public void decreaseHp() => --hp;
}
