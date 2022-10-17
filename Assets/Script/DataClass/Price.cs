using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Price
{
    //* value
    public enum TP {COIN, DIAMOND};
    [SerializeField] TP type;   public TP Type {get => type;}
    [SerializeField] int coin;   public int Coin {get => coin;  set => coin = value;}
    [SerializeField] int diamond;    public int Diamond {get => diamond;  set => diamond = value;}

    //* function
    public int getValue(){
        Debug.Log("Price class:: getInfo():: type= " + type);
        //* Check Type
        if(type == Price.TP.COIN)
            return Coin;
        else
            return Diamond;
    }
}