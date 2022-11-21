using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class UpgradeDt {
    public string name;
    public int lv, maxLv;
    public float unit;
    public UpgradeDt(string name, float unit, int maxLv){
        this.name = name;
        this.unit = unit;
        this.maxLv = maxLv;
    }
    public void setLvUp(){
        if(lv < maxLv)
            this.lv++;
    }

    public float getValue() => lv * unit;

    public string getVal2Str(){
        var value = getValue();
        if(name == DM.UPGRADE.Dmg.ToString())
            return $"{value}";
        if(name == DM.UPGRADE.BallSpeed.ToString())
            return $"{value * 100}m/s";
        else
            return $"{value * 100}%";
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class UpgradeList {
    //* Value
    const int DMG_UNIT = 1;
    const float BALL_SPEED_UNIT = 0.05f;
    const float CRITICAL_UNIT = 0.01f;
    const float CRITICAL_DMG_UNIT = 0.10f;
    const float BOSS_DMG_UNIT = 0.10f;
    const float COIN_BONUS_UNIT = 0.1f;
    const float DEFENCE_UNIT = 0.05f;

    //* Init
    [SerializeField] UpgradeDt[] arr = {
        new UpgradeDt(DM.UPGRADE.Dmg.ToString(), DMG_UNIT, 100),
        new UpgradeDt(DM.UPGRADE.BallSpeed.ToString(), BALL_SPEED_UNIT, 20),
        new UpgradeDt(DM.UPGRADE.Critical.ToString(), CRITICAL_UNIT, 30),
        new UpgradeDt(DM.UPGRADE.CriticalDamage.ToString(), CRITICAL_DMG_UNIT, 20),
        new UpgradeDt(DM.UPGRADE.BossDamage.ToString(), BOSS_DMG_UNIT, 30),
        new UpgradeDt(DM.UPGRADE.CoinBonus.ToString(), COIN_BONUS_UNIT, 20),
        new UpgradeDt(DM.UPGRADE.Defence.ToString(), DEFENCE_UNIT, 10),
    };
    public UpgradeDt[] Arr {get => arr; set => arr = value;}
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
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
    //* value
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
        new ItemPsvDt(DM.PSV.CriticalDamage.ToString()),
        new ItemPsvDt(DM.PSV.Laser.ToString()),
        new ItemPsvDt(DM.PSV.FireProperty.ToString()),
        new ItemPsvDt(DM.PSV.IceProperty.ToString()),
        new ItemPsvDt(DM.PSV.ThunderProperty.ToString()),
        new ItemPsvDt(DM.PSV.DamageTwice.ToString()),
        new ItemPsvDt(DM.PSV.GiantBall.ToString()),
        new ItemPsvDt(DM.PSV.DarkOrb.ToString()),
        new ItemPsvDt(DM.PSV.GodBless.ToString()),
        new ItemPsvDt(DM.PSV.BirdFriend.ToString()),
    };
    public ItemPsvDt[] Arr {get => arr; set => arr = value;}

    //* method
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
    public const int ONE_KILL_DMG = 999999;
    public const int CRIT_DMG_DEFAULT = 200;
    public const float MULTI_SHOT_DEG = 25;
    public const float LASER_DEG = 17.5f;
    public const float GIANTBALL_SCALE = 2.25f;

    [SerializeField] string name;    public string Name {get=> name;} 
    [SerializeField] int level; public int Level {get=>level;}
    [SerializeField] T val;   public T Val {get=>val; set=>val=value;}
    [SerializeField] T unit;    public T Unit {get=>unit;}
    [SerializeField] int maxLevel;    public int MaxLevel {get=>maxLevel;}


    //*constructor
    public PsvSkill(string name, int level, T value, T unit, int maxLevel = MAX_LV){
        this.name = name;
        this.level = level > maxLevel? maxLevel : level; //* (BUG)PSV MaxLevel以上に上がったら、Out of Index エラー防止。
        this.val = value;
        this.unit = unit;
        this.maxLevel = maxLevel;
    }

    //*method
    public void setLvUp(T value){
        if(level <= MaxLevel){
            level++;
            this.val = value;
        }
    }

    public void initSkillDt(T value){
        Debug.LogFormat($"<color=yellow>initSkillDt(value={value}):: name={name}, lv={level}, maxLv={maxLevel}, value={value}</color>");
        // if(level > 0){
            this.val = value;
        // }
    }

    public bool setHitTypeSkill(float per, ref int result, Collision col, EffectManager em, Player pl, GameObject ballPref = null){
        int rand = Random.Range(0, 100);
        int percent = Mathf.RoundToInt(per * 100); //百分率

        //* PSV Unique Skill
        int DMG_TWICE = (pl.damageTwice.Level == 1)? 2 : 1;
        int GIANT_BALL_CALC = (pl.giantBall.Level == 1)? pl.giantBall.Val * 2 : 1;

        var psv = DM.ins.convertPsvSkillStr2Enum(Name);
        if(Level > 0 && rand <= percent){
            if(psv == DM.PSV.NULL) Debug.LogError("convertPsvSkillStr2Enum()へ string[]変数を追加してください！");
            Debug.LogFormat("PassiveSkill:: setHitTypeSkill:: Name= {0}, rand({1}) <= per({2}) : {3}",
                Name.ToString(), rand, percent, ((rand <= percent)? "<color=blue>true</color>" : "false"));
            switch(psv){
                case DM.PSV.InstantKill: 
                    em.createInstantKillTextEF(col.transform.position);
                    result = PsvSkill<int>.ONE_KILL_DMG;
                    break;
                case DM.PSV.Critical: 
                    int dmg = (int)(pl.dmg.Val * (2 + pl.criticalDamage.Val) * DMG_TWICE);
                    em.createCritTxtEF(col.transform.position, dmg);
                    result = dmg;
                    break;
                case DM.PSV.FireProperty:
                    col.transform.GetComponent<Block_Prefab>().FireDotDmg.IsOn = true;
                    break;
                case DM.PSV.IceProperty:
                    col.transform.GetComponent<Block_Prefab>().Freeze.IsOn = true;
                    break;
                case DM.PSV.ThunderProperty:
                    em.createThunderStrikeEF(col.transform.position);
                    em.createCritTxtEF(col.transform.position, pl.dmg.Val);
                    result *= 2;
                    break;

                case DM.PSV.Explosion:
                    em.createExplosionEF(ballPref.transform.position, pl.explosion.Val.range);
                    return true;
            }
        }

        //*「InstantKill」とか「Critical」が発動しなかったら
        if(result == 0){
            result = pl.dmg.Val * DMG_TWICE * GIANT_BALL_CALC; //普通のダメージをそのまま代入。
        }
        return false;
    }

    public Vector3 calcMultiShotDeg2Dir(float arrowDeg, int i){
        Debug.Log($"calcMultiShotDeg2Dir():: Name = {Name}");

        //* Arrow Direction + Extra Deg
        List<float> extraDegList = new List<float>();
        switch(DM.ins.convertPsvSkillStr2Enum(Name)){
            case DM.PSV.MultiShot:
                extraDegList.Add(-MULTI_SHOT_DEG + arrowDeg); //! BUG) Hit Ball Direction (Deg)が 設定されていなかったので対応。
                extraDegList.Add(MULTI_SHOT_DEG + arrowDeg);
                extraDegList.Add(-(MULTI_SHOT_DEG*2) + arrowDeg);
                extraDegList.Add((MULTI_SHOT_DEG*2) + arrowDeg);
                break;
            case DM.PSV.Laser:
                extraDegList.Add(arrowDeg);
                extraDegList.Add(-LASER_DEG + arrowDeg);
                extraDegList.Add(LASER_DEG + arrowDeg);
                break;
        }
        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (extraDegList[i])), 0, Mathf.Cos(Mathf.Deg2Rad * (extraDegList[i]))).normalized;
        return direction;
    }

    public static List<string> getPsvStatusInfo2Str(Player pl){
        //* PSV Unique Skill
        int DMG_TWICE = (pl.damageTwice.Level == 1)? 2 : 1;
        int GIANT_BALL_CALC = (pl.giantBall.Level == 1)? pl.giantBall.Val / 2 : 1;

        return new List<string>(){
            pl.dmg.Name,                    ((pl.dmg.Val * DMG_TWICE * GIANT_BALL_CALC).ToString()),
            pl.speed.Name,                  (pl.speed.Val * 100 + "m/s").ToString(),
            pl.multiShot.Name,              ("⇔: " + (pl.multiShot.Val) + ", ⇑: " + (pl.verticalMultiShot.Val)).ToString(),
            // pl.verticalMultiShot.Name,      (pl.verticalMultiShot.Val * 1).ToString(),
            pl.laser.Name,                  (pl.laser.val).ToString(),
            pl.instantKill.Name,            (pl.instantKill.Val * 100 + "%").ToString(),
            pl.critical.Name,               (pl.critical.Val * 100 + "%").ToString(),
            pl.criticalDamage.Name,         (CRIT_DMG_DEFAULT + (pl.criticalDamage.Val * 100) + "%").ToString(),
            pl.explosion.Name,              (pl.explosion.Val.per * 100 + "%").ToString(),
            pl.expUp.Name,                  (pl.expUp.Val * 100 + "%").ToString(),
            pl.itemSpawn.Name,              (pl.itemSpawn.Val * 100 + "%").ToString(),
            pl.fireProperty.Name,           (pl.fireProperty.Val * 100 + "%").ToString(),
            pl.iceProperty.Name,            (pl.iceProperty.Val * 100 + "%").ToString(),
            pl.thunderProperty.Name,        (pl.thunderProperty.Val * 100 + "%").ToString(),
        };
    }

    public static List<KeyValuePair<string, int>> getPsvLVList(Player pl){
        //* Key : Name, Value : Level
        List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
        //* set All Skills curLevel
        list.Add(new KeyValuePair<string, int>(pl.dmg.Name, pl.dmg.Level));
        list.Add(new KeyValuePair<string, int>(pl.multiShot.Name, pl.multiShot.Level));
        list.Add(new KeyValuePair<string, int>(pl.speed.Name, pl.speed.Level));
        list.Add(new KeyValuePair<string, int>(pl.instantKill.Name, pl.instantKill.Level));
        list.Add(new KeyValuePair<string, int>(pl.critical.Name, pl.critical.Level));
        list.Add(new KeyValuePair<string, int>(pl.explosion.Name, pl.explosion.Level));
        list.Add(new KeyValuePair<string, int>(pl.expUp.Name, pl.expUp.Level));
        list.Add(new KeyValuePair<string, int>(pl.itemSpawn.Name, pl.itemSpawn.Level));
        list.Add(new KeyValuePair<string, int>(pl.verticalMultiShot.Name, pl.verticalMultiShot.Level));
        list.Add(new KeyValuePair<string, int>(pl.criticalDamage.Name, pl.criticalDamage.Level));
        list.Add(new KeyValuePair<string, int>(pl.laser.Name, pl.laser.Level));
        list.Add(new KeyValuePair<string, int>(pl.fireProperty.Name, pl.fireProperty.Level));
        list.Add(new KeyValuePair<string, int>(pl.iceProperty.Name, pl.iceProperty.Level));
        list.Add(new KeyValuePair<string, int>(pl.thunderProperty.Name, pl.thunderProperty.Level));
        list.Add(new KeyValuePair<string, int>(pl.damageTwice.Name, pl.damageTwice.Level));
        list.Add(new KeyValuePair<string, int>(pl.giantBall.Name, pl.giantBall.Level));
        list.Add(new KeyValuePair<string, int>(pl.darkOrb.Name, pl.darkOrb.Level));
        list.Add(new KeyValuePair<string, int>(pl.godBless.Name, pl.godBless.Level));
        list.Add(new KeyValuePair<string, int>(pl.birdFriend.Name, pl.birdFriend.Level));
        return list;
    }

    public static List<KeyValuePair<string, int>> getPsvMaxLVList(Player pl){
        //* Key : Name, Value : Level
        List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
        //* set All Skills curLevel
        list.Add(new KeyValuePair<string, int>(pl.dmg.Name, pl.dmg.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.multiShot.Name, pl.multiShot.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.speed.Name, pl.speed.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.instantKill.Name, pl.instantKill.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.critical.Name, pl.critical.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.explosion.Name, pl.explosion.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.expUp.Name, pl.expUp.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.itemSpawn.Name, pl.itemSpawn.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.verticalMultiShot.Name, pl.verticalMultiShot.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.criticalDamage.Name, pl.criticalDamage.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.laser.Name, pl.laser.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.fireProperty.Name, pl.fireProperty.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.iceProperty.Name, pl.iceProperty.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.thunderProperty.Name, pl.thunderProperty.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.damageTwice.Name, pl.damageTwice.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.giantBall.Name, pl.giantBall.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.darkOrb.Name, pl.darkOrb.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.godBless.Name, pl.godBless.MaxLevel));
        list.Add(new KeyValuePair<string, int>(pl.birdFriend.Name, pl.birdFriend.MaxLevel));

        return list;
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public struct Explosion{
    public float per;
    public float range;
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
    public static int COLORBALL_POP_CNT;
    public static float FIREBALL_DOT;
    public static float POISONSMOKE_DOT;
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
        FIREBALL_DMG = pl.dmg.Val + pl.dmg.Val * (Util._.getCalcEquivalentSequence(gm.stage, 4) / 4);// FIREBALL_DMG = pl.dmg.Value + pl.dmg.Value * (int)(gm.stage * 0.15f);
        COLORBALL_POP_CNT = 5;
        FIREBALL_DOT = 0.15f;
        POISONSMOKE_DOT = 0.25f;
        ICEWAVE_DMG = pl.dmg.Val + pl.dmg.Val * (Util._.getCalcEquivalentSequence(gm.stage, 4) / 2);// ICEWAVE_DMG = pl.dmg.Value + pl.dmg.Value * (int)(gm.stage * 0.3f);
    }

    //* method
    public void checkBlocksIsDotDmg(GameManager gm){
        var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>();
        Array.ForEach(blocks, block => {
            if(block.IsDotDmg) {
                float dmg = AtvSkill.POISONSMOKE_DOT;
                for(int i=0; i<block.transform.childCount; i++){
                    if(block.transform.GetChild(i).name.Contains(DM.NAME.FireBallDotEffect.ToString())){
                        dmg = AtvSkill.FIREBALL_DOT;
                        break;
                    }
                }
                block.decreaseHp(block.getDotDmg(dmg));
                gm.em.createCritTxtEF(block.transform.position, block.getDotDmg(AtvSkill.POISONSMOKE_DOT));
            }
        });
    }

    public void setColorBallSkillGlowEF(GameManager gm, ref BlockMaker bm, RaycastHit hit, ref GameObject hitBlockByBallPreview){
        bool isColorBallSkill = gm.activeSkillBtnList.Exists(btn => btn.Trigger && btn.Name == DM.ATV.ColorBall.ToString());
        // Debug.Log("setColorBallSkillGlowEF():: hit= " + hit);
        if(isColorBallSkill && hit.transform.name.Contains(DM.NAME.Block.ToString())){
            var hitBlock = hit.transform.GetComponent<Block_Prefab>();

            if(hitBlock.kind == BlockMaker.KIND.TreasureChest || hitBlock.kind == BlockMaker.KIND.Heal){
                return;
            }

            var sameColorBlocks = findSameColorBlocks(gm, hit.transform.gameObject);

            //* Glow Effect On
            bm.setGlowEF(sameColorBlocks, true);
            
            //* Reset
            if(hitBlockByBallPreview != hit.transform.gameObject){
                var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>();
                bm.setGlowEF(blocks, false);
            }
            hitBlockByBallPreview = hit.transform.gameObject;
        }
    }

    public static Block_Prefab[] findSameColorBlocks(GameManager gm, GameObject hitObj){
        //* Hit Color
        var meshRd = hitObj.GetComponent<MeshRenderer>();
        Color hitColor = meshRd.material.GetColor("_ColorTint");
        //* Find Same Color Blocks
        var blocks = gm.blockGroup.GetComponentsInChildren<Block_Prefab>();
        return Array.FindAll(blocks, bl => 
            (bl.kind == BlockMaker.KIND.Normal || bl.kind == BlockMaker.KIND.Long)//* (BUG) 宝箱はmeshRendererがないので場外。
            && bl.GetComponent<MeshRenderer>().material.GetColor("_ColorTint") == hitColor
        );
    }
}
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------
[System.Serializable]
public class AtvSkillBtnUI{
    //*value                        //*get set
    [SerializeField] int index;  public int Index {get=> index;}
    [SerializeField] float coolDownUnit;    public float CoolDownUnit {get=> coolDownUnit; set=> coolDownUnit=value;}// Decrease Fill Amount Unit
    [SerializeField] string name;    public string Name {get=> name; set=> name=value;}
    [SerializeField] bool trigger;    public bool Trigger {get=> trigger; set=> trigger=value;}
    [SerializeField] Image panel;    public Image Panel {get=> panel; set=> panel=value;}
    [SerializeField] Image img;    public Image Img {get=> img; set=> img=value;}
    [SerializeField] Image coolDownImg;    public Image CollDownImg {get=> coolDownImg; set=> coolDownImg=value;}
    [SerializeField] Image selectCircleEF;    public Image SelectCircleEF {get=> selectCircleEF; set=> selectCircleEF=value;}
    [SerializeField] Material activeEFMt;   public Material ActiveEFMt {get=> activeEFMt;}

