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
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em;
        pl = gm.pl;
        bm = gm.bm;
        bs = gm.bs;

        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(this.transform.forward * speed, ForceMode.Impulse);

        //* B. Dmg
        new AtvSkill(gm, pl);
        //Debug.Log("HitRange:: startPosZ=" + gm.hitRangeStartTf.position.z +  ", endPosZ="+ gm.hitRangeEndTf.position.z);
    }
    void Update(){
            // Debug.Log("BALL:: Vector3.Normalize(rigid.velocity) =>"+ Vector3.Normalize(rigid.velocity));

            //* Destroy by Checking Velocity
            // Debug.Log("BallGroup.childCount=" + gm.ballGroup.childCount + ", Ball Velocity.magnitude="+rigid.velocity.magnitude);
            if(rigid.velocity.magnitude != 0 && rigid.velocity.magnitude < 0.9875f){
                checkDestroyBall();
            }

            //* Ball Comeing View Slider
            // float startPosZ = gm.hitRangeStartTf.position.z;
            // float endPosZ = gm.hitRangeEndTf.position.z;
            // if(!isHited && endPosZ <= this.transform.position.z && this.transform.position.z <= startPosZ){
            //     //HitRange Slider UI
            //     float offset = Mathf.Abs(startPosZ);
            //     float max = Mathf.Abs(endPosZ) - offset;
            //     float v = Mathf.Abs(this.transform.position.z) - offset;
            //     gm.hitRangeSlider.value = v / max;
            // }

            //* Ball Preview Dir Goal Img
            distance = Vector3.Distance(gm.ballPreviewDirGoal.transform.position, this.transform.position);
            gm.setBallPreviewImgAlpha(distance);
    }

    //** Control
    private void OnTriggerStay(Collider col) {
#region Hit Ball
        if(col.gameObject.tag == DM.TAG.HitRangeArea.ToString()){
            pl.setSwingArcColor("red");
            if(pl.DoSwing && gm.State == GameManager.STATE.PLAY){
                gm.switchCamScene();
                // isHited = true;
                pl.DoSwing = false;
                rigid.useGravity = true;

                //* STRIKEデータ 初期化
                gm.strikeCnt = 0;
                foreach(var img in gm.strikeCntImgs) img.gameObject.SetActive(false); 

                //* offset Axis
                const int leftSide = -1, rightSide = 1;
                int sign = pl.transform.localScale.x < 0 ? leftSide : rightSide;
                int offsetAxis = (sign < 0) ? (pl.MAX_HIT_DEG/2) * leftSide : (pl.MAX_HIT_DEG/2) * rightSide;
                // Debug.Log("Ball_Prefab:: ■offsetAxis=" + offsetAxis);

                float arrowDeg = pl.arrowAxisAnchor.transform.eulerAngles.y;
                Debug.Log("🌟HIT ARROW DIRECTION (DEG)🌟:" + arrowDeg);
                Vector3 arrowDir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * arrowDeg), 0, Mathf.Cos(Mathf.Deg2Rad * arrowDeg)).normalized;

                //* Set Power(distance range 1.5f ~ 0)
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
                        StartCoroutine(coPlayActiveSkillShotEF(skillBtn, 1f, arrowDir));
                        isActiveSkillTrigger = true;
                    }
                });

                //* Bat Hit Ball SparkEF
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
                #region PSV Hit Type
                //* Multi Shot
                for(int i=0; i<pl.multiShot.Value;i++){
                    Debug.Log($"PSV MULTI SHOT: {pl.multiShot.Value}");

                    //* Arrow Direction + Extra Deg
                    Vector3 direction = pl.multiShot.calcMultiShotDeg2Dir(arrowDeg, i);

                    var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup) as GameObject;
                    ins.GetComponent<Rigidbody>().AddForce(direction * force * 0.75f, ForceMode.Impulse);
                    var scale = ins.GetComponent<Transform>().localScale;
                    ins.GetComponent<Transform>().localScale = new Vector3(scale.x * 0.75f, scale.y * 0.75f, scale.z * 0.75f);
                }

                //* Vertical Multi Shot
                for(int i=0; i<pl.verticalMultiShot.Value;i++){
                    // Debug.Log("<color=white>Ball_Prefab.cs:: Vertical Multi Shot= " + pl.verticalMultiShot.Value);
                    var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup) as GameObject;
                    ins.GetComponent<Rigidbody>().AddForce(pl.arrowAxisAnchor.transform.forward * force * 0.85f, ForceMode.Impulse);
                    var scale = ins.GetComponent<Transform>().localScale;
                    ins.GetComponent<Transform>().localScale = new Vector3(scale.x * 0.8f, scale.y * 0.8f, scale.z * 0.8f);
                }

                //* Laser
                if(pl.laser.Level > 0){
                    var start = pl.arrowAxisAnchor.transform.position;
                    //* Level１以上 Multi Laser
                    for(int i=0; i < pl.laser.Value; i++){
                        //* Arrow Direction + Extra Deg
                        Vector3 direction = pl.laser.calcMultiShotDeg2Dir(arrowDeg, i);

                        em.createLaserEF(start, direction);
                        RaycastHit[] hits = Physics.RaycastAll(start, direction, 100);
                        Array.ForEach(hits, hit => {
                            if(hit.transform.tag == BlockMaker.NORMAL_BLOCK){
                                Debug.Log("LAZER!! Hit Obj-> " + hit.transform.name);
                                var block = hit.transform.gameObject.GetComponent<Block_Prefab>();
                                int dmg = pl.dmg.Value;
                                block.decreaseHp(dmg);
                                em.createCritTxtEF(hit.transform.position, dmg);
                            }
                        });
                    }

                }
                #endregion
            }
        }
        else if(col.gameObject.tag == "ActiveDownWall"){
            pl.DoSwing = false;
            if(gm.State == GameManager.STATE.WAIT){
                gm.downWall.isTrigger = false;//*下壁 物理O
            }
        }
