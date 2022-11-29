using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;


public class Util : MonoBehaviour
{
    public static Util _;

    [Header("NOTICE MESSAGE")]
    public int noticeMsgDisplayCnt = 1;
    public Text noticeMessageTxtPref;
    public Transform mainPanelTf;
    public GameObject debugSphereObjBlue;
    public GameObject debugSphereObjRed;

    void Awake() => singleton();

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    public float getAnimPlayTime(string str, Animator anim){
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        Debug.Log($"Util::getAnimPlayTime(<color=yellow>{str}</color>)::--------------------");
        int index = -1;
        //* Anim Clip List
        int i = 0;
        Array.ForEach(clips, clip=> {
            // Debug.Log($"Util::getAnimPlayTime():: Obj:{anim.name} i={i}, clipName= <b>{clip.name}</b>, str={str} , clip.time= {clip.length} => {(clip.name == str? "<color=blue>TRUE</color>" : "false")}");
            if(clip.name == str || clip.name == str.ToLower()){
                index = i;
                return;
            }
            i++;
        });
        float sec = clips[index].length;
        Debug.Log($"<size=15>Util::getAnimPlayTime:: index= {index}, clip.name= {clips[index].name} => return {sec}sec</size>");
        return sec;
    }

    public Transform getCharaRightArmPath(Transform charaTf){
        Debug.Log($"getCharaRightArmPath:: charaTf= {charaTf}");
        return  charaTf.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find(DM.NAME.RightArm.ToString());
    }

    public void displayNoticeMsgDialog(string msg){
        //* (BUG) IN-GAMEへ行ってHOMEに戻ったたら、Missingになる。
        if(!mainPanelTf)
            mainPanelTf = GameObject.Find(DM.NAME.MainPanel.ToString()).GetComponent<RectTransform>();

        noticeMessageTxtPref.text = msg;
        var ins = Instantiate(noticeMessageTxtPref, mainPanelTf.transform.position, Quaternion.identity, mainPanelTf);
        ins.rectTransform.localPosition = new Vector3(0,-900,-400);
        Destroy(ins.gameObject, noticeMsgDisplayCnt);
    }
    //* 等比数列
    public int getCalcEquivalentSequence(int n, int device){ 
        int res;
        bool isOdd = n % 2 != 0;
        if(isOdd){ // 奇数
            res = n + (n + (n * ((n - 1) / device)));
        }
        else{// 偶数
            res = n + (n + (n * ((n - 1) / device) + (n / device)));
        }
        int extraVal = Random.Range(-1, 1); // Extra
        return res + extraVal;
    }
    /*
    * フィボナッチ数列 @param {unit: 増加単位}, {fibRatio: 比率}, {maxLv: 生成するリスト長さ}
    */
    public List<float> getCalcFibonicciSequenceList(float unit, float fibRatio, int maxLv){ 
        List<float> res = new List<float>();
        float v1 = 1 * unit;
        float v2 = fibRatio * unit;
        for(int i=0; i<maxLv; i++){
            if(i == 0) {
                res.Add(v1);
                res[i] = v1;
            }else{
                res.Add(v1 + v2);
                v1 = v2;
                v2 = res[i];
            }
            Debug.Log($"fibo [{i}]: " + res[i]);
        }
        return res;
    }

    public bool checkRayHitTagIsExist(Vector3 touchPos, string findTagName){
        float maxDistance = 100;
        Debug.Log($"checkRayHitTagIsExist:: findTagName={findTagName}");
        Vector3 touchScreenPos = Camera.main.ScreenToWorldPoint(touchPos);
        Ray ray = Camera.main.ScreenPointToRay(touchPos);

        //* Shoot Ray
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, touchScreenPos - ray.origin, maxDistance);
        Debug.DrawLine(ray.origin, touchScreenPos - ray.origin, Color.red, 1);

