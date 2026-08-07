using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController1 : MonoBehaviour
{
    // Existing Load method
    public void LoadGameScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    // New method for Quit
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // New method for Controls
    public void ToggleControlsPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}