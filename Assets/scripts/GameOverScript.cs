using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameManager gm;
    public int currentFloor;

    private void Start()
    {
        if (gm != null)
        {
            setCurrentFloor();
        }
    }

    public void setCurrentFloor()
    {
        currentFloor = gm.currentFloor;
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
