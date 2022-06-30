using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillGizmos : MonoBehaviour
{
    [SerializeField] protected Player pl;
    [SerializeField] protected float width;
    [SerializeField] protected bool isGizmosOn = true;
    [SerializeField] protected DM.ATVSKILL type;
    protected void Start(){
        pl = GameObject.Find("Player").GetComponent<Player>();
        switch(type){
            case DM.ATVSKILL.Thunder : pl.ThunderCastWidth = width; break;
            case DM.ATVSKILL.FireBall : pl.FireBallCastWidth = width;   break;
            case DM.ATVSKILL.ColorBall :    break;
        }
    }

    protected void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("ActiveSkillGizmos:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(true);
        }
    }
    protected void OnTriggerExit(Collider col){
        if(col.gameObject.CompareTag("NormalBlock")){
            Debug.Log("ActiveSkillGizmos:: OnTriggerExit:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(false);
        }
    }

    protected virtual void OnDrawGizmos(){
        //? 子クラスで、再定義。
    }
}
