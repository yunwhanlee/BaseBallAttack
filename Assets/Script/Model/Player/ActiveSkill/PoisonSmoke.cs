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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        if(col.name.Contains(DM.NAME.Block.ToString())){
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}
