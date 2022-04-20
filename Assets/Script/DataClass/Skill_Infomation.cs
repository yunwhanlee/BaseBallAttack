using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Skill<T>{
    //value
    [SerializeField] int level;
    [SerializeField] T value;
    [SerializeField] T unit;

    //defualt
    public Skill(int level, T value, T unit){
        this.level = level;
        this.value = value;
        this.unit = unit;
    }

    //method
    public int getCurLv() => level;
    public T getValue() => value;
    public T getUnit() => unit;
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
    //value
    [SerializeField] private string name;
    [SerializeField] private Sprite uiSprite;
    [SerializeField] private GameObject batEfPref;
    [SerializeField] private GameObject shotEfPref;
    [SerializeField] private GameObject explosionEfPref;
    [SerializeField] private GameObject castEfPref;

    //contructor
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

    //get set
    public string Name {get=> name;} 
    public Sprite UISprite {get=> uiSprite;}
    public GameObject ShotEfPref {get=> shotEfPref;}
    public GameObject BatEfPref {get=> batEfPref;}
    public GameObject ExplosionEfPref {get=> explosionEfPref;}
    public GameObject CastEfPref {get=> castEfPref;}

    //method
}


[System.Serializable]
public class ActiveSkillBtnUI{
    //value
    [SerializeField] private float unit; //Decrease Fill Amount Unit
    [SerializeField] private string name;
    [SerializeField] private bool trigger;
    [SerializeField] private Image panel;
    [SerializeField] private Image img;
    [SerializeField] private Image grayBG;
    [SerializeField] private Image selectFence;
    [SerializeField] private Material activeEFMt;

    //contructor
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

    //get set
    public float Unit {get=> unit; set=> unit=value;}
    public string Name {get=> name; set=> name=value;}
    public bool Trigger {get=> trigger; set=> trigger=value;}
    public Image Panel {get=> panel; set=> panel=value;}
    public Image Img {get=> img; set=> img=value;}
    public Image GrayBG {get=> grayBG; set=> grayBG=value;}
    public Image SelectFence {get=> selectFence; set=> selectFence=value;}

    //method
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
