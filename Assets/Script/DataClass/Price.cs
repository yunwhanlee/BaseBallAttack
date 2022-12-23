using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Price
{
    //* value
    public enum TP {COIN, DIAMOND, CASH};
    [SerializeField] TP type;   public TP Type {get => type;}
    [SerializeField] int val;   public int Val {get => val;  set => val = value;}

    //* function
    public int getValue(){
        Debug.Log($"Price::getValue():: type= {type}");
        return val;
    }
    public void setValue(float value){
        val = (int)value;
    }
}
