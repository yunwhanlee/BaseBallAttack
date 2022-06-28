using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using SpriteGlow;

public class Block_Prefab : MonoBehaviour
{
    public enum ColorIndex{RED, YELLOW, GREEN, BLUE};
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};
    public enum BlockType {BOMB, LR_ARROW, UPDOWN_ARROW, NORMAL};

    private GameManager gm;
    private EffectManager em;
    private Player pl;
    private BlockMaker bm;

    //* Material Instancing
    private MeshRenderer meshRd;
    private Color color;
    public Material[] mts;
    private Material originMt;
    public Material whiteHitMt;
    public Transform itemTypeImgGroup;
    public SpriteGlowEffect sprGlowEf;

    //* Value
    [SerializeField] BlockType itemType;
    [SerializeField] private int hp = 1;
    [SerializeField] private int exp = 10;  public int Exp {get => exp; set=> exp = value;}
    [SerializeField] private int itemTypePer;
    private Vector3 itemBlockExplostionBoxSize = new Vector3(3,2,2);

    //* GUI
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();
        bm = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();
        sprGlowEf = GetComponentInChildren<SpriteGlowEffect>();

        //* Material Instancing🌟
        meshRd = GetComponent<MeshRenderer>();
        meshRd.material = Instantiate(meshRd.material);

        itemType = BlockType.NORMAL;

        //* Type Apply
        bool isItemBlock = false;
        int rand = Random.Range(0,100);
        itemTypePer = (int)(100 * pl.itemSpawn.Value); //百分率
        Debug.Log("PassiveSkill:: Block_Prefab:: 「ItemSwpan Up」 rand("+rand+") <= per("+itemTypePer+") : " + ((rand <= itemTypePer)? "<color=green>true</color>" : "false"));
        isItemBlock = (rand < itemTypePer)? true : false;
        if(isItemBlock){
            int typeCnt = System.Enum.GetValues(typeof(BlockType)).Length - 1; //enum Type Cnt Without Normal
            itemType = (BlockType)Random.Range(0, typeCnt);
            Debug.Log("Block_Prefab:: typeCnt= " + typeCnt + ", itemType=" + itemType + " " + (int)itemType);

            //既にあるイメージObj中の一つをランダムで活性化
            itemTypeImgGroup.GetChild((int)itemType).gameObject.SetActive(true);
        }

        //TODO Leveling HP
        //hp = (gm.stage <= 5) ? 1 : (gm.stage <= 10) ? 2 : (gm.stage <= 15) ? 3 : (gm.stage <= 20) ? 4 : 5;
        rand = Random.Range(0,100);
        if      (gm.stage <=  5) hp = rand < 85 ? 1 : 2;
        else if (gm.stage <=  9) hp = rand < 85 ? 2 : 1;
        else if (gm.stage <= 13) hp = rand < 85 ? 3 : (rand <= 95)? 2 : 1;
        else if (gm.stage <= 17) hp = rand < 75 ? 4 : (rand <= 90)? 3 : 2;
        else if (gm.stage <= 21) hp = rand < 50 ? 6 : (rand <= 75)? 5 : (rand <= 85)? 4 : (rand <= 95)? 3 : 2;
        else if (gm.stage <= 26) hp = rand < 60 ? 7 : (rand <= 85)? 6 : 5;
        else if (gm.stage <= 31) hp = rand < 65 ? 9 : (rand <= 80)? 8 : 7;
        else if (gm.stage <= 35) hp = rand < 60 ? 10 : (rand <= 75)? 9 : (rand <= 85)? 8 : 7;
        else if (gm.stage <= 40) hp = rand < 55 ? 12 : (rand <= 75)? 11 : (rand <= 90)? 10 : 9;
        hpTxt.text = hp.ToString();

        //* Material
        switch(hp){
            case 1 : Exp = 10;  meshRd.material = bm.Mts[(int)BlockMt.PLAIN]; break;
            case 2 : Exp = 20;  meshRd.material = bm.Mts[(int)BlockMt.WOOD]; break;
            case 3 : Exp = 30;  meshRd.material = bm.Mts[(int)BlockMt.SAND]; break;
            case 4 : Exp = 40;  meshRd.material = bm.Mts[(int)BlockMt.REDBRICK]; break;
            case 5 : Exp = 50;  meshRd.material = bm.Mts[(int)BlockMt.IRON]; break;
        }

        //* 色
        int randIdx = Random.Range(0, bm.Colors.Length);
        color = bm.Colors[randIdx];
        meshRd.material.SetColor("_ColorTint", color);
        switch(randIdx){
            case (int)ColorIndex.RED:       color = Color.red; break;
            case (int)ColorIndex.YELLOW:    color = Color.yellow; break;
            case (int)ColorIndex.GREEN:     color = Color.green; break;
            case (int)ColorIndex.BLUE:      color = Color.blue; break;
        }
        sprGlowEf.GlowColor = color;
        
        originMt = meshRd.material; // Save Original Material
    }

    void Update(){
        hpTxt.text = hp.ToString();
    }

    public void setEnabledSpriteGlowEF(bool isTrigger){
        if(isTrigger){sprGlowEf.GlowBrightness = 8;   sprGlowEf.OutlineWidth = 8;}
        else{sprGlowEf.GlowBrightness = 0;   sprGlowEf.OutlineWidth = 0;}
    }

    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.gameObject.tag == "GameOverLine" && gm.STATE != GameManager.State.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void decreaseHp(int dmg) {
        hp -= dmg;
        StartCoroutine(coWhiteHitEffect(meshRd.material));
        if(hp <= 0) {
            //* アイテムブロック 処理
            switch (itemType){
                case BlockType.BOMB:
                    em.createItemBlockExplosionEF(this.transform);
                    RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, itemBlockExplostionBoxSize / 2, Vector3.up);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.tag == "NormalBlock")  onDestroy(hit.transform.gameObject);
                    });
                    break;
                case BlockType.LR_ARROW:
                    em.createItemBlockDirLineTrailEF(this.transform, transform.right);
                    em.createItemBlockDirLineTrailEF(this.transform, -transform.right);
                    break;
                case BlockType.UPDOWN_ARROW:
                    em.createItemBlockDirLineTrailEF(this.transform, transform.forward);
                    em.createItemBlockDirLineTrailEF(this.transform, -transform.forward);
                    break;
            }
            onDestroy(this.gameObject);
        }

        //* ActiveSkill CoolTime Amount Down
        gm.activeSkillBtnList.ForEach(btn=>{
            btn.decreaseFillAmount();
        });
    }
    

    public void onDestroy(GameObject target, bool isInitialize = false) {
        em.createBrokeBlockEF(target.transform, color);
        int resultExp = (!isInitialize)? (int)(exp * pl.expUp.Value) : 0; //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        bm.createDropItemOrb(this.transform, resultExp);
        Destroy(target);
    }

    IEnumerator coWhiteHitEffect(Material curMt){ //* 体力が減ったら、一瞬間白くなって戻すEFFECT
        meshRd.material = whiteHitMt;
        yield return new WaitForSeconds(0.05f);
        meshRd.material = originMt;//* (BUG) WaitForSeconds間にまた衝突が発生したら、白くなる。
    }

    void OnDrawGizmos(){
        if(itemType == BlockType.BOMB){
            Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(this.transform.position, itemBlockExplostionRadius);
            Gizmos.DrawWireCube(this.transform.position, new Vector3(3,2,2));
        }
    }
}
