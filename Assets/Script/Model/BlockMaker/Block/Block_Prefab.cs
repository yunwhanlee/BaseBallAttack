using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block_Prefab : MonoBehaviour
{
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};

    public GameManager gm;

    //* Material Instancing
    private MeshRenderer meshRenderer;
    public Color[] colors;
    public Material[] materials;

    public int hp;
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //* Material Instancing
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = Instantiate(meshRenderer.material);

        // Leveling HP
        hp = (gm.stage <= 5) ? 1
        : (gm.stage <= 10) ? 2
        : (gm.stage <= 15) ? 3
        : (gm.stage <= 20) ? 4
        : 5;
        hpTxt.text = hp.ToString();

        // Material
        switch(hp){
            case 1 : meshRenderer.material = materials[(int)BlockMt.PLAIN]; break;
            case 2 : meshRenderer.material = materials[(int)BlockMt.WOOD]; break;
            case 3 : meshRenderer.material = materials[(int)BlockMt.SAND]; break;
            case 4 : meshRenderer.material = materials[(int)BlockMt.REDBRICK]; break;
            case 5 : meshRenderer.material = materials[(int)BlockMt.IRON]; break;
        }

        // 色
        int randIdx = Random.Range(0, colors.Length);
        meshRenderer.material.SetColor("_ColorTint", colors[randIdx]);
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
