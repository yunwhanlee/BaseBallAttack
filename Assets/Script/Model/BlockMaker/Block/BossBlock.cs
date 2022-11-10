using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BossBlock : Block_Prefab{

    const int COL = 6;
    const int ROW = 3;
    const float STONE_SCALE_X = 2.8f;
    const int OBSTACLE_STONE_CNT = 1;
    const int BOSS_DIE_ORB_CNT = 80;
    const float BOSS_HEAL_RATIO = 0.2f;

    const int STONE_PER = 100;
    const int OBSTACLE_RESET_SPAN = 5;
    int obstacleResetCnt = 0;
    [SerializeField] Transform bossDieOrbSpawnTf;

    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;
    [SerializeField] List<Vector3> obstaclePosList;

    private void OnDisable() {
        eraseObstacle();
    }

    void Start() {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bossDieOrbSpawnTf = GameObject.Find(DM.NAME.BossDieDropOrbSpot.ToString()).transform;
        activeBossSkill();
    }

    public void activeBossSkill(){ //* at NextStage
        var rand = Random.Range(0, 100);
        this.anim.SetTrigger(DM.ANIM.Scream.ToString());
        if(rand < STONE_PER && obstacleResetCnt % OBSTACLE_RESET_SPAN == 0){
            //* Skill #1
            eraseObstacle();
            createObstacleStoneSkill(OBSTACLE_STONE_CNT);
        }
        else{
            //* Skill #2
            StartCoroutine(coBossHealSkill());
        }
        obstacleResetCnt++;
    }

    IEnumerator coBossHealSkill(){
        yield return new WaitForSeconds(1);
        gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
        gm.em.createBossHealSkillEF(bossDieOrbSpawnTf.position);
        yield return new WaitForSeconds(1);
        Array.ForEach(gm.blockGroup.GetComponentsInChildren<Block_Prefab>(), block => {
            int val = (int)(block.Hp * BOSS_HEAL_RATIO);
            var hp = (val == 0)? 1 : block.Hp * BOSS_HEAL_RATIO;
            block.increaseHp((int)hp);
        });
    }

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        anim.SetBool(DM.ANIM.IsFly.ToString(), trigger);
    }

    public new void decreaseHp(int dmg) {
        base.decreaseHp(dmg);
        anim.SetTrigger(DM.ANIM.GetHit.ToString());
    }

    public override void onDestroy(GameObject target, bool isInitialize = false) {
        Debug.Log("bossBlock:: onDestroy()::");
        StartCoroutine(coPlayBossDieAnim(target));
    }

    IEnumerator coPlayBossDieAnim(GameObject target){
        this.transform.rotation = Quaternion.Euler(0,250,0);
        boxCollider.enabled = false;
        int resultExp = gm.stage * 10;

        const int DIE = 2;
        float playSec = Util._.getAnimPlayTime(DIE, this.anim);

        anim.SetTrigger(DM.ANIM.DoDie.ToString());

        yield return new WaitForSecondsRealtime(playSec * 0.7f);
        for(int i=0; i < BOSS_DIE_ORB_CNT; i++)
            bm.createDropItemExpOrbPf(bossDieOrbSpawnTf, resultExp, popPower: 3500);

        yield return new WaitForSecondsRealtime(playSec * 0.3f + playSec); //* 消すのが早すぎ感じで、少し待機。
        Destroy(target);
    }


    //* Skill #1
    private void createObstacleStoneSkill(int cnt){
        Debug.Log("BossBlock:: createObstacleStoneSkill");
        const float OFFSET_Z = -9;

        //* OBSTACLE LIST 準備
        obstaclePosList = new List<Vector3>(){};
        int len = COL * ROW;
        
        //TODO) BOSS SUMON OBSTACLE MAKE SEVERAL PATTERN。
        for(int i=0; i < len; i++){
            int rowIdx = i % COL;
            float x = BlockMaker.OFFSET_POS_X + (BlockMaker.SCALE_X * rowIdx);
            float z = OFFSET_Z - (i / COL);
            Debug.Log($"[{i}] -> rowIdx= {rowIdx} vector3({x}, 0, {z})");
            obstaclePosList.Add(new Vector3(x, 0, z));
        }

        //** OBSTACLE INS 生成
        Debug.Log($"obstaclePosList.Count= {obstaclePosList.Count}");

        // int bossLevel = gm.stage / LM._.BOSS_STAGE_SPAN - 1;
        // switch(bossLevel){
        //     case 0:
        //         break;
        // }
        
        // patternRandom(Random.Range(1, 3));
        // patternEven();
        // patternOdd();
        patternCutColumnLine();
        // patternCntRowLine();
        // patternGoBoard();
        // patternGoBoardRandom();
        // patternTriangle();
    }

    private void createObstacleStone(int i){
        gm.em.createBossObstacleSpawnEF(obstaclePosList[i]);
        Instantiate(obstacleStonePf, obstaclePosList[i], Quaternion.identity, gm.obstacleGroup);
    }
    public void eraseObstacle(){
        if(gm.obstacleGroup.childCount <= 0) return; 
        for(int i=0; i<gm.obstacleGroup.childCount; i++){
            // Debug.Log($"eraseObstacle():: obstacleGroup.GetChild({i})= {gm.obstacleGroup.GetChild(i)}");
            var childTf = gm.obstacleGroup.GetChild(i);
            Destroy(childTf.gameObject);
        }
    }

