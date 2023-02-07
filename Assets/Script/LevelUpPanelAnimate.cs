using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Serialization;

[System.Serializable]
public struct LevelUpSkillPanelBtn{
    public RectTransform contentRectTf;
    public Text name;
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
    // int skillImgCnt;
    
    [Header("STATE")][Header("__________________________")]
    [SerializeField] int scrollSpeed;
    // [SerializeField] bool isRollingStop = false;
    [SerializeField] bool isPsvSkillTicket; public bool IsPsvSkillTicket { get => isPsvSkillTicket; set => isPsvSkillTicket = value;}
    [SerializeField] List<string> exceptSkNameList = new List<string>();
    [SerializeField] public List<KeyValuePair<int, GameObject>> skillList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。

    [Header("GUI")][Header("__________________________")]
    [FormerlySerializedAs("titleTxt")] public Text titleTxt;
    [FormerlySerializedAs("levelTxt")] public Text levelTxt;
    [FormerlySerializedAs("explainTxt")] public Text explainTxt;
    [FormerlySerializedAs("colSkillBtns")] public LevelUpSkillPanelBtn[] skillSlotBtns;
    [FormerlySerializedAs("centerWingImg")] public Image centerWingImg;
    [FormerlySerializedAs("centerMarkImg")] public Image centerMarkImg;
    [FormerlySerializedAs("rerotateBtn")] public Button rerotateBtn;

    private void OnEnable() => Time.timeScale = 0; //* このパンネルが出たら、政界の時間 停止
    private void OnDisable() => Time.timeScale = 1; //* このパンネルが消えたら、政界の時間 戻す

    public void Start(){
        Debug.Log("LevelUpPanelAnimate::Start() DM.ins.gm= " + DM.ins.gm);
        gm = DM.ins.gm;
        pl = gm.pl;

        for(int i=0; i<gm.PsvImgRectTfTemp.childCount; i++){
            Debug.Log($"gm.PsvImgRectTfTemp.getChild({i}) -> " + gm.PsvImgRectTfTemp.GetChild(i));
        };

        //* Init Value
        scrollSpeed = 2500;
        spriteH = (int)gm.PsvSkillImgPrefs[0].GetComponent<RectTransform>().rect.height; //270
        btnIdx = 0;
        time = 0;
        span = 2;
        // isRollingStop = false;
        exceptSkNameList = new List<string>();
        skillList = new List<KeyValuePair<int, GameObject>>();

        //* Set GUI Data
        setUI(DM.NAME.LevelUp.ToString());

        //* スロット
        clearSlotSkillImgs(); //* Reset Before Child : これがないと、クリックでスクロール止まらなく選択されてしまう。
        exceptMaxLvSkill();
        createSlotSkillImgs();

        //* スロットへ配置するSkillList作成
        setSkillList(skillList);
    }

