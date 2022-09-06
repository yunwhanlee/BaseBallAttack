using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonSmoke : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int keepStageSpan = 2;
    [SerializeField] Text KeepCntTxt;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        keepStageSpan += gm.stage;
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    void Update()
    {
        int cnt = keepStageSpan - gm.stage + 1;
        KeepCntTxt.text = cnt.ToString();

        if(gm.stage > keepStageSpan){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag(BlockMaker.NORMAL_BLOCK)){
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}
