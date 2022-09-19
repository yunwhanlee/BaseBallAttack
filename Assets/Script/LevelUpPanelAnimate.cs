using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;

public class LevelUpPanelAnimate : MonoBehaviour
{
    //* OutSide Component
    public GameManager gm;
    public Player pl;

    private const int SPRITE_W = 270; //Heightも同一
    private int btnIdx;
    private float time;

    private int skillImgCnt;
    public List<KeyValuePair<int, GameObject>> selectSkillList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。
    [SerializeField] private List<string> exceptSkNameList = new List<string>();
    public int scrollingSpeed;
    public bool isRollingStop = false;

    [Header("<---- GUI ---->")]
    public Text titleTxt;
    public Text levelTxt;
    public Text explainTxt;
    public Button[] skillBtns;
    //[System.Serializable] -> Show Inspector View

    public struct SkillBtn{
        public RectTransform imgRectTf;
        public Text name;

        public SkillBtn(Button skillBtn){//Contructor
            imgRectTf = skillBtn.transform.GetChild(0).GetComponent<RectTransform>();
            name = skillBtn.transform.GetChild(1).GetComponent<Text>();
        }
    }
    public SkillBtn[] SkillBtns; //struct宣言



    public void Start(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //Init Value
        time = 0;
        btnIdx = 0;
        isRollingStop = false;
        exceptSkNameList = new List<string>();
        
        //* GUI
        titleTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Title.ToString());
        levelTxt.text = LANG.getTxt(LANG.TXT.Level.ToString()) + " : " + (pl.BefLv + 1).ToString();
        explainTxt.text = LANG.getTxt(LANG.TXT.LevelUpPanel_Explain.ToString());

        //* Set SkillBtn
        SkillBtns = new SkillBtn[3]{
            new SkillBtn(skillBtns[0]), new SkillBtn(skillBtns[1]), new SkillBtn(skillBtns[2])
        };

        //* Init Child Obj Remove All
        if(0 < SkillBtns[0].imgRectTf.childCount){
            foreach(var btn in SkillBtns){
                btn.name.text = "";
                foreach(RectTransform befChild in btn.imgRectTf)
                    Destroy(befChild.gameObject);
            }
        }

        //Init SelectList
        selectSkillList = new List<KeyValuePair<int, GameObject>>();

        //* Set SelectList
        skillImgCnt = gm.PsvSkillImgPrefs.Length - 1; //* ★ 自然なスクロールのため、末端に1番目Imageを追加したので、この分は消す-1。

        var psvLvList = PsvSkill<int>.getPsvLVList(pl);
        
        psvLvList.ForEach(lv => {
            var maxLv = PsvSkill<int>.getPsvMaxLVList(pl).Find(maxLv => (maxLv.Key == lv.Key));
            if(lv.Value >= maxLv.Value){
                exceptSkNameList.Add(lv.Key);
            }
        });

        exceptSkNameList.ForEach(name => {
            Debug.Log("exceptSkNameList= <color=red>" + name + "</color>");
        });
        
        for(int i=0;i<skillImgCnt;i++){ 
            int pos = SPRITE_W * i;
            selectSkillList.Add( new KeyValuePair<int, GameObject>(pos, gm.PsvSkillImgPrefs[i+1]));
            Debug.LogFormat("selectList[{0}].key= {1}, .value= {2}", i, selectSkillList[i].Key, selectSkillList[i].Value);
        }
        
        //* Set Scroll Sprite Imgs
        foreach(var btn in SkillBtns){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var psvSkImg in gm.PsvSkillImgPrefs){
                Instantiate(psvSkImg, Vector3.zero, Quaternion.identity, btn.imgRectTf);
            }
                
            //Set Auto Scroll Start Pos
            btn.imgRectTf.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
        }
    }

    void Update(){
        //* Scroll Sprite Animation
        if(2 >= btnIdx && selectSkillList.Count > 0){
            time += Time.deltaTime;
            foreach(var btn in SkillBtns){
                // #1.Scrolling
                if(time < 2){
                    if(btn.imgRectTf.localPosition.y >= 0){
                        btn.imgRectTf.Translate(0,-Time.deltaTime * scrollingSpeed, 0);
                    }
                    else{
                        btn.imgRectTf.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
                    }
                }
                // #2.Stop
                else{
                    int randIdx = Random.Range(0, selectSkillList.Count);
                    btn.imgRectTf.localPosition = new Vector3(0, selectSkillList[randIdx].Key + SPRITE_W / 2, 0);// Scroll Down a Half of Height PosY for Animation
                    string skillName = selectSkillList[randIdx].Value.name.Split(char.Parse("_"))[1];
                    btn.name.text = LANG.getTxt(skillName);

                    //* MAXレベールだったら、できるまで他に切り替える。
                    bool isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
                    while(isSkLvMax){
                        Debug.LogFormat("<color=yellow>BEF: btn[{0}]-----------------------\n, btn.skillName= {1}, randIdx= {2}", btnIdx, btn.name.text, randIdx);

                        randIdx = ++randIdx % selectSkillList.Count;
                        btn.imgRectTf.localPosition = new Vector3(0, selectSkillList[randIdx].Key + SPRITE_W / 2, 0);// Scroll Down a Half of Height PosY for Animation
                        btn.name.text = selectSkillList[randIdx].Value.name.Split(char.Parse("_"))[1];

                        Debug.Log("<color=yellow>AFT: btn.name.text= " + btn.name.text + "randIdx= " + randIdx + "</color>");
                        isSkLvMax = exceptSkNameList.Exists(name => btn.name.text.Contains(name));
                    }

                    selectSkillList.RemoveAt(randIdx);
                    btnIdx++;
                    if(btnIdx == 2){
                        isRollingStop = true;
                    }
                }
            }
        }
        // #3.Animation: Scroll Up to Correct PosY
        else if(isRollingStop){
            int speed = scrollingSpeed / 2;
            for(int i=0; i<SkillBtns.Length;i++){
                float posY = SkillBtns[i].imgRectTf.localPosition.y % SPRITE_W;
                float lastIdxPosY = SkillBtns[2].imgRectTf.localPosition.y % SPRITE_W;
                if(SPRITE_W/2 <= posY && posY <= SPRITE_W){
                    //Scroll Up
                    SkillBtns[i].imgRectTf.Translate(0, (speed / (i+1)) * Time.deltaTime, 0);
                    //最後Idxが終わるまで待つ
                    if(Mathf.RoundToInt(lastIdxPosY) == SPRITE_W) isRollingStop = false;
                }
            }
        }
    }

    //* GUI Button
    public void onClickSkillUpBtn(int index){//* Skill Level Up
        //* (BUG)曲がりが終わるまで、SkillBtnクリックできないように(スキル名が在るかを確認)。
        if(Array.Exists(SkillBtns, btn => btn.name.text == "")) {
            time = 2;// スロットが曲がるのを止める。
            return;
        }
        Debug.LogFormat("onClickSkillUpBtn({0}):: pl.Lv= {1}, name= {2}",index , pl.Lv, SkillBtns[index].name.text);

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