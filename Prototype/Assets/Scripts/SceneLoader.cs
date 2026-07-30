using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "ValScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    public void RestartGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}


