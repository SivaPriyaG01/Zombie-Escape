using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonsManagerScript : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game1");
    }

    public void LoadSettings()
    {
         SceneManager.LoadScene("Settings");
    }

    public void LoadLeaderboard()
    {
         SceneManager.LoadScene("Leaderboard");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameOverMenu()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}

