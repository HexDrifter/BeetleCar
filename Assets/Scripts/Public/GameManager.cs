using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("Level 1");
    }
    public void OnClickEndGame()
    {
         Application.Quit();
    }

    public void OnClickDebug()
    {
        SceneManager.LoadScene("DebugRoom");
    }

}
