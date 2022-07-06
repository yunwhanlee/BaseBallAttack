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
    public List<KeyValuePair<int, GameObject>> selectList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。
    public int scrollingSpeed;
    public bool isRollingStop = false;

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
        selectList = new List<KeyValuePair<int, GameObject>>();

        //* Set SelectList
        skillImgCnt = gm.getSkillImgObjPrefs().Length - 1; //自然なスクロールのため、末端に1番目Spriteを追加したので、この分は消す-1。
        for(int i=0;i<skillImgCnt;i++){
            int pos = SPRITE_W * i;
            selectList.Add( new KeyValuePair<int, GameObject>(pos, gm.getSkillImgObjPrefs()[i+1]));
            // print("selectList["+i+"]:: key="+ selectList[i].Key +", value="+ selectList[i].Value);
        }
        
        //* Set ScrollSpriteImgs
        foreach(var btn in SkillBtns){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var child in gm.getSkillImgObjPrefs())
                Instantiate(child, Vector3.zero, Quaternion.identity, btn.imgRectTf);
            //Set Auto Scroll Start Pos
            btn.imgRectTf.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
        }
    }

    void Update(){
        //* Scroll Sprite Animation
        if(2 >= btnIdx && selectList.Count > 0){
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
                    print("STOP Scrolling Btn[" + btnIdx + "]");
                    int randIdx = Random.Range(0, selectList.Count);
                    btn.imgRectTf.localPosition = new Vector3(0, selectList[randIdx].Key + SPRITE_W / 2, 0);// Scroll Down a Half of Height PosY for Animation
                    btn.name.text = selectList[randIdx].Value.name.Split(char.Parse("_"))[1];
                    selectList.RemoveAt(randIdx);
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
                    SkillBtns[i].imgRectTf.Translate(0, Time.deltaTime * speed / (i+1), 0);
                    //最後Idxが終わるまで待つ
                    if(Mathf.RoundToInt(lastIdxPosY) == SPRITE_W) isRollingStop = false;
                }
            }
        }
    }

    //* GUI Button
    public void onClickSkillUpBtn(int index){//* Skill Level Up
        //* (BUG)曲がりが終わるまで、SkillBtnクリックできないように(スキル名が在るかを確認)。
        if(Array.Exists(SkillBtns, btn => btn.name.text == "")) return;

        Debug.Log("onClickSkillUpBtn:: skillName= " + SkillBtns[index].name.text);

        //* Set Data
        switch(SkillBtns[index].name.text){
            case "Dmg Up":
                pl.dmg.setLvUp(pl.dmg.Value + pl.dmg.Unit);
                break;
            case "Multi Shot":
                pl.multiShot.setLvUp(pl.multiShot.Value + pl.multiShot.Unit);
                Debug.Log(pl.multiShot.Level + pl.multiShot.Value);
                break;
            case "Speed Up":
                pl.speed.setLvUp(pl.speed.Value + pl.speed.Unit); //20% Up
                break;
            case "Immediate Kill":
                pl.instantKill.setLvUp(pl.instantKill.Value + pl.instantKill.Unit); //2% Up
                break;
            case "Critical Up":
                pl.critical.setLvUp(pl.critical.Value + pl.critical.Unit); //10% Up
                break;
            case "Explosion":
                var percent = pl.explosion.Value.per + pl.explosion.Unit.per;
                var range = pl.explosion.Value.range + pl.explosion.Unit.range;
                pl.explosion.setLvUp(new Explosion(percent, range)); //Active:20% Up, Radius:+0.25
                break;
            case "Exp Up":
                pl.expUp.setLvUp(pl.expUp.Value + pl.expUp.Unit);
                break;
            case "ItemSwpan Up":
                pl.itemSpawn.setLvUp(pl.itemSpawn.Value + pl.itemSpawn.Unit);
                Debug.Log("ItemSwpan Up : 未実装");
                break;
        }
        //* 終了
        this.gameObject.SetActive(false);
        gm.displayCurPassiveSkillUI("INGAME");
    }
}