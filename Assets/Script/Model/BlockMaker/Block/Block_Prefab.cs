using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using SpriteGlow;

[System.Serializable]
public class SkillProperty{
    string name; public string Name {get => name;}
    [SerializeField] bool isOn;  public bool IsOn {get => isOn; set => isOn = value;}
    [SerializeField] int befCnt; public int BefCnt {get => befCnt; set => befCnt = value;}
    [SerializeField] int duration;   public int Duration {get => duration; set => duration = value;}
    [SerializeField] GameObject dotDmgEF;   public GameObject DotDmgEF {get => dotDmgEF; set => dotDmgEF = value;}

    public SkillProperty(string name){
        this.name = name;
        init();
    }
    public void init(){
        this.isOn = false;
        this.befCnt = -1;
        this.duration = 0;
        this.dotDmgEF = null;
    }
    public void startDuration(Block_Prefab bl){
        if(BefCnt == -1){
            BefCnt = Duration;
            //* EXTRA 処理
            if(Name == DM.PSV.FireProperty.ToString()){
                DotDmgEF = bl.gm.em.directlyCreateFireBallDotEF(bl.transform);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){
                //なし
            }
        }
    }

    public void endDuration(int span, Block_Prefab bl){
        if(Duration >= LM._.FIRE_DOT_DMG_DURATION){
            init();
            //* EXTRA 処理
            if(Name == DM.PSV.FireProperty.ToString()){
                GameObject.Destroy(DotDmgEF);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){
                int i=0;
                Array.ForEach(bl.mesh.block, mesh => mesh.materials = new Material[]{bl.originMts[i++]});
            }
            return;
        }
    }

    public void setDataNextStage(Block_Prefab bl){
        if(BefCnt != Duration){
            BefCnt = Duration;
            //* EXTRA 処理
            if(Name == DM.PSV.FireProperty.ToString()){
                int dmg = ((bl.Hp / 10) < 1)? 1 : (bl.Hp / 10); //* 10Percent Damage
                bl.decreaseHp(dmg);
                bl.gm.em.createCritTxtEF(bl.transform.position, dmg);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){
                Transform blTf = bl.transform;
                blTf.localPosition = new Vector3(
                    blTf.localPosition.x, 
                    blTf.localPosition.y, 
                    blTf.localPosition.z + 1
                );
            }
        }
    }
}

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(Animator))]
public class Block_Prefab : MonoBehaviour
{
    public enum ColorIndex{RED, YELLOW, GREEN, BLUE};
    public enum BlockMt {PLAIN, WOOD, SAND, REDBRICK, IRON};
    public enum BlockType {NORMAL, BOMB, LR_ARROW, UPDOWN_ARROW};

    //* OutSide
    public GameManager gm; 
    EffectManager em; 
    Player pl; 
    public BlockMaker bm;
    SpriteGlowEffect itemUISprGlowEf;
    protected BoxCollider boxCollider;
    protected Animator anim;

    //* Value
    private const int TREASURECHEST_ORB_CNT = 10;
    private const int ITEMUI_SPRGLOW_MIN = 1;
    private float itemUISprGlowCnt = 0;
    private float itemUISprGlowSpan = 7.5f;
    private int itemUISprGlowSpd = 5;
    private bool itemUISprGlowTrigger = false;
    Vector3 itemBlockBombBoxSizeVec = new Vector3(3,2,2);

    [Header("Mesh Material")]
    [SerializeField] public MyMesh mesh; //* Material Instancing
    private Color color;
    public Material[] originMts;
    public Material[] mts;
    public Material whiteHitMt;
    public Material iceMt;
    public Transform itemTypeImgGroup;
    public SpriteGlowEffect sprGlowEf;

    [Header("TYPE")]
    public BlockMaker.KIND kind;
    [SerializeField] BlockType type = BlockType.NORMAL;

    [Header("STATUS")]
    [SerializeField] bool isDotDmg;  public bool IsDotDmg {get => isDotDmg; set => isDotDmg = value;}
    [SerializeField] SkillProperty fireDotDmg;  public SkillProperty FireDotDmg {get => fireDotDmg; set => fireDotDmg = value;}
    [SerializeField] SkillProperty freeze;  public SkillProperty Freeze {get => freeze; set => freeze = value;}

    [SerializeField] int maxHp = 1;    public int MaxHp {get => maxHp; set => maxHp = value;}
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

        MaxHp = Hp;
        sprGlowEf = GetComponentInChildren<SpriteGlowEffect>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

        //* Material Instancing🌟
        mesh = new MyMesh(this);
        originMts = mesh.getOriginalMts();

        //* 値 初期化
        fireDotDmg = new SkillProperty(DM.PSV.FireProperty.ToString());
        freeze = new SkillProperty(DM.PSV.IceProperty.ToString());
    }

    void Start(){
        setType(); //* (NormalBlockのみ)
        setHp();
        setStyle(); //* (TreasureChest、healBlock、Boss 除外)
        spawnAnim("Init");
    }

    protected void Update(){
        hpTxt.text = Hp.ToString();
        spawnAnim("Play");
        animateItemTypeUISprGlowEF(ref itemUISprGlowCnt);//* ItemType Glow Animation

        //* Property
        checkFireDotDmg();
        checkIceFreeze();
    }
    private void OnTriggerEnter(Collider col) {
        //* GAMEOVER
        if(col.transform.CompareTag(DM.TAG.GameOverLine.ToString()) && gm.State != GameManager.STATE.GAMEOVER){
            gm.setGameOver();
        }
    }
