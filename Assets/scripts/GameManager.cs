using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public int currentFloor;
    public SaveLoadManager sl;
    public PlayerMovement max;//, lucia;
    public GameObject gameOverScreen;
    public void Awake()
    {
        sl = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();
        max = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        gameOverScreen = GameObject.FindWithTag("Game Over");

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        if (max != null)
        {
            sl.loadGame();
            //max.refillHealth();
        }
    }

    public void gameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        else
            Debug.Log("GameManager: gameOverScreen is null.");
    }

    public void tryAgain()
    {
        SceneManager.LoadScene(currentFloor);
    }

    public void giveUp()
    {
        SceneManager.LoadScene(0);
    }
}

