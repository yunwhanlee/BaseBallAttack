using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Skill<T>{
    //*value                     //*get set
    [SerializeField] int level; public int Level {get=>level;}
    [SerializeField] T value;   public T Value {get=>value;}
    [SerializeField] T unit;    public T Unit {get=>unit;}

    //*constructor
    public Skill(int level, T value, T unit){
        this.level = level;
        this.value = value;
        this.unit = unit;
    }

    //*method
    public void setLvUp(T value){
        level++;
        this.value = value;
    }
}

[System.Serializable]
public struct Explosion{
    public float per, range;
    public Explosion(float per = 0, float range = 0.75f){
        this.per = per;
        this.range = range;
    }
}

//-------------------------------------------------
//-------------------------------------------------
[System.Serializable]
public class ActiveSkill{
    //*value                        //*get set
    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] Sprite uiSprite;    public Sprite UISprite {get=> uiSprite;}
    [SerializeField] GameObject batEfPref;    public GameObject ShotEfPref {get=> shotEfPref;}
    [SerializeField] GameObject shotEfPref;    public GameObject BatEfPref {get=> batEfPref;}
    [SerializeField] GameObject explosionEfPref;    public GameObject ExplosionEfPref {get=> explosionEfPref;}
    [SerializeField] GameObject castEfPref;    public GameObject CastEfPref {get=> castEfPref;}

    //*constructor
    public ActiveSkill(string name, ActiveSkill[] activeSkillTable){//Sprite uiSprite, GameObject batEfPref, GameObject shotEfPref, GameObject explosionEfPref){
        Array.ForEach(activeSkillTable, skillList=>{
            if(name == skillList.Name){//* Regist Select Active Skill
                this.name = skillList.Name;
                this.uiSprite = skillList.uiSprite;
                this.batEfPref = skillList.batEfPref;
                this.shotEfPref = skillList.shotEfPref;
                this.explosionEfPref = skillList.explosionEfPref;
                this.castEfPref = skillList.castEfPref;
            }
        });
    }
    //*method
}


[System.Serializable]
public class ActiveSkillBtnUI{
    //*value                        //*get set

    [SerializeField] float unit;    public float Unit {get=> unit; set=> unit=value;}// Decrease Fill Amount Unit
    [SerializeField] string name;    public string Name {get=> name; set=> name=value;}
    [SerializeField] bool trigger;    public bool Trigger {get=> trigger; set=> trigger=value;}
    [SerializeField] Image panel;    public Image Panel {get=> panel; set=> panel=value;}
    [SerializeField] Image img;    public Image Img {get=> img; set=> img=value;}
    [SerializeField] Image grayBG;    public Image GrayBG {get=> grayBG; set=> grayBG=value;}
    [SerializeField] Image selectFence;    public Image SelectFence {get=> selectFence; set=> selectFence=value;}
    [SerializeField] Material activeEFMt;   public Material ActiveEFMt {get=> activeEFMt;}

    //*contructor
    public ActiveSkillBtnUI(float unit, string name, Button skillBtn, Sprite sprite, Material activeSkillEffectMt){
        this.unit = unit;
        this.name = name;
        panel = skillBtn.GetComponent<Image>();
        this.img = skillBtn.transform.GetChild(0).GetComponent<Image>();
        this.img.sprite = sprite;
        grayBG = skillBtn.transform.GetChild(1).GetComponent<Image>();
        selectFence = skillBtn.transform.GetChild(2).GetComponent<Image>();
        activeEFMt = activeSkillEffectMt;
    }

    //*method
    public void init(Player pl){
        Trigger = false;
        Panel.material = null;
        GrayBG.fillAmount = 1;
        selectFence.gameObject.SetActive(false);
        pl.BatEffectTf.gameObject.SetActive(false);
        foreach(Transform child in pl.castEFArrowTf) GameObject.Destroy(child.gameObject);
        foreach(Transform child in pl.castEFBallPreviewTf) GameObject.Destroy(child.gameObject);
    }
    public void onTriggerActive(int selectIdx, Player pl, EffectManager em){
        if(GrayBG.fillAmount == 0){
            Trigger = !Trigger;
            selectFence.gameObject.SetActive(Trigger);
            //* Bat Effect
            pl.BatEffectTf.gameObject.SetActive(Trigger);
            foreach(Transform child in pl.BatEffectTf){
                int childIdx = child.GetSiblingIndex();
                if(selectIdx == childIdx)
                    child.gameObject.SetActive(true);
                else 
                    child.gameObject.SetActive(false);
            }

            //* Cast Effect
            Debug.Log("ActiveSkillBtnUI:: this.name=" + this.name);
            if(Trigger){
                Transform parentTf = null;
                switch(this.name){
                    case "Thunder":     parentTf = pl.castEFArrowTf;        break;
                    case "FireBall":    parentTf = pl.castEFBallPreviewTf;  break;
                }
                em.createActiveSkillCastEF(selectIdx, parentTf);
            }
            else{
                foreach(Transform child in pl.castEFArrowTf) GameObject.Destroy(child.gameObject);
                foreach(Transform child in pl.castEFBallPreviewTf) GameObject.Destroy(child.gameObject);
            }
        }
    }
    public void decreaseFillAmount(){
        GrayBG.fillAmount -= unit;
    }
    public void setActiveSkillEF(){
        Panel.material = (GrayBG.fillAmount == 0)? activeEFMt : null;
    }

}
