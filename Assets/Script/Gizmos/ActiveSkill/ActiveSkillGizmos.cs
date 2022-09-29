using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillGizmos : MonoBehaviour
{
    [SerializeField] protected Player pl;
    [SerializeField] protected SpriteRenderer sprRdr;
    [SerializeField] protected float width;
    [SerializeField] protected bool isGizmosOn = true;
    [SerializeField] protected DM.ATV type;
    protected void Start(){
        pl = GameObject.Find("Player").GetComponent<Player>();
        sprRdr = GetComponent<SpriteRenderer>();

        switch(type){
            case DM.ATV.Thunder : pl.ThunderCastWidth = width; break;
            case DM.ATV.FireBall : pl.FireBallCastWidth = width;   break;
            case DM.ATV.PoisonSmoke : pl.PoisonSmokeCastWidth = width;   break;
            case DM.ATV.ColorBall :    break;
            case DM.ATV.IceWave :    break;
        }
    }


    protected void OnTriggerEnter(Collider col){
        if(col.gameObject.name.Contains(DM.NAME.Block.ToString())){
            Debug.Log("ActiveSkillGizmos:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(true);
        }
    }
    protected void OnTriggerExit(Collider col){
        if(col.gameObject.name.Contains(DM.NAME.Block.ToString())){
            Debug.Log("ActiveSkillGizmos:: OnTriggerExit:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().setEnabledSpriteGlowEF(false);
        }
    }

    protected virtual void OnDrawGizmos(){
        //? 子クラスで、再定義。
    }
}
