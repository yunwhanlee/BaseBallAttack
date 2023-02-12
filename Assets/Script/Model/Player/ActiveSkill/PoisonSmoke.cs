using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonSmoke : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int keepDuration = 2;     public int KeepDuration { get => keepDuration; set => keepDuration = value;}
    [SerializeField] Text KeepCntTxt;
    void Start(){
        gm = DM.ins.gm;
        KeepDuration += gm.stage;
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    void Update(){
        int cnt = KeepDuration - gm.stage + 1;
        KeepCntTxt.text = cnt.ToString();

        if(gm.stage > KeepDuration){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col){
        if(Util._.isColBlockOrObstacle(col)
            && !col.name.Contains(DM.NAME.Ball.ToString())){
            Debug.Log("PoisonSmoke:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}
