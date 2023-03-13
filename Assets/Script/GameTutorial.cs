using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTutorial : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int pgIdx;
    [SerializeField] GameObject[] contents;
    
    void OnEnable() {
        StartCoroutine(coDelaytimeScale());
        contents[0].gameObject.SetActive(true);
    }

    void OnDisable() {
        Time.timeScale = 1;
        pgIdx = 0;
        for(int i=0; i<contents.Length; i++)
            contents[i].gameObject.SetActive(false);
    }

    void Start(){
        gm = DM.ins.gm;

        //* Language
        for(int i = 0; i < contents.Length; i++){
            Text txt = contents[i].GetComponentInChildren<Text>();
            if(i == 0) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content1.ToString());
            if(i == 1) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content2.ToString());
            if(i == 2) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content3.ToString());
            if(i == 3) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content4.ToString());
            if(i == 4) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content5.ToString());
            if(i == 5) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content6.ToString());
            if(i == 6) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content7.ToString());
            if(i == 7) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content8.ToString());
            if(i == 8) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content9.ToString());
            if(i == 9) txt.text = LANG.getTxt(LANG.TXT.GameTutorial_Content10.ToString());
        }
    }

    public void onClickScreenBtn(){
        SM.ins.sfxPlay(SM.SFX.BtnClick.ToString());
        pgIdx++;
        if(pgIdx == contents.Length){
            gameObject.SetActive(false);
            return;
        }   

        for(int i = 0; i < contents.Length; i++){
            contents[i].gameObject.SetActive((pgIdx == i));
        }
        
        // Camera
        gm.setActiveCam(isPlayCamOn: pgIdx == 6 || pgIdx == 7);
    }

    IEnumerator coDelaytimeScale(){
        yield return Util.delay0_5;
        SM.ins.sfxPlay(SM.SFX.PlayerPointerUp.ToString());
        yield return Util.delay1;
        Time.timeScale = 0;
    }
}