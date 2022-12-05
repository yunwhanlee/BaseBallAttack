using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]    float speed;    public float Speed {get => speed; set => speed = value;}
    float distance;
    public Rigidbody rigid;

    [Header("PSV UNIQUE")][Header("__________________________")]
    [SerializeField]    bool isOnDarkOrb; public bool IsOnDarkOrb {get => isOnDarkOrb; set => isOnDarkOrb = value;}
    [SerializeField]    GameObject darkOrbPf; public GameObject DarkOrbPf {get => darkOrbPf; set => darkOrbPf = value;}

    void Awake() {
        rigid = GetComponent<Rigidbody>();
    }
    void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = gm.em; pl = gm.pl; bm = gm.bm; bs = gm.bs;

        // rigid.AddForce(this.transform.forward * Speed, ForceMode.Impulse);

        //* B. Dmg
        new AtvSkill(gm, pl);

        IsOnDarkOrb = false;
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
        #region SWING BALL
        if(col.transform.CompareTag(DM.TAG.HitRangeArea.ToString())){
            pl.setSwingArcColor("red");
            if(gm.State == GameManager.STATE.PLAY && pl.DoSwing){
                gm.switchCamera();
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
                // const int S=0, A=1, B=2, C=3, D=4, E=5;
                var hitRank = LM._.HIT_RANK;
                float power = gm.setHitPower(distance, hitRank);
                
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
                if(power >= LM._.HOMERUN_MIN_POWER){
                    em.createHomeRunHitSparkEF(this.transform.position);
                    StartCoroutine(coPlayHomeRunAnim(isActiveSkillTrigger));
                }
                // else if(isActiveSkillTrigger){ //* ActiveSkill Before
                    // StartCoroutine(coPlayActiveSkillBefSpotLightAnim());
                // }
                var force = setgetHitBallSpeed(power, arrowDir);

                //* Show HitBall Infomation
                string rankTxt = gm.setHitRankTxt(power, hitRank);
                float per = Util._.getCalcCurValPercentage(distance, LM._.MAX_DISTANCE);

                gm.showHitBallInfoTf.GetComponent<Animator>().SetTrigger(DM.ANIM.IsHitBall.ToString());
                var iconTxtList = gm.showHitBallInfoTf.transform.GetComponentsInChildren<Text>();
                const int POWER_RANK = 0, BALL_SPEED = 1, ACCURACY = 2;

                iconTxtList[POWER_RANK].text = rankTxt;
                iconTxtList[BALL_SPEED].text = " : " + power.ToString();
                iconTxtList[ACCURACY].text = " : " + ((100 - per) + "%").ToString();

                //* RankTxt Color Style
                iconTxtList[POWER_RANK].color =
                    (rankTxt == DM.HITRANK.S.ToString() || rankTxt == DM.HITRANK.A.ToString())? Color.red : Color.white;

                //* Active Skillなら、下記は実行しない----------------------------------------
                if(isActiveSkillTrigger) return;

                #region PSV (SWING BALL)
                //*【 Bird Friend 】
                if(pl.birdFriend.Level == 1){
                    var ins = Instantiate(gm.eggPf, pl.BirdFriendObj.transform.position, Quaternion.identity, gm.ballGroup);
                    Vector3 throwDir = new Vector3(arrowDir.x, 1, arrowDir.z);
                    ins.GetComponent<Rigidbody>().AddForce((throwDir * 90), ForceMode.Impulse);
                }

                //* 【 Dark Orb 】
                if(pl.darkOrb.Level == 1){
                    Debug.Log("DARKORB ON!");
                    IsOnDarkOrb = true;
                    DarkOrbPf.SetActive(IsOnDarkOrb);
                }

                //* 【 Giant Ball 】
                if(pl.giantBall.Level == 1){
                    var ballTexture = this.transform.GetChild(0).GetChild(0);
                    const float sc = PsvSkill<float>.GIANTBALL_SCALE;
                    ballTexture.transform.localScale = new Vector3(sc, sc, sc);
                }
                else{
                    //* 【 Multi Shot (横) 】
                    for(int i=0; i<pl.multiShot.Val;i++){ // Debug.Log($"<color=blue>【 Multi Shot (横) 】: {pl.multiShot.Value}</color>");
                        Vector3 dir = pl.multiShot.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                        instantiateMultiShot(dir, force, ratio: 0.75f);
                    }

                    //* 【 Vertical Multi Shot (縦) 】
                    for(int i=0; i<pl.verticalMultiShot.Val;i++){ // Debug.Log($"<color=blue>【 Vertical Multi Shot (縦) 】: {pl.verticalMultiShot.Value}</color>");
                        Vector3 dir = pl.arrowAxisAnchor.transform.forward;
                        instantiateMultiShot(dir, force, ratio: 0.8f);
                    }
                }
                //* 【 Laser 】
                for(int i=0; i < pl.laser.Val; i++){  // Debug.Log($"<color=blue>【 Laser 】: {pl.laser.Value}</color>");
                    Vector3 start = pl.arrowAxisAnchor.transform.position;
                    Vector3 dir = pl.laser.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                    em.createLaserEF(start, dir);

                    RaycastHit[] hits = Physics.RaycastAll(start, dir, 100);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.name.Contains(DM.NAME.Block.ToString())){  // Debug.Log("LAZER Hit Obj -> " + hit.transform.name);
                            int laserDmg = pl.dmg.Val * (pl.laser.Level + 1);
                            em.createCritTxtEF(hit.transform.position, laserDmg);
                            bm.decreaseBlockHP(hit.transform.gameObject, laserDmg);
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

//----------------------------------------------------------------
//*
//----------------------------------------------------------------
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
//----------------------------------------------------------------
//*
//----------------------------------------------------------------
    void OnCollisionEnter(Collision col) { 
        #region ATV (HIT BLOCK)
        if(col.transform.name.Contains(DM.NAME.Block.ToString())
        || col.transform.name.Contains(DM.NAME.Obstacle.ToString())){ //* (BUG) 障害物もFreezeからだめーず受けるように。
            isHitedByBlock = true;
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
                            decreaseBlocksHp(atv, AtvSkill.FIREBALL_DMG);
                            if(isHomeRun)
                                decreaseBlocksHp(atv, 0, AtvSkill.FIREBALL_DOT); //* + DOT DAMAGE
                            // this.rigid.velocity = Vector3.zero; //! ボール速度が０になると、待機されず次のステージに進むBUG。
                            break;
                        }
                        case DM.ATV.ColorBall:{
                            if(col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.KIND.TreasureChest){
                                Block_Prefab[] sameColorBlocks = AtvSkill.findSameColorBlocks(gm, col.transform.gameObject);
                                //* Set Max Cnt
                                int max = AtvSkill.COLORBALL_POP_CNT;
                                if(isHomeRun) 
                                    max = sameColorBlocks.Length;

                                //* Erase Same Color Blocks
                                for(int i=0; i<max; i++){
                                    em.createAtvSkExplosionEF(skillIdx, sameColorBlocks[i].transform);
                                    if(isHomeRun)
                                        em.createColorBallStarExplosionEF(sameColorBlocks[i].transform.position);
                                    sameColorBlocks[i].transform.GetComponent<Block_Prefab>().decreaseHp(PsvSkill<int>.ONE_KILL_DMG);
                                }
                            }
                            // this.rigid.velocity = Vector3.zero;
                            break;
                        }
                        case DM.ATV.PoisonSmoke:{
                            int destroyCnt = 999;
                            var ins = em.createAtvSkExplosionEF(skillIdx, this.transform, destroyCnt);
                            if(isHomeRun){
                                float sc = 1.3f;
                                ins.GetComponent<PoisonSmoke>().KeepDuration += 2;
                                ins.transform.localScale = new Vector3(sc, sc, sc);
                            }
                            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.PoisonSmokeCastWidth, Vector3.up, 0);
                            decreaseBlocksHp(atv, 0, AtvSkill.POISONSMOKE_DOT);
                            // this.rigid.velocity = Vector3.zero;
                            break;
                        }
                        case DM.ATV.IceWave:{
                            var effect = em.createAtvSkExplosionEF(skillIdx, this.transform);
                            // this.rigid.velocity = Vector3.zero;
                            if(isHomeRun) {
                                effect.transform.GetChild(effect.transform.childCount-1).gameObject.SetActive(true);
                            }
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
        #region PSV (HIT BLOCK) + DAMAGE RESULT
            int result = 0;
            bool isOnExplosion = false;

            //* GodBless
            if(pl.godBless.Level == 1){
                if(gm.comboCnt != 0 && gm.comboCnt % LM._.GODBLESS_SPAN == 0){
                    Debug.Log("GOD BLESS YOU!");
                    float radius = 4;
                    Util._.DebugSphere(this.transform.position, radius, 1, "blue");

                    //* Explosion
                    em.createGodBlessEF(this.transform.position);
                    Util._.sphereCastAllDecreaseBlocksHp(this.transform, radius, pl.dmg.Val * 3);
            }
            }

            //* InstantKill
            pl.instantKill.setHitTypeSkill(pl.instantKill.Val, ref result, col, em, pl);

            if(result != PsvSkill<int>.ONE_KILL_DMG){
                //* Critical
                pl.critical.setHitTypeSkill(pl.critical.Val, ref result, col, em, pl);
                //* FireProperty
                pl.fireProperty.setHitTypeSkill(pl.fireProperty.Val, ref result, col, em, pl);
                //* IceProperty
                pl.iceProperty.setHitTypeSkill(pl.iceProperty.Val, ref result, col, em, pl);
                //* ThunderProperty
                pl.thunderProperty.setHitTypeSkill(pl.thunderProperty.Val, ref result, col, em, pl);
                //* Explosion（最後 ダメージ適用）
                isOnExplosion = pl.explosion.setHitTypeSkill(pl.explosion.Val.per, ref result, col, em, pl, this.gameObject);
            }
            if(isOnExplosion){//* Explosion (爆発)
                RaycastHit[] hits = Physics.SphereCastAll(this.gameObject.transform.position, pl.explosion.Val.range, Vector3.up, 0);
                Array.ForEach(hits, hit => {
                    if(hit.transform.name.Contains(DM.NAME.Block.ToString())){ //! (BUG) 爆発ができないこと対応。(このIF文が無かったら、UnHitAreaのみ受け取りできない)
                        Debug.Log("Set DAMAGE:: Explostion:: result= " + result + ", hit.obj.name=" + hit.transform.gameObject.name);
                        bm.decreaseBlockHP(hit.transform.gameObject, result);
                    }
                });
            }
            else{//* Damage Result
                Debug.Log("Set DAMAGE:: result= " + result);
                bm.decreaseBlockHP(col.gameObject, result);
            }
        #endregion            
        }
        else if(col.transform.CompareTag(DM.TAG.Wall.ToString()) && col.gameObject.name == DM.NAME.DownWall.ToString()){
            Vector3 pos = new Vector3(this.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z);
            em.createDownWallHitEF(pos);
        }
    }
    // private IEnumerator coDelayThunderPropertyHit(int result, Collision col){
    //     em.createThunderStrikeEF(col.transform.position);
    //     yield return new WaitForSeconds(0.0875f);
    //     pl.thunderProperty.setHitTypeSkill(pl.thunderProperty.Value, ref result, col, em, pl);
    // }
//----------------------------------------------------------------
//*
//----------------------------------------------------------------
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
                this.rigid.velocity = this.rigid.velocity / 5;
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
    private float setgetHitBallSpeed(float power, Vector3 arrowDir){
        rigid.velocity = Vector3.zero;
        float force = Speed * power * pl.speed.Val;
        rigid.AddForce(arrowDir * force, ForceMode.Impulse);
        return force;
    }

    void instantiateMultiShot(Vector3 dir, float force, float ratio){
        var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup);
        ins.GetComponent<Rigidbody>().AddForce(dir * force * ratio, ForceMode.Impulse);
        Vector3 scale = ins.transform.localScale;
        ins.transform.localScale = new Vector3(scale.x * ratio, scale.y * ratio, scale.z * ratio);
    }

    private void decreaseBlocksHp(DM.ATV atv, int dmg, float dotDmgPer = 0){
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

    IEnumerator coPlayHomeRunAnim(bool isActiveSkillTrigger){
        isHomeRun = true;
        gm.IsPlayingAnim = true;
        Debug.Log("HOMERUH!!!!" + "isHomeRun= " + isHomeRun);
        Time.timeScale = 0;
        pl.setAnimTrigger(DM.ANIM.HomeRun.ToString());

        yield return new WaitForSecondsRealtime(2);
        //* Animation Finish
        gm.IsPlayingAnim = false;
        gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
        gm.homeRunTxtTf.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
        em.enableUITxtEF(DM.ANIM.HomeRun.ToString());
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
        List<GameObject> effectList = new List<GameObject>(); //* HPが０になったら、発生するERROR対応。

        //* HomeRun Bonus
        float critDmgRatio = (isHomeRun)? 
            AtvSkill.THUNDERSHOT_CRT + 1 : AtvSkill.THUNDERSHOT_CRT;

        //* Set Dmg & CriticalTextEF
        const int attackCnt = 5;
        int dmg = (int)(pl.dmg.Val * critDmgRatio);
        bm.decreaseBlockHP(hit.transform.gameObject, dmg * attackCnt);
        StartCoroutine(coMultiCriticalDmgEF(critDmgRatio, attackCnt, hit.transform.position));
    }
    IEnumerator coMultiCriticalDmgEF(float critDmgRatio, int cnt, Vector3 hitPos){
        float span = 0.0875f;
        for(int i=0; i<cnt; i++){
            em.createCritTxtEF(hitPos, (int)(pl.dmg.Val * critDmgRatio));
            if(isHomeRun) 
                em.createThunderStrikeEF(hitPos);
            yield return new WaitForSeconds(i * span);
        }
    }

    void OnDrawGizmos(){
        //* Explosion Skill Range Preview
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, pl.explosion.Val.range);

        //* ThunderShot Skill Range Preview => ArrowAxisAnchorObjに付いているThunderGizmosスクリプト。

        //* FireBall Skill Range Preview
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(this.transform.position, pl.FireBallCastWidth);
    }
}