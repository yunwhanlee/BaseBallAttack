using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                float power = (distance <= 0.1f) ? 10 //-> BEST HIT (HOMERUH!)
                : (distance <= 0.25f) ? 7
                : (distance <= 0.5f) ? 5
                : (distance <= 0.85f)? 4
                : (distance <= 1.125f)? 3
                : 1.5f; //-> WORST HIT (distance <= 1.5f)
                

                //HomeRun
                if(power >= 5){
                    StartCoroutine(coPlayHomeRunAnimation());
                }

                rigid.velocity = Vector3.zero;
                float force = speed * power * pl.getSpeedPer();
                rigid.AddForce(dir * force, ForceMode.Impulse);
                Debug.Log(
                    "HIT Ball! <color=yellow>distance=" + distance.ToString("N2") + "</color>"
                    + ", <color=red>power=" + power + ", Rank: " + ((power==10)? "A" : (power==7)? "B" : (power==5)? "C" : (power==4)? "D" : (power==3)? "E" : "F").ToString() + "</color>"
                    + ", Force=" + force);

                //* Multi Shot
                for(int i=0; i<pl.getMultiShot();i++){
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
            pl.setDoSwing(false);
            if(gm.state == GameManager.State.WAIT){
                gm.downWall.isTrigger = false;//*下壁 物理O
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
        // Give Damage
        if(col.gameObject.tag == "NormalBlock"){
            int result = 0;
            //Immediate Kill
            int rand = Random.Range(0, 100);
            int v = Mathf.RoundToInt(pl.getImmediateKillPer() * 100); //百分率
            Debug.Log("Hit Block:: ImmediateKill:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=blue>true</color>" : "<color=red>false</color>"));
            if(rand <= v) Destroy(col.gameObject);

            //Critical x 2
            rand = Random.Range(0, 100);
            v = Mathf.RoundToInt(pl.getCriticalPer() * 100); //百分率
            Debug.Log("Hit Block:: Critical:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=blue>true</color>" : "<color=red>false</color>"));
            result = (rand <= v)? pl.getDmg() * 2 : pl.getDmg();

            //Explosion
            rand = Random.Range(0, 100);
            v = Mathf.RoundToInt(pl.getExplosion().per * 100); //百分率
            Debug.Log("Hit Block:: Explosion:: rand("+rand+") <= v("+v+") : " + ((rand <= v)? "<color=blue>true</color>" : "<color=red>false</color>"));
            if(rand <= v){
                //Effect
                em.createEffectExplosion(this.transform, pl.getExplosion().range);

                //Sphere Collider
                RaycastHit[] rayHits = Physics.SphereCastAll(this.transform.position, pl.getExplosion().range, Vector3.up, 0, LayerMask.GetMask("Block"));
                foreach(var hitObj in rayHits){
                    hitObj.transform.GetComponent<Block_Prefab>().decreaseHp(result);
                }
            }

            col.gameObject.GetComponent<Block_Prefab>().decreaseHp(result);
        }
    }

    //*---------------------------------------
    //*  関数
    //*---------------------------------------
    private void onDestroyMe(bool isStrike = false){
        if(!isStrike){
            //* ★Next Stage：ボールが消えたら次に進む。
            Debug.Log("onDestroyMe:: NEXT STAGE(Ball Is Destroyed)");
            gm.setNextStage();
            gm.setState(GameManager.State.WAIT);
            gm.downWall.isTrigger = true; //*下壁 物理X
            gm.readyBtn.gameObject.SetActive(true);
            
            blockMaker.setCreateBlockTrigger(true);
            ballShooter.setIsBallExist(false);
            pl.previewBundle.SetActive(true);

            //Level Up?
            gm.checkLevelUp();
            
        }else{
            gm.setStrike();
        }
        gm.setBallPreviewGoalRandomPos();
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

    void OnDrawGizmos(){
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, pl.getExplosion().range);
    }
}
