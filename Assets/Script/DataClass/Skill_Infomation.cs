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
public class ActiveSkillBtn{
    //value
    [SerializeField] private Image panel;
    [SerializeField] private Image img;
    [SerializeField] private Image grayBG;

    //contructor
    public ActiveSkillBtn(Button skillBtn){
        panel = skillBtn.GetComponent<Image>();
        img = skillBtn.transform.GetChild(0).GetComponent<Image>();
        grayBG = skillBtn.transform.GetChild(1).GetComponent<Image>();
    }

    //get set
    public Image Panel {get=> panel; set=> panel=value;}
    public Image Img {get=> img; set=> img=value;}
    public Image GrayBG {get=> grayBG; set=> grayBG=value;}

    //method
    public void setFillAmount(float v){
        GrayBG.fillAmount += v;
    }

}
