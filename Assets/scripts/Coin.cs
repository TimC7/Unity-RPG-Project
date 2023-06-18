using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject floatingTextPrefab; //to show text of coin collected or xp gained
    public SpriteRenderer sprite,sprite2;
    public Collider2D colliderOfCoin;
    public int amountValue;
    public Inventory playerInventory;

    public Transform objTrans;
    private float delay = 0;
    private float pasttime = 0;
    private float when = 1.0f;
    private Vector3 off;
    public Rigidbody2D rig;
    public GameObject player;
    
    

    private void Awake()
    {
        //random for x axis
        off = new Vector3(Random.Range(-1, 1), off.y, off.z);
        //random for y axis
        off = new Vector3(off.x, Random.Range(-1, 1), off.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        objTrans = GetComponent<Transform>();
        rig = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
        //coins stop moving after 1 sec
        if(when >= delay)
        {
            pasttime = Time.deltaTime;
            //position of coin
            objTrans.position += off * Time.deltaTime;
            delay += pasttime;
        }
    }


    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1.1f);
        Destroy(this.gameObject);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if( other.gameObject.CompareTag("Player"))
        {
            colliderOfCoin.enabled = false;
            playerInventory.addMoney(amountValue);
            Destroy(this.gameObject);
            //Debug.Log("");
            //playerInventory.gold += amountValue;
        }

        if(other.gameObject.CompareTag("Wall"))
        {
            off = Vector3.zero;
            
        }
        
    }
}
