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
    private bool isHited = false;
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
            //* Destroy by Checking Velocity
            // Debug.Log("BallGroup.childCount=" + gm.ballGroup.childCount + ", Ball Velocity.magnitude="+rigid.velocity.magnitude);
            if(rigid.velocity.magnitude != 0 && rigid.velocity.magnitude < 0.9875f){
                if(gm.ballGroup.childCount <= 1)
                    onDestroyMe();
                else
                    Destroy(this.gameObject);
            }

            //* Ball Comeing View Slider
            float startPosZ = gm.hitRangeStartTf.position.z;
            float endPosZ = gm.hitRangeEndTf.position.z;
            if(!isHited && endPosZ <= this.transform.position.z && this.transform.position.z <= startPosZ){
                //HitRange Slider UI
                float offset = Mathf.Abs(startPosZ);
                float max = Mathf.Abs(endPosZ) - offset;
                float v = Mathf.Abs(this.transform.position.z) - offset;
                gm.hitRangeSlider.value = v / max;
            }

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
            if(pl.getDoSwing() && gm.state == GameManager.State.PLAY){
                gm.switchCamScene();
                isHited = true;
                pl.setDoSwing(false);
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
                
                //* HomeRun
                if(power >= pl.hitRank[B].Power){
                    StartCoroutine(coPlayHomeRunAnimation());
                }

                rigid.velocity = Vector3.zero;
                float force = speed * power * pl.speed.getValue();
                rigid.AddForce(dir * force, ForceMode.Impulse);
                Debug.Log(
                    "HIT Ball! <color=yellow>distance=" + distance.ToString("N2") + "</color>"
                    + ", <color=red>power=" + power + ", Rank: " + ((power==pl.hitRank[A].Power)? "A" : (power==pl.hitRank[B].Power)? "B" : (power==pl.hitRank[C].Power)? "C" : (power==pl.hitRank[D].Power)? "D" : (power==pl.hitRank[E].Power)? "E" : "F").ToString() + "</color>"
                    + ", Force=" + force);

                //! Active Skills
                if(gm.activeSkillBtnList[0].Trigger){
                    Debug.Log("Active Skill Trigger ON");
                    //Thunder
                    // this.gameObject.transform.position = new Vector3(9999,9999,9999); //メインボールを遠い場所に送る。
                    // StartCoroutine(coPlayThunderShotSkillEffect(dir, 1f));

                    //FireBall
                    Instantiate(pl.activeSkill_1.ShotEfPref, transform.position, Quaternion.identity, this.gameObject.transform);
                }
                else{
                    //* Multi Shot
                    for(int i=0; i<pl.multiShot.getValue();i++){
                        float [] addDegList = {-15, 15, -30, 30};
                        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (deg + addDegList[i])), 0, Mathf.Cos(Mathf.Deg2Rad * (deg + addDegList[i]))).normalized;
                        var ins = Instantiate(this.gameObject, this.transform.position, Quaternion.identity, gm.ballGroup) as GameObject;
                        ins.GetComponent<Rigidbody>().AddForce(direction * force * 0.75f, ForceMode.Impulse);
                        var scale = ins.GetComponent<Transform>().localScale;
                        ins.GetComponent<Transform>().localScale = new Vector3(scale.x * 0.75f, scale.y * 0.75f, scale.z * 0.75f);
                    }
                }
            }
        }
        else if(col.gameObject.tag == "ActiveDownWall"){
            pl.setDoSwing(false);
            if(gm.state == GameManager.State.WAIT){
                gm.downWall.isTrigger = false;//*下壁 物理O
            }
        }
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "HitRangeArea"){
            pl.setSwingArcColor("yellow");
        }
        //* ストライク！
        else if(col.gameObject.tag == "StrikeLine"){
            onDestroyMe(true);
        }
    }

    //* Hit Block
    private void OnCollisionEnter(Collision col) {
        //* Give Damage
        if(col.gameObject.tag == "NormalBlock"){
            int result = 0;
            //* InstantKill
            int rand = Random.Range(0, 100);
            int v = Mathf.RoundToInt(pl.instantKill.getValue() * 100); //百分率
            Debug.Log("Hit Block:: InstantKill:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=blue>true</color>" : "<color=blue>false</color>"));
            if(rand <= v){
                em.createEffectInstantKillText(col.transform);
                Destroy(col.gameObject);
            }
            else{
                result = pl.dmg.getValue();
            }

            //* Critical x 2
            rand = Random.Range(0, 100);
            v = Mathf.RoundToInt(pl.critical.getValue() * 100); //百分率
            Debug.Log("Hit Block:: Critical:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=orange>true</color>" : "<color=orange>false</color>"));
            if(rand <= v){
                em.createEffectCriticalText(col.transform, pl.dmg.getValue() * 2);
                result = pl.dmg.getValue() * 2;
            }
            else{
                result = pl.dmg.getValue();
            }

            //* Explosion
            rand = Random.Range(0, 100);
            v = Mathf.RoundToInt(pl.explosion.getValue().per * 100); //百分率
            Debug.Log("Hit Block:: Explosion:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=purple>true</color>" : "<color=purple>false</color>"));
            if(rand <= v){
                em.createEffectExplosion(this.transform, pl.explosion.getValue().range);

                //Sphere Collider
                RaycastHit[] rayHits = Physics.SphereCastAll(this.transform.position, pl.explosion.getValue().range, Vector3.up, 0);
                foreach(var hitObj in rayHits){
                    if(hitObj.transform.tag == "NormalBlock")
                        hitObj.transform.GetComponent<Block_Prefab>().decreaseHp(result);
                }
                return;
            }
            col.gameObject.GetComponent<Block_Prefab>().decreaseHp(result);
        }
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void onDestroyMe(bool isStrike = false){
        if(!isStrike){
            gm.setNextStage();//* ボールが消えたら次に進む。
        }else{
            gm.setStrike();
            gm.setBallPreviewGoalRandomPos();
        }
        
        Destroy(this.gameObject);
    }

    public void setBallSpeed(int v){
        speed = v;
    }

    IEnumerator coPlayHomeRunAnimation(){        
        Debug.Log("HOMERUH!!!!");
        Time.timeScale = 0;
        pl.setAnimTrigger("HomeRun");
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
    }

    IEnumerator coPlayThunderShotSkillEffect(Vector3 dir, float waitTime){
        const int maxDistance = 50;
        const int width = 1;
        Debug.DrawRay(this.transform.position, dir * maxDistance, Color.blue, 2f);

        RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, Vector3.one * width, dir, Quaternion.identity, maxDistance);
        Array.ForEach(hits, hit => {
            if(hit.transform.tag == "NormalBlock"){
                em.createEffectCriticalText(hit.transform, pl.dmg.getValue() * 2);
                hit.transform.gameObject.GetComponent<Block_Prefab>().decreaseHp(pl.dmg.getValue() * 2);
            }
        });
        em.createEffectThunderShot(this.gameObject.transform, pl.arrowAxisAnchor.transform.rotation);
        gm.activeSkillBtnList[0].init(pl.batEffectTf);
        
        //Before go up NextStage Wait for Second
        yield return new WaitForSeconds(waitTime);
        Debug.Log("coPlayThunderShotSkillEffect:: onDestroyBall");
        onDestroyMe();
    }

    void OnDrawGizmos(){
        // Explosion Skill Range Preview
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, pl.explosion.getValue().range);

        // ThunderShot Skill Range Preview
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireCube(this.transform.position, Vector3.one * 0.5f);
    }
}
