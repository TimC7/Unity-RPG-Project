using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCheck : MonoBehaviour
{
    public BotBSceneManager sm;

    private void Start()
    {
        sm = GameObject.Find("Scene Manager").GetComponent<BotBSceneManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sm.removePrisonBars();
        }
    }
}
