using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Util _;
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
}
