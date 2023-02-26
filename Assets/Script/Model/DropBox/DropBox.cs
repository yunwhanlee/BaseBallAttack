using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour{ //* Create By BlockMaker.cs
    GameManager gm;
    [SerializeField] int aliveSpan;     public int AliveSpan { get => aliveSpan; set => aliveSpan = value;}
    public const int MIN_X = -5, MAX_X = 5;
    public const int MIN_Z = -12,  MAX_Z = -6;

    void OnEnable(){
        aliveSpan = LM._.DROPBOX_ALIVE_SPAN;
    }

    void Awake(){
        gm = DM.ins.gm;
    }

    void Update(){
        if(aliveSpan <= 0)
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropBoxGroup));
    }

    public Vector3 setRandPos(){
        int rx = Random.Range(DropBox.MIN_X, DropBox.MAX_X+1);
        int rz = Random.Range(DropBox.MIN_Z, DropBox.MAX_Z+1);
        Vector3 randPos = new Vector3(rx, 1, rz);
        return randPos;
    }

    void OnTriggerEnter(Collider col){
        if(Util._.isColBlockOrObstacle(col.transform.GetComponent<Collider>())){
            Debug.Log("DropBox::OnCollisionEnter:: col= " + col);
            // StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropBoxGroup));
            this.transform.position = setRandPos();
        }
        else if(col.transform.CompareTag(DM.NAME.Ball.ToString())){
            //* (BUG-17) ボールがベットから打たれる前に(BallShooterから投げる)時にはDropBoxと当たり判定処理しない。
            var ballRigid = col.GetComponent<Rigidbody>();
            if(ballRigid.useGravity == false) return;
            
            Debug.Log("DropBox::OnCollisionEnter:: col= Ball, this.name= " + this.name);
            
            if(this.gameObject.name == ObjectPool.DIC.DropBoxQuestionPf.ToString()){
                int cnt = 0;
                int rand = Random.Range(0, 5);
                switch(rand){
                    case 0: cnt = 10;   break;
                    case 1: cnt = 15;   break;
                    case 2: cnt = 20;   break;
                    case 3: cnt = 25;   break;
                    case 4: cnt = 30;   break;
                }
                Debug.Log("DropBox::OnCollisionEnter:: DropBoxQuestionPf:: max= " + cnt);
                for(int i=0; i<cnt; i++)
                    gm.bm.createCoinIconPf(this.transform, i, cnt);

                SM.ins.sfxPlay(SM.SFX.DropBoxCoinPick.ToString());
                gm.em.createDropBoxQuestionEF(this.transform, LM._.DROPBOX_COIN_VALUE * cnt);
                gm.em.createDropBoxQuestionMoneyEF(this.transform);

                //* 処理
                // gm.coin += cnt * LM._.DROPBOX_COIN_VALUE;
                gm.rewardItemCoin += cnt * LM._.DROPBOX_COIN_VALUE;
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxShieldPf.ToString()){
                SM.ins.sfxPlay(SM.SFX.DropBoxPick.ToString());
                //* (BUG-93) DropBox Sheildを習得してもうpl.IsBarrierがTrueであれば、もう一ど習得しても処理しない。
                if(!gm.pl.IsBarrier){
                    gm.em.createDropBoxShieldEF(this.transform);
                    gm.em.createDropBoxShieldBarrierEF(gm.pl.modelMovingTf);
                    //* 処理
                    gm.pl.IsBarrier = true;
                }
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxSpeedPf.ToString()){
                SM.ins.sfxPlay(SM.SFX.DropBoxPick.ToString());
                gm.em.createDropBoxSpeedEF(this.transform);
                for(int i=0; i<gm.ballGroup.childCount; i++){
                    gm.em.createDropBoxSpeedTrailEF(gm.ballGroup.GetChild(i));
                    //* 処理
                    gm.ballGroup.GetChild(i).GetComponent<Rigidbody>().velocity *= 2;
                }
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxStarPf.ToString()){
                SM.ins.sfxPlay(SM.SFX.DropBoxPick.ToString());
                gm.em.createDropBoxStarEF(this.transform);
                for(int i=0; i<gm.ballGroup.childCount; i++){
                    if(gm.ballGroup.GetChild(i).CompareTag(DM.NAME.Ball.ToString())){
                        gm.em.createDropBoxStarTrailEF(gm.ballGroup.GetChild(i));
                        //* 処理
                        gm.ballGroup.GetChild(i).GetComponent<Ball_Prefab>().IsDmgX2 = true;
                    }
                }
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxMagicPf.ToString()){
                SM.ins.sfxPlay(SM.SFX.DropBoxPick.ToString());
                gm.em.createDropBoxMagicEF(this.transform);
                //*処理
                gm.activeSkillBtnList.ForEach((atvSkillBtn) => atvSkillBtn.CollDownImg.fillAmount = 0);
            }
            

            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropBoxGroup));
        }
    }
}
