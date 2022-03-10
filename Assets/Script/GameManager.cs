
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
    private RectTransform hitRangeSliderTf;
    public RectTransform HomeRunRangeTf;
    public float HomeRunRangePer = 0.2f;
    public Image hitRangeHandleImg;

    void Start() {
        // Debug.Log("deadLine.pos.z=" + deadLineTf.position.z);
        hitRangeSliderTf = hitRangeSlider.GetComponent<RectTransform>();

        //* Set UI HomeRunRange
        float HomeRunRangeWidth = hitRangeSliderTf.rect.width * HomeRunRangePer;
        HomeRunRangeTf.sizeDelta = new Vector2(HomeRunRangeWidth, 12);

        float randPosX = Mathf.RoundToInt(Random.Range(0, hitRangeSliderTf.rect.width - HomeRunRangeWidth));
        float parentPivotOffset = hitRangeSliderTf.rect.width / 2;
        Debug.Log("random.Range("+0+"～"+ (hitRangeSliderTf.rect.width - HomeRunRangeWidth).ToString() +")"+ ", randPosX=" + randPosX + ", parentPivotOffset=" + parentPivotOffset);
        HomeRunRangeTf.localPosition = new Vector3(randPosX - parentPivotOffset,0,0);
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
