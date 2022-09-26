using System.Collections;
using UnityEngine;
using System;


[System.Serializable]
public class MyMesh
{
    public SkinnedMeshRenderer[] boss;
    public MeshRenderer[] block;
    private Block_Prefab me;

    //*Contructor
    public MyMesh(Block_Prefab me){
        this.me = me;
        switch(me.kind){
            case BlockMaker.BLOCK.Boss:
                boss = me.GetComponentsInChildren<SkinnedMeshRenderer>();
                break;
            default:
                block = me.GetComponentsInChildren<MeshRenderer>();
                break;
        }
    }

    public Material[] getOriginalMts(){
        Material[] originMts;
        switch(me.kind){
            case BlockMaker.BLOCK.Boss:
                Array.ForEach(boss, meshRd=> meshRd.material = UnityEngine.Object.Instantiate(meshRd.material));
                originMts = new Material[boss.Length];
                for(int i=0; i<boss.Length;i++)
                    originMts[i] = boss[i].material; //* オリジナルMt 保存。(材質X、色X ➡ TreasureChest用)
                break;
            default:
                Array.ForEach(block, meshRd=> meshRd.material = UnityEngine.Object.Instantiate(meshRd.material));
                originMts = new Material[block.Length];
                for(int i=0; i<block.Length;i++)
                    originMts[i] = block[i].material; //* オリジナルMt 保存。(材質X、色X ➡ TreasureChest用)
                break;
        }
        return originMts;
    }

    public void setWhiteHitEF(){
        // Debug.Log($"setWhiteHitEF():: me.kind= {me.kind}");
        switch(me.kind){
            case BlockMaker.BLOCK.Boss:
                me.callStartCoWhiteHitEF(boss);
                break;
            default:
                me.callStartCoWhiteHitEF(block);
                break;
        }
    }

    public IEnumerator coWhiteHitEffect(MeshRenderer[] msRds){ //* 体力が減ったら、一瞬間白くなって戻すEFFECT
        Debug.Log("WHITE HIT!");
        Array.ForEach(msRds, ms => ms.material = me.whiteHitMt);

        yield return new WaitForSeconds(0.05f);

        for(int i=0; i<msRds.Length; i++)
            msRds[i].material = me.originMts[i];//* (BUG) WaitForSeconds間にまた衝突が発生したら、白くなる。
    }
    public IEnumerator coWhiteHitEffect(SkinnedMeshRenderer[] msRds){ //* 体力が減ったら、一瞬間白くなって戻すEFFECT
        Debug.Log("WHITE HIT!");
        Array.ForEach(msRds, ms => ms.material = me.whiteHitMt);

        yield return new WaitForSeconds(0.05f);

        for(int i=0; i<msRds.Length; i++)
            msRds[i].material = me.originMts[i];//* (BUG) WaitForSeconds間にまた衝突が発生したら、白くなる。
    }
}
