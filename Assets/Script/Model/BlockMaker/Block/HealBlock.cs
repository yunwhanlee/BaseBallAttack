using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBlock : Block_Prefab
{
    [Header("【子】HEAL STATUS")][Header("__________________________")]
    [SerializeField] bool isHeal;   public bool IsHeal {get => isHeal; set => isHeal = value;}
    [SerializeField] float healRadius = 1.5f;   public float HealRadius {get => healRadius; set => healRadius = value;}

    new void Update(){
        base.Update();
        //* Heal Block
        if(kind == BlockMaker.KIND.Heal){
            if(IsHeal){
                IsHeal = false;
                //Sphere Collider
                RaycastHit[] rayHits = Physics.SphereCastAll(this.gameObject.transform.position, HealRadius, Vector3.up, 0);
                foreach(var hit in rayHits){
                    var hitBlock = hit.transform.GetComponent<Block_Prefab>();
                    if(hit.transform.name.Contains(DM.NAME.Block.ToString())
                        && hitBlock.kind != BlockMaker.KIND.TreasureChest
                        && hitBlock.kind != BlockMaker.KIND.Heal){
                        int v = (int)(hitBlock.Hp * LM._.HEAL_BLOCK_INCREASE_PER);
                        int addHp = (v==0)? 1 : v;
                        hitBlock.increaseHp(addHp);
                    }
                }
            }
        }
    }
    //-----------------------------------------------------------------
#if UNITY_EDITOR
    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(this.transform.position, HealRadius);
    }
#endif
}
