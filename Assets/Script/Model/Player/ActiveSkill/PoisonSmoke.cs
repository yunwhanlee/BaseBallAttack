using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSmoke : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int keepStageSpan = 2;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        keepStageSpan += gm.stage;
    }

    void Update()
    {
        if(gm.stage > keepStageSpan){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("NormalBlock")){
            col.GetComponent<Block_Prefab>().IsDotDmg = true;
        }
    }
}
