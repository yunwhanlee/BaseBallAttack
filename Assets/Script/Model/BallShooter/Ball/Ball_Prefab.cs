using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    GameManager gm; EffectManager em; Player pl; BlockMaker bm; BallShooter bs;
    public int aliveTime;

    //* Value
    bool isHitedByBlock = false;
    bool isHomeRun = false;
    float deleteLimitTime = 2.0f;
    float speed;
    float distance;

    Rigidbody rigid;
    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em; pl = gm.pl; bm = gm.bm; bs = gm.bs;

        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(this.transform.forward * speed, ForceMode.Impulse);

        //* B. Dmg
        new AtvSkill(gm, pl);
    }
    void Update(){
        //* Destroy by Checking Velocity
        if(rigid.velocity.magnitude != 0 && rigid.velocity.magnitude < 0.9875f){
            // Debug.Log($"BallGroup.childCount= {gm.ballGroup.childCount}, velocity.magnitude= {rigid.velocity.magnitude}");
            checkDestroyBall();
        }

        //* Ball Preview Direction Goal Img
        distance = Vector3.Distance(gm.ballPreviewDirGoal.transform.position, this.transform.position);
        gm.setBallPreviewImgAlpha(distance);

        /* Ball Comeing View Slider
        float startPosZ = gm.hitRangeStartTf.position.z;
        float endPosZ = gm.hitRangeEndTf.position.z;
        if(!isHited && endPosZ <= this.transform.position.z && this.transform.position.z <= startPosZ){
        // HitRange Slider UI
            float offset = Mathf.Abs(startPosZ);
            float max = Mathf.Abs(endPosZ) - offset;
            float v = Mathf.Abs(this.transform.position.z) - offset;
            gm.hitRangeSlider.value = v / max;
        }
        */
    }

    void OnTriggerStay(Collider col) {
#region HIT BALL
        if(col.transform.CompareTag(DM.TAG.HitRangeArea.ToString())){
            pl.setSwingArcColor("red");
            if(gm.State == GameManager.STATE.PLAY && pl.DoSwing){
                gm.switchCamScene();
                pl.DoSwing = false;
                rigid.useGravity = true;

                //* Init STRIKEデータ
                gm.strikeCnt = 0;
                foreach(var img in gm.strikeCntImgs) img.gameObject.SetActive(false); 

                //* Set Offset Axis
                const int left = -1, right = 1;
                int sign = pl.transform.localScale.x < 0 ? left : right;
                int swingDeg = pl.MAX_SWING_HIT_DEG / 2;
                int offsetAxis = sign * swingDeg;
                // Debug.Log("Ball_Prefab:: ■offsetAxis=" + offsetAxis);

                //* Set Arrow Direction
                float arrowDeg = pl.arrowAxisAnchor.transform.eulerAngles.y;
                Vector3 arrowDir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * arrowDeg), 0, Mathf.Cos(Mathf.Deg2Rad * arrowDeg)).normalized;
                // Debug.Log($"Arrow Deg({arrowDeg}) -Atan2-> Dir({arrowDir})");

                //* Set Hit Power (distance range 1.5f ~ 0)
                const int A=0, B=1, C=2, D=3, E=4, F=5;
                float power = (distance <= pl.hitRank[A].Dist) ? pl.hitRank[A].Power //-> BEST HIT (HOMERUH!)
                : (distance <= pl.hitRank[B].Dist) ? pl.hitRank[B].Power
                : (distance <= pl.hitRank[C].Dist) ? pl.hitRank[C].Power
                : (distance <= pl.hitRank[D].Dist)? pl.hitRank[D].Power
                : (distance <= pl.hitRank[E].Dist)? pl.hitRank[E].Power
                : pl.hitRank[F].Power; //-> WORST HIT (distance <= 1.5f)
                
                //* #1. Active SHOT Skill
                bool isActiveSkillTrigger = false;
                gm.activeSkillBtnList.ForEach(skillBtn=>{
                    if(skillBtn.Trigger){
                        isActiveSkillTrigger = true;
                        StartCoroutine(coPlayActiveSkillShotEF(skillBtn, 1f, arrowDir));
                    }
                });

                //* Effect
                em.createBatHitSparkEF(this.transform.position);

                //* HomeRun
                if(power >= pl.hitRank[C].Power){
                    em.createHomeRunHitSparkEF(this.transform.position);
                    StartCoroutine(coPlayHomeRunAnim(isActiveSkillTrigger));
                }
                else if(isActiveSkillTrigger){ //* ActiveSkill Before
                    // StartCoroutine(coPlayActiveSkillBefSpotLightAnim());
                }

                rigid.velocity = Vector3.zero;
                float force = speed * power * pl.speed.Value;
                rigid.AddForce(arrowDir * force, ForceMode.Impulse);
                Debug.Log(
                    "HIT Ball! <color=yellow>distance=" + distance.ToString("N2") + "</color>"
                    + ", <color=red>power=" + power + ", Rank: " + ((power==pl.hitRank[A].Power)? "A" : (power==pl.hitRank[B].Power)? "B" : (power==pl.hitRank[C].Power)? "C" : (power==pl.hitRank[D].Power)? "D" : (power==pl.hitRank[E].Power)? "E" : "F").ToString() + "</color>"
                    + ", Force=" + force);

                //* Active Skillなら、下記は実行しない----------------------------------------
                if(isActiveSkillTrigger) return;

    #region PSV (HIT BALL)
                //* 【 Multi Shot (横) 】
                for(int i=0; i<pl.multiShot.Value;i++){ // Debug.Log($"<color=blue>【 Multi Shot (横) 】: {pl.multiShot.Value}</color>");
                    Vector3 dir = pl.multiShot.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                    instantiateMultiShot(dir, force, ratio: 0.75f);
                }

                //* 【 Vertical Multi Shot (縦) 】
                for(int i=0; i<pl.verticalMultiShot.Value;i++){ // Debug.Log($"<color=blue>【 Vertical Multi Shot (縦) 】: {pl.verticalMultiShot.Value}</color>");
                    Vector3 dir = pl.arrowAxisAnchor.transform.forward;
                    instantiateMultiShot(dir, force, ratio: 0.8f);
                }

                //* 【 Laser 】
                for(int i=0; i < pl.laser.Value; i++){  // Debug.Log($"<color=blue>【 Laser 】: {pl.laser.Value}</color>");
                    Vector3 start = pl.arrowAxisAnchor.transform.position;
                    Vector3 dir = pl.laser.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                    em.createLaserEF(start, dir);

                    RaycastHit[] hits = Physics.RaycastAll(start, dir, 100);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.name.Contains(DM.NAME.Block.ToString())){  // Debug.Log("LAZER Hit Obj -> " + hit.transform.name);
                            int laserDmg = pl.dmg.Value * (pl.laser.Level + 1);
                            em.createCritTxtEF(hit.transform.position, laserDmg);
                            bm.setDecreaseHP(hit.transform.gameObject, laserDmg);
                        }
                    });
                }
    #endregion
            }
        }
        else if(col.transform.CompareTag(DM.TAG.ActiveDownWall.ToString())){
            pl.DoSwing = false;
            if(gm.State == GameManager.STATE.WAIT){
                gm.downWallCollider.isTrigger = false;//*下壁 物理O
            }
        }
