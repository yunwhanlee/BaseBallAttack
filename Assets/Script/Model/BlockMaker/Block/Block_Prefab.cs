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

    private void OnCollisionEnter(Collision col) {
        //* GAME OVER
        if(col.gameObject.tag == "GameOverLine"){
            Debug.Log("GAME OVER!!!!!!");
        }
    }

    private void OnTriggerEnter(Collider col) {
        //* GAME OVER
        if(col.gameObject.tag == "GameOverLine"){
            Debug.Log("GAME OVER!!!!!!");
        }
    }

    public void decreaseHp() => --hp;
}
