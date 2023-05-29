using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGuy : MonoBehaviour
{
    public int price = 50, strIncrement = 1;
    public Inventory inv;
    public PlayerMovement max, lucia;

    private bool playerInRange = false;
    private Coroutine checkInputCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            checkInputCoroutine = StartCoroutine(CheckInput());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            StopCoroutine(checkInputCoroutine);
        }
    }



    private IEnumerator CheckInput()
    {
        while (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                //Debug.Log("Press Interact.");
                if (inv.gold >= price)
                {
                    inv.takeMoney(price);
                    max.strUp(strIncrement);
                    //lucia.strUp(strIncrement);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
            }
            yield return null;
        }
    }
}
