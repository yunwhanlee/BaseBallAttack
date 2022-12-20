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
        else if(col.transform.CompareTag("Ball")){
            Debug.Log("DropBox::OnCollisionEnter:: col= Ball, this.name= " + this.name);
            if(this.gameObject.name == ObjectPool.DIC.DropBoxQuestionPf.ToString()){
                gm.em.createDropBoxQuestionEF(this.transform);
                gm.em.createDropBoxQuestionMoneyEF(this.transform);
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxShieldPf.ToString()){
                gm.em.createDropBoxShieldEF(this.transform);
                gm.em.createDropBoxShieldBarrierEF(gm.pl.modelMovingTf);
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxSpeedPf.ToString()){
                gm.em.createDropBoxSpeedEF(this.transform);
                gm.em.createDropBoxSpeedTrailEF(col.transform);
            }
            else if(this.gameObject.name == ObjectPool.DIC.DropBoxStarPf.ToString()){
                gm.em.createDropBoxStarEF(this.transform);
                gm.em.createDropBoxStarTrailEF(col.transform);
            }

            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }
}
