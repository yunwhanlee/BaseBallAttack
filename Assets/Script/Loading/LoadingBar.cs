using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadingBar : MonoBehaviour
{
    [FormerlySerializedAs("loadingBar")] public Slider loadingBar;
    [FormerlySerializedAs("loadTxt")] public Text loadTxt;

    void Start() {
        
        StartCoroutine(loadScene(DM.SCENE.Play.ToString()));
    }
    //非同期
    IEnumerator loadScene(string sceneName){
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = true;

        while(!operation.isDone){
            yield return null;
            if(loadingBar.value < 1f){
                //* MoveTowardsはLerpと同じだ。しかし、速度がmaxDeltaを超過しないことを保障する。
                loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1f, Time.deltaTime); 
            }
            else{
                loadTxt.text = "LOADING FINISH!";
            }
        }
    }
}
