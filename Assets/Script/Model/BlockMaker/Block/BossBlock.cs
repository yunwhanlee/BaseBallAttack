using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BossBlock : Block_Prefab{

    const float STONE_SCALE_X = 2.8f;
    const int OBSTACLE_STONE_CNT = 1;
    const int BOSS_DIE_ORB_CNT = 80;
    const float BOSS_HEAL_RATIO = 0.2f;

    const int STONE_PER = 100;



    [SerializeField] Transform bossDieOrbSpawnTf;

    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;
    [SerializeField] List<Vector3> obstaclePosList;

    private void OnDisable() {
        bm.eraseObstacle();
    }

    void Start() {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bossDieOrbSpawnTf = GameObject.Find(DM.NAME.BossDieDropOrbSpot.ToString()).transform;
        activeBossSkill();
    }

    public void activeBossSkill(){ //* at NextStage
        var rand = Random.Range(0, 100);
        this.anim.SetTrigger(DM.ANIM.Scream.ToString());
        if(rand < STONE_PER){
            //* Skill #1
            createObstacleStoneSkill(OBSTACLE_STONE_CNT);
        }
        else{
            //* Skill #2
            StartCoroutine(coBossHealSkill());
        }
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
        const int COL = 6;
        const int ROW = 3;
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

        //* RANDOM
        // for(int i=0; i< cnt; i++){
        //     int randIdx = Random.Range(0, obstaclePosList.Count);
        //     gm.em.createBossObstacleSpawnEF(obstaclePosList[i]);
        //     Instantiate(obstacleStonePf, obstaclePosList[i], Quaternion.identity, gm.obstacleGroup);
        //     obstaclePosList.RemoveAt(i);
        // }

        //* PATTERN 1) EVEN
        // for(int i=0; i < obstaclePosList.Count; i++){
        //     if(i % 2 == 0){
        //         gm.em.createBossObstacleSpawnEF(obstaclePosList[i]);
        //         Instantiate(obstacleStonePf, obstaclePosList[i], Quaternion.identity, gm.obstacleGroup);
        //     }
        // }

        //* PATTERN 2) ODD
        // for(int i=0; i < obstaclePosList.Count; i++){
        //     if(i % 2 == 1){
        //         gm.em.createBossObstacleSpawnEF(obstaclePosList[i]);
        //         Instantiate(obstacleStonePf, obstaclePosList[i], Quaternion.identity, gm.obstacleGroup);
        //     }
        // }

        //* PATTERN 3) CUT COLUMN LINE
        // const int COL_LINE_IDX = 1;
        // for(int i=0; i < obstaclePosList.Count; i++){
        //     if(i % COL != COL_LINE_IDX){
        //         createObstacleStone(i);
        //     }
        // }

        //* PATTERN 4) 三角形
        for(int r = 0; r < ROW; r++){
            //* 直角三角形(◥)
            // for(int c = r; c < COL; c++){
            //     int idx = c + COL * r;
            //     createObstacleStone(idx);
            // }
            //* 直角三角形(REVERSE ◤)
            // for(int c = 0; c < COL - r; c++){
            //     int idx = c + COL * r;
            //     createObstacleStone(idx);
            // }
            //* 三角形(▼)
            for(int c = r; c < COL - r; c++){
                int idx = c + COL * r;
                createObstacleStone(idx);
            }
        }
    }

    private void createObstacleStone(int i){
        gm.em.createBossObstacleSpawnEF(obstaclePosList[i]);
        Instantiate(obstacleStonePf, obstaclePosList[i], Quaternion.identity, gm.obstacleGroup);
    }
}
