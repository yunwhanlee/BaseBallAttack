using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BossBlock : Block_Prefab{

    const float STONE_SCALE_X = 2.8f;
    public int obstacleCnt = 2;
    public int bossDieOrbCnt = 80;

    [Header("【BOSS STATUS】")]
    [SerializeField] GameObject obstacleStonePf;

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
        const int DIE = 2;
        AnimationClip[] clips = this.anim.runtimeAnimatorController.animationClips;
        float playSec = clips[DIE].length;

        int resultExp = gm.stage * 10;
        var spotTf = GameObject.Find(DM.NAME.BossDieDropOrbSpot.ToString()).transform;
        anim.SetTrigger(DM.ANIM.DoDie.ToString());

        yield return new WaitForSecondsRealtime(playSec * 0.7f);
        for(int i=0; i < bossDieOrbCnt; i++)
            bm.createDropItemExpOrbPf(spotTf, resultExp, popPower: 3500);

        yield return new WaitForSecondsRealtime(playSec * 0.3f);

        yield return new WaitForSecondsRealtime(playSec); //* 消すのが早すぎ感じで、少し待機。
        Destroy(target);
    }

    public void activeBossSkill(){
        createObstacleStone(obstacleCnt);


    }

    //* Skill #1
    private void createObstacleStone(int cnt){
        Debug.Log("<color=red>activeBossKill():: createObstacleStone()</color>");
        const float UNIT_X = BlockMaker.SCALE_X;
        const float GAP_X = BlockMaker.CENTER_GAP;
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
            Instantiate(obstacleStonePf, obstaclePosList[randIdx], Quaternion.identity, gm.obstacleGroup);
            obstaclePosList.RemoveAt(randIdx);
        }

        //* DEBUG用。全体の位置確認。
        // obstaclePosList.ForEach(pos=> Instantiate(obstacleStonePf, pos, Quaternion.identity, gm.obstacleGroup));
    }
}
