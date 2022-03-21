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

    public Button[] skillBtns;
    [SerializeField] private RectTransform[] imgRectTfs = {null,null,null};
    [SerializeField] private Text[] nameTxts = {null,null,null};

    [System.Serializable]
    public struct SkillBtn{
        public RectTransform imgRectTf;
        public Text name;
        public SkillBtn(Button skillBtn){
            imgRectTf = skillBtn.transform.GetChild(0).GetComponent<RectTransform>();
            name = skillBtn.transform.GetChild(1).GetComponent<Text>();
        }
    }

    public int scrollingSpeed;

    //struct
    public SkillBtn[] SkillBtns;

    void Start(){
        btnIdx = 0;

        SkillBtns = new SkillBtn[3]{
            new SkillBtn(skillBtns[0]), new SkillBtn(skillBtns[1]), new SkillBtn(skillBtns[2])
        };

        for(int i=0;i<skillBtns.Length;i++){
            imgRectTfs[i] = skillBtns[i].transform.GetChild(0).GetComponent<RectTransform>();
            nameTxts[i] = skillBtns[i].transform.GetChild(1).GetComponent<Text>();
        }


        //* Set SelectList
        skillImgCnt = skillImgObjPrefs.Length - 1; //自然なスクロールのため、末端に1番目Spriteを追加したので、この分は消す-1。
        for(int i=0;i<skillImgCnt;i++){
            int pos = SPRITE_W * i;
            selectList.Add( new KeyValuePair<int, GameObject>(pos, skillImgObjPrefs[i]));
            //print("selectList["+i+"]:: key="+ selectList[i].Key +", value="+ selectList[i].Value);
        }
        
        //* Set ScrollSpriteImgs
        foreach(var imgTf in imgRectTfs){
            //Insert SkillSprite into btnImgRectTf as Child
            foreach(var child in skillImgObjPrefs)
                Instantiate(child, Vector3.zero, Quaternion.identity, imgTf);
            //Set Auto Scroll Start Pos
            imgTf.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
        }
    }

    void Update(){
        //* Scroll Animation
        if(2 <= btnIdx && selectList.Count <= 0) return;

        time += Time.deltaTime;
        foreach(var imgTf in imgRectTfs){
            //Auto Scrolling Animation
            if(time < 2){
                if(imgTf.localPosition.y >= 0){
                    imgTf.Translate(0,-Time.deltaTime * scrollingSpeed, 0);
                }
                else{
                    imgTf.transform.localPosition = new Vector3(0, SPRITE_W * skillImgCnt, 0);
                }
            }
            //Stop Scrolling & Set Random Skill Img
            else{
                int randIdx = Random.Range(0, selectList.Count);
                imgTf.transform.localPosition = new Vector3(0,selectList[randIdx].Key,0);
                nameTxts[btnIdx].text = selectList[randIdx].Value.ToString();
                print("selectList[randIdx].value=" + selectList[randIdx].Value);
                selectList.RemoveAt(randIdx);
                btnIdx++;
            }
        }
    }
}