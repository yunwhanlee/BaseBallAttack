using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Ball_Prefab : MonoBehaviour
{
    //* OutSide
    public GameManager gm;
    public EffectManager em;
    public Player pl;
    public BlockMaker blockMaker;
    public BallShooter ballShooter;
    public int aliveTime;

    //* Value
    private bool isHitedByBlock = false;
    private float deleteLimitTime = 2.0f;
    private int speed;
    private float distance;

    Rigidbody rigid;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        em = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        pl = GameObject.Find("Player").GetComponent<Player>();
        blockMaker = GameObject.Find("BlockMaker").GetComponent<BlockMaker>();
        ballShooter = GameObject.Find("BallShooter").GetComponent<BallShooter>();

        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(this.transform.forward * speed, ForceMode.Impulse);
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
        {
        //* HIT BALL
        if(col.gameObject.tag == "HitRangeArea"){
            pl.setSwingArcColor("red");
            if(pl.DoSwing && gm.STATE == GameManager.State.PLAY){
                gm.switchCamScene();
                // isHited = true;
                pl.DoSwing = false;
                rigid.useGravity = true;

                //STRIKEデータ 初期化
                gm.strikeCnt = 0;
                foreach(var img in gm.strikeBallImgs) img.gameObject.SetActive(false); 

                //offset Axis
                const int leftSide = -1, rightSide = 1;
                int sign = pl.transform.localScale.x < 0 ? leftSide : rightSide;
                int offsetAxis = (sign < 0) ? (pl.MAX_HIT_DEG/2) * leftSide : (pl.MAX_HIT_DEG/2) * rightSide;
                // Debug.Log("Ball_Prefab:: ■offsetAxis=" + offsetAxis);

                float deg = pl.arrowAxisAnchor.transform.eulerAngles.y;
                Debug.Log("BALL DEGREE:" + deg);
                Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * deg), 0, Mathf.Cos(Mathf.Deg2Rad * deg)).normalized;

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
                        StartCoroutine(coPlayActiveSkillShotEF(skillBtn, 1f, dir));
                        isActiveSkillTrigger = true;
                    }
                });

                //* Anim
                if(power >= pl.hitRank[B].Power){//* HomeRun
                    StartCoroutine(coPlayHomeRunAnim(isActiveSkillTrigger));
                }
                else if(isActiveSkillTrigger){ //* ActiveSkill Before
                    // StartCoroutine(coPlayActiveSkillBefSpotLightAnim());
                }

                rigid.velocity = Vector3.zero;
                float force = speed * power * pl.speed.Value;
                rigid.AddForce(dir * force, ForceMode.Impulse);
                Debug.Log(
                    "HIT Ball! <color=yellow>distance=" + distance.ToString("N2") + "</color>"
                    + ", <color=red>power=" + power + ", Rank: " + ((power==pl.hitRank[A].Power)? "A" : (power==pl.hitRank[B].Power)? "B" : (power==pl.hitRank[C].Power)? "C" : (power==pl.hitRank[D].Power)? "D" : (power==pl.hitRank[E].Power)? "E" : "F").ToString() + "</color>"
                    + ", Force=" + force);

                if(isActiveSkillTrigger) return;
                
                //* Multi Shot
                for(int i=0; i<pl.multiShot.Value;i++){
                    float [] addDegList = {-15, 15, -30, 30};
                    Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (deg + addDegList[i])), 0, Mathf.Cos(Mathf.Deg2Rad * (deg + addDegList[i]))).normalized;
                    var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup) as GameObject;
                    ins.GetComponent<Rigidbody>().AddForce(direction * force * 0.75f, ForceMode.Impulse);
                    var scale = ins.GetComponent<Transform>().localScale;
                    ins.GetComponent<Transform>().localScale = new Vector3(scale.x * 0.75f, scale.y * 0.75f, scale.z * 0.75f);
                }
            }
        }
        else if(col.gameObject.tag == "ActiveDownWall"){
            pl.DoSwing = false;
            if(gm.STATE == GameManager.State.WAIT){
                gm.downWall.isTrigger = false;//*下壁 物理O
            }
        }
        }
    }

    private void OnTriggerExit(Collider col) {
        //* SWING Ball
        if(col.gameObject.tag == "HitRangeArea"){ //* HIT BALL
            Debug.Log("OnTriggerExit:: BAT SWING BALL");
            pl.setSwingArcColor("yellow");
            //* 日程時間が過ぎたら、ボール削除。
            isHitedByBlock = true;
            InvokeRepeating("checkLimitTimeToDeleteBall", 0, deleteLimitTime);
        }
        else if(col.gameObject.tag == "StrikeLine"){ //* ストライク
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

    //* Hit Block
    private void OnCollisionEnter(Collision col) {//* Give Damage
        if(col.gameObject.tag == "NormalBlock"){
            isHitedByBlock = true;
            //* #2. Active Skill HIT
            gm.activeSkillBtnList.ForEach(skillBtn => {
                if(skillBtn.Trigger){
                    float delayTime = 2;
                    int selectAtvSkillIdx = DM.ins.personalData.SelectSkillIdx;
                    gm.cam1.GetComponent<CamResolution>().setAnimTrigger("doShake");
                    switch(skillBtn.Name){
                        case "Thunder":
                            //なし
                            break;
                        case "FireBall":{
                            em.createActiveSkillExplosionEF(selectAtvSkillIdx, this.transform);
                            decreaseHpSphereCastAll(10);
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                        case "PoisonSmoke":{
                            int destroyCnt = 999;
                            em.createActiveSkillExplosionEF(selectAtvSkillIdx, this.transform, destroyCnt);
                            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.PoisonSmokeCastWidth, Vector3.up, 0);
                            decreaseHpSphereCastAll(0, 2);
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        }
                        case "ColorBall":
                            //* Hit Color
                            var meshRd = col.gameObject.GetComponent<MeshRenderer>();
                            Color hitColor = meshRd.material.GetColor("_ColorTint");
                            Debug.Log("OnCollisionEnter:: ColorBall AtvSkill -> hitColor=" + hitColor);
                            //* Find Same Color Blocks
                            var blocks = gm.bm.GetComponentsInChildren<Block_Prefab>();
                            var sameColorBlocks = Array.FindAll(blocks, bl => 
                                bl.GetComponent<MeshRenderer>().material.GetColor("_ColorTint") == hitColor
                            );
                            //* Destroy
                            Array.ForEach(sameColorBlocks, bl => {
                                em.createActiveSkillExplosionEF(selectAtvSkillIdx, bl.transform);
                                bl.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(100);
                            });

                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                        case "IceWave":
                            delayTime = 2f;
                            em.createActiveSkillExplosionEF(selectAtvSkillIdx, this.transform);
                            skillBtn.init(gm);
                            this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                            break;
                    }

                    //* Delay Next Stage
                    Invoke("onDestroyMeInvoke", delayTime);
                }
            });
            //* HIT Base Passive Skills
            int result = 0;
            float per = 0f;
            //* InstantKill
            per = pl.instantKill.Value;
            pl.instantKill.setHitTypePsvSkill(per, ref result, col, em, pl);
            //* Critical
            per = pl.critical.Value;
            pl.critical.setHitTypePsvSkill(per, ref result, col, em, pl);
            //* Explosion（最後 ダメージ適用）
            per = pl.explosion.Value.per;
            pl.explosion.setHitTypePsvSkill(per, ref result, col, em, pl, this.gameObject);
        }
        else if(col.gameObject.tag == "Wall" && col.gameObject.name == "DownWall"){
            Vector3 pos = new Vector3(this.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z);
            em.createDownWallHitEF(pos);
        }
    }

    private void onDestroyMeInvoke() => onDestroyMe();
    

    private void decreaseHpSphereCastAll(int dmg, int dotDmgDevideVal = 0){
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, pl.FireBallCastWidth, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.tag == "NormalBlock"){
                var block = hit.transform.gameObject.GetComponent<Block_Prefab>();
                block.decreaseHp((dotDmgDevideVal==0)? dmg : block.getDotDmg(dotDmgDevideVal));
            }
        });
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
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

    private void checkDestroyBall(){
        if(this.name == "Ball(Clone)" && this.transform.localScale.x == 0.4f)
            onDestroyMe();
        else
            Destroy(this.gameObject);
    }

    public void setBallSpeed(int v){
        speed = v;
    }

    IEnumerator coPlayHomeRunAnim(bool isActiveSkillTrigger){
        // if(isActiveSkillTrigger){
        //     yield return coPlayActiveSkillBefSpotLightAnim();
        // }
        Debug.Log("HOMERUH!!!!");
        Time.timeScale = 0;
        pl.setAnimTrigger("HomeRun");
        yield return new WaitForSecondsRealtime(2);
        gm.cam1.GetComponent<CamResolution>().setAnimTrigger("doShake");
        Time.timeScale = 1;
    }

    IEnumerator coPlayActiveSkillBefSpotLightAnim(){
        Debug.Log("ActiveSkill Before Anim");
        Time.timeScale = 0;
        pl.setAnimTrigger("ActiveSkillBefSpotLight");
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
    }

    IEnumerator coPlayActiveSkillShotEF(ActiveSkillBtnUI btn, float waitTime, Vector3 dir){
        Debug.LogFormat("coPlayActiveSkillShotEF:: btn={0}, waitTite={1}, dir={2}", btn.Name, waitTime, dir);
        float delayTime = 0;
        int selectAtvSkillIdx = DM.ins.personalData.SelectSkillIdx;
        switch(btn.Name){
            case "Thunder":
                delayTime = 2;
                const int maxDistance = 50;
                const int width = 1;
                Debug.DrawRay(this.transform.position, dir * maxDistance, Color.blue, 2f);
                gm.cam1.GetComponent<CamResolution>().setAnimTrigger("doShake");

                em.createActiveSkillShotEF(selectAtvSkillIdx, this.gameObject.transform, pl.arrowAxisAnchor.transform.rotation);
                //* Collider 
                RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, Vector3.one * width, dir, Quaternion.identity, maxDistance);
                Array.ForEach(hits, hit => {
                    if(hit.transform.tag == "NormalBlock"){
                        em.createCriticalTextEF(hit.transform, pl.dmg.Value * 2);
                        hit.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(pl.dmg.Value * 2);
                    }
                });
                gm.activeSkillBtnList[0].init(gm);
                this.gameObject.GetComponent<SphereCollider>().enabled = false;//ボール動きなし
                yield return new WaitForSeconds(delayTime);
                onDestroyMe();
                break;
            case "FireBall":
            case "ColorBall":
            case "PoisonSmoke":
            case "IceWave":
                em.createActiveSkillShotEF(selectAtvSkillIdx, this.gameObject.transform, Quaternion.identity, true); //Trail
                break;
        }
        //Before go up NextStage Wait for Second
    }

    

    void OnDrawGizmos(){
        //* Explosion Skill Range Preview
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(this.transform.position, pl.explosion.getValue().range);

        //* ThunderShot Skill Range Preview => ArrowAxisAnchorObjに付いているThunderGizmosスクリプト。

        //* FireBall Skill Range Preview
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, pl.FireBallCastWidth);
    }
}