#endregion
    }

    private void OnTriggerExit(Collider col) {
        //* SWING Ball
        if(col.gameObject.tag == DM.TAG.HitRangeArea.ToString()){ //* HIT BALL
            Debug.Log("OnTriggerExit:: BAT SWING BALL");
            pl.setSwingArcColor("yellow");
            //* 日程時間が過ぎたら、ボール削除。
            isHitedByBlock = true;
            InvokeRepeating("checkLimitTimeToDeleteBall", 0, deleteLimitTime);
        }
        else if(col.gameObject.tag == DM.TAG.StrikeLine.ToString()){ //* ストライク
            onDestroyMe(true);
        }
    }
    private void checkLimitTimeToDeleteBall(){
        if(isHitedByBlock)
            isHitedByBlock = false;
        else{
            checkDestroyBall();
        }
    }

    //* ---------------------------------------------------------------------------------
    //* Hit Block (Explosion EF)
    //* ---------------------------------------------------------------------------------
    private void OnCollisionEnter(Collision col) {//* Give Damage
        if(col.gameObject.tag == BlockMaker.NORMAL_BLOCK){
#region #2. Active Skill 「HIT BALL」
            isHitedByBlock = true;
            gm.activeSkillBtnList.ForEach(skillBtn => {
                if(skillBtn.Trigger){
                    float delayTime = 2;
                    int skillIdx = gm.getCurSkillIdx();
                    gm.cam1.GetComponent<CamResolution>().setAnimTrigger(DM.ANIM.DoShake.ToString());
                    var atv = DM.ins.convertAtvSkillStr2Enum(skillBtn.Name);
                    switch(atv){
                        case DM.ATV.Thunder:
                            //なし
                            break;
                        case DM.ATV.FireBall:{
                            em.createAtvSkExplosionEF(skillIdx, this.transform);
                            decreaseHpSphereCastAll(atv, AtvSkill.FIREBALL_DMG);
                            if(isHomeRun){
                                decreaseHpSphereCastAll(atv, 0, AtvSkill.FIREBALL_DOT);
                            }
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                        case DM.ATV.ColorBall:{
                            if(col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.BLOCK.TreasureChest){
                                var sameColorBlocks = AtvSkill.findSameColorBlocks(gm, col.transform.gameObject);
                                
                                //* Destroy
                                Array.ForEach(sameColorBlocks, bl => {
                                    em.createAtvSkExplosionEF(skillIdx, bl.transform);
                                    bl.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(AtvSkill.COLORBALL_DMG);
                                });
                            }

                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                        case DM.ATV.PoisonSmoke:{
                            int destroyCnt = 999;
                            var ins = em.createAtvSkExplosionEF(skillIdx, this.transform, destroyCnt);
                            if(isHomeRun){
                                ins.GetComponent<PoisonSmoke>().KeepStageSpan += 2;
                                float sc = 1.3f;
                                ins.transform.localScale = new Vector3(sc, sc, sc);
                            }
                            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.PoisonSmokeCastWidth, Vector3.up, 0);
                            decreaseHpSphereCastAll(atv, 0, AtvSkill.POISONSMOKE_DOT);
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                        case DM.ATV.IceWave:{
                            delayTime = 2f;
                            em.createAtvSkExplosionEF(skillIdx, this.transform);
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                    }
                    //* Delay Next Stage
                    Invoke("onDestroyMeInvoke", delayTime);
                }
            });
#endregion
#region Passive Skill 「HIT BALL」
            bool isOnExplosion = false;
            int result = 0;

            //* InstantKill
            pl.instantKill.setHitTypeSkill(pl.instantKill.Value, ref result, col, em, pl);

            if(result != Player.ONE_KILL){
                //* Critical
                pl.critical.setHitTypeSkill(pl.critical.Value, ref result, col, em, pl);

                //* Explosion（最後 ダメージ適用）
                isOnExplosion = pl.explosion.setHitTypeSkill(pl.explosion.Value.per, ref result, col, em, pl, this.gameObject);
            }

            
            //* Apply Damage Result
            Debug.Log("result= " + result);
            if(isOnExplosion){
                //Sphere Collider
                RaycastHit[] rayHits = Physics.SphereCastAll(this.gameObject.transform.position, pl.explosion.Value.range, Vector3.up, 0);
                foreach(var hit in rayHits){
                    if(hit.transform.tag == BlockMaker.NORMAL_BLOCK)
                        hit.transform.GetComponent<Block_Prefab>().decreaseHp(result);
                }
            }else{
                col.gameObject.GetComponent<Block_Prefab>().decreaseHp(result);
            }
#endregion            
        }
        else if(col.gameObject.tag == "Wall" && col.gameObject.name == "DownWall"){
            Vector3 pos = new Vector3(this.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z);
            em.createDownWallHitEF(pos);
        }
    }

    private void onDestroyMeInvoke() => onDestroyMe();
    
    private void decreaseHpSphereCastAll(DM.ATV atv, int dmg, float dotDmgPer = 0){
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.FireBallCastWidth, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.tag == BlockMaker.NORMAL_BLOCK){
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

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
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
            Debug.Log("✓Ball＿Prefab:: onDestroyMe:: gm.setNextStage()");
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
        gm.cam1.GetComponent<CamResolution>().setAnimTrigger(DM.ANIM.DoShake.ToString());

        gm.homeRunTxtTf.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
        em.enableUIStageTxtEF("HomeRun");
        Time.timeScale = 1;
    }

    IEnumerator coPlayActiveSkillBefSpotLightAnim(){
        Debug.Log("ActiveSkill Before Anim");
        Time.timeScale = 0;
        pl.setAnimTrigger("ActiveSkillBefSpotLight");
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
    }

    //* ---------------------------------------------------------------------------------
    //* Swing Ball (Shot EF)
    //* ---------------------------------------------------------------------------------
#region Passive Skill 「HIT BAT」
    IEnumerator coPlayActiveSkillShotEF(AtvSkillBtnUI btn, float waitTime, Vector3 dir){
        Debug.LogFormat("coPlayActiveSkillShotEF:: btn={0}, waitTite={1}, dir={2}, isHomeRun={3}", btn.Name, waitTime, dir, isHomeRun);
        float delayTime = 0;
        int skillIdx = gm.getCurSkillIdx();
        var atv = DM.ins.convertAtvSkillStr2Enum(btn.Name);
        switch(atv){
            case DM.ATV.Thunder:
                delayTime = 2;
                const int maxDistance = 50;
                const int width = 1;
                Debug.DrawRay(this.transform.position, dir * maxDistance, Color.blue, 2f);
                gm.cam1.GetComponent<CamResolution>().setAnimTrigger("doShake");

                em.createAtvSkShotEF(skillIdx, this.gameObject.transform, pl.arrowAxisAnchor.transform.rotation);
                //* Collider 
                RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, Vector3.one * width, dir, Quaternion.identity, maxDistance);
                Array.ForEach(hits, hit => {
                    if(hit.transform.tag == BlockMaker.NORMAL_BLOCK){
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


    IEnumerator coSetThunderSkill(RaycastHit hit){
        yield return new WaitForSeconds(0.1f);
        Debug.Log("<color=red>coSetThunderSkill():: isHomeRun= " + isHomeRun + "</color>");
        const int multiCnt = 5;
        List<GameObject> effectList = new List<GameObject>(); //* HPが０になったら、発生するERROR対応。

        //* HomeRun Bonus
        float critDmgRatio = (isHomeRun)? 
            AtvSkill.THUNDERSHOT_CRT + 1 : AtvSkill.THUNDERSHOT_CRT;

        //* Set Dmg & Multi CriticalTextEF
        hit.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(((int)(pl.dmg.Value * critDmgRatio) * multiCnt));
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
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(this.transform.position, pl.explosion.getValue().range);

        //* ThunderShot Skill Range Preview => ArrowAxisAnchorObjに付いているThunderGizmosスクリプト。

        //* FireBall Skill Range Preview
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(this.transform.position, pl.FireBallCastWidth);
    }
}
