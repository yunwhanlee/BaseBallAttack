using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //* OutSide
    public GameObject hitBox;
    [SerializeField]int hitBoxDegOffset = 50;

    //* GUI
    public Slider hitBoxDegSlider;


    void Update()
    {
        //* GUI
        //HitBox Degree Slider
        float degY = hitBox.transform.rotation.eulerAngles.y;
        degY = (degY < 180) ? -degY : +(360 - degY);
        float v = degY + hitBoxDegOffset;
        Debug.Log("GUI:: HitBox degY v=" + v);

        int hitBoxDegSliderMax = hitBoxDegOffset * 2;
        hitBoxDegSlider.value = v / hitBoxDegSliderMax;
    }
}
