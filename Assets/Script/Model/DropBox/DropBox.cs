using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour{ //* Create By BlockMaker.cs
    GameManager gm;

    public const int MIN_X = -5, MAX_X = 5;
    public const int MIN_Z = -12,  MAX_Z = -6;

    void Awake(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        
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
            this.transform.position = setRandPos();
        }
        else if(col.transform.CompareTag(DM.NAME.Ball.ToString())){
            Debug.Log("DropBox::OnCollisionEnter:: col= Ball, this.name= " + this.name);
            if(this.gameObject.name == ObjectPool.DIC.DropBoxQuestionPf.ToString()){
                gm.em.createDropBoxQuestionEF(this.transform);
                gm.em.createDropBoxQuestionMoneyEF(this.transform);

                //* 処理
                int max = 0;
                int rand = Random.Range(0, 5);
                switch(rand){
                    case 0: max = 10;   break;
                    case 1: max = 20;   break;
                    case 2: max = 30;   break;
                    case 3: max = 40;   break;
                    case 4: max = 50;   break;
                }
                Debug.Log("DropBox::OnCollisionEnter:: DropBoxQuestionPf:: max= " + max);
                for(int i=0; i<max; i++)
                    gm.bm.createCoinIconPf(this.transform, i, max);
                
                gm.coin += max * 50;
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxShieldPf.ToString()){
                gm.em.createDropBoxShieldEF(this.transform);
                gm.em.createDropBoxShieldBarrierEF(gm.pl.modelMovingTf);
                //* 処理
                gm.pl.IsBarrier = true;
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxSpeedPf.ToString()){
                gm.em.createDropBoxSpeedEF(this.transform);
                for(int i=0; i<gm.ballGroup.childCount; i++){
                    gm.em.createDropBoxSpeedTrailEF(gm.ballGroup.GetChild(i));
                    //* 処理
                    gm.ballGroup.GetChild(i).GetComponent<Rigidbody>().velocity *= 2;
                }
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxStarPf.ToString()){
                gm.em.createDropBoxStarEF(this.transform);
                for(int i=0; i<gm.ballGroup.childCount; i++){
                    if(gm.ballGroup.GetChild(i).CompareTag(DM.NAME.Ball.ToString())){
                        gm.em.createDropBoxStarTrailEF(gm.ballGroup.GetChild(i));
                        //* 処理
                        gm.ballGroup.GetChild(i).GetComponent<Ball_Prefab>().IsDmgX2 = true;
                    }
                }
            }

            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }
}
