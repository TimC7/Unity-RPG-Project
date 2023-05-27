using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isBattle = false;
    public int currentFloor = 2;
    public SaveLoadManager sl;
    public PlayerMovement max;//, lucia;
    public void Awake()
    {
        sl.loadGame();
        max.refillHealth();
        //lucia.refillHealth();
    }
}

