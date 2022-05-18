using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactorInfo : MonoBehaviour
{
    [SerializeField] DataManager.RANK rank;     public DataManager.RANK Rank {get => rank; set => rank = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] int psvSkillAbility;     public int PsvSkillAbility {get => psvSkillAbility;}
    void Start(){
        //* Set Price By Rank
        
        switch(rank){
            case DataManager.RANK.GENERAL : Price = 100; break;
            case DataManager.RANK.RARE : Price = 300; break;
            case DataManager.RANK.UNIQUE : Price = 700; break;
            case DataManager.RANK.LEGEND : Price = 1500; break;
            case DataManager.RANK.GOD : Price = 4000; break;
        }
    }

    void Update()
    {
        
    }
}
