using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM : MonoBehaviour
{
    public enum SFX {
        //* UI
        BtnClick, 
        PurchaseSuccess,
        PurchaseFail,
        Upgrade,
        PlayBtn,
        Warning,
        LevelUpPanel,
        Victory,
        Defeat,
        Revive,
    }

    static public SM ins;
    [SerializeField] AudioSource audio;
    //* UI
    [SerializeField] AudioClip BtnClickSFX;
    [SerializeField] AudioClip PurchaseSuccessSFX;
    [SerializeField] AudioClip PurchaseFailSFX;
    [SerializeField] AudioClip UpgradeSFX;
    [SerializeField] AudioClip PlayBtnSFX;
    [SerializeField] AudioClip WarningSFX;
    [SerializeField] AudioClip LevelUpPanelSFX;
    [SerializeField] AudioClip VictorySFX;
    [SerializeField] AudioClip DefeatSFX;
    [SerializeField] AudioClip ReviveSFX;

    void Awake() => singleton();

/// ---------------------------------------------------------------------------
/// FUNCTION
/// ---------------------------------------------------------------------------
    public void sfxPlay(string name){
        //* UI
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
        else if(name == SFX.Warning.ToString())
            audio.clip = WarningSFX;
        else if(name == SFX.LevelUpPanel.ToString())
            audio.clip = LevelUpPanelSFX;
        else if(name == SFX.Victory.ToString())
            audio.clip = VictorySFX;
        else if(name == SFX.Defeat.ToString())
            audio.clip = DefeatSFX;
        else if(name == SFX.Revive.ToString())
            audio.clip = ReviveSFX;
            
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
