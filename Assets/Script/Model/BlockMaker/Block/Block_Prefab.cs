using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block_Prefab : MonoBehaviour
{
    public GameManager gm;

    //Material Instancing
    private MeshRenderer meshRenderer;
    public Color color;

    public int hp;
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //Material Instancing
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = Instantiate(meshRenderer.material);
        meshRenderer.material.SetColor("_ColorTint", color);

        //TODO Hp Leveling by Stage Number
        hp = (gm.stage <= 5) ? 1
        : (gm.stage <= 10) ? 2
        : (gm.stage <= 15) ? 3 : 4;
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
