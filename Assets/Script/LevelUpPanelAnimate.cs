using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Serialization;

[System.Serializable]
public struct LevelUpSkillPanelBtn{
    public RectTransform colImgRectTf;
    public Text name;

    // public LevelUpSkillPanelBtn(Button skillBtn){//Contructor
    //     colImgRectTf = skillBtn.transform.GetChild(0).GetComponent<RectTransform>();
    //     name = skillBtn.transform.GetChild(1).GetComponent<Text>();
    // }
}

public class LevelUpPanelAnimate : MonoBehaviour{
    //* OutSide
    GameManager gm; Player pl;

    //* Value
    const int SKILL_BTN_IDX_LEN = 2;
    int spriteH;
    int btnIdx;
    float time;
    float span;
    int skillImgCnt;
    
    [Header("STATE")][Header("__________________________")]
    [SerializeField] int scrollingSpeed;
    [SerializeField] bool isRollingStop = false;
    [SerializeField] bool isPsvSkillTicket; public bool IsPsvSkillTicket { get => isPsvSkillTicket; set => isPsvSkillTicket = value;}
    [SerializeField] List<string> exceptSkNameList = new List<string>();
    public List<KeyValuePair<int, GameObject>> skillList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。

    [Header("GUI")][Header("__________________________")]
    [FormerlySerializedAs("titleTxt")] public Text titleTxt;
    [FormerlySerializedAs("levelTxt")] public Text levelTxt;
    [FormerlySerializedAs("explainTxt")] public Text explainTxt;
    [FormerlySerializedAs("colSkillBtns")] public LevelUpSkillPanelBtn[] colSkillBtns;
    [FormerlySerializedAs("centerWingImg")] public Image centerWingImg;
    [FormerlySerializedAs("centerMarkImg")] public Image centerMarkImg;
    [FormerlySerializedAs("rerotateBtn")] public Button rerotateBtn;

    private void OnEnable() => Time.timeScale = 0; //* このパンネルが出たら、政界の時間 停止
    private void OnDisable() => Time.timeScale = 1; //* このパンネルが消えたら、政界の時間 戻す

    public void Start(){
        Debug.Log("onClickRewardChestOkButton:: Start!");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pl = gm.pl;

        //* Init Value
        spriteH = (int)gm.PsvSkillImgPrefs[0].GetComponent<RectTransform>().rect.height; //270
        time = 0;
        span = 2;
        btnIdx = 0;
        isRollingStop = false;
        exceptSkNameList = new List<string>();
        skillImgCnt = gm.PsvSkillImgPrefs.Length - 1; //* ★自然なスクロールのため、末端に1番目Imageを追加したので、この分は消す-1。
        skillList = new List<KeyValuePair<int, GameObject>>();

        //* Set GUI Data
        setUI(DM.NAME.LevelUp.ToString());

        resetSkillImgChild(); //* Reset Before Child : これがないと、クリックでスクロール止まらなく選択されてしまう。
        setSkillList(skillList);
        setExceptList(); //* Check Max Skill Level
        createColSprite();
    }

