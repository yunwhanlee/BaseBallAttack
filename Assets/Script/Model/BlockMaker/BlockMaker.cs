using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class BlockMaker : MonoBehaviour
{
    public enum KIND {Normal, Long, TreasureChest, Heal, Boss, Obstacle, Null};

    //* OutSide
    public GameManager gm;

    public const int MAX_HORIZONTAL_GRID = 6, FIRST_CREATE_VERTICAL_CNT = 4; //DEAD_MAX-> 13
    public const float SCALE_X = 1.8f, SCALE_Y = 1, SPAWN_POS_X = -5;
    public const float OFFSET_POS_Z = -2;
    public const float OFFSET_POS_X = -4.5f;
    public const float LONG_OFFSET_POS_X = -2.7f;

    [Header("STATUS")][Header("__________________________")]
    public bool doCreateBlock;  public bool DoCreateBlock {get => doCreateBlock; set => doCreateBlock = value;}

    [Header("RESOURCE")][Header("__________________________")]
    public GameObject[] blockPrefs;
    public GameObject[] bossPrefs;
    public RectTransform bossStgBarRectTf;
    public Color[] colors;   public Color[] Colors {get => colors;}
    public Material[] mts;   public Material[] Mts {get => mts;}

    [Header("DROP ITEM PREFAB")][Header("__________________________")]
    public GameObject dropItemExpOrbPf;
    public GameObject dropItemRewardChestPf;

    public void Start() {
        //* Init Or AD-Revive
        var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>(); //* Previous Blocks Erase
        foreach(var block in blocks) block.onDestroy(block.gameObject, true);

        var obstacles = gm.obstacleGroup.GetComponentsInChildren<Block_Prefab>();//* Previous Obstacles Erase
        foreach(var obstacle in obstacles) obstacle.onDestroy(obstacle.gameObject, true);

        this.transform.position = new Vector3(0, 0.5f, -2);
        createBlockRow(KIND.Normal, true, FIRST_CREATE_VERTICAL_CNT);
    }

    void Update(){
        if(DoCreateBlock){
            Debug.Log($"BlockMaker::Update():: DoCreateBlock= {DoCreateBlock}");
            DoCreateBlock = false;
            moveDownBlock();
            bossSpawn();
        }
    }
//*---------------------------------------
//*  関数
//*---------------------------------------
    public void createBlockRow(KIND type, bool isFirstStage = false, int verticalCnt = 1){
        //* Value
        float xs = blockPrefs[(int)type].transform.localScale.x;
        float offsetPosX = (type == KIND.Normal)? OFFSET_POS_X : LONG_OFFSET_POS_X; //* Pivotが真ん中なので、OffsetPosX設定。

        switch(type){
            case KIND.Normal : 
                int i=0;
                for(int v=0; v<verticalCnt;v++){ //* 縦↕
                    Debug.Log("---------------------");
                    for(int h=0; h<MAX_HORIZONTAL_GRID;h++){ //* 横⇔
                        i++;
                        var ins = blockPrefs[(int)type];
                        #region Block Kind or Skip
                        //* #1. Block Skip?
                        int rand = Random.Range(0,100);
                        if(rand < LM._.skipBlockPer) continue; //Skip Block

                        //* #2. Block TreasureChest?
                        rand = Random.Range(0,100);
                        if(rand < LM._.treasureChestBlockPer)   ins = blockPrefs[(int)KIND.TreasureChest];

                        //* #3. Block HealBlock?
                        rand = Random.Range(0,100);
                        if(rand < LM._.healBlockPer)   ins = blockPrefs[(int)KIND.Heal];

                        //* #4. Block生成
                        float x = offsetPosX + h * xs;
                        float y = (isFirstStage)? 0 : ins.transform.position.y + gm.blockGroup.position.y;
                        float z = (isFirstStage)? -v : OFFSET_POS_Z;
                        Vector3 pos = new Vector3(x, y, z);
                        Vector3 setPos = (isFirstStage)? pos + gm.blockGroup.position : pos;
                        var block = Instantiate(ins, setPos, Quaternion.identity, gm.blockGroup);
                        block.name = (i + block.name).ToString();
                        // block.transform.localPosition = new Vector3(x, y, z);
                        #endregion
                    }
                }
                break;
            case KIND.Long : 
                for(int h=0; h<2; h++){
                    var ins = blockPrefs[(int)type];
                    float x = (h < 1)? offsetPosX + h * xs : offsetPosX + h * xs;
                    float y = ins.transform.position.y + gm.blockGroup.position.y;
                    Vector3 pos = new Vector3(x, y, OFFSET_POS_Z);
                    var block = Instantiate(ins, pos, Quaternion.identity, gm.blockGroup);
                    // block.transform.localPosition = new Vector3(x, y, OFFSET_POS_Z);
                }
                break;
        }
    }

    public void createDropItemExpOrbPf(Transform blockTf, int resultExp, int popPower = 350){
        Debug.Log("createDropItemExpOrbPf:: blockTf= " + blockTf + ", resultExp= " + resultExp);

        var ins = ObjectPool.getObject(ObjectPool.DIC.DropItemExpOrbPf.ToString(), blockTf.position, Quaternion.identity, gm.dropItemGroup);
        var block = blockTf.GetComponent<Block_Prefab>();
        ins.GetComponent<DropItem>().Exp = resultExp;
        ins.GetComponent<DropItem>().spawnPopUp(popPower);

        //* RewardChest生成(Only単一)。
        var dropItemObjs = gm.dropItemGroup.GetComponentsInChildren<Transform>();
        bool isExistRewardChest = Array.Find(dropItemObjs, obj => obj.name.Contains(DM.NAME.RewardChest.ToString()));

        if(isExistRewardChest) return; //存在するなら、下記の処理しない。
        int rand = Random.Range(0, 100);
        if(rand < LM._.REWARD_CHEST_PER){
            var ins2 = Instantiate(dropItemRewardChestPf, blockTf.position, Quaternion.identity, gm.dropItemGroup);
            ins2.GetComponent<DropItem>().spawnPopUp(popPower);
        }
    }

    public void moveDownBlock(){
        Debug.Log("moveDownBlock:: MOVE DOWN BLOCK ↓, gm.stage= " + gm.stage);
        // gm.blockGroup.position = new Vector3(gm.blockGroup.position.x, gm.blockGroup.position.y, gm.blockGroup.position.z - 1);
        for(int i=0; i < gm.blockGroup.childCount; i++){
            //* Init
            // var childs = gm.blockGroup.GetComponents<OverLapCheckBoxCollider>();
            // Array.ForEach(childs, child => child.IsMoved = false);
            
            var blockPos = gm.blockGroup.GetChild(i).transform.localPosition;
            gm.blockGroup.GetChild(i).transform.localPosition = new Vector3(
                blockPos.x, blockPos.y, blockPos.z - 1
            );

        }

        //* Next Set Block Type
        if(gm.stage % LM._.LONG_BLOCK_SPAN == 0){
            createBlockRow(KIND.Long);
        }else{
            createBlockRow(KIND.Normal);
        }
    }
#region BOSS
    public BossBlock getBoss(){
        return (gm.bossGroup.childCount > 0)?
            gm.bossGroup.GetChild(0).GetComponent<BossBlock>() : null;
    }
    public void bossSpawn(){
        if(gm.stage % LM._.BOSS_STAGE_SPAN == 0 && gm.bossGroup.childCount == 0){
                int idx = gm.stage / LM._.BOSS_STAGE_SPAN - 1;
                Debug.Log($"BOSS SPAWN!! index= {idx}");

                var pos = new Vector3(0, 0, bossPrefs[idx].transform.position.z + 2);
                var boss = Instantiate(bossPrefs[idx], pos, bossPrefs[idx].transform.rotation, gm.bossGroup);
                string NameStr = boss.name.Split('(')[0];
                NameStr = NameStr.Split('_')[2];

                // bossStgBarRectTf.anchorMin = new Vector2(0.1f, 0.5f);

                StartCoroutine(coPlayBossSpawnAnim(NameStr));
        }
    }
    private IEnumerator coPlayBossSpawnAnim(string bossName){
        gm.IsPlayingAnim = true;

        //* 再生時間 習得
        Animator camAnim = gm.cam1.GetComponent<Animator>();
        Animator uiAnim = gm.bossSapwnAnimObj.GetComponent<Animator>();
        var bossNameTxt = gm.bossNameTxt.GetComponent<Text>();

        //* InActive
        foreach(Transform child in gm.activeSkillBtnGroup){
            Button btn = child.GetComponent<Button>();
            btn.gameObject.SetActive(false);
        }
        gm.readyBtn.gameObject.SetActive(false);
        gm.statusFolderPanel.gameObject.SetActive(false);

        float animPlayTime = Util._.getAnimPlayTime(DM.ANIM.BossSpawnTxt_Spawn.ToString(), uiAnim);

        Time.timeScale = 0.1f;
        camAnim.SetTrigger(DM.ANIM.DoBossSpawn.ToString());
        uiAnim.SetTrigger(DM.ANIM.DoSpawn.ToString());
        bossNameTxt.text = bossName;

        yield return new WaitForSecondsRealtime(animPlayTime);
        //* Animation Finish
        gm.IsPlayingAnim = false;
        gm.setActiveSkillBtns(true);
        gm.readyBtn.gameObject.SetActive(true);
        gm.statusFolderPanel.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
    public void setGlowEF(Block_Prefab[] blocks, bool isOn){
        Array.ForEach(blocks, bl => bl.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(isOn));
    }
#endregion
    public void setGlowEFAllBlocks(bool isOn){ //* Block Grow EF 解除
        var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>();
        setGlowEF(blocks, isOn);
    }

    public KIND convertBlockStr2Enum(string name){
        return (name == KIND.Normal.ToString())? KIND.Normal 
        : KIND.Long;
    }
    public void checkIsHealBlock(){
        var blocks = gm.blockGroup.GetComponentsInChildren<HealBlock>();
        Array.ForEach(blocks, block => {
            if(block.kind == KIND.Heal){
                block.IsHeal = true;
            }
        });
    }
    public void decreaseBlockHP(GameObject obj, int dmg){
        Debug.Log($"decreaseBlockHP:: obj.tag= {obj.transform.tag}, obj.name= {obj.name}");
        //* TAGで親と子の正しいCLASSを判別してから、処理
        if(obj.CompareTag(DM.TAG.BossBlock.ToString()))
            obj.GetComponent<BossBlock>().decreaseHp(dmg);
        else if(obj.CompareTag(DM.TAG.NormalBlock.ToString())
            || obj.CompareTag(DM.TAG.LongBlock.ToString())
            || obj.CompareTag(DM.TAG.TreasureChestBlock.ToString())
            || obj.CompareTag(DM.TAG.HealBlock.ToString())
            )
            obj.GetComponent<Block_Prefab>().decreaseHp(dmg);
    }

    public void setBlockPropertyDuration(){
        for(int i=0; i<gm.blockGroup.childCount; i++){
            var block = gm.blockGroup.GetChild(i).GetComponent<Block_Prefab>();
            if(!block) return;
            if(block.Freeze.IsOn) block.Freeze.Duration++;
            if(block.FireDotDmg.IsOn) block.FireDotDmg.Duration++;
            //* 属性が増えたら、下へ追加。
        }

        //* (BUG-3) 障害物もFreezeからダメージ受けるように。
        for(int i=0; i<gm.obstacleGroup.childCount; i++){
            var obstacle = gm.obstacleGroup.GetChild(i).GetComponentInChildren<Block_Prefab>();
            Debug.Log("setBlockPropertyDuration:: obstacle= " + obstacle);
            if(!obstacle) return;
            if(obstacle.Freeze.IsOn) {obstacle.Freeze.Duration++;}
            if(obstacle.FireDotDmg.IsOn) obstacle.FireDotDmg.Duration++;
            //* 属性が増えたら、下へ追加。

        }
    }
}
