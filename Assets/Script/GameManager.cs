
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State {PLAY, WAIT, GAMEOVER};
    public State state = State.PLAY;

    //* OutSide
    //public GameObject hitBox;
    public Transform hitRangeStartTf;
    public Transform hitRangeEndTf;

    public Transform deadLineTf;
    public BoxCollider downWall;

    //* GUI
    public Text stateTxt;
    public Text shootCntTxt;

    public Slider hitRangeSlider;
    public RectTransform hitSliderHomeRunRangeTf;
    public float HomeRunRangePercent = 20;
    public Image hitRangeHandleImg;

    void Start() {
        Debug.Log("deadLine.pos.z=" + deadLineTf.position.z);
        hitSliderHomeRunRangeTf.sizeDelta = new Vector2(hitRangeSlider.GetComponent<RectTransform>().rect.width * (HomeRunRangePercent / 100), 12);
    }


    void Update()
    {
        //* GUI
        //* State Txt
        stateTxt.text = state.ToString();

        
        //* HitBox Degree Slider
        // float degY = hitBox.transform.rotation.eulerAngles.y;
        // degY = (degY < 180) ? -degY : +(360 - degY);
        // float v = degY + hitBoxDegOffset;
        //Debug.Log("GUI:: HitBox degY v=" + v);

        // int hitBoxDegSliderMax = hitBoxDegOffset * 2;
        // hitBoxDegSlider.value = v / hitBoxDegSliderMax;
    }

    public void setState(State st){
        Debug.Log("GameManager:: state=" + st);
        state = st;
    }
}
