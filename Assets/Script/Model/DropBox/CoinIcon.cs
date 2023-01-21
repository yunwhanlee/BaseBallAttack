using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinIcon : MonoBehaviour
{   
    //* OutSide
    GameManager gm;

    //* Value
    const float START_DELAY_SPAN_TIME = 0.06875f;
    const float WAIT_COLLECTION_TIME = 0.7f;
    [SerializeField] Transform target;
    float startDelay = 0;
    [SerializeField] float speed = 8;  public float Speed { get => speed; set => speed = value;}
    
    void Awake(){
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void OnEnable(){
        target = null;
        startDelay = 0;
        StartCoroutine(coDelay());
    }

    void FixedUpdate(){
        if(target){
            this.transform.position = Vector3.Lerp(this.transform.position, target.position, speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag(DM.TAG.Player.ToString())){
            StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
        }
    }

    public void setStartDelay(int i) => startDelay = i * START_DELAY_SPAN_TIME;

    IEnumerator coDelay(){
        yield return new WaitForSeconds(WAIT_COLLECTION_TIME);
        yield return new WaitForSeconds(startDelay);
        target = gm.pl.modelMovingTf;
    }


}
