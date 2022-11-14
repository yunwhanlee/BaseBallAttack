using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class Util : MonoBehaviour
{
    public static Util _;

    [Header("NOTICE MESSAGE")]
    public int noticeMsgDisplayCnt = 1;
    public Text noticeMessageTxtPref;
    public Transform mainPanelTf;
    public GameObject debugSphereObj;

    void Awake() => singleton();

    void singleton(){
        if(_ == null)  _ = this;
        else if(_ != null)  Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    public float getAnimPlayTime(int index, Animator anim){
        // Array.ForEach(clips, clip=> Debug.Log($"clip= {clip.name}, clip.length= {clip.length}"));
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        float sec = clips[index].length;
        return sec;
    }

    public Transform getCharaRightArmPath(Transform charaTf){
        Debug.Log($"getCharaRightArmPath:: charaTf= {charaTf}");
        return  charaTf.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find(DM.NAME.RightArm.ToString());
    }

    public void displayNoticeMsgDialog(string msg){
        noticeMessageTxtPref.text = msg;
        var ins = Instantiate(noticeMessageTxtPref, mainPanelTf.transform.position, Quaternion.identity, mainPanelTf);
        ins.rectTransform.localPosition = new Vector3(0,-900,-400);
        Destroy(ins.gameObject, noticeMsgDisplayCnt);
    }

    public int getCalcEquivalentSequence(int n, int device){ //* 等比数列
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

    public List<float> getCalcFibonicciSequence(float unit, float fibRatio){ //* フィボナッチ数列
        List<float> res = new List<float>();
        float v1 = 1 * unit;
        float v2 = fibRatio * unit;
        for(int i=0; i<10; i++){
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

    public void DebugSphere(Vector3 pos, float raidus, float destroyCnt){
        var ins = Instantiate(debugSphereObj, pos, Quaternion.identity);
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
}
