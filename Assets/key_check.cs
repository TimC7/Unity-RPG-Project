using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class key_check : MonoBehaviour
{
    public key_for_dlvl k;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && k.key == 1)
        {
            k.removePrisonBars();
            
        }
    }
}
