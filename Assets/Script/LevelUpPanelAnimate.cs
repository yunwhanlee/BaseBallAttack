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
    int spriteHeight;
    int btnIdx;
    float time;
    float span;
    int skillImgCnt;

    [Header("STATE")]
    [SerializeField] int scrollingSpeed;
    [SerializeField] bool isRollingStop = false;
    [SerializeField] List<string> exceptSkNameList = new List<string>();
    public List<KeyValuePair<int, GameObject>> skillList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。

    [Header("GUI")]
    [FormerlySerializedAs("titleTxt")] public Text titleTxt;
    [FormerlySerializedAs("levelTxt")] public Text levelTxt;
    [FormerlySerializedAs("explainTxt")] public Text explainTxt;
    [FormerlySerializedAs("colSkillBtns")] public LevelUpSkillPanelBtn[] colSkillBtns;

    private void OnEnable() => Time.timeScale = 0; //* このパンネルが出たら、政界の時間 停止
    private void OnDisable() => Time.timeScale = 1; //* このパンネルが消えたら、政界の時間 戻す

    public void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pl = gm.pl;

        //* Init Value
        spriteHeight = (int)gm.PsvSkillImgPrefs[0].GetComponent<RectTransform>().rect.width;
        time = 0;
        span = 2;
        btnIdx = 0;
        isRollingStop = false;
        exceptSkNameList = new List<string>();
        skillImgCnt = gm.PsvSkillImgPrefs.Length - 1; //* ★自然なスクロールのため、末端に1番目Imageを追加したので、この分は消す-1。
        skillList = new List<KeyValuePair<int, GameObject>>();

        //* GUI
        titleTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Title.ToString());
        levelTxt.text = LANG.getTxt(LANG.TXT.Level.ToString()) + " : " + (pl.BefLv + 1).ToString();
        explainTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Explain.ToString());

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
            foreach(var btn in colSkillBtns){
                //* #1.Scrolling Down
                if(time < span){
                    if(btn.colImgRectTf.localPosition.y >= 0) //* ↓
                        btn.colImgRectTf.Translate(0,-speed, 0);
                    else //* 位置初期化
                        btn.colImgRectTf.localPosition = new Vector3(0, spriteHeight * skillImgCnt, 0);
                }
                //* #2.Stop
                else{
                    int randIdx = Random.Range(0, skillList.Count);
                    int halfHeightMorePosY = skillList[randIdx].Key + spriteHeight / 2;
                    // Position (.Key -> PosY)
                    btn.colImgRectTf.localPosition = new Vector3(0, halfHeightMorePosY, 0);
                    // Skill Name
                    string skillName = skillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                    btn.name.text = LANG.getTxt(skillName);

                    // ExceptSkillListが存在したら、できるまで他に切り替える
                    bool isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
                    while(isSkLvMax){
                        Debug.Log($"<color=yellow>BEFORE:: btn[{btnIdx}]\n, btn.skillName= {btn.name.text}, randIdx= {randIdx}");
                        randIdx = ++randIdx % skillList.Count;
                        btn.colImgRectTf.localPosition = new Vector3(0, skillList[randIdx].Key + spriteHeight / 2, 0);// Scroll Down a Half of Height PosY for Animation
                        btn.name.text = skillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                        Debug.Log($"<color=yellow>AFTER:: btn.name.text= {btn.name.text}, randIdx= {randIdx}</color>");
                        isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
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
        //* #3.Scroll Up To Right PosY
        else if(isRollingStop){
            for(int i=0; i<colSkillBtns.Length;i++){
                float posY = colSkillBtns[i].colImgRectTf.localPosition.y % spriteHeight;
                float lastIdxPosY = colSkillBtns[2].colImgRectTf.localPosition.y % spriteHeight;
                if(spriteHeight / 2 <= posY && posY <= spriteHeight){
                    //Scrolling Up
                    colSkillBtns[i].colImgRectTf.Translate(0, speed / (i+1), 0);
                    //最後Idxが終わるまで待つ
                    if(Mathf.RoundToInt(lastIdxPosY) == spriteHeight) 
                        isRollingStop = false;
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
                pl.dmg.setLvUp(pl.dmg.Value + pl.dmg.Unit);
                break;
            case DM.PSV.MultiShot:
                pl.multiShot.setLvUp(pl.multiShot.Value + pl.multiShot.Unit);
                Debug.Log(pl.multiShot.Level + pl.multiShot.Value);
                break;
            case DM.PSV.Speed:
                pl.speed.setLvUp(pl.speed.Value + pl.speed.Unit); //20% Up
                break;
            case DM.PSV.InstantKill:
                pl.instantKill.setLvUp(pl.instantKill.Value + pl.instantKill.Unit); //2% Up
                break;
            case DM.PSV.Critical:
                pl.critical.setLvUp(pl.critical.Value + pl.critical.Unit); //10% Up
                break;
            case DM.PSV.Explosion:
                var percent = pl.explosion.Value.per + pl.explosion.Unit.per;
                var range = pl.explosion.Value.range + pl.explosion.Unit.range;
                pl.explosion.setLvUp(new Explosion(percent, range)); //Active:20% Up, Radius:+0.25
                break;
            case DM.PSV.ExpUp:
                pl.expUp.setLvUp(pl.expUp.Value + pl.expUp.Unit);
                break;
            case DM.PSV.ItemSpawn:
                pl.itemSpawn.setLvUp(pl.itemSpawn.Value + pl.itemSpawn.Unit);
                break;
            case DM.PSV.VerticalMultiShot:
                pl.verticalMultiShot.setLvUp(pl.verticalMultiShot.Value + pl.verticalMultiShot.Unit);
                break;
            case DM.PSV.Laser:
                pl.laser.setLvUp(pl.laser.Value + pl.laser.Unit);
                break;
        }
        
        //* 終了
        pl.BefLv++;
        // Debug.LogFormat("onClickSkillUpBtn({0}):: <color=yellow> pl.Lv= {1}, pl.befLv= {2}</color>",index, pl.Lv, pl.BefLv);

        this.gameObject.SetActive(false);
        gm.displayCurPassiveSkillUI("INGAME");
        
        //* １回以上 LEVEL-UPした場合、順番に進む
        if(pl.BefLv != pl.Lv){
            pl.Lv--;
            pl.setLevelUp();
            gm.checkLevelUp();
        }
    }

//* --------------------------------------------------------------------------------------
//* 関数
//* --------------------------------------------------------------------------------------
#region Start()メソッド
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
            int posY = spriteHeight * i;
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
            btn.colImgRectTf.localPosition = new Vector3(0, spriteHeight * skillImgCnt, 0);
        }
    }
#endregion
}