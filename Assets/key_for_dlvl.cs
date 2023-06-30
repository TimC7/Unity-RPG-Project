using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class key_for_dlvl : MonoBehaviour
{
    public int key = 0;
    public TilemapRenderer door;

/*    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
*/
    public void addKey()
    {
        key = 1;
    }

    public void removePrisonBars()
    {
        if (key == 1)
        {
            door.enabled = false;
            door.GetComponent<TilemapCollider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            addKey();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


}
