using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpPanelAnimate : MonoBehaviour
{
    //* OutSide Component
    public Player pl;

    private const int SPRITE_W = 270; //Heightも同一
    private int btnIdx;
    private float time;

    public GameObject[] skillImgObjPrefs;
    private int skillImgCnt;
    public List<KeyValuePair<int, GameObject>> selectList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。
    public int scrollingSpeed;
    public bool isStop = false;

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
    //struct
    public SkillBtn[] SkillBtns;


    public void Start(){
        //Init Value
        time = 0;
        btnIdx = 0;
        isStop = false;


        //* Set SkillBtn
        SkillBtns = new SkillBtn[3]{
            new SkillBtn(skillBtns[0]), new SkillBtn(skillBtns[1]), new SkillBtn(skillBtns[2])
        };

        //Init
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
        skillImgCnt = skillImgObjPrefs.Length - 1; //自然なスクロールのため、末端に1番目Spriteを追加したので、この分は消す-1。
        for(int i=0;i<skillImgCnt;i++){
            int pos = SPRITE_W * i;
            selectList.Add( new KeyValuePair<int, GameObject>(pos, skillImgObjPrefs[i+1]));
            print("selectList["+i+"]:: key="+ selectList[i].Key +", value="+ selectList[i].Value);
        }
        
        //* Set ScrollSpriteImgs
        foreach(var btn in SkillBtns){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var child in skillImgObjPrefs)
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
                        isStop = true;
                    }
                }
            }
        }
        // #3.Animation: Scroll Up to Correct PosY
        else if(isStop){
            int speed = scrollingSpeed / 2;
            for(int i=0; i<SkillBtns.Length;i++){
                float posY = SkillBtns[i].imgRectTf.localPosition.y % SPRITE_W;
                float lastIdxPosY = SkillBtns[2].imgRectTf.localPosition.y % SPRITE_W;
                if(SPRITE_W/2 <= posY && posY <= SPRITE_W){
                    //Scroll Up
                    SkillBtns[i].imgRectTf.Translate(0, Time.deltaTime * speed / (i+1), 0);
                    //最後Idxが終わるまで待つ
                    if(Mathf.RoundToInt(lastIdxPosY) == SPRITE_W) isStop = false;
                }
            }
        }
    }

    //* GUI Button
    public void onClickSkillUpBtn(int index){//Skill Level Up
        Debug.Log("onClickSkillUpBtn:: skillName= " + SkillBtns[index].name.text);
        //* Set Data
        switch(SkillBtns[index].name.text){
            case "Dmg Up": 
                pl.setDmg(pl.getDmg()+1);
                break;
            case "Multi Shot":
                pl.setMultiShot(pl.getMultiShot()+1);
                break;
            case "Speed Up":
                pl.setSpeedPer(pl.getSpeedPer() + 0.2f); //20% Up
                break;
            case "Immediate Kill":
                pl.setImmediateKillPer(pl.getImmediateKillPer() + 0.02f); //2% Up
                break;
            case "Critical Up":
                pl.setCriticalPer(pl.getCriticalPer() + 0.1f); //10% Up
                break;
            case "Explosion":
                pl.setExplosion(pl.getExplosion().per + 0.25f, pl.getExplosion().range + 0.75f); //Active:20% Up, Radius:+0.5
                break;

        }
        this.gameObject.SetActive(false);
    }
}