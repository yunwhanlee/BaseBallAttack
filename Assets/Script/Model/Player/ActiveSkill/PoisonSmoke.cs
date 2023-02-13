using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonSmoke : MonoBehaviour
{
    GameManager gm;
    int cnt;
    [SerializeField] int duration = 2;     public int Duration { get => duration; set => duration = value;}
    [SerializeField] Text durationTxt;
    void Start(){
        gm = DM.ins.gm;
        Debug.Log($"PoisonSmoke:: gm.bossLimitCnt= {gm.bossLimitCnt}, duration= {duration}");
        //* (BUG-62) POISONSMOKEスキルがボース戦で、カウントが下がらない。
        cnt = (gm.bossLimitCnt > 0)? gm.bossLimitCnt - duration : gm.stage + duration;
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    void Update(){
        //* (BUG-62) POISONSMOKEスキルがボース戦で、カウントが下がらない。
        durationTxt.text = ((gm.bossLimitCnt > 0)? gm.bossLimitCnt - cnt : cnt - gm.stage).ToString();
        if(gm.bossLimitCnt > 0){
            if(gm.bossLimitCnt < cnt) Destroy(this.gameObject);
        }
        else{
            if(cnt < gm.stage) Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col){
        if(Util._.isColBlockOrObstacle(col) && !col.name.Contains(DM.NAME.Ball.ToString())){
            Debug.Log("PoisonSmoke:: OnTriggerEnter:: col.name= " + col.name);
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}