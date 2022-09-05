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
    const int TREASURECHEST_ORB_CNT = 15;
    private GameManager gm;
    private EffectManager em;
    private Player pl;
    private BlockMaker bm;

    //* Material Instancing
    [SerializeField] private MeshRenderer[] meshRds;
    [SerializeField] private Material[] originMts;
    private Color color;
    public Material[] mts;
    public Material whiteHitMt;
    public Transform itemTypeImgGroup;

    private SpriteGlowEffect itemUISprGlowEf;
    private const int iTEM_UI_SPR_GLOW_MIN = 1;
    private float itemUISprGlowCnt = 0;
    private float itemUISprGlowSpan = 7.5f;
    private int itemUISprGlowSpd = 5;
    private bool itemUISprGlowTrigger = false;

    public SpriteGlowEffect sprGlowEf;

    //* Value
    public BlockMaker.BLOCK kind;
    [SerializeField] BlockType itemType;
    [SerializeField] int hp = 1;    public int Hp {get => hp; set => hp = value;}
    [SerializeField] int exp = 10;  public int Exp {get => exp; set => exp = value;}
    [SerializeField] bool isDotDmg;  public bool IsDotDmg {get => isDotDmg; set => isDotDmg = value;}
    [SerializeField] int itemTypePer;

    // Spawn Animation
    [SerializeField] Vector3 defScale;
    [SerializeField] float minLimitVal;
    [SerializeField] float spawnAnimSpeed;

    Vector3 itemBlockExplostionBoxSize = new Vector3(3,2,2);

    //* GUI
    public Text hpTxt;

    void Start() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();
        bm = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();

        kind = setBlockKindEnum();
        itemType = BlockType.NORMAL;

        //* Material Instancing🌟
        sprGlowEf = GetComponentInChildren<SpriteGlowEffect>();
        meshRds = this.GetComponentsInChildren<MeshRenderer>(); 
        
        Array.ForEach(meshRds, meshRd=> meshRd.material = Instantiate(meshRd.material));
        
        originMts = new Material[meshRds.Length];
        for(int i=0; i<meshRds.Length;i++){
            originMts[i] = meshRds[i].material; //* オリジナルMt 保存。(材質X、色X ➡ TreasureChest用)
        }

        //* Type Apply
        bool isItemBlock = false;
        int rand = Random.Range(0,100);
        itemTypePer = (int)(100 * pl.itemSpawn.Value); //百分率
        // Debug.Log("PassiveSkill:: Block_Prefab:: 「ItemSwpan Up」 rand("+rand+") <= per("+itemTypePer+") : " + ((rand <= itemTypePer)? "<color=green>true</color>" : "false"));
        if(kind == BlockMaker.BLOCK.Normal){
            isItemBlock = (rand < itemTypePer)? true : false;
        }
        if(isItemBlock){
            int typeCnt = System.Enum.GetValues(typeof(BlockType)).Length - 1; //enum Type Cnt Without Normal
            itemType = (BlockType)Random.Range(0, typeCnt);
            // Debug.Log("Block_Prefab:: typeCnt= " + typeCnt + ", itemType=" + itemType + " " + (int)itemType);

            //既にあるイメージObj中の一つをランダムで活性化
            var obj = itemTypeImgGroup.GetChild((int)itemType).gameObject;//.SetActive(true);
            obj.SetActive(true);
            itemUISprGlowEf = obj.GetComponent<SpriteGlowEffect>();
        }

        //* HP
        switch(kind){
            case BlockMaker.BLOCK.TreasureChest:
                Hp = 1;
                break;
            case BlockMaker.BLOCK.Normal:
            case BlockMaker.BLOCK.Long:
                Hp = (gm.stage % bm.BLOCK2_SPAN == 0)? gm.stage * 5 : gm.stage; //* Block2 : Block1
                break;
        }
        
        hpTxt.text = Hp.ToString();

        //* Set Block Style (TreasureChest除外)
        if(kind != BlockMaker.BLOCK.TreasureChest){
            Debug.LogFormat("Block_Prefab:: kind={0}", kind);
            //* Material
            if(0 < Hp && Hp <= 10){
                Exp = 10;
                meshRds[0].material = bm.Mts[(int)BlockMt.PLAIN]; 
            }
            else if(11 < Hp && Hp <= 20){
                Exp = 20;
                meshRds[0].material = bm.Mts[(int)BlockMt.WOOD];
            }
            else if(21 < Hp && Hp <= 30){
                Exp = 30;  
                meshRds[0].material = bm.Mts[(int)BlockMt.SAND];
            }
            else if(31 < Hp && Hp <= 40){
                Exp = 40;  
                meshRds[0].material = bm.Mts[(int)BlockMt.REDBRICK];
            }
            else if(41 < Hp){
                Exp = 50;  
                meshRds[0].material = bm.Mts[(int)BlockMt.IRON];
            }

            //* 色
            int randIdx = Random.Range(0, bm.Colors.Length);
            color = bm.Colors[randIdx];
            meshRds[0].material.SetColor("_ColorTint", color);
            switch(randIdx){
                case (int)ColorIndex.RED:       color = Color.red; break;
                case (int)ColorIndex.YELLOW:    color = Color.yellow; break;
                case (int)ColorIndex.GREEN:     color = Color.green; break;
                case (int)ColorIndex.BLUE:      color = Color.blue; break;
            }
            sprGlowEf.GlowColor = color;
            originMts[0] = meshRds[0].material; //* オリジナルMt 保存。(材質O、色O ➡ Block用)
        }

        //* Init Scale For Spawn Anim
        defScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        spawnAnimSpeed = 6f;
        minLimitVal = defScale.x * 0.99f;
        transform.localScale = Vector3.zero;
    }

    void Update(){
        hpTxt.text = hp.ToString();

        //* Spawn Animation
        if(transform.localScale.x < defScale.x){
            //* 99%まで大きくなったら、既存のサイズにする。(無駄な処理をしないため)
            // Debug.Log("Block_Prefab:: Update():: transform.localScale= " + transform.localScale);
            if(transform.localScale.x >= minLimitVal) {
                transform.localScale = defScale;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, defScale, Time.deltaTime * spawnAnimSpeed);
        }

        //* ItemType Glow Animation
        animateItemTypeUISprGlowEF(ref itemUISprGlowCnt);
    }

    private void animateItemTypeUISprGlowEF(ref float cnt){
        if(itemUISprGlowEf){
            int min = iTEM_UI_SPR_GLOW_MIN;
            float span = itemUISprGlowSpan;
            cnt = Mathf.Clamp(cnt, min, span);
            float val = (Time.deltaTime * itemUISprGlowSpd);

            // Set Trigger
            if(cnt == min)         itemUISprGlowTrigger = true;
            else if(cnt == span)   itemUISprGlowTrigger = false;

            // Set Value
            cnt += (itemUISprGlowTrigger)? val : -val;

            // Apply
            itemUISprGlowEf.GlowBrightness = cnt;
        }
    }

    public void setEnabledSpriteGlowEF(bool isTrigger){
        if(isTrigger){sprGlowEf.GlowBrightness = 8;   sprGlowEf.OutlineWidth = 8;}
        else{sprGlowEf.GlowBrightness = 0;   sprGlowEf.OutlineWidth = 0;}
    }

    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.gameObject.tag == "GameOverLine" && gm.State != GameManager.STATE.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void decreaseHp(int dmg) {
        Hp -= dmg;
        
        gm.comboCnt++;
        gm.comboTxt.GetComponent<Animator>().SetTrigger("isHit");

        Array.ForEach(meshRds, meshRd=> {
            StartCoroutine(coWhiteHitEffect(meshRd.material));
        });
        if(Hp <= 0) {
            //* アイテムブロック 処理
            switch (itemType){
                case BlockType.BOMB:
                    em.createItemBlockExplosionEF(this.transform);
                    RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, itemBlockExplostionBoxSize / 2, Vector3.up);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.tag == BlockMaker.NORMAL_BLOCK)  onDestroy(hit.transform.gameObject);
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
        if(kind == BlockMaker.BLOCK.TreasureChest){
            for(int i=0; i<TREASURECHEST_ORB_CNT; i++)
                bm.createDropItemOrb(this.transform, resultExp);
        }
        Destroy(target);
    }

    IEnumerator coWhiteHitEffect(Material curMt){ //* 体力が減ったら、一瞬間白くなって戻すEFFECT
        Array.ForEach(meshRds, meshRd => {
            meshRd.material = whiteHitMt;
        });
        yield return new WaitForSeconds(0.05f);

        for(int i=0; i<meshRds.Length; i++){
            meshRds[i].material = originMts[i];//* (BUG) WaitForSeconds間にまた衝突が発生したら、白くなる。
        }
    }

    public int getDotDmg(float per){
        int res = (Hp >= 1)? (int)(Hp * per) : 1;
        res = (res <= 0)? 1 : res;
        Debug.LogFormat("<color=green>getDotDmg(per):: {0} * {1} = {2}</color>", Hp, per, res);
        return res;
    }

    void OnDrawGizmos(){
        if(itemType == BlockType.BOMB){
            Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(this.transform.position, itemBlockExplostionRadius);
            Gizmos.DrawWireCube(this.transform.position, new Vector3(3,2,2));
        }
    }

    private BlockMaker.BLOCK setBlockKindEnum(){
        var res = gameObject.name.Contains(BlockMaker.BLOCK.Normal.ToString())? kind = BlockMaker.BLOCK.Normal
                : gameObject.name.Contains(BlockMaker.BLOCK.Long.ToString())? kind = BlockMaker.BLOCK.Long
                : gameObject.name.Contains(BlockMaker.BLOCK.TreasureChest.ToString())? kind = BlockMaker.BLOCK.TreasureChest : BlockMaker.BLOCK.Null;
        Debug.Log("setBlockKindEnum():: res= " + res);
        return res;
    }
}
