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
        if(_ == null) _ = this;
        else if(_ != null) DontDestroyOnLoad(this.gameObject);
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
}
