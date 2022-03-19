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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //TODO Set Hp by Stage Number
        hpTxt.text = hp.ToString();
    }

    void Update(){
        if(hp <= 0) Destroy(this.gameObject);
        hpTxt.text = hp.ToString();
    }

    private void OnTriggerEnter(Collider col) {
        //* GAME OVER
        if(col.gameObject.tag == "GameOverLine" && gm.state != GameManager.State.GAMEOVER){
            Debug.Log("--GAME OVER--");
            gm.setState(GameManager.State.GAMEOVER);
        }
    }

    public void decreaseHp() => --hp;
}
