using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block_Prefab : MonoBehaviour
{
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};

    public GameManager gm;
    public EffectManager em;
    public Player pl;

    //* Material Instancing
    private MeshRenderer meshRd;
    public Color[] colorList;
    private Color color;
    public Material[] mts;
    private Material originMt;
    public Material whiteHitMt;

    //* Value
    [SerializeField] private int hp = 1;
    [SerializeField] private int exp = 10;

    //* GUI
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();

        //* Material Instancing
        meshRd = GetComponent<MeshRenderer>();
        meshRd.material = Instantiate(meshRd.material);

        //TODO Leveling HP--------------------
        //hp = (gm.stage <= 5) ? 1 : (gm.stage <= 10) ? 2 : (gm.stage <= 15) ? 3 : (gm.stage <= 20) ? 4 : 5;
        int rand = Random.Range(0,100);
        if      (gm.stage <=  5) hp = rand < 85 ? 1 : 2;
        else if (gm.stage <=  9) hp = rand < 85 ? 2 : 1;
        else if (gm.stage <= 13) hp = rand < 85 ? 3 : (rand <= 95)? 2 : 1;
        else if (gm.stage <= 17) hp = rand < 75 ? 4 : (rand <= 90)? 3 : 2;
        else if (gm.stage <= 21) hp = rand < 50 ? 6 : (rand <= 75)? 5 : (rand <= 85)? 4 : (rand <= 95)? 3 : 2;
        else if (gm.stage <= 26) hp = rand < 60 ? 7 : (rand <= 85)? 6 : 5;
        else if (gm.stage <= 31) hp = rand < 65 ? 9 : (rand <= 80)? 8 : 7;
        else if (gm.stage <= 35) hp = rand < 60 ? 10 : (rand <= 75)? 9 : (rand <= 85)? 8 : 7;
        else if (gm.stage <= 40) hp = rand < 55 ? 12 : (rand <= 75)? 11 : (rand <= 90)? 10 : 9;

        //------------------------------------
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
        int randIdx = Random.Range(0, colorList.Length);
        color = colorList[randIdx];
        meshRd.material.SetColor("_ColorTint", color);
        
        originMt = meshRd.material; // Save Original Material
    }

    void Update(){
        hpTxt.text = hp.ToString();
    }

    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.gameObject.tag == "GameOverLine" && gm.state != GameManager.State.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void decreaseHp(int dmg) {
        hp -= dmg;
        StartCoroutine(coWhiteHitEffect(meshRd.material));
        if(hp <= 0) {
            em.createEffectBrokeBlock(this.transform, color);
            onDestroy();
        }
    }
    

    public void onDestroy(bool isInitialize = false) {
        if(!isInitialize) pl.addExp(exp); //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        Destroy(this.gameObject);
    }

    IEnumerator coWhiteHitEffect(Material curMt){ //* 体力が減ったら、一瞬間白くなって戻すEFFECT
        meshRd.material = whiteHitMt;
        yield return new WaitForSeconds(0.05f);
        meshRd.material = originMt;//* (BUG) WaitForSeconds間にまた衝突が発生したら、白くなる。
    }
}