    void Update(){
        float speed = scrollSpeed * Time.unscaledDeltaTime;
        
        //* SCROLL ANIMATION
        if(SKILL_BTN_IDX_LEN >= btnIdx && skillList.Count > 0){
            time += Time.unscaledDeltaTime;
            for(int i = 0; i < skillSlotBtns.Length; i++){
                //* Scrolling
                if(time < span){
                    if(skillSlotBtns[i].contentRectTf.localPosition.y >= 0) //* ↓
                        skillSlotBtns[i].contentRectTf.Translate(0,-speed, 0);
                    else //* 一番目に戻す
                        skillSlotBtns[i].contentRectTf.localPosition = new Vector3(0, spriteH * gm.psvImgRectTfTemp.childCount, 0);
                }
                //* Stop
                else{
                    Debug.Log("LevelUpPanelAnimate::Slot Stop!");
                    int randIdx = Random.Range(0, skillList.Count);
                    int randPer = Random.Range(0, 100);
                    string tagName = skillList[randIdx - 1].Value.transform.tag;
                    setRandomPsvSkillSlot(skillSlotBtns[i], out randIdx, randPer, out tagName);

                    //* Set Unique or Normal
                    if(randPer < LM._.LEVELUP_SLOTS_UNIQUE_PER){
                        while(tagName == DM.TAG.PsvSkillNormal.ToString())
                            setRandomPsvSkillSlot(skillSlotBtns[i], out randIdx, randPer, out tagName);
                    }else{
                        while(tagName == DM.TAG.PsvSkillUnique.ToString())
                            setRandomPsvSkillSlot(skillSlotBtns[i], out randIdx, randPer, out tagName);
                    }

                    //* 指定したIndexは他のスロットと重ならないように消す
                    skillList.RemoveAt(randIdx);
                    btnIdx++;

                    // if(btnIdx == SKILL_BTN_IDX_LEN)
                        // isRollingStop = true; //* Stop Trigger On
                }
            }
        }
        //! (BUG-48) モバイルビルドで、LevelUpScrollBtnの位置がずれるバグがありため、半分下に位置して上に戻す処理しない。
        /*
        //* #3.Scroll Re-Back to Correct PosY
        // else if(isRollingStop){
        //     Debug.Log("LevelUpPanelAnimate::isRollingStop");
        //     for(int i = 0; i<skillSlotBtns.Length;i++){
        //         float posY = skillSlotBtns[i].contentRectTf.localPosition.y % spriteH;
        //         const int offsetY = 1;
        //         float lastIdxPosY = (skillSlotBtns[2].contentRectTf.localPosition.y % spriteH) + offsetY;
        //         if(spriteH / 2 <= posY && posY <= spriteH){
        //             //Scrolling Up
        //             skillSlotBtns[i].contentRectTf.Translate(0, speed / (i+1), 0);
        //             Debug.Log($"LevelUpPanelAnimate::Update():: lastIdxPosY:{lastIdxPosY}->MathRound:{Mathf.Round(lastIdxPosY)} == spriteHeight{spriteH - spriteH * 0.1f}");
        //             //最後Idxが終わるまで待つ
        //             if(Mathf.Round(lastIdxPosY) >= spriteH - spriteH * 0.1f){ //* ピッタリ止まらないので、10％周りの範囲に来たら処理することで対応。
        //                 isRollingStop = false;
        //                 // Debug.Log($"最後まで待ちました。btnImg[0].Y={colSkillBtns[0].colImgRectTf.localPosition.y}, btnImg[1].Y={colSkillBtns[1].colImgRectTf.localPosition.y}, btnImg[2].Y={colSkillBtns[2].colImgRectTf.localPosition.y}");
        //                 //* 少しずれた位置をピッタリ直す。
        //                 int btnIdx=0;
        //                 Array.ForEach(skillSlotBtns, btn => {
        //                     int rest = Mathf.RoundToInt(btn.contentRectTf.localPosition.y) % spriteH;
        //                     btn.contentRectTf.localPosition = new Vector3(
        //                         btn.contentRectTf.localPosition.x, 
        //                         btn.contentRectTf.localPosition.y - rest + (btnIdx==2? spriteH : 0), //* なぜか、３番目には高さを一回足さないと、ずれるBUGある。
        //                         btn.contentRectTf.localPosition.z);
        //                     btnIdx++;
        //                 });
        //             }
        //         }
        //     }
        // }
        */
    }
//* --------------------------------------------------------------------------------------
//* GUI Button
//* --------------------------------------------------------------------------------------
    private bool stopScrolling(){
        //* (BUG)曲がりが終わるまで、SkillBtnクリックできないように(スキル名が在るかを確認)。
        if(Array.Exists(skillSlotBtns, btn => btn.name.text == "")) {
            // isRollingStop = true;
            time = span; // スロットが曲がるのを止める。
            return true;
        }
        return false;
    }
    public void onClickPanelStopScrolling(){
        stopScrolling();
    }
    public void onClickSkillUpBtn(int index){//* Skill Level Up
        if(stopScrolling()) return; //* スロットを止めることなら、以下の処理をしない。

        Debug.LogFormat("<color=green>onClickSkillUpBtn({0}):: pl.Lv= {1}, name= {2}</color>", index , pl.Lv, skillSlotBtns[index].name.text);
        //* Set Data
        var psv = DM.ins.convertPsvSkillStr2Enum(skillSlotBtns[index].name.text);
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
            case DM.PSV.CriticalDamage:
                pl.criticalDamage.setLvUp(pl.criticalDamage.Val + pl.criticalDamage.Unit);
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
    private string setRandomPsvSkillSlot(LevelUpSkillPanelBtn slotBtn, out int randIdx, int randPer, out string tagName){
        randIdx = Random.Range(0, skillList.Count);
        Debug.Log("setRandomPsvSkill:: randIdx= " + randIdx);

        //* Set Position (.Key -> PosY)
        // int spriteHalfHeight = spriteH / 2;
        int posY = skillList[randIdx].Key + spriteH;// + spriteHalfHeight; 
        slotBtn.contentRectTf.localPosition = new Vector3(0, posY, 0);

        //* Set Name 
        string skillName = skillList[randIdx].Value.name.Split(char.Parse("_"))[1];
        tagName = skillList[randIdx].Value.transform.tag;
        slotBtn.name.text = LANG.getTxt(skillName);

        Debug.Log("LevelUpPanelAnimate Skill Slots Stop:: " + slotBtn.contentRectTf.parent.name
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
    private void clearSlotSkillImgs(){
        if(0 < skillSlotBtns[0].contentRectTf.childCount){
            foreach(var btn in skillSlotBtns){
                btn.name.text = "";
                foreach(RectTransform child in btn.contentRectTf)
                    Destroy(child.gameObject);
            }
        }
    }
    private void exceptMaxLvSkill(){
        var psvLvList = PsvSkill<int>.getPsvLVList(pl);
        //* Except List 作成
        psvLvList.ForEach(lv => {
            var maxLv = PsvSkill<int>.getPsvMaxLVList(pl).Find(maxLv => (maxLv.Key == lv.Key));
            if(lv.Value >= maxLv.Value){
                string curLangSkillName = lv.Key;
                exceptSkNameList.Add(curLangSkillName);
            }
        });

        //* Except Max Level List
        exceptSkNameList.ForEach(exceptSkill => {
            for(int i = 0; i<gm.PsvImgRectTfTemp.childCount; i++){
                if(i == 0) continue;
                
                string psvSkillName = gm.PsvImgRectTfTemp.GetChild(i).name.Split('_')[1];
                string psvSkill = LANG.getTxt(psvSkillName); // exceptSkillと言語合わせる。
                if(exceptSkill == psvSkill){
                    Debug.Log($"ExceptMaxLv:: {exceptSkill} == {psvSkill} : <color=red>{exceptSkill == psvSkill}</color>");

                    //! Destroy()はフレームの末端に行うので、次の Result処理で確認しても反映されていない。
                    //* 同じフレームで行うことであれば、即時破壊にする必要がある。
                    DestroyImmediate(gm.PsvImgRectTfTemp.GetChild(i).gameObject); 
                }
            };
        });

        //* Result
        for(int i=0; i<gm.PsvImgRectTfTemp.childCount; i++)
            Debug.Log($"Result gm.PsvImgRectTfTemp -> " + gm.PsvImgRectTfTemp.GetChild(i).name);
    }
    private void createSlotSkillImgs(){
        for(int i=0; i<gm.PsvImgRectTfTemp.childCount; i++){
            var psvSkill = gm.PsvImgRectTfTemp.GetChild(i);
            Instantiate(psvSkill, skillSlotBtns[0].contentRectTf);
            Instantiate(psvSkill, skillSlotBtns[1].contentRectTf);
            Instantiate(psvSkill, skillSlotBtns[2].contentRectTf);
        }
    }
    private void setSkillList(List<KeyValuePair<int, GameObject>> skillList){
        for(int i = 0; i < gm.psvImgRectTfTemp.childCount - 1; i++){
            int posY = spriteH * i;
            var child = gm.PsvImgRectTfTemp.GetChild(i+1).gameObject;
            skillList.Add(new KeyValuePair<int, GameObject>(posY, child));
            Debug.LogFormat("setSkillList:: skillList[{0}].key= {1}, .value= <color=white>{2}</color>", i, skillList[i].Key, skillList[i].Value.name);
        }
    }
#endregion
}