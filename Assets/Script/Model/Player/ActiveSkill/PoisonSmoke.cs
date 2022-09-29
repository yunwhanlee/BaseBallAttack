using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonSmoke : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int keepStageSpan = 2;     public int KeepStageSpan { get => keepStageSpan; set => keepStageSpan = value;}
    [SerializeField] Text KeepCntTxt;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        KeepStageSpan += gm.stage;
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    void Update()
    {
        int cnt = KeepStageSpan - gm.stage + 1;
        KeepCntTxt.text = cnt.ToString();

        if(gm.stage > KeepStageSpan){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.name.Contains(DM.NAME.Block.ToString())){
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}
