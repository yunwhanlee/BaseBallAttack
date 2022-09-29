using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBlock : Block_Prefab
{
    [Header("【子】HEAL STATUS")]
    [SerializeField] bool isHeal;   public bool IsHeal {get => isHeal; set => isHeal = value;}
    [SerializeField] float healRadius = 1.5f;   public float HealRadius {get => healRadius; set => healRadius = value;}
    [SerializeField][Range(0, 1)] float healValPer = 0.15f;   public float HealValPer {get => healValPer; set => healValPer = value;}


    void Update(){
        base.Update();
        //* Heal Block
        if(kind == BlockMaker.KIND.Heal){
            if(IsHeal){
                IsHeal = false;
                //Sphere Collider
                RaycastHit[] rayHits = Physics.SphereCastAll(this.gameObject.transform.position, HealRadius, Vector3.up, 0);
                foreach(var hit in rayHits){
                    var hitBlock = hit.transform.GetComponent<Block_Prefab>();
                    if(hit.transform.CompareTag(DM.TAG.Block.ToString()) && hitBlock.kind != BlockMaker.KIND.TreasureChest){
                        int v = (int)(hitBlock.Hp * healValPer);
                        int addHp = (v==0)? 2 : v;
                        hitBlock.increaseHp(addHp);
                    }
                }
            }
        }
    }
    //-----------------------------------------------------------------
    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(this.transform.position, HealRadius);
    }
}