    void Update(){
        float speed = scrollingSpeed * Time.unscaledDeltaTime;
        //* SCROLL ANIMATION
        if(SKILL_BTN_IDX_LEN >= btnIdx && skillList.Count > 0){
            time += Time.unscaledDeltaTime;
            foreach(LevelUpSkillPanelBtn slotBtn in colSkillBtns){
                //* #1.Scrolling Down
                if(time < span){
                    if(slotBtn.colImgRectTf.localPosition.y >= 0) //* ↓
                        slotBtn.colImgRectTf.Translate(0,-speed, 0);
                    else //* 位置初期化
                        slotBtn.colImgRectTf.localPosition = new Vector3(0, spriteH * skillImgCnt, 0);
                }
                //* #2.Stop
                else{
                    int randIdx = Random.Range(0, skillList.Count);
                    int randPer = Random.Range(0, 100);
                    string tagName = skillList[randIdx].Value.transform.tag;
                    setRandomPsvSkill(slotBtn, out randIdx, randPer, out tagName);

                    //* Set Unique or Normal
                    if(randPer < LM._.LEVELUP_SLOTS_UNIQUE_PER){
                        while(tagName == DM.TAG.PsvSkillNormal.ToString())
                            setRandomPsvSkill(slotBtn, out randIdx, randPer, out tagName);
                    }else{
                        while(tagName == DM.TAG.PsvSkillUnique.ToString())
                            setRandomPsvSkill(slotBtn, out randIdx, randPer, out tagName);
                    }

                    //* ExceptSkillListが存在したら、できるまで他に切り替える
                    bool isSkLvMax = exceptSkNameList.Exists(name => slotBtn.name.text.Contains(name));
                    while(isSkLvMax){
                        Debug.Log($"<color=yellow>BEFORE:: btn[{btnIdx}]\n, btn.skillName= {slotBtn.name.text}, randIdx= {randIdx}");
                        randIdx = ++randIdx % skillList.Count;
                        slotBtn.colImgRectTf.localPosition = new Vector3(0, skillList[randIdx].Key + spriteH / 2, 0);// Scroll Down a Half of Height PosY for Animation
                        slotBtn.name.text = skillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                        Debug.Log($"<color=yellow>AFTER:: btn.name.text= {slotBtn.name.text}, randIdx= {randIdx}</color>");
                        isSkLvMax = exceptSkNameList.Exists(name => slotBtn.name.text.Contains(name));
                    }

                    // 指定したIndexは重ならないように消す
                    skillList.RemoveAt(randIdx);
                    btnIdx++;
                    if(btnIdx == SKILL_BTN_IDX_LEN){
                        isRollingStop = true;// Scroll Stop Trigger On
                    }
                }
            }
        }
        //* #3.Scroll Re-Back to Correct PosY
        else if(isRollingStop){
            for(int i=0; i<colSkillBtns.Length;i++){
                float posY = colSkillBtns[i].colImgRectTf.localPosition.y % spriteH;
                const int offsetY = 1;
                float lastIdxPosY = (colSkillBtns[2].colImgRectTf.localPosition.y % spriteH) + offsetY;
                if(spriteH / 2 <= posY && posY <= spriteH){
                    //Scrolling Up
                    colSkillBtns[i].colImgRectTf.Translate(0, speed / (i+1), 0);
                    Debug.Log($"LevelUpPanelAnimate::Update():: lastIdxPosY:{lastIdxPosY}->MathRound:{Mathf.Round(lastIdxPosY)} == spriteHeight{spriteH - spriteH * 0.1f}");
                    //最後Idxが終わるまで待つ
                    if(Mathf.Round(lastIdxPosY) >= spriteH - spriteH * 0.1f){ //* ピッタリ止まらないので、10％周りの範囲に来たら処理することで対応。
                        isRollingStop = false;
                        // Debug.Log($"最後まで待ちました。btnImg[0].Y={colSkillBtns[0].colImgRectTf.localPosition.y}, btnImg[1].Y={colSkillBtns[1].colImgRectTf.localPosition.y}, btnImg[2].Y={colSkillBtns[2].colImgRectTf.localPosition.y}");
                        //* 少しずれた位置をピッタリ直す。
                        int btnIdx=0;
                        Array.ForEach(colSkillBtns, btn => {
                            int rest = Mathf.RoundToInt(btn.colImgRectTf.localPosition.y) % spriteH;
                            btn.colImgRectTf.localPosition = new Vector3(
                                btn.colImgRectTf.localPosition.x, 
                                btn.colImgRectTf.localPosition.y - rest + (btnIdx==2? spriteH : 0), //* なぜか、３番目には高さを一回足さないと、ずれるBUGある。
                                btn.colImgRectTf.localPosition.z);
                            btnIdx++;
                        });
                    }
                }
            }
        }
    }

//* --------------------------------------------------------------------------------------
//* GUI Button
//* --------------------------------------------------------------------------------------
    public void onClickSkillUpBtn(int index){//* Skill Level Up
        //* (BUG)曲がりが終わるまで、SkillBtnクリックできないように(スキル名が在るかを確認)。
        if(Array.Exists(colSkillBtns, btn => btn.name.text == "")) {
            time = span; // スロットが曲がるのを止める。
            return;
        }
        Debug.LogFormat("onClickSkillUpBtn({0}):: pl.Lv= {1}, name= {2}", index , pl.Lv, colSkillBtns[index].name.text);

        //* Set Data
        var psv = DM.ins.convertPsvSkillStr2Enum(colSkillBtns[index].name.text);
        switch(psv){
            case DM.PSV.Dmg:
                UpgradeDt[] upgradeArr = DM.ins.personalData.Upgrade.Arr;
                int upgradeDmg = (int)upgradeArr[(int)DM.UPGRADE.Dmg].getValue();
                //* Calc
                int val = (pl.dmg.Unit * (pl.dmg.Level+1)) + upgradeDmg;
                float plusPer = 1 + (pl.dmg.Level+1) * 0.1f;
                //* Set
                pl.dmg.setLvUp((int)(val * plusPer));
                break;
            case DM.PSV.MultiShot:
                pl.multiShot.setLvUp(pl.multiShot.Val + pl.multiShot.Unit);
                if(pl.giantBall.Level > 0){
                    float CALC_GIANTBALL_VAL = (pl.multiShot.Val + pl.verticalMultiShot.Val + pl.giantBall.Level) * pl.giantBall.Unit;
                    Debug.Log($"CALC_GIANTBALL_VAL({CALC_GIANTBALL_VAL}) -> (multi= {pl.multiShot.Val} + vMulti= {pl.verticalMultiShot.Val} + giantBall.Lv= {pl.giantBall.Level}) * giantBall.Unit={pl.giantBall.Unit}" );
                    pl.giantBall.Val = (CALC_GIANTBALL_VAL);
                }
                break;
            case DM.PSV.Speed:
                pl.speed.setLvUp(pl.speed.Val + pl.speed.Unit); //20% Up
                break;
            case DM.PSV.InstantKill:
                pl.instantKill.setLvUp(pl.instantKill.Val + pl.instantKill.Unit); //2% Up
                break;
            case DM.PSV.Critical:
                pl.critical.setLvUp(pl.critical.Val + pl.critical.Unit); //10% Up
                break;
            case DM.PSV.Explosion:
                var percent = pl.explosion.Val.per + pl.explosion.Unit.per;
                var range = pl.explosion.Val.range + pl.explosion.Unit.range;
                pl.explosion.setLvUp(new Explosion(percent, range)); //Active:20% Up, Radius:+0.25
                break;
            case DM.PSV.ExpUp:
                pl.expUp.setLvUp(pl.expUp.Val + pl.expUp.Unit);
                break;
            case DM.PSV.ItemSpawn:
                pl.itemSpawn.setLvUp(pl.itemSpawn.Val + pl.itemSpawn.Unit);
                break;
            case DM.PSV.VerticalMultiShot:
                pl.verticalMultiShot.setLvUp(pl.verticalMultiShot.Val + pl.verticalMultiShot.Unit);
                if(pl.giantBall.Level > 0){
                    float CALC_GIANTBALL_VAL = (pl.multiShot.Val + pl.verticalMultiShot.Val + pl.giantBall.Level) * pl.giantBall.Unit;
                    Debug.Log($"CALC_GIANTBALL_VAL({CALC_GIANTBALL_VAL}) -> (multi= {pl.multiShot.Val} + vMulti= {pl.verticalMultiShot.Val} + giantBall.Lv= {pl.giantBall.Level}) * giantBall.Unit={pl.giantBall.Unit}" );
                    pl.giantBall.Val = (CALC_GIANTBALL_VAL);
                }
                break;
            case DM.PSV.Laser:
                pl.laser.setLvUp(pl.laser.Val + pl.laser.Unit);
                break;
            case DM.PSV.FireProperty:
                pl.fireProperty.setLvUp(pl.fireProperty.Val + pl.fireProperty.Unit);
                break;
            case DM.PSV.IceProperty:
                pl.iceProperty.setLvUp(pl.iceProperty.Val + pl.iceProperty.Unit);
                break;
            case DM.PSV.ThunderProperty:
                pl.thunderProperty.setLvUp(pl.thunderProperty.Val + pl.thunderProperty.Unit);
                break;
            //* Unique-------------------------------------------------------------------------------------
            case DM.PSV.DamageTwice:
                pl.damageTwice.setLvUp(pl.damageTwice.Val + pl.damageTwice.Unit);
                break;
            case DM.PSV.GiantBall:{ //* (BUG-5) レベルアップする前に処理されるので、０になるバグ対応。
                float CALC_GIANTBALL_VAL = (pl.multiShot.Val + pl.verticalMultiShot.Val + 1) * pl.giantBall.Unit;
                Debug.Log($"CALC_GIANTBALL_VAL({CALC_GIANTBALL_VAL}) -> (multi= {pl.multiShot.Val} + vMulti= {pl.verticalMultiShot.Val} + {1}) * giantBall.Unit={pl.giantBall.Unit}" );
                pl.giantBall.setLvUp(CALC_GIANTBALL_VAL);
            }
                break;
            case DM.PSV.DarkOrb:
                pl.darkOrb.setLvUp(pl.darkOrb.Val + pl.darkOrb.Unit);
                break;
            case DM.PSV.GodBless:
                pl.godBless.setLvUp(pl.godBless.Val + pl.godBless.Unit);
                break;
            case DM.PSV.BirdFriend:
                pl.birdFriend.setLvUp(pl.birdFriend.Val + pl.birdFriend.Unit);
                break;
        }
        
        //* 終了
        if(!isPsvSkillTicket) //* (BUG-14) LevelUpPanelAnimate:: isPsvSkillTicket変数を生成、PSVチケットの場合は、befLv++しない。
            pl.BefLv++;
        else
            isPsvSkillTicket = false;
        // Debug.LogFormat("onClickSkillUpBtn({0}):: <color=yellow> pl.Lv= {1}, pl.befLv= {2}</color>",index, pl.Lv, pl.BefLv);

        this.gameObject.SetActive(false);
        gm.displayCurPassiveSkillUI("INGAME");
        
        //* １回以上 LEVEL-UPした場合、順番に進む
        if(pl.BefLv != pl.Lv){
            pl.Lv--;
            pl.setLevelUp();
            gm.coCheckLevelUp();
        }
    }

/// --------------------------------------------------------------------------------------
/// 関数
/// --------------------------------------------------------------------------------------
    private string setRandomPsvSkill(LevelUpSkillPanelBtn slotBtn, out int randIdx, int randPer, out string tagName){
        randIdx = Random.Range(0, skillList.Count);

        //* Set Position (.Key -> PosY)
        int halfHeightMorePosY = skillList[randIdx].Key + spriteH / 2; 
        slotBtn.colImgRectTf.localPosition = new Vector3(0, halfHeightMorePosY, 0);

        //* Set Name 
        string skillName = skillList[randIdx].Value.name.Split(char.Parse("_"))[1];
        tagName = skillList[randIdx].Value.transform.tag;
        slotBtn.name.text = LANG.getTxt(skillName);

        Debug.Log("LevelUpPanelAnimate Skill Slots Stop:: " + slotBtn.colImgRectTf.parent.name
            + ":: randPer(" + randPer + ") : " + (randPer < LM._.LEVELUP_SLOTS_UNIQUE_PER? "<color=red>UNIQUE!</color>" : "GENERAL")
            + " ➡ <b>" + skillName + " </b>" 
            + " ,Tag= " + (tagName == DM.TAG.PsvSkillUnique.ToString()? "<color=red>" + tagName + "</color>" : tagName)
        );
        return tagName;
    }

#region Start()メソッド
    public void setUI(string type){
        SM.ins.sfxPlay(SM.SFX.LevelUpPanel.ToString());
        explainTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Explain.ToString());

