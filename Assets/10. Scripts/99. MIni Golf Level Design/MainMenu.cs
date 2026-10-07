using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("01. List");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}