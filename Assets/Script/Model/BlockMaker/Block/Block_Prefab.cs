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
        this.befCnt = -1;       //直前前のカウント
        this.duration = 0;      //CoolTime
        this.dotDmgEF = null;
    }
    public void setStart(Block_Prefab bl){
        if(BefCnt == -1){
            BefCnt = Duration;
            //* EXTRA 処理
            if(Name == DM.PSV.FireProperty.ToString()){
                DotDmgEF = bl.gm.em.directlyCreateFireBallDotEF(bl.transform);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){}//なし
        }
    }

    public void setUpdate(Block_Prefab bl, int span){
        if(Duration >= span){
            init();
            if(Name == DM.PSV.FireProperty.ToString()){
                GameObject.Destroy(DotDmgEF);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){
                int i=0;
                Array.ForEach(bl.mesh.block, mesh => mesh.materials = new Material[]{bl.originMts[i++]});
            }
        }
        else{
            if(Name == DM.PSV.FireProperty.ToString()) {}//なし
            else if(Name == DM.PSV.IceProperty.ToString())
                Array.ForEach(bl.mesh.block, mesh => mesh.materials = new Material[]{bl.iceMt});
        }
    }

    public void setDamage(Block_Prefab bl, float per){
        if(BefCnt != Duration){
            BefCnt = Duration;
            //* EXTRA 処理
            if(Name == DM.PSV.FireProperty.ToString()){
                SM.ins.sfxPlay(SM.SFX.FireHit.ToString());
                int dmg = (int)((bl.Hp * per < 1)? 1 : bl.Hp * per); //* 10 Percent Damage
                bl.decreaseHp(dmg);
                bl.gm.em.createCritTxtEF(bl.transform.position, dmg);
            }
            else if(Name == DM.PSV.IceProperty.ToString()){
                SM.ins.sfxPlay(SM.SFX.IceHit.ToString());
                const float RADIUS = 1.2f;
                int dmg = (int)((bl.Hp * per < 1)? 1 : bl.Hp * per); //* 5 Percent Damage
                bl.gm.em.createIcePropertyNovaFrostEF(new Vector3(bl.transform.position.x, bl.transform.localPosition.y + 1.3f, bl.transform.position.z));
                Util._.DebugSphere(bl.transform.position, radius: RADIUS);
                Util._.sphereCastAllDecreaseBlocksHp(bl.transform, radius: RADIUS, dmg);
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
    private const int ITEMUI_SPRGLOW_MIN = 1;
    private float itemUISprGlowCnt = 0;
    private float itemUISprGlowSpan = 7.5f;
    private int itemUISprGlowSpd = 5;
    private bool itemUISprGlowTrigger = false;
    Vector3 itemBlockBombBoxSizeVec = new Vector3(3,2,2);

    [Header("Mesh Material")][Header("__________________________")]
    [SerializeField] public MyMesh mesh; //* Material Instancing
    private Color color;
    public Material[] originMts;
    public Material[] mts;
    public Material whiteHitMt;
    public Material iceMt;
    public Transform itemTypeImgGroup;
    public SpriteGlowEffect sprGlowEf;

    [Header("TYPE")][Header("__________________________")]
    public BlockMaker.KIND kind;
    [SerializeField] BlockType type = BlockType.NORMAL;

    [Header("STATUS")][Header("__________________________")]
    [SerializeField] bool isDotDmg;  public bool IsDotDmg {get => isDotDmg; set => isDotDmg = value;}
    [SerializeField] SkillProperty fireDotDmg;  public SkillProperty FireDotDmg {get => fireDotDmg; set => fireDotDmg = value;}
    [SerializeField] SkillProperty freeze;  public SkillProperty Freeze {get => freeze; set => freeze = value;}

    [SerializeField] int maxHp = 1;    public int MaxHp {get => maxHp; set => maxHp = value;}
    [SerializeField] int hp = 1;    public int Hp {get => hp; set => hp = value;}
    [SerializeField] int exp = 10;  public int Exp {get => exp; set => exp = value;}
    [SerializeField][Range(1, 100)] int itemTypePer;

    [Header("SPAWN ANIMATION")][Header("__________________________")]
    [SerializeField] Vector3 defScale;
    [SerializeField] float minLimitVal;
    [SerializeField] float spawnAnimSpeed;

    [Header("GUI")][Header("__________________________")]
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
        setExpAndStyle(); //* (TreasureChest、healBlock、Boss 除外)
        spawnAnim("Init");
    }

    protected void Update(){
        hpTxt.text = Hp.ToString();
        spawnAnim("Play");
        animateItemTypeUISprGlowEF(ref itemUISprGlowCnt);//* ItemType Glow Animation

        //* Property
        if(!this.name.Contains("Boss")){
            checkFireDotDmg();
            checkIceFreeze();
        }
    }
    void OnTriggerEnter(Collider col) {
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
        itemTypePer = (int)(100 * ((LM._.ITEM_TYPE_PER * 0.01f) + pl.itemSpawn.Val)); //百分率

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
        //* ブロック HPリスト 準備
        const int OFFSET = 100;
        int resHp = bm.HpCalcList[gm.stage] / OFFSET;

        //* ランダム要素
        int rand = Random.Range(0, 100);
        int extraVal = (rand < 60)? -1 : (rand < 90)? 0 : 1;

        switch(kind){
            case BlockMaker.KIND.TreasureChest:
                Hp = 1;
                break;
            case BlockMaker.KIND.Long:
                Hp = resHp * 5;
                break;
            case BlockMaker.KIND.Heal:
                Hp =  resHp / 2;
                break;
            case BlockMaker.KIND.Normal:
            case BlockMaker.KIND.Obstacle:
                Hp =  resHp;
                break;
        }
        hpTxt.text = Hp.ToString();
    }

    public void setExpAndStyle(){
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
            FireDotDmg.setStart(this);
            FireDotDmg.setUpdate(this, LM._.FIRE_DOT_DMG_DURATION);
            fireDotDmg.setDamage(this, LM._.FIRE_DOT_DMG_PER);//* Duration++は, BlockMakerスクリプトで行う。
        }
    }
    private void checkIceFreeze(){
        if(Freeze.IsOn && !this.name.Contains("Boss")){
            Freeze.setStart(this);
            Freeze.setUpdate(this, LM._.ICE_FREEZE_DURATION);
            Freeze.setDamage(this, LM._.ICE_FREEZE_DMG_PER);//* Duration++は, BlockMakerスクリプトで行う。
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
        if(this.name.Contains("Boss")) return;
        if(this.name.Contains(DM.NAME.Obstacle.ToString())) return;
        
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
        SM.ins.sfxPlay(SM.SFX.Heal.ToString());
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

        //* ブロックが凍った場合、当たったらICE-HITエフェクト追加 実行。
        if(this.freeze.IsOn)
            em.createSnowExplosionEF(this.transform.position);

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
                            Debug.Log("decreaseHp::type=BOMB:: hit.name=" + hit.transform.name);
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
        /* 
        * (BUG-18) BombBlockの処理が可笑しい。(エフェクトとかか重なったり)
        * 1.原因：OnDestroy()で処理を行っているので、この中にも同じ処理が必要。
        * 2.原因：GameObject targetをパラメータで渡すのに、この後this.transformで処理をする誤り。
        */
        SM.ins.sfxPlay(SM.SFX.DestroyBlock.ToString());
        Transform targetTf = target.transform; 
        int resultExp = (!isInitialize)? (int)(Exp * pl.expUp.Val) : 0; //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        Debug.Log("virtual onDestroy():: resultExp= " + resultExp);
        em.createBrokeBlockEF(targetTf.position, color);
        
        bm.createDropItemExpOrbPf(targetTf, resultExp);
        bm.createBossTargetMisslePf(targetTf);
        
        if(kind == BlockMaker.KIND.TreasureChest)
            for(int i=0; i<LM._.TREASURECHEST_ORB_CNT; i++)
                bm.createDropItemExpOrbPf(targetTf, resultExp);
        else if(kind == BlockMaker.KIND.Obstacle)
            em.createRockObstacleBrokenEF(targetTf.position);

        //* Add Destroy Blocks
        if(!isInitialize) 
            AcvDestroyBlocks.addDestroyBlockCnt();

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
