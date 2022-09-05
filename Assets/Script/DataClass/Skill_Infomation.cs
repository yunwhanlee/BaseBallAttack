using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class ItemPsvDt {
    public string name;
    public int lv;
    public GameObject imgPref;

    public ItemPsvDt(string name){
        this.name = name;
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class ItemPsvList{
    [SerializeField] ItemPsvDt[] arr = {
        new ItemPsvDt(DM.PSV.Dmg.ToString()),
        new ItemPsvDt(DM.PSV.MultiShot.ToString()),
        new ItemPsvDt(DM.PSV.Speed.ToString()), 
        new ItemPsvDt(DM.PSV.InstantKill.ToString()), 
        new ItemPsvDt(DM.PSV.Critical.ToString()), 
        new ItemPsvDt(DM.PSV.Explosion.ToString()), 
        new ItemPsvDt(DM.PSV.ExpUp.ToString()), 
        new ItemPsvDt(DM.PSV.ItemSpawn.ToString()),
        new ItemPsvDt(DM.PSV.VerticalMultiShot.ToString()),
    };  public ItemPsvDt[] Arr {get => arr; set => arr = value;}

    public void setImgPrefs(ItemPsvList itemPsvList){
        int i=0;
        Array.ForEach(itemPsvList.arr, dtArr => arr[i++].imgPref = dtArr.imgPref);
    }
    
    public void setLvArr(int[] lvArrTemp){
        for(int i=0;i<lvArrTemp.Length;i++){
            Debug.Log("result-> lv["+i+"]=" + lvArrTemp[i]);
            Arr[i].lv = lvArrTemp[i];
        }
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class PsvSkill<T> where T: struct {
    //*value                     //*get set
    public const int MAX_LV = 5;
    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] int level; public int Level {get=>level;}
    [SerializeField] T value;   public T Value {get=>value;}
    [SerializeField] T unit;    public T Unit {get=>unit;}

    //*constructor
    public PsvSkill(string name, int level, T value, T unit){
        this.name = name;
        this.level = level;
        this.value = value;
        this.unit = unit;
    }

    //*method
    public void setLvUp(T value){
        if(level <= MAX_LV){
            level++;
            this.value = value;
        }
    }

    public void initPsvSkillDt(T value){
        // Debug.LogFormat("<color=yellow>initPsvSkillDt():: this.value({0}) = para({1})</color>", this.value, value);
        if(level > 0)
            this.value = value;
    }

    public bool setHitTypePsvSkill(float per, ref int result, Collision col, EffectManager em, Player pl, GameObject ballPref = null){
        int rand = Random.Range(0, 100);
        int percent = Mathf.RoundToInt(per * 100); //百分率
        Debug.LogFormat("PassiveSkill:: setHitTypePsvSkill::「{0}」rand({1}) <= per({2}) : {3}",
            Name.ToString(), rand, percent, ((rand <= percent)? "<color=blue>true</color>" : "false"));
        var psv = DM.ins.convertPsvSkillStr2Enum(Name);

        if(Level > 0 && rand <= percent){
            switch(psv){
                case DM.PSV.InstantKill: 
                    em.createInstantKillTextEF(col.transform);
                    result = Player.ONE_KILL;
                    break;
                case DM.PSV.Critical: 
                    em.createCritTxtEF(col.transform, pl.dmg.Value * 2);
                    result = pl.dmg.Value * 2;
                    break;
                case DM.PSV.Explosion:
                    em.createExplosionEF(ballPref.transform, pl.explosion.Value.range);
                    return true;
            }
        }

        //*「InstantKill」とか「Critical」が発動しなかったら
        if(result == 0){
            result = pl.dmg.Value; //普通のダメージをそのまま代入。
        }
        return false;
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public struct Explosion{
    public float per, range;
    public Explosion(float per = 0, float range = 0.75f){
        this.per = per;
        this.range = range;
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class AtvSkill{
    //* Resource value                        //* get set
    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] Sprite uiSprite;    public Sprite UISprite {get=> uiSprite;}
    [SerializeField] GameObject batEfPref;    public GameObject ShotEfPref {get=> shotEfPref;}
    [SerializeField] GameObject shotEfPref;    public GameObject BatEfPref {get=> batEfPref;}
    [SerializeField] GameObject explosionEfPref;    public GameObject ExplosionEfPref {get=> explosionEfPref;}
    [SerializeField] GameObject castEfPref;    public GameObject CastEfPref {get=> castEfPref;}

    //* Damage value
    public static float THUNDERSHOT_CRT;
    public static int FIREBALL_DMG;
    public static int COLORBALL_DMG;
    public static float POISONSMOKE_PER;
    public static int ICEWAVE_DMG;

    //* constructor
    //* A. Resource
    public AtvSkill(string name, AtvSkill[] activeSkillTable){//Sprite uiSprite, GameObject batEfPref, GameObject shotEfPref, GameObject explosionEfPref){
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

    //* B. Set Dmg
    public AtvSkill(GameManager gm, Player pl){ //@ Overload
        // Debug.Log("ActiveSkill(gm, pl):: gm=" + gm.stage + ", pl=" + pl.dmg.Value);
        THUNDERSHOT_CRT = 2;
        FIREBALL_DMG = pl.dmg.Value + pl.dmg.Value * (int)(gm.stage * 0.25f);
        COLORBALL_DMG = Player.ONE_KILL;
        POISONSMOKE_PER = 0.3f;
        ICEWAVE_DMG = pl.dmg.Value + pl.dmg.Value * (int)(gm.stage * 0.3f);
    }

    //* method
    //* なし
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class AtvSkillBtnUI{
    //*value                        //*get set

    [SerializeField] int index;  public int Index {get=> index;}
    [SerializeField] float unit;    public float Unit {get=> unit; set=> unit=value;}// Decrease Fill Amount Unit
    [SerializeField] string name;    public string Name {get=> name; set=> name=value;}
    [SerializeField] bool trigger;    public bool Trigger {get=> trigger; set=> trigger=value;}
    [SerializeField] Image panel;    public Image Panel {get=> panel; set=> panel=value;}
    [SerializeField] Image img;    public Image Img {get=> img; set=> img=value;}
    [SerializeField] Image grayBG;    public Image GrayBG {get=> grayBG; set=> grayBG=value;}
    [SerializeField] Image selectCircleEF;    public Image SelectCircleEF {get=> selectCircleEF; set=> selectCircleEF=value;}
    [SerializeField] Material activeEFMt;   public Material ActiveEFMt {get=> activeEFMt;}

    //*contructor
    public AtvSkillBtnUI(int index, float unit, string name, Button skillBtn, Sprite sprite, Material activeSkillEffectMt){
        this.index = index;
        this.unit = unit;
        this.name = name;
        panel = skillBtn.GetComponent<Image>();
        this.img = skillBtn.transform.GetChild(0).GetComponent<Image>();
        this.img.sprite = sprite;
        grayBG = skillBtn.transform.GetChild(1).GetComponent<Image>();
        selectCircleEF = skillBtn.transform.GetChild(2).GetComponent<Image>();
        activeEFMt = activeSkillEffectMt;
    }

    //*method
    public void init(GameManager gm, bool isSelectBtnInit = false){
        Trigger = false;
        if(!isSelectBtnInit)   Panel.material = null;
        if(!isSelectBtnInit)   GrayBG.fillAmount = 1;
        selectCircleEF.gameObject.SetActive(false);
        gm.pl.BatEffectTf.gameObject.SetActive(false);
        foreach(Transform child in gm.pl.CastEFArrowTf) GameObject.Destroy(child.gameObject);
        foreach(Transform child in gm.pl.CastEFBallPreviewTf) GameObject.Destroy(child.gameObject);
        gm.setLightDarkness(false);
        gm.bm.setGlowEFAllBlocks(false);
    }
    public void onTriggerActiveSkillBtn(GameManager gm){
        //TODO idxの処理しないと、現在はスキルボタン１個として対応。
        int skillIdx = gm.getCurSkillIdx();

        if(GrayBG.fillAmount == 0){
            Debug.LogFormat("ActiveSkillBtnUI:: onTriggerActive:: trigger= {0}, skillIdx= {1}, name= {2}",Trigger, skillIdx, name);
            Trigger = !Trigger;
            gm.setLightDarkness(true);
            selectCircleEF.gameObject.SetActive(Trigger);

            //* Bat Effect
            gm.pl.BatEffectTf.gameObject.SetActive(Trigger);
            gm.em.enableSelectedActiveSkillBatEF(skillIdx, gm.pl.BatEffectTf);

            //* Cast Effect
            if(Trigger){
                Transform parentTf = null;
                var atv = DM.ins.convertAtvSkillStr2Enum(this.name);
                switch(atv){
                    case DM.ATV.Thunder:     parentTf = gm.pl.CastEFArrowTf;        break;
                    case DM.ATV.FireBall:    parentTf = gm.pl.CastEFBallPreviewTf;  break;
                    case DM.ATV.PoisonSmoke: parentTf = gm.pl.CastEFBallPreviewTf;  break;
                    case DM.ATV.IceWave:     parentTf = gm.pl.CastEFBallPreviewTf;  break;
                    case DM.ATV.ColorBall:   parentTf = gm.pl.CastEFArrowTf;        break;
                }
                gm.em.createActiveSkillCastEF(skillIdx, parentTf);
            }
            else{
                foreach(Transform child in gm.pl.CastEFArrowTf) GameObject.Destroy(child.gameObject);
                foreach(Transform child in gm.pl.CastEFBallPreviewTf) GameObject.Destroy(child.gameObject);
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
