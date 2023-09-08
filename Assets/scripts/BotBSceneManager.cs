using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BotBSceneManager : MonoBehaviour
{
    public GameObject door;
    public int keys, keysNeeded = 2;
    

    public void addKey()
    {
        keys++;
    }

    public void removePrisonBars()
    {
        if (keys >= keysNeeded)
        {
            door.SetActive(false);
        }
    }

}