#endregion
    }

    void OnTriggerExit(Collider col) {
#region SWING BAT
        if(col.transform.CompareTag(DM.TAG.HitRangeArea.ToString())){ //* HIT BALL
            isHitedByBlock = true;
            pl.setSwingArcColor("yellow");
            InvokeRepeating("checkLimitTimeToDeleteBall", 0, deleteLimitTime);//* 日程時間が過ぎたら、ボール削除。
        }
        else if(col.transform.CompareTag(DM.TAG.StrikeLine.ToString())){ //* ストライク
            onDestroyMe(true);
        }
    }
#endregion
    void OnCollisionEnter(Collision col) { //* Give Damage
        if(col.transform.name.Contains(DM.NAME.Block.ToString())){
            isHitedByBlock = true;
#region #2. ATV (BALL)
            gm.activeSkillBtnList.ForEach(skillBtn => {
                if(skillBtn.Trigger){
                    const float delayTime = 2;
                    int skillIdx = gm.getCurSkillIdx();
                    var atv = DM.ins.convertAtvSkillStr2Enum(skillBtn.Name);

                    switch(atv){
                        case DM.ATV.Thunder: //なし
                            break;
                        case DM.ATV.FireBall:{
                            em.createAtvSkExplosionEF(skillIdx, this.transform);
                            decreaseHpSphereCastAll(atv, AtvSkill.FIREBALL_DMG);
                            if(isHomeRun) decreaseHpSphereCastAll(atv, 0, AtvSkill.FIREBALL_DOT); //* + DOT DAMAGE
                            break;
                        }
                        case DM.ATV.ColorBall:{
                            if(col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.KIND.TreasureChest){
                                var sameColorBlocks = AtvSkill.findSameColorBlocks(gm, col.transform.gameObject);
                                //* Destroy
                                Array.ForEach(sameColorBlocks, bl => {
                                    em.createAtvSkExplosionEF(skillIdx, bl.transform);
                                    bl.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(AtvSkill.COLORBALL_DMG);
                                });
                            }
                            break;
                        }
                        case DM.ATV.PoisonSmoke:{
                            int destroyCnt = 999;
                            var ins = em.createAtvSkExplosionEF(skillIdx, this.transform, destroyCnt);
                            if(isHomeRun){
                                float sc = 1.3f;
                                ins.GetComponent<PoisonSmoke>().KeepStageSpan += 2;
                                ins.transform.localScale = new Vector3(sc, sc, sc);
                            }
                            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.PoisonSmokeCastWidth, Vector3.up, 0);
                            decreaseHpSphereCastAll(atv, 0, AtvSkill.POISONSMOKE_DOT);
                            break;
                        }
                        case DM.ATV.IceWave:{
                            em.createAtvSkExplosionEF(skillIdx, this.transform);
                            break;
                        }
                    }
                    skillBtn.init(gm);
                    gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
                    this.gameObject.GetComponent<SphereCollider>().enabled = false; //* ボール動きなし

                    //* Delay Next Stage
                    Invoke("onDestroyMeInvoke", delayTime);
                }
            });
#endregion
#region BALL DAMAGE
            int result = 0;
            bool isOnExplosion = false;
    #region CHECK PSV
            //* InstantKill
            pl.instantKill.setHitTypeSkill(pl.instantKill.Value, ref result, col, em, pl);
            if(result != Player.ONE_KILL_DMG){
                //* Critical
                pl.critical.setHitTypeSkill(pl.critical.Value, ref result, col, em, pl);
                //* Explosion（最後 ダメージ適用）
                isOnExplosion = pl.explosion.setHitTypeSkill(pl.explosion.Value.per, ref result, col, em, pl, this.gameObject);
            }
    #endregion
    #region SET DAMAGE
            if(isOnExplosion){//* Explosion (爆発)
                RaycastHit[] hits = Physics.SphereCastAll(this.gameObject.transform.position, pl.explosion.Value.range, Vector3.up, 0);
                Array.ForEach(hits, hit => {
                    if(hit.transform.name.Contains(DM.NAME.Block.ToString())){ //! (BUG) 爆発ができないこと対応。(このIF文が無かったら、UnHitAreaのみ受け取りできない)
                        Debug.Log("Set DAMAGE:: Explostion:: result= " + result + ", hit.obj.name=" + hit.transform.gameObject.name);
                        bm.setDecreaseHP(hit.transform.gameObject, result);
                    }
                });
            }
            else{//* Normal Damage Result
                Debug.Log("Set DAMAGE:: result= " + result);
                bm.setDecreaseHP(col.gameObject, result);
            }
    #endregion
#endregion            
        }
        else if(col.transform.CompareTag(DM.TAG.Wall.ToString()) && col.gameObject.name == DM.NAME.DownWall.ToString()){
            Vector3 pos = new Vector3(this.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z);
            em.createDownWallHitEF(pos);
        }
    }

