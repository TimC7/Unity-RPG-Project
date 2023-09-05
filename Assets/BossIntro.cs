using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossIntro : MonoBehaviour
{
    public delegate void DefeatedEvent();

    public event DefeatedEvent OnDefeated;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Intro collided with trigger.");
        if (col.gameObject.CompareTag("Player Attack"))
        {
            Debug.Log("Intro collided with Player Attack.");
            OnDefeated.Invoke();
            gameObject.SetActive(false);
        }
    }
}
