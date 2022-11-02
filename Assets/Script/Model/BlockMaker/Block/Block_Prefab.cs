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
    public GameManager gm; 
    EffectManager em; 
    Player pl; 
    public BlockMaker bm;
    SpriteGlowEffect itemUISprGlowEf;
    protected BoxCollider boxCollider;
    protected Animator anim;

    //* Value
    private const int TREASURECHEST_ORB_CNT = 15;
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
    private int befStageCnt;
    [SerializeField] int propertyDuration;  public int PropertyDuration {get => propertyDuration; set => propertyDuration = value;}
    [SerializeField] bool isDotDmg;  public bool IsDotDmg {get => isDotDmg; set => isDotDmg = value;}
    [SerializeField] bool isFreeze;  public bool IsFreeze {get => isFreeze; set => isFreeze = value;}
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
        anim = GetComponent<Animator>();

        //* Material Instancing🌟
        mesh = new MyMesh(this);
        originMts = mesh.getOriginalMts();
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
        itemTypePer = (int)(100 * ((LM._.itemTypePer * 0.01f) + pl.itemSpawn.Value)); //百分率

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
    private void checkIceFreeze(){
        if(IsFreeze){
            //* Set Duration (Enter 1Time)
            if(PropertyDuration == 0){
                befStageCnt = gm.stage;
                PropertyDuration = LM._.ICE_FREEZE_DURATION + gm.stage;
            }

            //* Back Original Mt (End 1Time)
            if(gm.stage > PropertyDuration){
                IsFreeze = false;
                int i=0;
                Array.ForEach(mesh.block, mesh => mesh.materials = new Material[]{originMts[i++]});
                return;
            }

            //* Change Ice Mt (毎プレーム)
            Array.ForEach(mesh.block, mesh => mesh.material = iceMt);

            //* 重なるBLOCK止め (NextStage毎に)
            if(befStageCnt != gm.stage){
                befStageCnt = gm.stage;

                //* Ray
                float maxDistance = 100;
                Vector3 originPos = this.transform.position;
                Vector3 dir = Vector3.forward;
                RaycastHit hit;
                Debug.DrawRay(originPos, dir * maxDistance, Color.red, 3);
                if(this.kind != BlockMaker.KIND.Long){
                    if(Physics.Raycast(originPos, dir, out hit, maxDistance)){
                        bool isIceMat = checkExistMaterial(hit.transform, DM.NAME.IceMat.ToString());
                        
                        if(!isIceMat && hit.transform.name.Contains(DM.NAME.Block.ToString())){
                            const int MAX_Y = 2;
                            Debug.Log($"checkIceFreeze:: Hit.name= {hit.transform.name}, this.localPos.z= {this.transform.localPosition.z}, hit.localPos.z= {hit.transform.localPosition.z}");
                            // hit.transform.localScale = new Vector3(hit.transform.localScale.x, hit.transform.localScale.y + 1f, hit.transform.localScale.z);

                            //* Create Skip Block
                            if(hit.transform.localPosition.z >= MAX_Y){
                                Destroy(hit.transform.gameObject, 0.5f);
                            }
                            else if(hit.transform.localPosition.z == this.transform.localPosition.z){
                                hit.transform.localPosition = new Vector3(hit.transform.localPosition.x, hit.transform.localPosition.y, hit.transform.localPosition.z + 1);
                            }
                        }
                    }
                }
                else{
                    for(int i=-1; i<2; i++){
                        originPos = new Vector3(originPos.x + (i * 1.8f), originPos.y, originPos.z);
                        if(Physics.Raycast(originPos, dir, out hit, maxDistance)){
                        bool isMySelf = hit.transform.gameObject == this.gameObject;//* 自分自身なら、処理を抜ける。
                        if(!isMySelf && hit.transform.name.Contains(DM.NAME.Block.ToString())){
                            const int MAX_Y = 2;
                            Debug.Log($"checkIceFreeze:: Hit.name= {hit.transform.name}, this.localPos.z= {this.transform.localPosition.z}, hit.localPos.z= {hit.transform.localPosition.z}");
                            // hit.transform.localScale = new Vector3(hit.transform.localScale.x, hit.transform.localScale.y + 1f, hit.transform.localScale.z);

                            //* Create Skip Block
                            if(hit.transform.localPosition.z >= MAX_Y){
                                Destroy(hit.transform.gameObject, 0.5f);
                            }
                            else if(hit.transform.localPosition.z == this.transform.localPosition.z){
                                hit.transform.localPosition = new Vector3(hit.transform.localPosition.x, hit.transform.localPosition.y, hit.transform.localPosition.z + 1);
                            }
                        }
                    }
                    }
                }
                //* Fix Position 
                Vector3 freezingPos = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z + 1);
                this.transform.localPosition = freezingPos;
            }
        }
    }
    private bool checkExistMaterial(Transform tf, string mtName){
        bool result = false;
        Array.ForEach(tf.GetComponents<MeshRenderer>(), meshRdr => {
            result = Array.Exists(meshRdr.materials, mt => mt.name == DM.NAME.IceMat.ToString());
            if(result) return;
        });
        return result;
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
        Debug.Log("virtual onDestroy()::");
        int resultExp = (!isInitialize)? (int)(Exp * pl.expUp.Value) : 0; //* (BUG) GAMEOVER後、再スタートときは、EXPを増えないように。
        em.createBrokeBlockEF(target.transform.position, color);
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
