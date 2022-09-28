using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using SpriteGlow;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(Animator))]
public class Block_Prefab : MonoBehaviour
{
    public enum ColorIndex{RED, YELLOW, GREEN, BLUE};
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};
    public enum BlockType {NORMAL, BOMB, LR_ARROW, UPDOWN_ARROW};

    //* OutSide
    public GameManager gm; EffectManager em; Player pl; BlockMaker bm;
    SpriteGlowEffect itemUISprGlowEf;
    BoxCollider boxCollider;
    Animator animator;

    //* Value
    private const int TREASURECHEST_ORB_CNT = 15;
    private const int ITEMUI_SPRGLOW_MIN = 1;
    private float itemUISprGlowCnt = 0;
    private float itemUISprGlowSpan = 7.5f;
    private int itemUISprGlowSpd = 5;
    private bool itemUISprGlowTrigger = false;
    Vector3 itemBlockBombBoxSize = new Vector3(3,2,2);

    [Header("Mesh Material")]
    [SerializeField] MyMesh mesh; //* Material Instancing
    public Material[] originMts;
    private Color color;
    public Material[] mts;
    public Material whiteHitMt;
    public Transform itemTypeImgGroup;
    public SpriteGlowEffect sprGlowEf;

    [Header("TYPE")]
    public BlockMaker.BLOCK kind;
    [SerializeField] BlockType type = BlockType.NORMAL;

    [Header("STATUS")]
    [SerializeField] bool isDotDmg;  public bool IsDotDmg {get => isDotDmg; set => isDotDmg = value;}
    [SerializeField] int hp = 1;    public int Hp {get => hp; set => hp = value;}
    [SerializeField] int exp = 10;  public int Exp {get => exp; set => exp = value;}
    [SerializeField][Range(1, 100)] int itemTypePer;

    [Header("SPAWN ANIMATION")]
    [SerializeField] Vector3 defScale;
    [SerializeField] float minLimitVal;
    [SerializeField] float spawnAnimSpeed;

    [Header("GUI")]
    public Text hpTxt;

    void Awake(){ //! 継承：親の場合、定義するソースはAwake()にしないと、Nullになる。
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em;
        pl = gm.pl;
        bm = gm.bm;

        sprGlowEf = GetComponentInChildren<SpriteGlowEffect>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        //* Material Instancing🌟
        mesh = new MyMesh(this);
        originMts = mesh.getOriginalMts();
    }

    void Start() {
        setType();　//* (NormalBlockのみ)
        setHp();
        setStyle(); //* (TreasureChest、healBlock、Boss 除外)
        spawnAnim("Init");
    }

    protected void Update(){
        hpTxt.text = Hp.ToString();
        spawnAnim("Play");
        animateItemTypeUISprGlowEF(ref itemUISprGlowCnt);//* ItemType Glow Animation
    }

