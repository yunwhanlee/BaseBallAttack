using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block_Prefab : MonoBehaviour
{
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};

    public GameManager gm;
    public Player pl;

    //* Material Instancing
    private MeshRenderer meshRd;
    public Color[] colors;
    public Material[] mts;

    //* Value
    [SerializeField] private int hp = 1;
    [SerializeField] private int exp = 10;

    //* GUI
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();

        //* Material Instancing
        meshRd = GetComponent<MeshRenderer>();
        meshRd.material = Instantiate(meshRd.material);

        // Leveling HP
        hp = (gm.stage <= 5) ? 1
        : (gm.stage <= 10) ? 2
        : (gm.stage <= 15) ? 3
        : (gm.stage <= 20) ? 4
        : 5;
        hpTxt.text = hp.ToString();

        // Material
        switch(hp){
            case 1 : exp = 10;  meshRd.material = mts[(int)BlockMt.PLAIN]; break;
            case 2 : exp = 20;  meshRd.material = mts[(int)BlockMt.WOOD]; break;
            case 3 : exp = 30;  meshRd.material = mts[(int)BlockMt.SAND]; break;
            case 4 : exp = 40;  meshRd.material = mts[(int)BlockMt.REDBRICK]; break;
            case 5 : exp = 50;  meshRd.material = mts[(int)BlockMt.IRON]; break;
        }

        // 色
        int randIdx = Random.Range(0, colors.Length);
        meshRd.material.SetColor("_ColorTint", colors[randIdx]);
    }

    void Update(){
        if(hp <= 0) onDestroy();
        hpTxt.text = hp.ToString();
    }

    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.gameObject.tag == "GameOverLine" && gm.state != GameManager.State.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void decreaseHp() => --hp;

    public void onDestroy(bool isInitialize = false) {
        if(!isInitialize) pl.addExp(exp); //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        Destroy(this.gameObject);
    }
}
