using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibalArea : MonoBehaviour
{
    
    GameManager gm;

    void Start() {
        gm = DM.ins.gm;
    }

    private void OnTriggerExit(Collider col) {
        //* (BUG-82) ボールがスピードが早くて、床を通り抜けて、スピードが止まらないバグあるため、VisibleArea生成、抜けたら非活性化する処理。
        if(col.CompareTag(DM.NAME.Ball.ToString())){
            Debug.Log($"<color=red>VisibalArea:: OnTriggerExit:: col= {col.name}, pos= {col.transform.position}</color>");
            col.GetComponent<Ball_Prefab>().checkDestroyObjName();
        }
    }
}
