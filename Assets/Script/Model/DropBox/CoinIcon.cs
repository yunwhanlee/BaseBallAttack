using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinIcon : MonoBehaviour
{   
    //* OutSide
    GameManager gm;

    //* Value
    const float DELAY_TIME = 1.4f;
    [SerializeField] Transform target;
    [SerializeField] float speed = 10;  public float Speed { get => speed; set => speed = value;}
    void Awake(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void OnEnable(){
        target = null;
        StartCoroutine(coDelay());
    }

    void FixedUpdate(){
        if(target){
            this.transform.position = Vector3.Lerp(this.transform.position, target.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag(DM.TAG.Player.ToString())){
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }

    IEnumerator coDelay(){
        yield return new WaitForSeconds(DELAY_TIME);
        target = gm.pl.modelMovingTf;
    }


}