//*-----------------------------------------
//* 関数
//*-----------------------------------------
    private void setType(){
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
            type = (BlockType)Random.Range(0, typeCnt);
            // Debug.Log("Block_Prefab:: typeCnt= " + typeCnt + ", itemType=" + itemType + " " + (int)itemType);

            //既にあるイメージObj中の一つをランダムで活性化
            var obj = itemTypeImgGroup.GetChild((int)type).gameObject;//.SetActive(true);
            obj.SetActive(true);
            itemUISprGlowEf = obj.GetComponent<SpriteGlowEffect>();
        }
    }
    private void setHp(){
        switch(kind){
            case BlockMaker.BLOCK.TreasureChest:
                Hp = 1;
                break;    
            case BlockMaker.BLOCK.Normal:
            case BlockMaker.BLOCK.Long:
            case BlockMaker.BLOCK.Heal:
                Hp = (gm.stage % bm.BLOCK2_SPAN == 0)? gm.stage * 5 : gm.stage; //* Block2 : Block1
                break;
        }
        hpTxt.text = Hp.ToString();
    }
    private void setStyle(){
        if(kind == BlockMaker.BLOCK.Normal || kind == BlockMaker.BLOCK.Long){
            // Debug.LogFormat("Block_Prefab:: kind={0}", kind);
            //* Material
            if(0 < Hp && Hp <= 10){
                Exp = 10;   mesh.block[0].material = bm.Mts[(int)BlockMt.PLAIN]; 
            }
            else if(11 < Hp && Hp <= 20){
                Exp = 20;   mesh.block[0].material = bm.Mts[(int)BlockMt.WOOD];
            }
            else if(21 < Hp && Hp <= 30){
                Exp = 30;   mesh.block[0].material = bm.Mts[(int)BlockMt.SAND];
            }
            else if(31 < Hp && Hp <= 40){
                Exp = 40;   mesh.block[0].material = bm.Mts[(int)BlockMt.REDBRICK];
            }
            else if(41 < Hp){
                Exp = 50;   mesh.block[0].material = bm.Mts[(int)BlockMt.IRON];
            }

            //* 色
            int randIdx = Random.Range(0, bm.Colors.Length);
            color = bm.Colors[randIdx];
            mesh.block[0].material.SetColor("_ColorTint", color);
            switch(randIdx){
                case (int)ColorIndex.RED:       color = Color.red; break;
                case (int)ColorIndex.YELLOW:    color = Color.yellow; break;
                case (int)ColorIndex.GREEN:     color = Color.green; break;
                case (int)ColorIndex.BLUE:      color = Color.blue; break;
            }

            //* Apply
            sprGlowEf.GlowColor = color;
            originMts[0] = mesh.block[0].material; //* オリジナルMt 保存。(材質O、色O ➡ Block用)
        }
    }
    private void spawnAnim(string type){
        switch(type){
            case "Init":
                defScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                spawnAnimSpeed = 6f;
                minLimitVal = defScale.x * 0.99f;
                transform.localScale = Vector3.zero;
                break;
            case "Play":
                if(transform.localScale.x < defScale.x){
                    // Debug.Log("Block_Prefab:: Update():: transform.localScale= " + transform.localScale);
                    //* 99%まで大きくなったら、既存のサイズにする。(無駄な処理をしないため)
                    if(transform.localScale.x >= minLimitVal) 
                        transform.localScale = defScale;
                    transform.localScale = Vector3.Lerp(
                        transform.localScale, defScale, Time.deltaTime * spawnAnimSpeed
                    );
                }
                break;
        }
    }
    private void animateItemTypeUISprGlowEF(ref float cnt){
        if(itemUISprGlowEf){
            int min = ITEMUI_SPRGLOW_MIN;
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
        if(isTrigger){
            sprGlowEf.GlowBrightness = 8;   
            sprGlowEf.OutlineWidth = 8;
        }
        else{
            sprGlowEf.GlowBrightness = 0;
            sprGlowEf.OutlineWidth = 0;
        }
    }

    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.gameObject.tag == DM.TAG.GameOverLine.ToString() && gm.State != GameManager.STATE.GAMEOVER){
            gm.setGameOver();
        }
    }

    public void increaseHp(int heal){
        Hp += heal;
        em.createHealTxtEF(this.transform.position, heal);
        em.createHeartEF(new Vector3(transform.position.x, transform.position.y+2, transform.position.z));
    }

    public void decreaseHp(int dmg) {
        // Debug.Log($"decreaseHp(dmg= {dmg}):: gm= {gm}");
        Hp -= dmg;
        gm.comboCnt++;
        gm.comboTxt.GetComponent<Animator>().SetTrigger(DM.ANIM.IsHit.ToString());
        mesh.setWhiteHitEF();

        if(Hp <= 0) {
            //* アイテムブロック 処理
            switch (type){
                case BlockType.BOMB:
                    em.createItemBlockExplosionEF(this.transform.position);
                    RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, itemBlockBombBoxSize / 2, Vector3.up);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.tag == BlockMaker.NORMAL_BLOCK)  onDestroy(hit.transform.gameObject);
                    });
                    break;
                case BlockType.LR_ARROW:
                    em.createItemBlockDirLineTrailEF(this.transform.position, transform.right);
                    em.createItemBlockDirLineTrailEF(this.transform.position, -transform.right);
                    break;
                case BlockType.UPDOWN_ARROW:
                    em.createItemBlockDirLineTrailEF(this.transform.position, transform.forward);
                    em.createItemBlockDirLineTrailEF(this.transform.position, -transform.forward);
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
        int resultExp = (!isInitialize)? (int)(exp * pl.expUp.Value) : 0; //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        em.createBrokeBlockEF(target.transform.position, color);
        bm.createDropItemExpOrbPf(this.transform, resultExp);
        
        if(kind == BlockMaker.BLOCK.TreasureChest){
            for(int i=0; i<TREASURECHEST_ORB_CNT; i++){
                bm.createDropItemExpOrbPf(this.transform, resultExp);
            }
        }
        Destroy(target);
    }

    public int getDotDmg(float per){
        int res = (Hp >= 1)? (int)(Hp * per) : 1;
        res = (res <= 0)? 1 : res;
        Debug.LogFormat("<color=green>getDotDmg(per):: {0} * {1} = {2}</color>", Hp, per, res);
        return res;
    }

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        animator.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    //-------------------------------------------------------------
    /*  StartCoroutineは、
    /   MonoBehaviourを継承しないClassではできないから無理やりここで宣言。
    */
    public void callStartCoWhiteHitEF(SkinnedMeshRenderer[] msRds){
        StartCoroutine(mesh.coWhiteHitEffect(msRds));
    }
    public void callStartCoWhiteHitEF(MeshRenderer[] msRds){
        StartCoroutine(mesh.coWhiteHitEffect(msRds));
    }

    //-------------------------------------------------------------
    void OnDrawGizmos(){
        //* Type
        if(type == BlockType.BOMB){
            Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(this.transform.position, itemBlockExplostionRadius);
            // Gizmos.DrawWireCube(this.transform.position, new Vector3(3,2,2));
        }
    }
}