#region ATV (BAT)
    IEnumerator coPlayActiveSkillShotEF(AtvSkillBtnUI btn, float waitTime, Vector3 dir){
        // Debug.LogFormat("coPlayActiveSkillShotEF:: btn={0}, waitTite={1}, dir={2}, isHomeRun={3}", btn.Name, waitTime, dir, isHomeRun);
        int skillIdx = gm.getCurSkillIdx();
        var atv = DM.ins.convertAtvSkillStr2Enum(btn.Name);
        switch(atv){
            case DM.ATV.Thunder:
                const float delayTime = 2;
                const int maxDistance = 50;
                const int width = 1;
                Debug.DrawRay(this.transform.position, dir * maxDistance, Color.blue, 2f);

                gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
                em.createAtvSkShotEF(skillIdx, this.gameObject.transform, pl.arrowAxisAnchor.transform.rotation);

                //* Collider 
                RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, Vector3.one * width, dir, Quaternion.identity, maxDistance);
                Array.ForEach(hits, hit => {
                    if(hit.transform.name.Contains(DM.NAME.Block.ToString())){
                        StartCoroutine(coSetThunderSkill(hit)); //* With HomeRun Bonus
                    }
                });
                this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし

                gm.activeSkillBtnList.ForEach(btn => btn.init(gm));
                yield return new WaitForSeconds(delayTime);
                onDestroyMe();
                break;
            case DM.ATV.FireBall:
            case DM.ATV.ColorBall:
            case DM.ATV.PoisonSmoke:
            case DM.ATV.IceWave:
                em.createAtvSkShotEF(skillIdx, this.gameObject.transform, Quaternion.identity, true); //Trail
                break;
        }
        //Before go up NextStage Wait for Second
    }
