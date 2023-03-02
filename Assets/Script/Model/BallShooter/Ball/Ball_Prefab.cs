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
    public Rigidbody myRigid;
    // public int aliveTime;
    Transform myTransform;
    SphereCollider myCollider;

    const int DELAY_INFINITE = 999;
    const string MAIN_BALL = "MainBall";

    //* Value
    [SerializeField] bool isHomeRun = false;
    [SerializeField] bool isActive = true;
    // [SerializeField] bool isHitedByBlock = false;
    // [SerializeField] float deleteLimitTime = 2.0f;
    [SerializeField] float speed;    public float Speed {get => speed; set => speed = value;}
    float distance;
    [Header("PSV UNIQUE")][Header("__________________________")]
    [SerializeField] bool isOnDarkOrb; public bool IsOnDarkOrb {get => isOnDarkOrb; set => isOnDarkOrb = value;}
    [SerializeField] GameObject darkOrbPf;

    [Header("DROP BOX")][Header("__________________________")]
    [SerializeField] bool isDmgX2;     public bool IsDmgX2 {get => isDmgX2; set => isDmgX2 = value;}

    void Awake() {
        gm = DM.ins.gm;
        em = gm.em; pl = gm.pl; bm = gm.bm; bs = gm.bs;

        myTransform = transform;
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponent<SphereCollider>();
        
        IsOnDarkOrb = false;
    }

    void OnDisable() => init(name);

    void FixedUpdate() {
        if(myCollider.isTrigger == true) return;

        if(transform.position.y > 0.5f){
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
    }
    void Update(){
        //* Destroy by Checking Velocity
        //! (BUG-80) Ball_Prefab:: FixedUpdate::checkDestroyMainBall()が重なって読み出すBUGある。
        //! まだ問題あり、isActiveトリガーを用意して対応。(まだ注意必要)
        if(isActive && myRigid.velocity != Vector3.zero && myRigid.velocity.sqrMagnitude < 0.9875f){
            // Debug.Log($"Ball_Prefab:: BallGroup.childCount= {gm.ballGroup.childCount}, velocity.magnitude= {Util._.setNumDP(myRigid.velocity.sqrMagnitude, 3)}, name= {name}, parent= {transform.parent.name}");
            isActive = false;
            myRigid.velocity = Vector3.zero;
            checkDestroyObjName();
        }

        //* Ball Preview Direction Goal Img
        distance = Vector3.Distance(gm.ballPreviewDirGoal.transform.position, myTransform.position);
        gm.setBallPreviewImgAlpha(distance);
    }
//----------------------------------------------------------------
//* Trigger Event
//----------------------------------------------------------------
    // void OnTriggerEnter(Collider col) {
    //     //? (BUG-61) nextStage()に行く時、時々downWallCollider.isTriggerがfalseのままになるBUG対応。
    //     //! 原因：ボールが来る間にプレイヤーがSwingするとpl.Doswingがtrueになり、
    //     //!      来ている領域がActiveDownWallだから、下の条件式に合ってしまうisTriggerが早くfalseなる。
        
    //     //* ActiveDownWall Areaへ衝突したら、DownWallの物理が活性化。
    //     if(pl.IsHitBall && pl.DoSwing && col.transform.CompareTag(DM.TAG.ActiveDownWall.ToString())){
    //         //* Main Ballではなければ、以下の処理しない。
    //         if(name != MAIN_BALL) return;
    //         Debug.Log("Ball::OnTriggerEnter:: col= " + col.name + ", this.name= " + this.name + ", downWallCollider.isTrigger= " + gm.downWallCollider.isTrigger);

    //         // pl.DoSwing = false;
    //         // pl.IsHitBall = false;

    //         // gm.downWallCollider.isTrigger = false;//* 衝突ON
    //         // #if UNITY_EDITOR
    //         // gm.debugDownWallColTrigger(false);
    //         // #endif

    //     }
    // }
    void OnTriggerStay(Collider col) {
        #region HIT BALL
        if(col.transform.CompareTag(DM.TAG.HitRangeArea.ToString())){
            // pl.setSwingArcColor("red");
            if(gm.State == GameManager.STATE.PLAY && pl.DoSwing){
                pl.IsHitBall = true;
                SM.ins.sfxPlay(SM.SFX.SwingHit.ToString());
                
                // StartCoroutine(coDelayActiveFastPlayBtn()); //* ホームランしたとき、見えないようにしてBUG防止。

                gm.switchCamera();
                // pl.DoSwing = false;
                myRigid.useGravity = true;
                
                //* (BUG-32) ボールが投げて来ているとき、途中でブロックとぶつかるバグ対応。
                myCollider.isTrigger = false;

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
                gm.activeSkillBtnList.ForEach(skillBtn => {
                    if(skillBtn.Trigger){
                        isActiveSkillTrigger = true;
                        StartCoroutine(coPlayActiveSkillShotEF(skillBtn, 1f, arrowDir));
                    }
                });

                //* Effect
                em.createBatHitSparkEF(myTransform.position);

                //* HomeRun
                if(power >= LM._.HOMERUN_MIN_POWER){
                    em.createHomeRunHitSparkEF(myTransform.position);
                    StartCoroutine(coPlayHomeRunAnim(isActiveSkillTrigger));
                }
                var force = setAddForce(power, arrowDir);

                //* Show HitBall Infomation
                string rankTxt = gm.setHitRankTxt(power, hitRank);
                float per = 100 - Util._.getCalcCurValPercentage(distance, LM._.MAX_DISTANCE);

                //* ボール打った情報表示
                gm.displayHitBallInfoUI(rankTxt, power.ToString(), per.ToString());

                //* Active Skillなら、下記は実行しない----------------------------------------
                if(isActiveSkillTrigger) return;

                #region PSV (SWING BALL)
                //*【 Bird Friend 】
                if(pl.birdFriend.Level == 1){
                    var ins = Instantiate(gm.eggPf, pl.BirdFriendObj.transform.position, Quaternion.identity, gm.ballGroup);
                    Vector3 throwDir = new Vector3(arrowDir.x, 1, arrowDir.z);
                    ins.GetComponent<Rigidbody>().AddForce((throwDir * 90), ForceMode.Impulse);
                    Destroy(ins, 10);
                }

                //* 【 Dark Orb 】
                if(pl.darkOrb.Level == 1){
                    Debug.Log("DARKORB ON!");
                    IsOnDarkOrb = true;
                    darkOrbPf.SetActive(IsOnDarkOrb);
                }

                //* 【 Giant Ball 】
                if(pl.giantBall.Level == 1){
                    var ballTexture = myTransform.GetChild(0).GetChild(0);
                    const float sc = PsvSkill<float>.GIANTBALL_SCALE;
                    ballTexture.transform.localScale = new Vector3(sc, sc, sc);
                }
                else{
                    //* 【 Multi Shot (横) 】
                    for(int i=0; i<pl.multiShot.Val;i++){ // Debug.Log($"<color=blue>【 Multi Shot (横) 】: {pl.multiShot.Value}</color>");
                        Vector3 dir = pl.multiShot.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                        instantiateMultiShot(dir, force, ratio: 0.9f);
                    }

                    //* 【 Vertical Multi Shot (縦) 】
                    for(int i=0; i<pl.verticalMultiShot.Val;i++){ // Debug.Log($"<color=blue>【 Vertical Multi Shot (縦) 】: {pl.verticalMultiShot.Value}</color>");
                        Vector3 dir = pl.arrowAxisAnchor.transform.forward;
                        instantiateMultiShot(dir, force, ratio: 0.9f);
                    }
                }
                //* 【 Laser 】
                for(int i=0; i < pl.laser.Val; i++){  // Debug.Log($"<color=blue>【 Laser 】: {pl.laser.Value}</color>");
                    SM.ins.sfxPlay(SM.SFX.LaserShoot.ToString());
                    Vector3 start = pl.arrowAxisAnchor.transform.position;
                    Vector3 dir = pl.laser.calcMultiShotDeg2Dir(arrowDeg, i); //* Arrow Direction with Extra Deg
                    em.createLaserEF(start, dir);

                    RaycastHit[] hits = Physics.RaycastAll(start, dir, 100);
                    Array.ForEach(hits, hit => {
                        if(hit.transform.name.Contains(DM.NAME.Block.ToString())){  // Debug.Log("LAZER Hit Obj -> " + hit.transform.name);
                            int laserDmg = pl.calcPlDmg();//pl.dmg.Val * (pl.laser.Level + 1);
                            em.createCritTxtEF(hit.transform.position, laserDmg);
                            bm.decreaseBlockHP(hit.transform.gameObject, laserDmg);
                        }
                    });
                }
                #endregion
            }
        }
        #endregion
    }
    void OnTriggerExit(Collider col) {
        //* HITED BALL GET OUT FROM HITAREA
        if(col.transform.CompareTag(DM.TAG.HitRangeArea.ToString())){ 
            // pl.setSwingArcColor("yellow");
            StartCoroutine(coFastPlayOn());
            // Debug.Log($"Ball_Prefab::OnTriggerExit(col= {col.name}):: Invoke(checkLimitTimeToDeleteBall, deleteLimitTime= {deleteLimitTime})");
        }
        //* STRIKE BALL
        else if(col.transform.CompareTag(DM.TAG.StrikeLine.ToString())){
            Debug.Log($"<color=red>Ball_Prefab:: OnTriggerExit:: this.name= {this.name}, col= {col.name}</color>");
            // checkDestroyMainBall();
            //* (BUG-79) MainBallをまずObjectPool化しましたが、localScale.xが0.4なのに、0.39999になって、Strike条件式に入らないBUGあるため、条件式からこの部分抜ける。
            if(this.name == MAIN_BALL){// && myTransform.localScale.x == 0.4f){
                Debug.Log("this.name :: Strike!");
                onDestroyMe(true);
            }
        }
    }
//----------------------------------------------------------------
//* Collision Event
//----------------------------------------------------------------
    void OnCollisionEnter(Collision col) {
        //* HIT BLOCK
        if(col.transform.CompareTag(DM.TAG.Wall.ToString())){
            if(name == "MainBall")
                Debug.Log($"<color=green>Ball_Prefab:: OnCollisionEnter:: name= {name} col= {col.transform.name}</color>");
            else
                Debug.Log($"<color=white>Ball_Prefab:: OnCollisionEnter:: name= {name} col= {col.transform.name}</color>");
            setDownWallTriggerOff();
            em.createDownWallHitEF(myTransform.position);
        }
        else if(col.transform.name.Contains(DM.NAME.Block.ToString()) || col.transform.name.Contains(DM.NAME.Obstacle.ToString())){ //* (BUG-3) 障害物もFreezeからだめーず受けるように。
            SM.ins.sfxPlay(SM.SFX.HitBlock.ToString());

            // isHitedByBlock = true;
            setDownWallTriggerOff();
            
            #region ATV
            gm.activeSkillBtnList.ForEach(skillBtn => {
                if(skillBtn.Trigger){
                    const float delayTime = 2;
                    int skillIdx = gm.getCurSkillIdx();
                    var atv = DM.ins.convertAtvSkillStr2Enum(skillBtn.Name);

                    switch(atv){
                        case DM.ATV.ThunderShot: //なし
                            break;
                        case DM.ATV.FireBall:{
                            em.createAtvSkExplosionEF(skillIdx, myTransform);
                            decreaseBlocksHp(atv, AtvSkill.FireballDmg);
                            if(isHomeRun)
                                decreaseBlocksHp(atv, 0, AtvSkill.FireballDot); //* + DOT DAMAGE
                            SM.ins.sfxPlay(SM.SFX.FireBallExplosion.ToString());
                            // this.rigid.velocity = Vector3.zero; //! ボール速度が０になると、待機されず次のステージに進むBUG。
                            break;
                        }
                        case DM.ATV.ColorBall:{
                            if(col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.KIND.TreasureChest 
                            && col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.KIND.Obstacle
                            && col.gameObject.GetComponent<Block_Prefab>().kind != BlockMaker.KIND.Boss
                            ){
                                SM.ins.sfxPlay(SM.SFX.ColorBallPop.ToString());
                                Block_Prefab[] sameColorBlocks = AtvSkill.findSameColorBlocks(gm, col.transform.gameObject);
                                //* Set Max Cnt (BUG-67) ColorBallPop Upgrade Max数値が基本を超えると、UndefinedIndexになるバグ対応。
                                int max = (sameColorBlocks.Length > AtvSkill.ColorBallPopCnt)? AtvSkill.ColorBallPopCnt : sameColorBlocks.Length;
                                if(isHomeRun) max = sameColorBlocks.Length;
                                Debug.Log($"Ball_Prefab:: OnCollisionEnter:: max= {max}");

                                //* Erase Same Color Blocks
                                for(int i=0; i<max; i++){
                                    
                                    em.createAtvSkExplosionEF(skillIdx, sameColorBlocks[i].transform);
                                    if(isHomeRun)
                                        em.createColorBallStarExplosionEF(sameColorBlocks[i].transform.position);
                                    sameColorBlocks[i].transform.GetComponent<Block_Prefab>().decreaseHp(PsvSkill<int>.ONE_KILL_DMG);
                                }
                            }
                            // this.rigid.velocity = Vector3.zero; //! ボール速度が０になると、待機されず次のステージに進むBUG。
                            break;
                        }
                        case DM.ATV.PoisonSmoke:{
                            SM.ins.sfxPlay(SM.SFX.PoisonExplosion.ToString());
                            int destroyCnt = DELAY_INFINITE; //* (BUG-60) PoisonSmokeスキルでない。CoRoutine WaitforSencondsCashingについて、999秒のことがなかったため。
                            var ins = em.createAtvSkExplosionEF(skillIdx, myTransform, destroyCnt);
                            if(isHomeRun){
                                float sc = 1.3f;
                                ins.GetComponent<PoisonSmoke>().Duration += 2;
                                ins.transform.localScale = new Vector3(sc, sc, sc);
                            }
                            RaycastHit[] hits = Physics.SphereCastAll(myTransform.position, pl.PoisonSmokeCastWidth, Vector3.up, 0);
                            decreaseBlocksHp(atv, 0, AtvSkill.PoisonSmokeDot);
                            // this.rigid.velocity = Vector3.zero;
                            break;
                        }
                        case DM.ATV.IceWave:{
                            SM.ins.sfxPlay(SM.SFX.IceExplosion.ToString());
                            var effect = em.createAtvSkExplosionEF(skillIdx, myTransform);
                            // this.rigid.velocity = Vector3.zero;
                            if(isHomeRun) {
                                effect.transform.GetChild(effect.transform.childCount-1).gameObject.SetActive(true);
                            }
                            break;
                        }
                    }
                    skillBtn.init(gm);
                    gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
                    myCollider.enabled = false; //* ボール動きなし

                    //* Delay Next Stage 
                    StartCoroutine(coDelayDestroyMe()); //* (BUG-65) ColorBallPopブロックにぶつかってもボールが破壊されない。-> coDelayDestroyMe()生成して対応。
                }
            });
            #endregion

            #region PSV + DAMAGE RESULT
            int result = 0;
            bool isInstantKill= false, isCritical= false, isOnExplosion = false;

            //* GodBless
            if(pl.godBless.Level == 1){
                if(gm.comboCnt != 0 && gm.comboCnt % LM._.GODBLESS_COMBO_SPAN == 0){
                    float radius = 4;
                    // Util._.displayDebugSphere(myTransform.position, radius, 0.5f, "blue");

                    //* Explosion
                    SM.ins.sfxPlay(SM.SFX.FlashHit.ToString());
                    em.createGodBlessEF(myTransform.position);
                    Util._.sphereCastAllDecreaseBlocksHp(myTransform, radius, pl.calcPlDmg() * 3);//pl.dmg.Val * 3);
                }
            }

            //* InstantKill
            isInstantKill = pl.instantKill.setHitTypeSkill(pl.instantKill.Val, ref result, col, em, pl);

            if(result != PsvSkill<int>.ONE_KILL_DMG){
                //* Critical
                isCritical = pl.critical.setHitTypeSkill(pl.critical.Val, ref result, col, em, pl, this.gameObject);
                //* FireProperty
                pl.fireProperty.setHitTypeSkill(pl.fireProperty.Val, ref result, col, em, pl, this.gameObject);
                //* IceProperty
                pl.iceProperty.setHitTypeSkill(pl.iceProperty.Val, ref result, col, em, pl, this.gameObject);
                //* ThunderProperty
                pl.thunderProperty.setHitTypeSkill(pl.thunderProperty.Val, ref result, col, em, pl, this.gameObject);
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
                if(isDmgX2) result *= 2;
                
                bm.decreaseBlockHP(col.gameObject, result);
                //* (BUG-30) Ball_Prefab::CritとかOneKillテキストEFとダメージ普通EFが一緒に発動するバグ対応。
                if(!isInstantKill && !isCritical)
                    em.createDmgTxtEF(myTransform.position, result);
            }
            #endregion
        }
        
    }
//----------------------------------------------------------------
//* Active Bat Skill
//----------------------------------------------------------------
    #region ATV (BAT)
    IEnumerator coPlayActiveSkillShotEF(AtvSkillBtnUI btn, float waitTime, Vector3 dir){
        Debug.LogFormat("coPlayActiveSkillShotEF:: btn={0}, waitTite={1}, dir={2}, isHomeRun={3}", btn.Name, waitTime, dir, isHomeRun);
        int skillIdx = gm.getCurSkillIdx();
        var atv = DM.ins.convertAtvSkillStr2Enum(btn.Name);
        switch(atv){
            case DM.ATV.ThunderShot:
                // const float delayTime = 2;
                const int maxDistance = 50;
                const int width = 1;
                Debug.DrawRay(myTransform.position, dir * maxDistance, Color.blue, 2f);

                gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
                em.createAtvSkShotEF(skillIdx, this.gameObject.transform, pl.arrowAxisAnchor.transform.rotation);

                //* Collider 
                RaycastHit[] hits = Physics.BoxCastAll(myTransform.position, Vector3.one * width, dir, Quaternion.identity, maxDistance);
                Array.ForEach(hits, hit => {
                    if(hit.transform.name.Contains(DM.NAME.Block.ToString())){
                        StartCoroutine(coSetThunderSkill(hit)); //* With HomeRun Bonus
                    }
                });
                myCollider.enabled = false;//ボール動きなし

                gm.activeSkillBtnList.ForEach(btn => btn.init(gm));
                yield return Util.delay2;//new WaitForSeconds(delayTime);
                Debug.Log("Ball_Prefab::coPlayActiveSkillShotEF()::");
                onDestroyMe();
                break;
            case DM.ATV.FireBall:
            case DM.ATV.ColorBall:
            case DM.ATV.PoisonSmoke:
            case DM.ATV.IceWave:
                this.myRigid.velocity = this.myRigid.velocity / 5;
                em.createAtvSkShotEF(skillIdx, this.gameObject.transform, Quaternion.identity, true); //Trail
                break;
        }
        //Before go up NextStage Wait for Second
    }
    #endregion

    IEnumerator coDelayDestroyMe() {
        Debug.Log("Ball_Prefab::coDelayDestroyMe()::");
        yield return Util.delay2;
        onDestroyMe();
    }
//*---------------------------------------------------------------
//*  関数
//*---------------------------------------------------------------
    private void init(string name){
        isActive = true;
        myRigid.velocity = Vector3.zero;
        myRigid.angularVelocity = Vector3.zero;
        myRigid.useGravity = (name == DM.NAME.MainBall.ToString())? false : true;
        myTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        myTransform.localScale = Vector3.one * 0.4f;
        myCollider.isTrigger = (name == DM.NAME.MainBall.ToString())? true : false;
        myCollider.enabled = true; //* (BUG-81) 急にボールがStrike領域とかHitAreaをそのまま追加していく。
        isHomeRun = false;
        IsOnDarkOrb = false;
        darkOrbPf.SetActive(false);
        isDmgX2 = false;
        speed = 0;

        //* Trailエフェクト消す（DropBoxアイテムで適用された）
        for(int i=0; i<transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if(child.name == DM.NAME.DropBoxStarTrailEF.ToString()) Destroy(child.gameObject);
            if(child.name == DM.NAME.DropBoxSpeedTrailEF.ToString()) Destroy(child.gameObject);
        }

        //* サイズ初期化 (GaintBallで大きくなった)
        Transform ballTf = Array.Find(transform.GetComponentsInChildren<Transform>(), tf => tf.name == DM.NAME.Box001.ToString());
        if(ballTf)  ballTf.localScale = Vector3.one;
    }

    private void setDownWallTriggerOff() {
        //* (★BUG-77) ブロックがActiveDownWall領域より下に来ると、ボールを釣っても急に無くなる。
        //* 対応：OncollisionEnter()で、BlockやWallにぶつかったら、DownWallのTriggerがOffになるようにする。
        if(gm.downWallCollider.isTrigger && name == MAIN_BALL){
            gm.downWallCollider.isTrigger = false;//* 衝突ON
            #if UNITY_EDITOR
                gm.debugDownWallColTrigger(false);
            #endif
            pl.DoSwing = false;
            pl.IsHitBall = false;
        }
    }
    IEnumerator coFastPlayOn(){
        yield return Util.delay2;
        Debug.Log($"Ball_Prefab::coFastPlayOn():: this.name= {name}");
        gm.onClickFastPlayButton();
    }

    private float setAddForce(float power, Vector3 arrowDir){
        myRigid.velocity = Vector3.zero;

        //* (BUG-72) ボールを打つ位置によって、PreviewBallと、ボールが飛ぶ角度がずれる問題：HitRangeAreaの原点(絶対座標)に設定することで対応。
        const float LEFT_OFFSET_X = -3f, RIGHT_OFFSET_Y = 3.1f;

        float posX = (gm.touchSlideControlPanel.playerSpotPosX == TouchSlideControl.POS_X.LEFT)? LEFT_OFFSET_X
            : (gm.touchSlideControlPanel.playerSpotPosX == TouchSlideControl.POS_X.CENTER)? 0
            : RIGHT_OFFSET_Y;
        transform.position = new Vector3(gm.hitRangeAreaTf.localPosition.x + posX, gm.hitRangeAreaTf.localPosition.y, gm.hitRangeAreaTf.localPosition.z);

        float force = Speed * power * pl.speed.Val * Time.fixedDeltaTime;
        myRigid.AddForce(arrowDir * force, ForceMode.Impulse);
        return force;
    }

    void instantiateMultiShot(Vector3 dir, float force, float ratio){
        // var ins = ObjectPool.getObject(ObjectPool.DIC.SubBall.ToString(), myTransform.position, Quaternion.identity, gm.ballStorage);
        // ins.name = DM.NAME.SubBall.ToString();
        // ins.transform.SetParent(gm.ballGroup);
        var ins = gm.bs.setBallObject(DM.NAME.SubBall.ToString(), myTransform, Quaternion.identity);

        ins.GetComponent<Rigidbody>().AddForce(dir * force * ratio, ForceMode.Impulse);
        Vector3 scale = ins.transform.localScale;
        // ins.transform.localScale = new Vector3(scale.x * ratio, scale.y * ratio, scale.z * ratio);
    }

    private void decreaseBlocksHp(DM.ATV atv, int dmg, float dotDmgPer = 0){
        RaycastHit[] hits = Physics.SphereCastAll(myTransform.position, pl.FireBallCastWidth, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.name.Contains(DM.NAME.Block.ToString())
            || hit.transform.name.Contains(DM.NAME.Obstacle.ToString())){
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
    // private void checkLimitTimeToDeleteBall(){
    //     if(isHitedByBlock)
    //         isHitedByBlock = false;
    //     else{
    //         Debug.Log("Ball_Prefab::checkLimitTimeToDeleteBall()::");
    //         checkDestroyMainBall();
    //     }
    // }
    public void checkDestroyObjName(){
        //* (BUG-79) MainBallをまずObjectPool化しましたが、localScale.xが0.4なのに、0.39999になって、Strike条件式に入らないBUGあるため、条件式からこの部分抜ける。
        if(name == MAIN_BALL){// && myTransform.localScale.x == 0.4f){
            Debug.Log($"AAA Ball_Prefab:: checkDestroyObjName():: MAIN BALL-> name= {name}");
            onDestroyMe();
        }
        else{
            Debug.Log($"Ball_Prefab:: checkDestroyObjName():: SUB BALL-> name= {name}");
            StartCoroutine(ObjectPool.coDestroyObject(gameObject, gm.ballStorage)); // Destroy(this.gameObject);
        }
    }

    private void onDestroyMe(bool isStrike = false){
        if(!isStrike){
            Debug.Log($"<color=red>Ball_Prefab:: onDestroyMe:: gm.setNextStage(), this.name= {this.name}</color>");
            gm.setNextStage();//* ボールが消えたら次に進む。
        }else{
            gm.setStrike();
            gm.setBallPreviewGoalRandomPos();
        }
        StartCoroutine(ObjectPool.coDestroyObject(gameObject, gm.ballStorage)); // Destroy(this.gameObject);
    }

    IEnumerator coPlayHomeRunAnim(bool isActiveSkillTrigger){
        isHomeRun = true;
        gm.IsPlayingAnim = true;
        gm.dontLookCam2ObjsGroup.SetActive(true);
        Debug.Log("HOMERUH!!!!" + "isHomeRun= " + isHomeRun);
        Time.timeScale = 0;
        pl.setAnimTrigger(DM.ANIM.HomeRun.ToString());

        yield return Util.delay2RT;
        gm.dontLookCam2ObjsGroup.SetActive(false);
        SM.ins.sfxPlay(SM.SFX.HomeRun.ToString());
        //* Animation Finish
        gm.IsPlayingAnim = false;
        gm.cam1.GetComponent<Animator>().SetTrigger(DM.ANIM.DoShake.ToString());
        gm.homeRunTxtTf.GetComponent<Animator>().SetTrigger(DM.ANIM.DoSpawn.ToString());
        em.playUIAnimEF(DM.ANIM.HomeRun.ToString());
        Time.timeScale = 1;
    }

    IEnumerator coPlayActiveSkillBefSpotLightAnim(){
        Debug.Log("ActiveSkill Before Anim");
        Time.timeScale = 0;
        pl.setAnimTrigger("ActiveSkillBefSpotLight");
        yield return Util.delay0_3RT; //new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
    }
    IEnumerator coSetThunderSkill(RaycastHit hit){
        yield return Util.delay0_1;//new WaitForSeconds(0.1f);
        Debug.Log("<color=red>coSetThunderSkill():: isHomeRun= " + isHomeRun + "</color>");
        List<GameObject> effectList = new List<GameObject>(); //* HPが０になったら、発生するERROR対応。

        //* HomeRun Bonus
        float critDmgRatio = (isHomeRun)? 
            AtvSkill.ThunderShotDmg + 1 : AtvSkill.ThunderShotDmg;

        //* Set Dmg & CriticalTextEF
        int dmg = Mathf.RoundToInt(pl.calcPlDmg() * critDmgRatio);//(int)(pl.dmg.Val * critDmgRatio);
        int hitCnt = AtvSkill.ThunderShotHitCnt;

        SM.ins.sfxPlay(SM.SFX.Lightning.ToString());

        //* Give Damage
        bm.decreaseBlockHP(hit.transform.gameObject, dmg * hitCnt);
        StartCoroutine(coMultiCriticalDmgEF(critDmgRatio, hitCnt, hit.transform.position));
    }
    IEnumerator coMultiCriticalDmgEF(float critDmgRatio, int cnt, Vector3 hitPos){
        cnt = (int)(cnt * 0.7f); //* カウント全てをすると、コストが掛かるため、70%のみ描画する。
        for(int i=0; i<cnt; i++){
            em.createCritTxtEF(hitPos, Mathf.RoundToInt(pl.calcPlDmg() * critDmgRatio));//(int)(pl.dmg.Val * critDmgRatio));
            if(isHomeRun) 
                em.createThunderStrikeEF(hitPos);
            yield return Util.delay0_1;
        }
    }
    IEnumerator coDelayActiveFastPlayBtn(){
        yield return Util.delay0_1;
        gm.fastPlayBtn.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    // void OnDrawGizmos(){
        //* Explosion Skill Range Preview
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(myTransform.position, pl.explosion.Val.range);

        //* ThunderShot Skill Range Preview => ArrowAxisAnchorObjに付いているThunderGizmosスクリプト。

        //* FireBall Skill Range Preview
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(myTransform.position, pl.FireBallCastWidth);
    // }
#endif
}