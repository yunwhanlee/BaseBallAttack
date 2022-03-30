using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill<T>{
    //value
    [SerializeField] int lv;
    [SerializeField] T value;
    [SerializeField] T unit;

    //defualt
    public Skill(int _level, T _value, T _unit){
        lv = _level;
        value = _value;
        unit = _unit;
    }

    //method
    public int getCurLv() => lv;
    public T getValue() => value;
    public T getUnit() => unit;
    public void setLvUp(T _value){
        lv++;
        value = _value;
    }
}

[System.Serializable]
public struct Explosion{
    public float per, range;
    public Explosion(float _per = 0, float _range = 0.75f){
        per = _per;
        range = _range;
    }
}
