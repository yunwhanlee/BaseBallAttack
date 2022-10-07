using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//* --------------------------------------------------------------------
//* Array Element名付ける
//* --------------------------------------------------------------------
 [CustomEditor(typeof(InspectorCustomizer))]
 public class InspectorCustomizer : Editor
 {
     public void ShowArrayProperty(SerializedProperty list)
     {
         EditorGUI.indentLevel += 1;
         for (int i = 0; i < list.arraySize; i++)
         {
             EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new UnityEngine.GUIContent("Bla" + (i + 1).ToString()));
         }
         EditorGUI.indentLevel -= 1;
     }

     public override void OnInspectorGUI()
     {
         ShowArrayProperty(serializedObject.FindProperty("langs"));
     }
 }

//* --------------------------------------------------------------------
//* Utility
//* --------------------------------------------------------------------
public class Util : MonoBehaviour
{
    public static Util _;

    void Awake() => singleton();

    void singleton(){
        if(_ == null) _ = this;
        else if(_ != null) DontDestroyOnLoad(this.gameObject);
    }

    public float getAnimPlayTime(int index, Animator anim){
        // Array.ForEach(clips, clip=> Debug.Log($"clip= {clip.name}, clip.length= {clip.length}"));
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        float sec = clips[index].length;
        return sec;
    }

    public Transform getCharaRightArmPath(Transform charaTf){
        return  charaTf.Find("Bone").transform.Find("Bone_R.001").transform.Find("Bone_R.002").transform.Find(DM.NAME.RightArm.ToString());
    }
}
