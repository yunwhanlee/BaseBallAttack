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
    const float BOSS_HEAL_RATIO = 0.2f;
    const int BOSS_DIE_ORB_CNT = 30;
    const int STONE_PER = 100;
    const int OBSTACLE_RESET_SPAN = 5;
    const string BOSSATK_ANIM_NAME_LV1 = "Fireball Shoot";
    const string BOSSATK_ANIM_NAME_LV2 = "Flame Attack";
    const string BOSSATK_ANIM_NAME_LV3 = "Horn Attack";
    const string BOSSATK_ANIM_NAME_LV4 = "Fly Flame Attack";
    // [SerializeField]  string skillType;

    [Header("【BOSS STATUS】")][Header("__________________________")]
    [SerializeField] int bossLevel;
    [SerializeField] int obstacleResetCnt = 0;
    [SerializeField] GameObject obstacleStonePf;
    [SerializeField] List<Vector3> obstaclePosList;
    [SerializeField] Transform bossDieOrbSpawnTf;
    [SerializeField] Transform mouthTf;
    public bool isAttack;   public bool IsAttack { get => isAttack; set => isAttack = value;}


    private void OnDisable() {
        eraseObstacle();
    }

    void Start() {
        Debug.Log("BossBlock:: this.name= " + this.name + "::Start():: DM.ins.gm= " + DM.ins.gm);
        GameManager gm = DM.ins.gm;
        gm.bossLimitCnt = LM._.BOSS_LIMIT_SPAN;
        gm.bossLimitCntTxt.gameObject.SetActive(true);
        bossDieOrbSpawnTf = GameObject.Find(DM.NAME.BossDieDropOrbSpot.ToString()).transform;
        activeBossSkill(isFirst: true);
    }

    public void activeBossSkill(bool isFirst = false){ //* at NextStage
        if(Hp <= 0) return; //* (BUG) ボースが死んだら、ボーススキル処理をしない。

        float delay = isFirst? 0.45f : 0.1f;
        StartCoroutine(coBossScreamSFX(delay));

        //* Scream
        this.anim.SetTrigger(DM.ANIM.Scream.ToString());
        // gm.postProcessAnim.SetTrigger(DM.ANIM.DoBlur.ToString());
        
        var randPer = Random.Range(0, 10);
        // if(obstacleResetCnt == 0) randPer = 0;
        Debug.Log($"<color=yellow>BossBlock::activeBossSkill():: isFirst= {isFirst}, bossLevel= {bossLevel}, randPer= {randPer}, obstacleResetCnt= {obstacleResetCnt} / {OBSTACLE_RESET_SPAN}</color>");
        switch(bossLevel){
            case 1:
                if(gm.obstacleGroup.childCount == 0){createObstacleSingleType(5);}
                else if(randPer <= 3){StartCoroutine(coBossHeal());}
                else StartCoroutine(coBossAttack(BOSSATK_ANIM_NAME_LV1));
                break;
            case 2:
                if(gm.obstacleGroup.childCount == 0){createObstaclePatternType(0, 2);}
                else if(randPer <= 3){StartCoroutine(coBossHeal());}
                else StartCoroutine(coBossAttack(BOSSATK_ANIM_NAME_LV2));
                break;
            case 3:
                if(gm.obstacleGroup.childCount == 0){createObstaclePatternType(2, 4);}
                else if(randPer <= 3){StartCoroutine(coBossHeal());}
                else StartCoroutine(coBossAttack(BOSSATK_ANIM_NAME_LV3));
                break;
            case 4:
                if(gm.obstacleGroup.childCount == 0){createObstaclePatternType(4, 7);}
                else if(randPer <= 3){StartCoroutine(coBossHeal());}
                else StartCoroutine(coBossAttack(BOSSATK_ANIM_NAME_LV4));
                break;
        }
        obstacleResetCnt++;

        //* Reset
        if(obstacleResetCnt == OBSTACLE_RESET_SPAN){
            obstacleResetCnt = 0;
            obstaclePosList = new List<Vector3>{};
            if(gm.obstacleGroup.childCount > 0) eraseObstacle();
        }
    }

    IEnumerator coBossScreamSFX(float delay){
        switch(bossLevel){
            case 1:
                yield return new WaitForSeconds(delay);
                SM.ins.sfxPlay(SM.SFX.BossScream1.ToString());
                break;
            case 2:
                yield return new WaitForSeconds(1 + delay);
                SM.ins.sfxPlay(SM.SFX.BossScream2.ToString());
                break;
            case 3:
                yield return new WaitForSeconds(0.5f + delay);
                SM.ins.sfxPlay(SM.SFX.BossScream3.ToString());
                break;
            case 4:
                yield return new WaitForSeconds(1 + delay);
                SM.ins.sfxPlay(SM.SFX.BossScream4.ToString());
                break;
        }
    }

    IEnumerator coBossAttack(string attackAnimName){
        isAttack = true;
        Debug.Log($"BossBlock::coBossAttack()::"); //2.333333f
        const float OFFSET_X = 1.3f;
        Vector3 playerPos = new Vector3(gm.pl.transform.position.x + OFFSET_X, gm.pl.transform.position.y, gm.pl.transform.position.z);
        float screamAnimTime = Util._.getAnimPlayTime(DM.ANIM.Scream.ToString(), this.anim);
        float attackAnimTime = Util._.getAnimPlayTime(attackAnimName, this.anim);
        float blessShootTiming = attackAnimTime * 0.5f;
        float targetReachTime = blessShootTiming;
        // float delayGUIActive = 0.2f;
        float playerStunTime = 3;

        if(gm.cam2.activeSelf){
            gm.switchCamera();
        }

        //* Button UI OFF
        gm.readyBtn.gameObject.SetActive(false);
        gm.activeSkillBtnGroup.gameObject.SetActive(false);

        //* Wait Scream Anim
        yield return new WaitForSeconds(screamAnimTime * 0.7f);
        gm.bs.BossFireBallMarkObj.SetActive(true);
        SM.ins.sfxPlay(SM.SFX.BossAttackWarning.ToString());
        yield return new WaitForSeconds(screamAnimTime * 0.3f);
        
        //* Begin Attack Anim
        Debug.Log($"coFireBallAttack:: attackAnimTime= {attackAnimTime}");
        this.anim.SetTrigger(DM.ANIM.Attack.ToString());

        #region BOSS ATTACK LV TYPE
        //* FireBallEF 生成
        if(attackAnimName == BOSSATK_ANIM_NAME_LV1){ //* LV 1
            Util._.DebugSphere(playerPos, radius: 1.25f);//* Preview Spot 生成
            gm.em.createAimingEF(playerPos);
            yield return new WaitForSeconds(blessShootTiming);
            SM.ins.sfxPlay(SM.SFX.BossFireBallShoot.ToString());

            setFireBallTrailEFToTargetPos(playerPos);
            yield return new WaitForSeconds(targetReachTime);
            SM.ins.sfxPlay(SM.SFX.BossFireBallExplosion.ToString());

            //* ExplosionEF 生成
            setExplosionEFAndPlayerStun(playerPos, playerStunTime);
        }
        else if(attackAnimName == BOSSATK_ANIM_NAME_LV2){ //* LV 2
            List<float> targetPosList = new List<float>(){-3.0f, 0, 3.0f};
            int rand = Random.Range(0, 2); // 0 or 1
            if(rand == 1) targetPosList.Reverse();
            Vector3[] targetPosArr = new Vector3[3];
            for(int i=0; i<targetPosArr.Length; i++){
                setAimingEFToTargetPos(i, ref targetPosArr, ref targetPosList, playerPos);
                yield return Util.delay0_5;
                SM.ins.sfxPlay(SM.SFX.BossFireBallShoot.ToString());

                setFireBallTrailEFToTargetPos(targetPosArr[i]);
                yield return new WaitForSeconds(targetReachTime * 0.3f);
                SM.ins.sfxPlay(SM.SFX.BossFireBallExplosion.ToString());

                //* ExplosionEF 生成
                setExplosionEFAndPlayerStun(targetPosArr[i], playerStunTime);
            }
        }
        else if(attackAnimName == BOSSATK_ANIM_NAME_LV3){ //* LV 3
            List<float> targetPosList = new List<float>(){-3.0f, 0, 3.0f};
            
            //* Set ターゲットポジションランダム(２個)
            Vector3[] targetPosArr = new Vector3[2];
            for(int i=0; i<targetPosArr.Length; i++){
                setAimingEFToTargetPos(i, ref targetPosArr, ref targetPosList, playerPos);
            }
            SM.ins.sfxPlay(SM.SFX.BossFireBallShoot.ToString());
            yield return new WaitForSeconds(attackAnimTime * 0.6f);
            SM.ins.sfxPlay(SM.SFX.BossFireBallExplosion.ToString());

            //* ExplosionEF 生成
            Array.ForEach(targetPosArr, targetPos => {
                setExplosionEFAndPlayerStun(targetPos, playerStunTime);
            });
        }
        else if(attackAnimName == BOSSATK_ANIM_NAME_LV4){ //* LV 4
            int randAttackCnt = Random.Range(3,6);
            for(int k = 0; k < randAttackCnt; k++){
                //* 途中でファイルボールを当たったら、すぐFor文を終了。
                if(gm.pl.IsStun) break;

                List<float> targetPosList = new List<float>(){-3.0f, 0, 3.0f};
                //* Set ターゲットポジションランダム(N個)
                int randSeed = Random.Range(4, 10);
                int shootCnt = (randSeed < 6)? 1 : 2;
                Vector3[] targetPosArr = new Vector3[shootCnt];
                for(int i=0; i<targetPosArr.Length; i++){
                    setAimingEFToTargetPos(i, ref targetPosArr, ref targetPosList, playerPos);
                    SM.ins.sfxPlay(SM.SFX.BossFireBallShoot.ToString());
                }
                yield return new WaitForSeconds(attackAnimTime * 0.45f);
                SM.ins.sfxPlay(SM.SFX.BossFireBallExplosion.ToString());

                //* Attackが終わるまで空に泊まるアニメーション繰り返す。
                if(k % 3 == 0) this.anim.SetTrigger(DM.ANIM.Attack.ToString());

                //* ExplosionEF 生成
                Array.ForEach(targetPosArr, targetPos => {
                    setExplosionEFAndPlayerStun(targetPos, playerStunTime);
                });
            }
        }
        #endregion

        gm.bs.BossFireBallMarkObj.SetActive(false);
        yield return Util.delay0_2; //* delayGUIActive

        if(gm.pl.IsStun){
            yield return Util.delay3; //* playerStunTime
            gm.pl.anim.SetBool(DM.ANIM.IsIdle.ToString(), false);
            gm.setNextStage();
        }

        //* Button UI ON
        gm.readyBtn.gameObject.SetActive(true);
        gm.activeSkillBtnGroup.gameObject.SetActive(true);
        isAttack = false;
    }

    private void setAimingEFToTargetPos(int i, ref Vector3[] targetPosArr, ref List<float> targetPosList, Vector3 playerPos){
        bool isLv2_FlameAttack = (targetPosArr.Length == 3);
        int index  = (isLv2_FlameAttack)? 0 : Random.Range(0, targetPosList.Count);
        targetPosArr[i] = new Vector3(targetPosList[index], playerPos.y, playerPos.z);
        targetPosList.RemoveAt(index);
        Util._.DebugSphere(targetPosArr[i], radius: 1.25f);//* Preview Spot 生成
        gm.em.createAimingEF(targetPosArr[i]);
    }
    private void setFireBallTrailEFToTargetPos(Vector3 tergetPos){
        GameObject fireBallIns = gm.em.createBossFireBallTrailEF(mouthTf.transform.position);
        fireBallIns.GetComponent<BossFireBallTrailEF>().TargetPos = tergetPos;
    }
    private void setExplosionEFAndPlayerStun(Vector3 targetPos, float playerStunTime){
        GameObject explosionIns = gm.em.createBossFireBallExplosionEF(targetPos);
        //* Playerか判別
        Vector3 pos = explosionIns.transform.position;
        var hitObj = Util._.getTagObjFromRaySphereCast(pos, 1, DM.TAG.Player.ToString());
        if(hitObj){
            //* Upgrade 内容 適用
            float rand = Random.Range(0, 1.0f);
            var defencePer = DM.ins.personalData.Upgrade.Arr[(int)DM.UPGRADE.Defence].getValue();
            if(gm.pl.IsBarrier || rand <= defencePer) {
                Debug.Log($"<color=blue>DEFENCE! gm.pl.IsBarrier= {gm.pl.IsBarrier} OR rand={rand} < defencePer={defencePer} </color>");
                SM.ins.sfxPlay(SM.SFX.Defence.ToString());
                gm.pl.IsBarrier = false;

                //* Get Barrier Object
                GameObject barrierObj = gm.pl.getBarrierObj();
                
                gm.em.createDefenceEF(gm.pl.modelMovingTf.position);
                StartCoroutine(ObjectPool.coDestroyObject(barrierObj, gm.dropItemGroup));
                return;
            }
            gm.pl.IsStun = true;
            //* Stun EF
            SM.ins.sfxPlay(SM.SFX.Stun.ToString());
            gm.em.createStunEF(gm.pl.modelMovingTf.position, playerStunTime);
            gm.pl.anim.SetBool(DM.ANIM.IsIdle.ToString(), true);
        }
    }

    IEnumerator coBossHeal(){
        Debug.Log("BossBlock::coBossHeal()");
        yield return Util.delay1; //new WaitForSeconds(1);
        SM.ins.sfxPlay(SM.SFX.BossHealSpell.ToString());
        gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
        gm.em.createBossHealSkillEF(bossDieOrbSpawnTf.position);
        yield return Util.delay1; //new WaitForSeconds(1);
        Array.ForEach(gm.blockGroup.GetComponentsInChildren<Block_Prefab>(), block => {
            int val = (int)(block.Hp * BOSS_HEAL_RATIO);
            var hp = (val == 0)? 1 : block.Hp * BOSS_HEAL_RATIO;
            block.increaseHp((int)hp);
        });
    }

    public void setBossComponent(bool trigger){
        boxCollider.enabled = !trigger;
        anim.SetBool(DM.ANIM.IsFly.ToString(), trigger);
        if(trigger)
            SM.ins.sfxPlay(SM.SFX.BossFly.ToString());
        else
            SM.ins.sfxStop(SM.SFX.BossFly.ToString());
    }

    public new void decreaseHp(int dmg) {
        if(PsvSkill<int>.ONE_KILL_DMG == dmg){
            
        }

        int extraBossDmg = Mathf.RoundToInt(dmg * DM.ins.personalData.Upgrade.Arr[(int)DM.UPGRADE.BossDamage].getValue());
        Debug.Log($"BossBlock::decreaseHp(dmg={dmg} + extraBossDmg={extraBossDmg})::");

        base.decreaseHp(dmg + extraBossDmg);
        // anim.SetTrigger(DM.ANIM.GetHit.ToString());
    }

    public override void onDestroy(GameObject target, bool isInitialize = false) {
        Debug.Log($"BossBlock:: onDestroy():: target= {target}, Contains Boss1 = {name.Contains("Boss1")}");
        //* 初期化
        // gm.bossLimitCnt = 0; 
        //* Achivement
        AcvBosskillCollection.collectBossKill(this.name);
        //* Boss Die        
        SM.ins.sfxStop(SM.SFX.BossFly.ToString());
        SM.ins.sfxPlay(SM.SFX.BossDie.ToString());
        StartCoroutine(coPlayBossDieAnim(target));
    }
    IEnumerator coPlayBossDieAnim(GameObject target){
        gm.BossKillCnt++;
        gm.bossLimitCntTxt.gameObject.SetActive(false);

        this.transform.rotation = Quaternion.Euler(0,250,0);
        boxCollider.enabled = false;
        int resultExp = gm.stage * 10;

        float playSec = Util._.getAnimPlayTime(DM.ANIM.Die.ToString(), this.anim);

        anim.SetTrigger(DM.ANIM.Die.ToString());

        yield return new WaitForSecondsRealtime(playSec * 0.7f);
        for(int i=0; i < BOSS_DIE_ORB_CNT; i++)
            bm.createDropItemExpOrbPf(bossDieOrbSpawnTf, resultExp, popPower: 600);

        yield return new WaitForSecondsRealtime(playSec * 0.3f + playSec); //* 消すのが早すぎ感じで、少し待機。
        Destroy(target);
    }

    //* Skill #1
    IEnumerator coObstacleSpawnSFX(int cnt){
        yield return Util.delay0_75; //new WaitForSeconds(0.75f);
        SM.ins.sfxPlay(SM.SFX.ObstacleSpawn.ToString());
    }
    private void createObstacleSingleType(int maxCnt){
        //* OBSTACLE LIST 準備
        var obstaclePosList = getObstaclePosList();
        Debug.Log($"BossBlock::createObstacleSingleType:: maxCnt= {maxCnt}, obstaclePosList= {obstaclePosList.Count}");
        if(maxCnt > obstaclePosList.Count)
            maxCnt = obstaclePosList.Count;
        StartCoroutine(coObstacleSpawnSFX(obstaclePosList.Count));
        singleRandom(Random.Range(3, maxCnt));
    }
    private void createObstaclePatternType(int minIndex, int maxIndex){
        Debug.Log($"BossBlock::createObstacleStoneSkillPatternType()");
        //* OBSTACLE LIST 準備
        var obstaclePosList = getObstaclePosList();
        StartCoroutine(coObstacleSpawnSFX(obstaclePosList.Count));
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
        if(gm == null) return; //* (BUG-24) BossBlock::eraseObstacle()がゲームオーバーになっても反応して、NULLになるBUG対応。
        if(gm.obstacleGroup.childCount <= 0) return;

        for(int i=0; i<gm.obstacleGroup.childCount; i++){
            // Debug.Log($"eraseObstacle():: obstacleGroup.GetChild({i})= {gm.obstacleGroup.GetChild(i)}");
            var childTf = gm.obstacleGroup.GetChild(i);
            if(gm.em != null)
                gm.em.createRockObstacleBrokenEF(childTf.position);
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