//* OBSTACLE PATTERN
    private void patternRandom(int createCnt){
        for(int i=0; i< createCnt; i++){
            int randIdx = Random.Range(0, obstaclePosList.Count);
            createObstacleStone(i);
            obstaclePosList.RemoveAt(i);
        }
    }
    private void patternEven(){
        for(int i=0; i < obstaclePosList.Count; i++){
            if(i % 2 == 0){
                    createObstacleStone(i);
            }
        }
    }
    private void patternOdd(){
        for(int i=0; i < obstaclePosList.Count; i++){
            if(i % 2 == 1){
                createObstacleStone(i);
            }
        }
    }
    private void patternCutColumnLine(){
        int randIdx = Random.Range(0, COL-1);
        for(int i=0; i < obstaclePosList.Count; i++){
            if(i % COL != randIdx){
                createObstacleStone(i);
            }
        }
    }
    private void patternCntRowLine(){
        int randIdx = Random.Range(0, ROW-1);
        for(int i=0; i < obstaclePosList.Count; i++){
            if(i / COL == randIdx)
                createObstacleStone(i);
        }
    }
    private void patternGoBoard(){
        int rand = Random.Range(0, 2);
        for(int i = 0; i < obstaclePosList.Count; i++){
            int rowIdx = i / COL;
            int colIdx = i % COL;
            int a = rand;
            int b = (a == 0)? 1 : 0;
            if(rowIdx % 2 == 0){ //even
                if(colIdx % 2 == a) createObstacleStone(i);
            }
            else{ //odd
                if(colIdx % 2 == b) createObstacleStone(i);
            }
        }
    }
    private void patternGoBoardRandom(){
        for(int i = 0; i < obstaclePosList.Count; i++){
            int rowIdx = i / COL;
            int colIdx = i % COL;
            int a = Random.Range(0, 2);
            int b = (a == 0)? 1 : 0;
            if(rowIdx % 2 == 0){
                if(colIdx % 2 == a) createObstacleStone(i);
            }
            else{
                if(colIdx % 2 == b) createObstacleStone(i);
            }
        }
    }
    private void patternTriangle(){
        int rand = Random.Range(0, 4);
        for(int r = 0; r < ROW; r++){
            switch(rand){
            case 0: //* 直角三角形(◥)
                for(int c = r; c < COL; c++){
                    int idx = c + COL * r;
                    createObstacleStone(idx);
                }
                break;
            case 1: //* 直角三角形REVERSE(◤)
                for(int c = 0; c < COL - r; c++){
                    int idx = c + COL * r;
                    createObstacleStone(idx);
                }
                break;
            case 2: //* 三角形REVERSE(▼)
                for(int c = r; c < COL - r; c++){
                    int idx = c + COL * r;
                    createObstacleStone(idx);
                }
                break;
            case 3: //* 三角形(▲)
                int reverseLen = (ROW - 1) - r;
                for(int c = reverseLen; c < COL - reverseLen; c++){
                    int idx = c + COL * r;
                    createObstacleStone(idx);
                }
                break;
        }
        

        }

    }
}
