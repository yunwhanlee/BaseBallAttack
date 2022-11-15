using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BossBlock : Block_Prefab{

    const int COL = 6;
    const int ROW = 3;
    const float STONE_SCALE_X = 2.8f;
    const float OBSTACLE_OFFSET_Z = -9;
    const int OBSTACLE_STONE_CNT = 1;
    const int BOSS_DIE_ORB_CNT = 80;
    const float BOSS_HEAL_RATIO = 0.2f;
    const int STONE_PER = 100;
    const int OBSTACLE_RESET_SPAN = 8;
    [SerializeField] int obstacleResetCnt = 0;
    // [SerializeField]  string skillType;
    [SerializeField] Transform bossDieOrbSpawnTf;
    [SerializeField] Transform mouthTf;

    [Header("【BOSS STATUS】")]
    [SerializeField] int level;
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
        this.anim.SetTrigger(DM.ANIM.Scream.ToString());
        const int SCREAM_INDEX = 6;
        float screamAnimTime = Util._.getAnimPlayTime(SCREAM_INDEX, this.anim);

        var rand = Random.Range(0, 100);
        level = gm.stage / LM._.BOSS_STAGE_SPAN;
        Debug.Log($"<color=yellow>BossBlock::activeBossSkill():: level= {level}, rand= {rand}, obstacleResetCnt= {obstacleResetCnt} / {OBSTACLE_RESET_SPAN}</color>");
        switch(level){
            case 1:
                // if(obstacleResetCnt == 0) createObstacleSingleType(4);
                // else{
                    // if(rand < 30){createObstacleSingleType(4);}
                    // else if(rand < 60){StartCoroutine(coBossHeal());}
                    // else 
                    bossAttack(screamAnimTime);
                // }
                obstacleResetCnt++;
                break;
            case 2:
                if(obstacleResetCnt == 0) createObstaclePatternType(0, 2);
                if(rand < 70){StartCoroutine(coBossHeal());}
                else{StartCoroutine(coBossHeal());}
                obstacleResetCnt++;
                break;
            case 3:
                if(obstacleResetCnt == 0) createObstaclePatternType(2, 4);
                if(rand < 70){StartCoroutine(coBossHeal());}
                else{StartCoroutine(coBossHeal());}
                obstacleResetCnt++;
                break;
            case 4:
                if(obstacleResetCnt == 0) createObstaclePatternType(4, 7);
                if(rand < 70){StartCoroutine(coBossHeal());}
                else{StartCoroutine(coBossHeal());}
                obstacleResetCnt++;
                break;
        }
        if(obstacleResetCnt == OBSTACLE_RESET_SPAN){
            obstacleResetCnt = 0;
            obstaclePosList = new List<Vector3>{};
            if(gm.obstacleGroup.childCount > 0) eraseObstacle();
        }
    }

    void bossAttack(float screamAnimTime){
        StartCoroutine(coFireBallAttack(screamAnimTime));
    }
    IEnumerator coFireBallAttack(float screamAnimTime){
        Debug.Log($"coFireBallAttack(screamAnimTime= {screamAnimTime})"); //2.333333f
        const float offsetX = 1.3f;
        Vector3 target = new Vector3(gm.pl.transform.position.x + offsetX, gm.pl.transform.position.y, gm.pl.transform.position.z);
        const int FIREBALLSHOOT_INDEX = 3;
        float attackAnimTime = Util._.getAnimPlayTime(FIREBALLSHOOT_INDEX, this.anim);
        float blessShootTiming = attackAnimTime * 0.5f;
        float targetReachTime = 0.5f;
        float delayGUIActive = 0.2f;

        //* GUI OFF
        gm.readyBtn.gameObject.SetActive(false);
        gm.activeSkillBtnGroup.gameObject.SetActive(false);

        //* Wait Scream Anim
        yield return new WaitForSeconds(screamAnimTime * 0.75f);
        gm.bs.ExclamationMarkObj.SetActive(true);
        Util._.DebugSphere(target, 1.25f, 2, "red");
        yield return new WaitForSeconds(screamAnimTime * 0.25f);
        
        //* Begin Attack Anim
        this.anim.SetTrigger(DM.ANIM.Attack.ToString());
        Debug.Log($"coFireBallAttack:: attackAnimSec= {attackAnimTime}"); // 1.0f
        yield return new WaitForSeconds(blessShootTiming);

        //* FireBallEF 生成
        GameObject fireBallIns = gm.em.createBossFireBallTrailEF(mouthTf.transform.position);
        fireBallIns.GetComponent<BossFireBallTrailEF>().Target = target;
        yield return new WaitForSeconds(targetReachTime);

        //* ExplosionEF 生成
        GameObject explosionIns = gm.em.createBossFireBallExplosionEF(target);
        // 衝突処理
        RaycastHit[] hits = Physics.SphereCastAll(explosionIns.transform.position, 1, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.CompareTag(DM.TAG.Player.ToString())){
                //TODO PLAYER STUN
                Debug.Log("EXPLOSION HIT PLAYER!! -> STUN");
            }
        });
        gm.bs.ExclamationMarkObj.SetActive(false);
        yield return new WaitForSeconds(delayGUIActive);
        
        //* GUI ON
        gm.readyBtn.gameObject.SetActive(true);
        gm.activeSkillBtnGroup.gameObject.SetActive(true);
    }

    IEnumerator coBossHeal(){
        Debug.Log("BossBlock::coBossHeal()");
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
        Debug.Log($"BossBlock:: decreaseHp({dmg})");
        base.decreaseHp(dmg);
        anim.SetTrigger(DM.ANIM.GetHit.ToString());
    }

    public override void onDestroy(GameObject target, bool isInitialize = false) {
        Debug.Log("BossBlock:: onDestroy()::");
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
    private void createObstacleSingleType(int maxCnt){
        //* OBSTACLE LIST 準備
        var obstaclePosList = getObstaclePosList();
        Debug.Log($"createObstacleSingleType:: maxCnt= {maxCnt}, obstaclePosList= {obstaclePosList.Count}");
        if(maxCnt > obstaclePosList.Count)
            maxCnt = obstaclePosList.Count;
        singleRandom(Random.Range(1, maxCnt));
    }
    private void createObstaclePatternType(int minIndex, int maxIndex){
        Debug.Log($"BossBlock::createObstacleStoneSkillPatternType()");
        //* OBSTACLE LIST 準備
        var obstaclePosList = getObstaclePosList();
        int rand = Random.Range(minIndex, maxIndex);
        switch(rand){
            case 0: patternColEven();       break;
            case 1: patternColOdd();        break;

            case 2: patternCutColumnLine(); break;
            case 3: patternCntRowLine();    break;

            case 4: patternGoBoard();       break;
            case 5: patternGoBoardRandom(); break;
            case 6: patternTriangle();      break;
        }
    }

    private List<Vector3> getObstaclePosList(){
        Debug.Log($"BossBlock::readyObstaclePosList:: obstaclePosList.Count= {obstaclePosList.Count}");
        if(obstaclePosList.Count > 0) return obstaclePosList; //* 障害物が存在するなら、初期化しない。
        
        //* OBSTACLE LIST 準備（初期化）
        obstaclePosList = new List<Vector3>(){};
        int len = COL * ROW;
        
        //TODO) BOSS SUMON OBSTACLE MAKE SEVERAL PATTERN。
        for(int i=0; i < len; i++){
            int rowIdx = i % COL;
            float x = BlockMaker.OFFSET_POS_X + (BlockMaker.SCALE_X * rowIdx);
            float z = OBSTACLE_OFFSET_Z - (i / COL);
            //* Float 0.1などが 0.8999..になる問題のため、小数点２桁までに設定。
            x = Util._.calcMathRoundDecimal(x,2);
            z = Util._.calcMathRoundDecimal(z,2);
            Debug.Log($"[{i}] -> rowIdx= {rowIdx} vector3({x}, 0, {z})");
            obstaclePosList.Add(new Vector3(x, 0, z));
        }
        return obstaclePosList;
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
    private void singleRandom(int createCnt){
        for(int i=0; i< createCnt; i++){
            int randIdx = Random.Range(0, obstaclePosList.Count);

            //* 位置が重ならないように処理。
            Vector3 randPos = obstaclePosList[randIdx];
            Debug.Log("randPos-->" + randPos);

            createObstacleStone(randIdx);
            obstaclePosList.RemoveAt(randIdx);
        }

        //* RESET
        if(obstaclePosList.Count <= 0)
            obstacleResetCnt = 0;
    }
    private void patternColEven(){
        for(int i=0; i < obstaclePosList.Count; i++){
            if(i % 2 == 0){
                    createObstacleStone(i);
            }
        }
    }
    private void patternColOdd(){
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
