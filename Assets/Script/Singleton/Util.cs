using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour
{
    public static Util _;

    [Header("NOTICE MESSAGE")]
    public int noticeMsgDisplayCnt = 1;
    public Text noticeMessageTxtPref;
    public Transform mainPanelTf;

    void Awake() => singleton();

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    public float getAnimPlayTime(int index, Animator anim){
        // Array.ForEach(clips, clip=> Debug.Log($"clip= {clip.name}, clip.length= {clip.length}"));
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        float sec = clips[index].length;
        return sec;
    }

    public Transform getCharaRightArmPath(Transform charaTf){
        return  charaTf.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find(DM.NAME.RightArm.ToString());
    }

    public void displayNoticeMsgDialog(string msg){
        noticeMessageTxtPref.text = msg;
        var ins = Instantiate(noticeMessageTxtPref, mainPanelTf.transform.position, Quaternion.identity, mainPanelTf);
        ins.rectTransform.localPosition = new Vector3(0,-900,-400);
        Destroy(ins.gameObject, noticeMsgDisplayCnt);
    }

    public int calcEquivalentSequence(int n, int device){ //* 等比数列
        int res;
        bool isOdd = n % 2 != 0;
        if(isOdd){ // 奇数
            res = n + (n + (n * ((n - 1) / device)));
        }
        else{// 偶数
            res = n + (n + (n * ((n - 1) / device) + (n / device)));
        }
        int extraVal = Random.Range(-1, 1); // Extra
        return res + extraVal;
    }

    public void calcFibonicciSequence(float unit, float fibRatio){
        Debug.Log($"calcFibonicciSequence(unit={unit}, fibRatio={fibRatio})::");
        float res;
        float v1 = 1 * unit;
        float v2 = fibRatio * unit;
        for(int i=0; i<10; i++){
            if(i == 0) {
                res = v1;
            }else{
                res = v1 + v2;
                v1 = v2;
                v2 = res;
            }
            Debug.Log($"fibo [{i}]: " + res);
        }
    }
}