//*-----------------------------------------
//* 関数
//*-----------------------------------------
    private void setType(){
        int rand = Random.Range(0,100);
        itemTypePer = (int)(100 * ((LM._.itemTypePer * 0.01f) + pl.itemSpawn.Val)); //百分率

        if(kind == BlockMaker.KIND.Normal && rand < itemTypePer){
            int len = System.Enum.GetValues(typeof(BlockType)).Length; 
            rand = Random.Range(0, len - 1); //* LastIndex
            type = (BlockType)rand + 1; //* BlockType.NORMAL 除外
            int tranformIdx = rand; 
            // Debug.Log($"ItemBlockType:: len= {len}, rand= {rand}, type= {type}");

            //* 該当なTransform活性化
            var obj = itemTypeImgGroup.GetChild(tranformIdx).gameObject;//.SetActive(true);
            obj.SetActive(true);
            itemUISprGlowEf = obj.GetComponent<SpriteGlowEffect>();
        }
    }
    private void setHp(){
        switch(kind){
            case BlockMaker.KIND.TreasureChest:
                Hp = 1;
                break;
            case BlockMaker.KIND.Long:
            case BlockMaker.KIND.Obstacle:
                Hp = gm.stage * 5;
                break;
            case BlockMaker.KIND.Normal:
            case BlockMaker.KIND.Heal:
                Hp = Util._.getCalcEquivalentSequence(gm.stage, 4);
                break;
        }
        hpTxt.text = Hp.ToString();
    }

    public void setStyle(){
        if(kind == BlockMaker.KIND.Normal || kind == BlockMaker.KIND.Long){
            // Debug.LogFormat("Block_Prefab:: kind={0}", kind);
            //* Set Exp & Material
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
        if(kind == BlockMaker.KIND.Boss) return;
        if(kind == BlockMaker.KIND.Obstacle) return;
        
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

    public void checkFireDotDmg(){
        if(FireDotDmg.IsOn){
            //* Enter 1Time
            FireDotDmg.startDuration(this);
            //* End 1Time
            FireDotDmg.endDuration(LM._.FIRE_DOT_DMG_DURATION, this);
            //* 毎タン一回 => ★Duration++は, BlockMakerスクリプトで行う。
            fireDotDmg.setDataNextStage(this);
        }
    }
    private void checkIceFreeze(){
        if(Freeze.IsOn && !this.name.Contains("Boss")){
            //* Enter 1Time
            Freeze.startDuration(this);
            //* End 1Time
            Freeze.endDuration(LM._.ICE_FREEZE_DURATION, this);
            //* なぜか毎プレームICEマテリアルに変更しないと適用できない。
            Array.ForEach(this.mesh.block, mesh => mesh.materials = new Material[]{iceMt});
            //* 毎タン一回 => ★Duration++は, BlockMakerスクリプトで行う。
            Freeze.setDataNextStage(this);
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
    public void increaseHp(int heal){
        Hp += heal;
        em.createHealTxtEF(this.transform.position, heal);
        em.createHeartEF(new Vector3(transform.position.x, transform.position.y+2, transform.position.z));
    }

    public void decreaseHp(int dmg) {
        // Debug.Log($"decreaseHp(dmg= {dmg}):: gm= {gm}");
        Hp -= dmg;
        if(Hp < 0) Hp = 0;
        gm.comboCnt++;
        gm.comboTxt.GetComponent<Animator>().SetTrigger(DM.ANIM.IsHit.ToString());
        mesh.setWhiteHitEF();

        //* 破壊
        if(Hp <= 0) {
            //* アイテムブロック 処理
            switch (type){
                case BlockType.BOMB:
                    em.createItemBlockExplosionEF(this.transform.position);
                    RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, itemBlockBombBoxSizeVec / 2, Vector3.up);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.name.Contains(DM.NAME.Block.ToString())){
                            if(hit.transform.CompareTag(DM.TAG.BossBlock.ToString())) return;
                            onDestroy(hit.transform.gameObject);
                        }
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
        gm.activeSkillBtnList.ForEach(atvSkillBtn=>{
            atvSkillBtn.coolDownFillAmount();
        });
    }
    

    public virtual void onDestroy(GameObject target, bool isInitialize = false) {
        int resultExp = (!isInitialize)? (int)(Exp * pl.expUp.Val) : 0; //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        Debug.Log("virtual onDestroy():: resultExp= " + resultExp);
        em.createBrokeBlockEF(target.transform.position, color); //! (BUG) このEFFECTは可なり重いらしい、複数になったら、シーンのフリーズが掛かる。
        bm.createDropItemExpOrbPf(this.transform, resultExp);
        
        if(kind == BlockMaker.KIND.TreasureChest){
            for(int i=0; i<TREASURECHEST_ORB_CNT; i++){
                bm.createDropItemExpOrbPf(this.transform, resultExp);
            }
        }
        else if(kind == BlockMaker.KIND.Obstacle){
            em.createRockObstacleBrokenEF(this.transform.position);
        }
        Destroy(target);
    }

    public int getDotDmg(float per){
        int res = (Hp >= 1)? (int)(Hp * per) : 1;
        res = (res <= 0)? 1 : res;
        Debug.LogFormat("<color=green>getDotDmg(per):: {0} * {1} = {2}</color>", Hp, per, res);
        return res;
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
            Gizmos.DrawWireCube(this.transform.position, itemBlockBombBoxSizeVec);
        }
    }
}