        //* check Is Exist
        return Array.Exists(hits, hit => hit.transform.CompareTag(findTagName));
    }

    public int getCalcCurValPercentage(float value, float max){
        return (int)(value / max * 100);
    }

    public void DebugSphere(Vector3 pos, float raidus, float destroyCnt, string color){
        GameObject obj = (color == "blue" || color == "Blue")? debugSphereObjBlue : debugSphereObjRed;
        var ins = Instantiate(obj, pos, Quaternion.identity);
        ins.transform.localScale = Vector3.one * (raidus * 2);
        Destroy(ins, destroyCnt);
    }

    public void sphereCastAllDecreaseBlocksHp(Transform my, float radius, int dmg){
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //* SphereCastAll
        RaycastHit[] hits = Physics.SphereCastAll(my.position, radius, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(isColBlock(hit.transform.GetComponent<Collider>())){
                gm.comboCnt--; //* (BUG) 爆発のカウントもされ、スキルが続けて発動されること防止。
                Debug.Log($"sphereCastAllDecreaseBlocksHp:: hit.transform.name= {hit.transform.name}");
                var block = hit.transform.gameObject.GetComponent<Block_Prefab>();
                block.decreaseHp(dmg);
                gm.em.createCritTxtEF(hit.transform.position, dmg);
            }
        });
    }
    public GameObject getTagObjFromRaySphereCast(Vector3 pos, float radius, string tag){
        GameObject obj = null;
        RaycastHit[] hits = Physics.SphereCastAll(pos, radius, Vector3.up, 0);
        Array.ForEach(hits, hit => {
            if(hit.transform.CompareTag(tag)){
                Debug.Log("getTagObjFromRaySphereCast:: HIT PLAYER!!");
                obj = hit.transform.gameObject;
            }
        });
        return obj;
    }
    public bool isColBlock(Collider col){
        Debug.Log($"Util::isColBlock:: col.name= {col.name}");
        return col.name.Contains(DM.NAME.Block.ToString())
            && col.name != ObjectPool.DIC.ItemBlockDirLineTrailEF.ToString(); //! (BUG) BlockではないのにBlock文字が含まれ、Nullエラーになること防止。
    }
    public float calcMathRoundDecimal(float value, int point){
        float decimalPoint = Mathf.Pow(10, point);
        Debug.Log($"calcMathRoundDecimal(value={value}, point={point}:: decimalPoint= {decimalPoint}, result= {Mathf.Round(value * decimalPoint) / decimalPoint}");
        return Mathf.Round(value * decimalPoint) / decimalPoint;
    }
    public bool isIntegerNum(float num){
        bool res = Mathf.Approximately(num, Mathf.RoundToInt(num));
        Debug.Log($"isIntegerNum({num}):: isInt? -> {res}");
        return res;
    }
    public string replaceSettingNumber(string txt, int i){ //* ただし、数字が二つあることはできない。
        //* 現在適用されたパンネル：Upgrade-Panel。
        string res = txt;
        string extractNum = Regex.Replace(txt, "[^0-9]", "");
        if(extractNum != ""){
            float realNum = DM.ins.personalData.Upgrade.Arr[i].unit;
            string numStr = (Util._.isIntegerNum(realNum)? realNum : realNum * 100).ToString();
            res = txt.Replace(extractNum, numStr);
            Debug.Log($"replaceSettingNumber({txt}, {i}):: Result=> " + res);
        }
        return res;
    }
    /**
    * * 等差数列 公式 : Sn = S1 + (n - 1) * d;
    *   @param 1. start (始め項)
    *   @param 2. max (最大値)
    *   @param 3. d (common Differnce:共差)
    *   @param 4. gradualUpValue (増加量) 0.00 ～ 1.00f EX) 0.01fは dの1%ずつ増える++
    */
    public List<int> calcArithmeticProgressionList(int start, int max, int d, float gradualUpValue = 0.00f){
        List<int> resList = new List<int>();
        for(int n=1; n<=max+1; n++){ //! max + 1下利用は、maxまでアップグレードしたらOutOfIndexバグが出るからです。
            int commonDifference = d + (int)(d * (n) * gradualUpValue);
            int v = start + (n - 1) * commonDifference;
            Debug.Log($"等差数列:: {n}回目 : {start} + ({n} - 1) * {commonDifference} = {v}");
            resList.Add(v);
        }
        return resList;
    }
}
