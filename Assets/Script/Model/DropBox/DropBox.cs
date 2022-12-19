using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour{
    public const int MIN_X = -5, MAX_X = 5;
    public const int MIN_Z = -12,  MAX_Z = -6;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Vector3 setRandPos(){
        int rx = Random.Range(DropBox.MIN_X, DropBox.MAX_X+1);
        int rz = Random.Range(DropBox.MIN_Z, DropBox.MAX_Z+1);
        Vector3 randPos = new Vector3(rx, 1, rz);
        return randPos;
    }

    private void OnCollisionEnter(Collision col) {
        if(Util._.isColBlockOrObstacle(col.transform.GetComponent<Collider>())){
            Debug.Log("DropBox::OnCollisionEnter:: col= " + col);
            this.transform.position = setRandPos();
        }
    }
}
