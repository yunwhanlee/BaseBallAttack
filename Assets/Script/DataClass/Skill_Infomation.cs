using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
