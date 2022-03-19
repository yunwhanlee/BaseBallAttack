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
        //* GAMEOVER
        if(col.gameObject.tag == "GameOverLine" && gm.state != GameManager.State.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void decreaseHp() => --hp;

    public void onDestroy() => Destroy(this.gameObject);
}
