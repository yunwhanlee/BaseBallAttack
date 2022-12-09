using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum TYPE {ExpOrb, RewardChest};

    //* OutSide
    GameManager gm;

    //* Value
    int popPower = 350; public int PopPower{ get => popPower; set => popPower = value;}
    [SerializeField] DropItem.TYPE type; public DropItem.TYPE Type { get => type; set => type = value;}
    [SerializeField] int exp; public int Exp{ get => exp; set => exp = value;}
    [SerializeField] bool isMoveToPlayer; public bool IsMoveToPlayer{ get => isMoveToPlayer; set => isMoveToPlayer = value;}
    [SerializeField] float moveSpeed = 5f;
    public float cnt = 0;
    [SerializeField] Rigidbody rigid;
    [SerializeField] MeshRenderer meshRdr;

    void Awake() {
        // Debug.Log("<color=yellow>DropItem Awake() </color>");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody>();
        meshRdr = GetComponent<MeshRenderer>();
    }

    void OnEnable() {
        spawnPopUp(PopPower);
    }

    void OnDisable() { //* (BUG) 再活性化するときに、動きOFF。
        isMoveToPlayer = false;
    }

    void Start(){
        
    }

    void Update(){
        if(isMoveToPlayer){
            //* 放物線
            transform.position = Vector3.Slerp(transform.position, gm.pl.modelMovingTf.position, moveSpeed * Time.deltaTime); // transform.position = Vector3.Lerp(transform.position, gm.pl.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    public void moveToTarget(Transform target) => isMoveToPlayer = true;

    public void spawnPopUp(int power){
        float v = 0.5f;
        float randX = Random.Range(-v, v);
        float randZ = Random.Range(-v, v);
        // Debug.LogFormat("DropItem:: shootPopUp():: randX= {0}, randZ= {1}", randX, randZ);
        Vector3 dir = new Vector3(randX, 1, randZ);
        float force = power * Time.deltaTime;
        // Debug.Log($"<color=yellow>DropItem spawnPopUp({power}):: force= {force}</color>");
        this.rigid.AddForce(dir * force, ForceMode.Impulse);
        this.rigid.AddTorque(dir * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col){
        if(col.transform.CompareTag(DM.TAG.Player.ToString())){
            switch(this.type){
                case DropItem.TYPE.ExpOrb:
                    gm.pl.addExp(Exp); //* (BUG) GAMEOVER後、再スタート場合、EXPが増えないように。
                    gm.em.createDropItemExpOrbEF(this.transform);
                    StartCoroutine(ObjectPool.coDestroyObject(this.gameObject, gm.dropItemGroup));
                    break;
                case DropItem.TYPE.RewardChest:
                    gm.getRewardChestPanel.SetActive(true);
                    gm.rewardChestIconImg.sprite = gm.defChestSpr;
                    gm.rewardChestContentTxt.text = LANG.getTxt(LANG.TXT.GetRewardChestPanel_Content.ToString());

                    //TODO REWARD
                    
                    break;
            }
            
        }
    }
}
