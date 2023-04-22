using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour
{
    public GameObject loseScriptUI;

    public GameObject winScriptUI;

    //public PlayerController player;


    public void loseShow()
    {
        Cursor.lockState = CursorLockMode.Confined;
        loseScriptUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void winShow()
    {
        Cursor.lockState = CursorLockMode.Confined;
        winScriptUI.SetActive(true);
        Time.timeScale = 0f;
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
