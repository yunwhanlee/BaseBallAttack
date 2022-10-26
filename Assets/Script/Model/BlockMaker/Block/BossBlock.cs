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

    private void OnDisable() {
        bm.eraseObstacle();
    }

    void Start() {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        bossDieOrbSpawnTf = GameObject.Find(DM.NAME.BossDieDropOrbSpot.ToString()).transform;
    }

    public void activeBossSkill(){ //* at NextStage
        var rand = Random.Range(0, 100);
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
        this.anim.SetTrigger(DM.ANIM.DoHeal.ToString());
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
        Debug.Log("<color=red>activeBossKill():: createObstacleStone()</color>");
        const float UNIT_X = BlockMaker.SCALE_X;
        const float GAP_X = BlockMaker.BOTH_SIDE_SPACE;
        const float OFFSET_X = -5;
        const float OFFSET_Y = -8;

        //* OBSTACLE LIST 準備
        List<Vector3> obstaclePosList = new List<Vector3>(){};
        for(int i=0; i<18;i++){
            const int MASS_ROW_CNT = 6;
            int rowIdx = i % MASS_ROW_CNT;
            int lastIdx = MASS_ROW_CNT/2 - 1;
            float gap = (i % MASS_ROW_CNT) > lastIdx ? GAP_X : 0;

            float x = gap + OFFSET_X + (UNIT_X * rowIdx);
            int z = (int)(OFFSET_Y - (i / MASS_ROW_CNT));
            obstaclePosList.Add(new Vector3(x, 0, z));
        }

        //* OBSTACLE INS 生成
        for(int i=0; i< cnt; i++){
            int randIdx = Random.Range(0, obstaclePosList.Count);
            gm.em.createBossObstacleSpawnEF(obstaclePosList[randIdx]);
            Instantiate(obstacleStonePf, obstaclePosList[randIdx], Quaternion.identity, gm.obstacleGroup);
            obstaclePosList.RemoveAt(randIdx);
        }

        //* DEBUG用。全体の位置確認。
        // obstaclePosList.ForEach(pos=> Instantiate(obstacleStonePf, pos, Quaternion.identity, gm.obstacleGroup));
    }
}