    //*contructor
    public AtvSkillBtnUI(int index, float coolDownUnit, string name, Button skillBtn, Sprite sprite, Material activeSkillEffectMt){
        this.index = index;
        this.CoolDownUnit = coolDownUnit;
        this.name = name;
        panel = skillBtn.GetComponent<Image>();
        this.img = skillBtn.transform.GetChild(0).GetComponent<Image>();
        this.img.sprite = sprite;
        coolDownImg = skillBtn.transform.GetChild(1).GetComponent<Image>();
        selectCircleEF = skillBtn.transform.GetChild(2).GetComponent<Image>();
        activeEFMt = activeSkillEffectMt;
    }

    //*method
    public void init(GameManager gm, bool isSelectBtnInit = false){
        Trigger = false;
        if(!isSelectBtnInit)   Panel.material = null;
        if(!isSelectBtnInit)   CollDownImg.fillAmount = 1;
        selectCircleEF.gameObject.SetActive(false);
        gm.pl.BatEffectTf.gameObject.SetActive(false);
        gm.pl.destroyAllCastEF();
        gm.setLightDarkness(false);
        gm.bm.setGlowEFAllBlocks(false);
    }
    public void onTriggerActiveSkillBtn(GameManager gm){
        //TODO idxの処理しないと、現在はスキルボタン１個として対応。
        int skillIdx = gm.getCurSkillIdx();

        if(CollDownImg.fillAmount == 0){
            Debug.LogFormat("ActiveSkillBtnUI:: onTriggerActive:: trigger= {0}, skillIdx= {1}, name= {2}",Trigger, skillIdx, name);
            Trigger = !Trigger;
            gm.setLightDarkness(true);
            selectCircleEF.gameObject.SetActive(Trigger);

            //* Bat Effect
            gm.pl.BatEffectTf.gameObject.SetActive(Trigger);
            gm.em.enableSelectedActiveSkillBatEF(skillIdx, gm.pl.BatEffectTf);

            //* Cast Effect
            if(Trigger){
                List<Transform> parentTfList = new List<Transform>();
                var atv = DM.ins.convertAtvSkillStr2Enum(this.name);
                switch(atv){
                    case DM.ATV.Thunder:     
                    case DM.ATV.ColorBall:   
                        parentTfList.Add(gm.pl.CastEFArrowTf);
                        break;
                    case DM.ATV.FireBall:
                    case DM.ATV.PoisonSmoke:
                    case DM.ATV.IceWave: 
                        Array.ForEach(gm.pl.CastEFBallPreviewTfs, tf => parentTfList.Add(tf));
                        break;
                }
                //* CastEF 生成
                parentTfList.ForEach(parentTf => gm.em.directlyCreateAtvSkCastEF(skillIdx, parentTf));
            }
            else{
                gm.pl.destroyAllCastEF();
            }
        }
    }
    public void coolDownFillAmount(){
        CollDownImg.fillAmount -= CoolDownUnit;
    }
    public void setActiveSkillEF(){
        Panel.material = (CollDownImg.fillAmount == 0)? activeEFMt : null;
    }

}
