using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScript : MonoBehaviour
{

    public static bool isLose = false;

    public GameObject loseScriptUI;

    //public PlayerController player;


    public void loseShow()
    {
        Cursor.lockState = CursorLockMode.Confined;
        loseScriptUI.SetActive(true);
        Time.timeScale = 0f;
        isLose = true;
    }

    public void replay()
    {
        SceneManager.LoadScene("Main");
    }

    public void loadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void quitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
