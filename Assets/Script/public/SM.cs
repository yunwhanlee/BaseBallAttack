using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM : MonoBehaviour
{
    public enum SFX {
        BtnClick,

    }

    static public SM ins;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip btnClickSFX;

    void Awake() => singleton();

/// ---------------------------------------------------------------------------
/// FUNCTION
/// ---------------------------------------------------------------------------
    public void sfxPlay(string name){
        if(name == SFX.BtnClick.ToString()){
            audio.clip = btnClickSFX;
        }

        audio.Play();
    }
    private void singleton(){
        if(ins == null) {
            ins = this;
            DontDestroyOnLoad(ins);
        }
        else
            Destroy(this.gameObject);
    }
}
