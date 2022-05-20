using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeEffectManager : MonoBehaviour
{
    public GameObject itemBuyEF;

    public void createItemBuyEF(){
        var ins = Instantiate(itemBuyEF, itemBuyEF.transform.position, Quaternion.identity) as GameObject;
        Destroy(ins, 3);
    }
}
