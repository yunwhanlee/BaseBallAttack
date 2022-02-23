using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamResolution : MonoBehaviour
{
    const int DEVICE_WIDTH = 9;
    const int DEVICE_HEIGHT = 16;

    void Awake() {
        Camera cam = GetComponent<Camera>();
        Rect rect = cam.rect;
        float scaleH = ((float)Screen.width / Screen.height) / ((float)DEVICE_WIDTH / DEVICE_HEIGHT); // (横 / 縦)
        float scaleW = 1f / scaleH;

        //* calculate
        if(scaleH < 1){//上・下が超える
            rect.height = scaleH;
            rect.y = (1f - scaleH) / 2f;
        }else{//左・右が超える
            rect.width = scaleW;
            rect.x = (1f - scaleW) / 2f;
        }

        cam.rect = rect;//apply
    }
}