#endregion

    private void onDestroyMeInvoke() => onDestroyMe();

    //*------------------------------------------------------------------------------
    //*  関数
    //*------------------------------------------------------------------------------
    void instantiateMultiShot(Vector3 dir, float force, float ratio){
        var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup);
        ins.GetComponent<Rigidbody>().AddForce(dir * force * ratio, ForceMode.Impulse);
        Vector3 scale = ins.transform.localScale;
        ins.transform.localScale = new Vector3(scale.x * ratio, scale.y * ratio, scale.z * ratio);
    }

    private void decreaseHpSphereCastAll(DM.ATV atv, int dmg, float dotDmgPer = 0){
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.FireBallCastWidth, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.name.Contains(DM.NAME.Block.ToString())){
                var block = hit.transform.gameObject.GetComponent<Block_Prefab>();
                //* Is DotDmg Type?
                if(!(dotDmgPer == 0) && !block.IsDotDmg){
                    block.IsDotDmg = true;
                    switch(atv){
                        case DM.ATV.FireBall:
                            em.directlyCreateFireBallDotEF(block.transform);
                            break;
                    }
                }
                //* Check Dot Dmg or General Dmg
                int val = (dotDmgPer == 0)? dmg : block.getDotDmg(dotDmgPer);
                block.decreaseHp(val);
                em.createCritTxtEF(hit.transform.position, val);
            }
        });
    }
    private void checkLimitTimeToDeleteBall(){
        if(isHitedByBlock)
            isHitedByBlock = false;
        else{
            checkDestroyBall();
        }
    }
    private void checkDestroyBall(){
        if(this.name == "Ball(Clone)" && this.transform.localScale.x == 0.4f){
            onDestroyMe();
            // for(int i=0;i<gm.ballGroup.childCount;i++)
            //     Destroy(gm.ballGroup.GetChild(i));
        }
        else
            Destroy(this.gameObject);
    }

    private void onDestroyMe(bool isStrike = false){
        if(!isStrike){
            Debug.Log("✓Ball_Prefab:: onDestroyMe:: gm.setNextStage()");
            gm.setNextStage();//* ボールが消えたら次に進む。
        }else{
            gm.setStrike();
            gm.setBallPreviewGoalRandomPos();
        }
        Destroy(this.gameObject);
    }

    public void setBallSpeed(float v){
        speed = v;
    }

    IEnumerator coPlayHomeRunAnim(bool isActiveSkillTrigger){
        // if(isActiveSkillTrigger){
        //     yield return coPlayActiveSkillBefSpotLightAnim();
        // }
        isHomeRun = true;
        Debug.Log("HOMERUH!!!!" + "isHomeRun= " + isHomeRun);
        Time.timeScale = 0;
        pl.setAnimTrigger("HomeRun");
        yield return new WaitForSecondsRealtime(2);
        gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());

        gm.homeRunTxtTf.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
        em.enableUITxtEF("HomeRun");
        Time.timeScale = 1;
    }

    IEnumerator coPlayActiveSkillBefSpotLightAnim(){
        Debug.Log("ActiveSkill Before Anim");
        Time.timeScale = 0;
        pl.setAnimTrigger("ActiveSkillBefSpotLight");
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
    }

    IEnumerator coSetThunderSkill(RaycastHit hit){
        yield return new WaitForSeconds(0.1f);
        Debug.Log("<color=red>coSetThunderSkill():: isHomeRun= " + isHomeRun + "</color>");
        const int multiCnt = 5;
        List<GameObject> effectList = new List<GameObject>(); //* HPが０になったら、発生するERROR対応。

        //* HomeRun Bonus
        float critDmgRatio = (isHomeRun)? 
            AtvSkill.THUNDERSHOT_CRT + 1 : AtvSkill.THUNDERSHOT_CRT;

        //* Set Dmg & Multi CriticalTextEF
        bm.setDecreaseHP(hit.transform.gameObject, ((int)(pl.dmg.Value * critDmgRatio) * multiCnt));
        StartCoroutine(coMultiCriticalDmgEF(critDmgRatio, multiCnt, hit.transform.position));
    }
    IEnumerator coMultiCriticalDmgEF(float critDmgRatio, int cnt, Vector3 hitPos){
        float span = 0.0875f;
        for(int i=0; i<cnt; i++){
            em.createCritTxtEF(hitPos, (int)(pl.dmg.Value * critDmgRatio));
            yield return new WaitForSeconds(i * span);
        }
    }

    void OnDrawGizmos(){
        //* Explosion Skill Range Preview
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, pl.explosion.Value.range);

        //* ThunderShot Skill Range Preview => ArrowAxisAnchorObjに付いているThunderGizmosスクリプト。

        //* FireBall Skill Range Preview
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(this.transform.position, pl.FireBallCastWidth);
    }
}