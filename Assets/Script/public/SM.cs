using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM : MonoBehaviour
{
    public enum SFX {
        BtnClick, 
        PurchaseSuccess,
        PurchaseFail,
        Upgrade,
        PlayBtn,
    }

    static public SM ins;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip BtnClickSFX;
    [SerializeField] AudioClip PurchaseSuccessSFX;
    [SerializeField] AudioClip PurchaseFailSFX;
    [SerializeField] AudioClip UpgradeSFX;
    [SerializeField] AudioClip PlayBtnSFX;

    void Awake() => singleton();

/// ---------------------------------------------------------------------------
/// FUNCTION
/// ---------------------------------------------------------------------------
    public void sfxPlay(string name){
        //* 追加
        if(name == SFX.BtnClick.ToString())
            audio.clip = BtnClickSFX;
        else if(name == SFX.PurchaseSuccess.ToString())
            audio.clip = PurchaseSuccessSFX;
        else if(name == SFX.PurchaseFail.ToString())
            audio.clip = PurchaseFailSFX;
        else if(name == SFX.Upgrade.ToString())
            audio.clip = UpgradeSFX;
        else if(name == SFX.PlayBtn.ToString())
            audio.clip = PlayBtnSFX;
            
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
