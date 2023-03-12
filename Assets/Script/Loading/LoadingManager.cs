using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;
using Random = UnityEngine.Random;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] bool isAllowNextSceneActive;
    [SerializeField] Transform howToPlayPanel;

    [FormerlySerializedAs("loadingBar")]
    [SerializeField] Slider loadingBar;

    [FormerlySerializedAs("loadTxt")]
    [SerializeField]  Text loadTxt;

    [SerializeField]  RectTransform loadingIcon;
    float rotSpeed = 100;
    
    void Start(){
        //* 初期化
        DM.ins.transform.position = new Vector3(-100, -100, -100);
        // var tuto = DM.ins.displayTutorialUI();
        // tuto.transform.Find("ScreenDim").gameObject.SetActive(false);
        // tuto.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        // Debug.Log($"LoadingManager:: Start:: tuto.gameObject.name= " + tuto.gameObject.name);
        // tuto.PageIdx = Random.Range(0, tuto.ContentArr.Length);
        //* 非同期 処理
        StartCoroutine(coLoadScene(DM.SCENE.Play.ToString()));
    }
    IEnumerator coLoadScene(string sceneName){
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        //* 読込みが完了出来たら、自動で次のシーンに進む。
        operation.allowSceneActivation = isAllowNextSceneActive;

        while(!operation.isDone){
            yield return null;
            if(loadingBar.value < 1f){
                //* MoveTowardsはLerpと同じだ。しかし、速度がmaxDeltaを超過しないことを保障する。
                loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1f, Time.deltaTime); 
                loadingIcon.Rotate(0, 0, rotSpeed * Time.deltaTime);
                
            }
            else{
                loadTxt.text = "LOADING FINISH!";
            }
        }
    }
}