        bool isLvUp = (type == DM.NAME.LevelUp.ToString());
        rerotateBtn.gameObject.SetActive(isLvUp);
        centerMarkImg.color = (isLvUp)? Color.white : new Color(0,1,1);
        centerWingImg.color = (isLvUp)? Color.white : new Color(0,1,1);
        levelTxt.text = (isLvUp)? (pl.BefLv + 1).ToString()
            : LANG.getTxt(LANG.TXT.Reward.ToString());
        titleTxt.text = (isLvUp)? LANG.getTxt(LANG.TXT.LevelUpPanel_Title.ToString())
            : LANG.getTxt(LANG.TXT.PsvSkillTicket.ToString());
        
        
    }
    private void resetSkillImgChild(){
        if(0 < colSkillBtns[0].colImgRectTf.childCount){
            foreach(var btn in colSkillBtns){
                btn.name.text = "";
                foreach(RectTransform befChild in btn.colImgRectTf)
                    Destroy(befChild.gameObject);
            }
        }
    }
    private void setSkillList(List<KeyValuePair<int, GameObject>> selectSkillList){
        for(int i = 0; i < skillImgCnt ; i++){
            int posY = spriteH * i;
            selectSkillList.Add( new KeyValuePair<int, GameObject>(posY, gm.PsvSkillImgPrefs[i+1]));
            // Debug.LogFormat("selectList[{0}].key= {1}, .value= {2}", i, selectSkillList[i].Key, selectSkillList[i].Value);
        }
    }
    private void setExceptList(){
        var psvLvList = PsvSkill<int>.getPsvLVList(pl);
        psvLvList.ForEach(lv => {
            var maxLv = PsvSkill<int>.getPsvMaxLVList(pl).Find(maxLv => (maxLv.Key == lv.Key));
            if(lv.Value >= maxLv.Value)
                exceptSkNameList.Add(lv.Key);
        });
        exceptSkNameList.ForEach(name => Debug.Log("exceptSkNameList= <color=red>" + name + "</color>"));
    }
    private void createColSprite(){
        foreach(var btn in colSkillBtns){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var psvSkImg in gm.PsvSkillImgPrefs){
                Instantiate(psvSkImg, Vector3.zero, Quaternion.identity, btn.colImgRectTf);
            }
            //Set Auto Scroll Start Pos
            btn.colImgRectTf.localPosition = new Vector3(0, spriteH * skillImgCnt, 0);
        }
    }
#endregion
}