using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;

public struct SkillBtn
{
    public RectTransform rowImgRectTf;
    public Text name;

    public SkillBtn(Button skillBtn){//Contructor
        rowImgRectTf = skillBtn.transform.GetChild(0).GetComponent<RectTransform>();
        name = skillBtn.transform.GetChild(1).GetComponent<Text>();
    }
}

public class LevelUpPanelAnimate : MonoBehaviour
{
    //* OutSide
    GameManager gm; Player pl;

    //* Value
    int spriteHeight;
    int btnIdx;
    float time;
    float span;
    int skillImgCnt;

    [Header("STATE")]
    [SerializeField] int scrollingSpeed;
    [SerializeField] bool isRollingStop = false;
    [SerializeField] List<string> exceptSkNameList = new List<string>();
    public List<KeyValuePair<int, GameObject>> selectSkillList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。

    [Header("GUI")]
    public Text titleTxt;
    public Text levelTxt;
    public Text explainTxt;
    public Button[] skillBtns;
    public SkillBtn[] SkillBtns; //struct宣言

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

        //* GUI
        titleTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Title.ToString());
        levelTxt.text = LANG.getTxt(LANG.TXT.Level.ToString()) + " : " + (pl.BefLv + 1).ToString();
        explainTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Explain.ToString());

        //* Set SkillBtns
        SkillBtns = new SkillBtn[3]{
            new SkillBtn(skillBtns[0]), new SkillBtn(skillBtns[1]), new SkillBtn(skillBtns[2])
        };
        
        //* Init Child Obj Remove All
        if(0 < SkillBtns[0].rowImgRectTf.childCount){
            foreach(var btn in SkillBtns){
                btn.name.text = "";
                foreach(RectTransform befChild in btn.rowImgRectTf)
                    Destroy(befChild.gameObject);
            }
        }

        //* Init Select SkillList
        skillImgCnt = gm.PsvSkillImgPrefs.Length - 1; //* ★ 自然なスクロールのため、末端に1番目Imageを追加したので、この分は消す-1。
        selectSkillList = new List<KeyValuePair<int, GameObject>>();
        for(int i = 0; i < skillImgCnt ; i++){
            int posY = spriteHeight * i;
            selectSkillList.Add( new KeyValuePair<int, GameObject>(posY, gm.PsvSkillImgPrefs[i+1]));
            Debug.LogFormat("selectList[{0}].key= {1}, .value= {2}", i, selectSkillList[i].Key, selectSkillList[i].Value);
        }

        //* Init ExceptSkillList (Max Lv)
        var psvLvList = PsvSkill<int>.getPsvLVList(pl);
        psvLvList.ForEach(lv => {
            var maxLv = PsvSkill<int>.getPsvMaxLVList(pl).Find(maxLv => (maxLv.Key == lv.Key));
            if(lv.Value >= maxLv.Value)
                exceptSkNameList.Add(lv.Key);
        });
        exceptSkNameList.ForEach(name => Debug.Log("exceptSkNameList= <color=red>" + name + "</color>"));

        //* Create Scroll Row Sprite Imgs
        foreach(var btn in SkillBtns){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var psvSkImg in gm.PsvSkillImgPrefs){
                Instantiate(psvSkImg, Vector3.zero, Quaternion.identity, btn.rowImgRectTf);
            }
            //Set Auto Scroll Start Pos
            btn.rowImgRectTf.localPosition = new Vector3(0, spriteHeight * skillImgCnt, 0);
        }
    }

    void Update(){
        float speed = scrollingSpeed * Time.unscaledDeltaTime;
        //* SCROLL ANIMATION
        if(2 >= btnIdx && selectSkillList.Count > 0){
            time += Time.unscaledDeltaTime;
            foreach(var btn in SkillBtns){
                //* #1.Scrolling Down
                if(time < span){
                    if(btn.rowImgRectTf.localPosition.y >= 0){ //* ↓
                        btn.rowImgRectTf.Translate(0,-speed, 0);
                    }
                    else{ //* 位置初期化
                        btn.rowImgRectTf.localPosition = new Vector3(0, spriteHeight * skillImgCnt, 0);
                    }
                }
                //* #2.Stop
                else{
                    int randIdx = Random.Range(0, selectSkillList.Count);
                    // Position (.Key -> PosY)
                    btn.rowImgRectTf.localPosition = new Vector3(0, selectSkillList[randIdx].Key + spriteHeight / 2, 0);
                    // Skill Name
                    string skillName = selectSkillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                    btn.name.text = LANG.getTxt(skillName);

                    // ExceptSkillListが存在したら、できるまで他に切り替える
                    bool isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
                    while(isSkLvMax){
                        Debug.Log($"<color=yellow>BEFORE:: btn[{btnIdx}]\n, btn.skillName= {btn.name.text}, randIdx= {randIdx}");
                        randIdx = ++randIdx % selectSkillList.Count;
                        btn.rowImgRectTf.localPosition = new Vector3(0, selectSkillList[randIdx].Key + spriteHeight / 2, 0);// Scroll Down a Half of Height PosY for Animation
                        btn.name.text = selectSkillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                        Debug.Log($"<color=yellow>AFTER:: btn.name.text= {btn.name.text}, randIdx= {randIdx}</color>");
                        isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
                    }

                    // 指定したIndexは重ならないように消す
                    selectSkillList.RemoveAt(randIdx);
                    btnIdx++;
                    if(btnIdx == 2){
                        isRollingStop = true;// Scroll Stop Trigger On
                    }
                }
            }
        }
        //* #3.Scroll up to right posY
        else if(isRollingStop){
            for(int i=0; i<SkillBtns.Length;i++){
                float posY = SkillBtns[i].rowImgRectTf.localPosition.y % spriteHeight;
                float lastIdxPosY = SkillBtns[2].rowImgRectTf.localPosition.y % spriteHeight;
                if(spriteHeight / 2 <= posY && posY <= spriteHeight){
                    //Scrolling Up
                    SkillBtns[i].rowImgRectTf.Translate(0, speed / (i+1), 0);
                    //最後Idxが終わるまで待つ
                    if(Mathf.RoundToInt(lastIdxPosY) == spriteHeight) 
                        isRollingStop = false;
                }
            }
        }
    }

    //* GUI Button
    public void onClickSkillUpBtn(int index){//* Skill Level Up
        //* (BUG)曲がりが終わるまで、SkillBtnクリックできないように(スキル名が在るかを確認)。
        if(Array.Exists(SkillBtns, btn => btn.name.text == "")) {
            time = span; // スロットが曲がるのを止める。
            return;
        }
        Debug.LogFormat("onClickSkillUpBtn({0}):: pl.Lv= {1}, name= {2}", index , pl.Lv, SkillBtns[index].name.text);

        //* Set Data
        var psv = DM.ins.convertPsvSkillStr2Enum(SkillBtns[index].name.text);
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
}