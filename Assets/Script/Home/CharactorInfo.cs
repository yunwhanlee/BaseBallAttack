using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInfo : MonoBehaviour
{
    public enum RANK {GENERAL, RARE, UNIQUE, LEGEND, GOD}
    [SerializeField] RANK rank;     public RANK Rank {get => rank; set => rank = value;}
    [SerializeField] int price;     public int Price {get => price; set => price = value;}
    [SerializeField] int psvSkillAbility;     public int PsvSkillAbility {get => psvSkillAbility;}
    void Start()
    {
        //* Set Price By Rank
        switch(rank){
            case RANK.GENERAL : Price = 100; break;
            case RANK.RARE : Price = 300; break;
            case RANK.UNIQUE : Price = 700; break;
            case RANK.LEGEND : Price = 1500; break;
            case RANK.GOD : Price = 4000; break;
        }
    }

    void Update()
    {
        
    }
}
