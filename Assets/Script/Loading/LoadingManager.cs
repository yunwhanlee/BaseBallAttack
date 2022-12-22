using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Transform howToPlayPanel;
    // Start is called before the first frame update
    void Start(){
        var tuto = DM.ins.displayTutorialUI();
        tuto.transform.Find("ScreenDim").gameObject.SetActive(false);

        // int randIdx = Random.Range(0, tuto.ContentArr.Length);
        // tuto.ContentArr[randIdx].SetActive(true);
    }

    // Update is called once per frame
    void Update(){
        
    }
}
