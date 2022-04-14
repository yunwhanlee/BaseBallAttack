using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //contructor
    public ActiveSkill(string name, Sprite uiSprite, GameObject batEfPref, GameObject shotEfPref, GameObject explosionEfPref){
        this.name = name;
        this.uiSprite = uiSprite;
        this.batEfPref = batEfPref;
        this.shotEfPref = shotEfPref;
        this.explosionEfPref = explosionEfPref;
    }

    //get set
    public string Name {get=> name;} 
    public Sprite UISprite {get=> uiSprite;}
    public GameObject ShotEfPref {get=> shotEfPref;}
    public GameObject BatEfPref {get=> batEfPref;}
    public GameObject ExplosionEfPref {get=> explosionEfPref;}
}


[System.Serializable]
public class ActiveSkillBtn{
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
    public ActiveSkillBtn(float unit, Button skillBtn, Sprite sprite, Material activeSkillEffectMt){
        this.unit = unit;
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
    public void init(Transform plBatEffectTf){
        Trigger = false;
        Panel.material = null;
        GrayBG.fillAmount = 1;
        selectFence.gameObject.SetActive(false);
        plBatEffectTf.gameObject.SetActive(false);
    }
    public void onTriggerActive(Transform plBatEffectTf){
        Debug.Log("onClickActiveSkillButton");
        if(GrayBG.fillAmount == 0){
            Trigger = !Trigger;
            selectFence.gameObject.SetActive(Trigger);
            plBatEffectTf.gameObject.SetActive(Trigger);
        }
    }
    public void decreaseFillAmount(){
        GrayBG.fillAmount -= unit;
    }
    public void setActiveSkillEF(){
        Panel.material = (GrayBG.fillAmount == 0)? activeEFMt : null;
    }

}
