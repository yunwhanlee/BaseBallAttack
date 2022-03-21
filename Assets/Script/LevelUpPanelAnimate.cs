using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LevelUpPanelAnimate : MonoBehaviour
{
    private const int SPRITE_W = 270; //Heightも同一
    private int btnIdx;
    private float time;

    public GameObject[] skillImgObjPrefs;
    private int skillImgCnt;
    public List<KeyValuePair<int, GameObject>> selectList = new List<KeyValuePair<int, GameObject>>(); //* 同じnewタイプ型を代入しないと、使えない。
    public int scrollingSpeed;

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

        //* Set SkillBtn
        SkillBtns = new SkillBtn[3]{
            new SkillBtn(skillBtns[0]), new SkillBtn(skillBtns[1]), new SkillBtn(skillBtns[2])
        };

        //Init childs of imgRectTf
        if(0 < SkillBtns[0].imgRectTf.GetChildCount()){
            foreach(var btn in SkillBtns)
                foreach(RectTransform befChild in btn.imgRectTf)
                    Destroy(befChild.gameObject);
        }

        //Init SelectList
        selectList = new List<KeyValuePair<int, GameObject>>();

        //* Set SelectList
        skillImgCnt = skillImgObjPrefs.Length - 1; //自然なスクロールのため、末端に1番目Spriteを追加したので、この分は消す-1。
        for(int i=0;i<skillImgCnt;i++){
            int pos = SPRITE_W * i;
            selectList.Add( new KeyValuePair<int, GameObject>(pos, skillImgObjPrefs[i]));
            //print("selectList["+i+"]:: key="+ selectList[i].Key +", value="+ selectList[i].Value);
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
        //* Scroll Animation
        if(2 <= btnIdx && selectList.Count <= 0) return;

        time += Time.deltaTime;
        foreach(var btn in SkillBtns){
            //Scrolling
            if(time < 2){
                if(btn.imgRectTf.localPosition.y >= 0){
                    btn.imgRectTf.Translate(0,-Time.deltaTime * scrollingSpeed, 0);
                }
                else{
                    btn.imgRectTf.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
                }
            }
            //Stop
            else{
                int randIdx = Random.Range(0, selectList.Count);
                btn.imgRectTf.localPosition = new Vector3(0, selectList[randIdx].Key + SPRITE_W / 2, 0);
                btn.name.text = selectList[randIdx].Value.name.Split(char.Parse("_"))[1];
                selectList.RemoveAt(randIdx);
                btnIdx++;
            }
        }
    }
}