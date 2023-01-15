using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SM : MonoBehaviour
{
    public enum SFX {
        //* UI
        BtnClick, PlayBtn,
        PurchaseSuccess, PurchaseFail, Upgrade, RouletteReward,
        Warning, LevelUpPanel, Victory, Defeat, Revive, 
        HomeRun, HomeRunCameraAnim,
        CountDown, CountDownShoot, CountDownStrike,
        //* PLAY
        PlayerLevelUp,
        Swing, SwingHit, HitBlock, DestroyBlock

    }

    static public SM ins;
    [Header("UI")][Header("__________________________")]
    [SerializeField] AudioSource BtnClickSFX;
    [SerializeField] AudioSource PurchaseSuccessSFX;
    [SerializeField] AudioSource PurchaseFailSFX;
    [SerializeField] AudioSource UpgradeSFX;
    [SerializeField] AudioSource PlayBtnSFX;
    [SerializeField] AudioSource WarningSFX;
    [SerializeField] AudioSource LevelUpPanelSFX;
    [SerializeField] AudioSource VictorySFX;
    [SerializeField] AudioSource DefeatSFX;
    [SerializeField] AudioSource ReviveSFX;
    [SerializeField] AudioSource RouletteRewardSFX;
    [SerializeField] AudioSource HomeRunSFX;
    [SerializeField] AudioSource HomeRunCameraAnimSFX;
    [SerializeField] AudioSource CountDownSFX;
    [SerializeField] AudioSource CountDownShootSFX;
    [SerializeField] AudioSource CountDownStrikeSFX;
    
    [Header("PLAY")][Header("__________________________")]
    [SerializeField] AudioSource PlayerLevelUpSFX;
    [SerializeField] AudioSource SwingSFX;
    [SerializeField] AudioSource[] SwingHitSFXs;
    [SerializeField] AudioSource HitBlockSFX;
    [SerializeField] AudioSource DestroyBlockSFX;

    

    void Awake() => singleton();

/// ---------------------------------------------------------------------------
/// FUNCTION
/// ---------------------------------------------------------------------------
    public void sfxPlay(string name){
        //* UI
        if(name == SFX.BtnClick.ToString())
            // audio.clip = BtnClickSFX;
            BtnClickSFX.Play();

        else if(name == SFX.PurchaseSuccess.ToString())
            PurchaseSuccessSFX.Play();
        else if(name == SFX.PurchaseFail.ToString())
            PurchaseFailSFX.Play();
        else if(name == SFX.Upgrade.ToString())
            UpgradeSFX.Play();
        else if(name == SFX.PlayBtn.ToString())
            PlayBtnSFX.Play();
        else if(name == SFX.Warning.ToString())
            WarningSFX.Play();
        else if(name == SFX.LevelUpPanel.ToString())
            LevelUpPanelSFX.Play();
        else if(name == SFX.Victory.ToString())
            VictorySFX.Play();
        else if(name == SFX.Defeat.ToString())
            DefeatSFX.Play();
        else if(name == SFX.Revive.ToString())
            ReviveSFX.Play();
        else if(name == SFX.RouletteReward.ToString())
            RouletteRewardSFX.Play();
        else if(name == SFX.HomeRun.ToString())
            HomeRunSFX.Play();
        else if(name == SFX.HomeRunCameraAnim.ToString())
            HomeRunCameraAnimSFX.Play();
        else if(name == SFX.CountDown.ToString())
            CountDownSFX.Play();
        else if(name == SFX.CountDownShoot.ToString())
            CountDownShootSFX.Play();
        else if(name == SFX.CountDownStrike.ToString())
            CountDownStrikeSFX.Play();
        //* PLAY
        else if(name == SFX.PlayerLevelUp.ToString())
            PlayerLevelUpSFX.Play();
        else if(name == SFX.Swing.ToString())
            SwingSFX.Play();    
        else if(name == SFX.SwingHit.ToString()){
            int rand = Random.Range(0, SwingHitSFXs.Length);
            SwingHitSFXs[rand].Play();
        }
        else if(name == SFX.HitBlock.ToString())
            HitBlockSFX.Play();
        else if(name == SFX.DestroyBlock.ToString())
            DestroyBlockSFX.Play();
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
