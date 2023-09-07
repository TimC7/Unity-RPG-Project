using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelManager : MonoBehaviour
{
    public Transform spawnPoint;

    public GameObject bigGoop;
    public GameObject darkMaxIntro;
    public GameObject HolyGrilledCheese;
    public GameObject darkMax;
    void Start()
    {
        darkMax.GetComponent<OverworldEnemy>().OnEnemyDefeated += GrilledCheeseSpawn;
        bigGoop.GetComponent<OverworldEnemy>().OnEnemyDefeated += spawnDarkMaxIntro;
        darkMaxIntro.GetComponent<BossIntro>().OnDefeated += spawnDarkMax;
    }

    public void spawnDarkMaxIntro()
    {
        darkMaxIntro.SetActive(true);
        darkMaxIntro.transform.position = spawnPoint.position;
    }

    public void spawnDarkMax()
    {
        darkMax.SetActive(true);
        darkMax.transform.position = spawnPoint.position;
        
    }
    
    public void GrilledCheeseSpawn()
    {
        HolyGrilledCheese.transform.position = spawnPoint.position;
        Debug.Log("queso");
    }
}
