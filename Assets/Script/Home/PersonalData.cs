using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalData {
    //* value
    [SerializeField] int coin; public int Coin {get => coin; set => coin = value;}
    [SerializeField] int diamond; public int Diamond {get => diamond; set => diamond = value;}
    [SerializeField] int selectCharaIdx;  public int SelectCharaIdx {get => selectCharaIdx; set => selectCharaIdx = value;}
    
    //TODO Chara OnLock List

    //TODO Item OnLock List

    //* constructor
    public PersonalData(int coin = 0, int diamond = 0, int selectCharaIdx = 0){
        this.Coin = coin;
        this.Diamond = diamond;
        this.SelectCharaIdx = selectCharaIdx;
    }

    //* method
}
