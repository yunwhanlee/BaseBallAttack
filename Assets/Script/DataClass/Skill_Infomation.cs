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

[System.Serializable]
public class ItemPassiveList{
    [SerializeField] ItemPsvDt[] arr = {
        new ItemPsvDt("Dmg"), new ItemPsvDt("MultiShot"), new ItemPsvDt("Speed"), 
        new ItemPsvDt("InstantKill"), new ItemPsvDt("Critical"), new ItemPsvDt("Explosion"), 
        new ItemPsvDt("ExpUp"), new ItemPsvDt("ItemSpawn")
    };
    public ItemPsvDt[] Arr {get => arr; set => arr = value;}

    [SerializeField] GameObject[] imgPrefs; GameObject[] ImgPrefs {get => imgPrefs; set => imgPrefs = value;}

    public void setImgPrefs(ItemPassiveList itemPsvList){
        int i=0;
        Array.ForEach(itemPsvList.arr, dtArr => arr[i++].imgPref = dtArr.imgPref);
    }

    public void setLvArr(ItemPassiveList itemPsvList){
        int i=0;
        Array.ForEach(itemPsvList.Arr, dtArr => arr[i++].lv += dtArr.lv);
    }
}

[System.Serializable]
public class PassiveSkill<T>{
    //*value                     //*get set
    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] int level; public int Level {get=>level;}
    [SerializeField] T value;   public T Value {get=>value;}
    [SerializeField] T unit;    public T Unit {get=>unit;}

    //*constructor
    public PassiveSkill(string name, int level, T value, T unit){
        this.name = name;
        this.level = level;
        this.value = value;
        this.unit = unit;
    }

    //*method
    public void setLvUp(T value){
        level++;
        this.value = value;
    }

    public void setHitTypePsvSkill(float per, ref int result, Collision col, EffectManager em, Player pl, GameObject ballPref = null){
        bool isLastExplosionSkill = (ballPref)? true : false;
        int rand = Random.Range(0, 100);
        int percent = Mathf.RoundToInt(per * 100); //百分率
        Debug.Log("PassiveSkill:: setHitTypePsvSkill:: 「" + Name.ToString() + "」 rand("+rand+") <= per("+per+") : " + ((rand <= per)? "<color=blue>true</color>" : "false"));
        if(Level > 0 && rand <= percent){
            switch(Name){
                case "instantKill": 
                    em.createInstantKillTextEF(col.transform);
                    result =  pl.dmg.Value * 999999;
                    break;
                case "critical": 
                    em.createCriticalTextEF(col.transform, pl.dmg.Value * 2);
                    result = pl.dmg.Value * 2;
                    break;
                case "explosion":
                    em.createExplosionEF(ballPref.transform, pl.explosion.Value.range);
                    //Sphere Collider
                    RaycastHit[] rayHits = Physics.SphereCastAll(ballPref.transform.position, pl.explosion.Value.range, Vector3.up, 0);
                    foreach(var hit in rayHits){
                        if(hit.transform.tag == "NormalBlock")
                            hit.transform.GetComponent<Block_Prefab>().decreaseHp(result);
                    }
                    break;
            }
        }
        //* result
        if(!isLastExplosionSkill)
            result = pl.dmg.Value;
        else
            col.gameObject.GetComponent<Block_Prefab>().decreaseHp(result);
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
    //*value                        //*get set
    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] Sprite uiSprite;    public Sprite UISprite {get=> uiSprite;}
    [SerializeField] GameObject batEfPref;    public GameObject ShotEfPref {get=> shotEfPref;}
    [SerializeField] GameObject shotEfPref;    public GameObject BatEfPref {get=> batEfPref;}
    [SerializeField] GameObject explosionEfPref;    public GameObject ExplosionEfPref {get=> explosionEfPref;}
    [SerializeField] GameObject castEfPref;    public GameObject CastEfPref {get=> castEfPref;}

    //*constructor
    public ActiveSkill(string name, ActiveSkill[] activeSkillTable){//Sprite uiSprite, GameObject batEfPref, GameObject shotEfPref, GameObject explosionEfPref){
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
    //*method
}


[System.Serializable]
public class ActiveSkillBtnUI{
    //*value                        //*get set

    [SerializeField] int index;  public int Index {get=> index;}
    [SerializeField] float unit;    public float Unit {get=> unit; set=> unit=value;}// Decrease Fill Amount Unit
    [SerializeField] string name;    public string Name {get=> name; set=> name=value;}
    [SerializeField] bool trigger;    public bool Trigger {get=> trigger; set=> trigger=value;}
    [SerializeField] Image panel;    public Image Panel {get=> panel; set=> panel=value;}
    [SerializeField] Image img;    public Image Img {get=> img; set=> img=value;}
    [SerializeField] Image grayBG;    public Image GrayBG {get=> grayBG; set=> grayBG=value;}
    [SerializeField] Image selectFence;    public Image SelectFence {get=> selectFence; set=> selectFence=value;}
    [SerializeField] Material activeEFMt;   public Material ActiveEFMt {get=> activeEFMt;}

    //*contructor
    public ActiveSkillBtnUI(int index, float unit, string name, Button skillBtn, Sprite sprite, Material activeSkillEffectMt){
        this.index = index;
        this.unit = unit;
        this.name = name;
        panel = skillBtn.GetComponent<Image>();
        this.img = skillBtn.transform.GetChild(0).GetComponent<Image>();
        this.img.sprite = sprite;
        grayBG = skillBtn.transform.GetChild(1).GetComponent<Image>();
        selectFence = skillBtn.transform.GetChild(2).GetComponent<Image>();
        activeEFMt = activeSkillEffectMt;
    }

    //*method
    public void init(Player pl, bool isSelectBtnInit = false){
        Trigger = false;
        if(!isSelectBtnInit)   Panel.material = null;
        if(!isSelectBtnInit)   GrayBG.fillAmount = 1;
        selectFence.gameObject.SetActive(false);
        pl.BatEffectTf.gameObject.SetActive(false);
        foreach(Transform child in pl.CastEFArrowTf) GameObject.Destroy(child.gameObject);
        foreach(Transform child in pl.CastEFBallPreviewTf) GameObject.Destroy(child.gameObject);
    }
    public void onTriggerActive(int selectIdx, Player pl, EffectManager em){
        if(GrayBG.fillAmount == 0){
            Debug.Log("ActiveSkillBtnUI:: onTriggerActive:: trigger= " + Trigger);

            Trigger = !Trigger;
            selectFence.gameObject.SetActive(Trigger);

            //* Bat Effect
            pl.BatEffectTf.gameObject.SetActive(Trigger);
            foreach(Transform child in pl.BatEffectTf){
                int childIdx = child.GetSiblingIndex();
                if(selectIdx == childIdx)
                    child.gameObject.SetActive(true);
                else 
                    child.gameObject.SetActive(false);
            }

            //* Cast Effect
            Debug.Log("ActiveSkillBtnUI:: this.name=" + this.name);
            if(Trigger){
                Transform parentTf = null;
                switch(this.name){
                    case "Thunder":     parentTf = pl.CastEFArrowTf;        break;
                    case "FireBall":    parentTf = pl.CastEFBallPreviewTf;  break;
                    case "ColorBall":   parentTf = pl.CastEFArrowTf;        break;
                }
                em.createActiveSkillCastEF(selectIdx, parentTf);
            }
            else{
                foreach(Transform child in pl.CastEFArrowTf) GameObject.Destroy(child.gameObject);
                foreach(Transform child in pl.CastEFBallPreviewTf) GameObject.Destroy(child.gameObject);
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
