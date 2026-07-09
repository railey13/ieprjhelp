using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ControlsController : MonoBehaviour
{
    [SerializeField] private UIDocument doc;
    private Button Main;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Main = doc.rootVisualElement.Q<Button>("Main");
        Main.RegisterCallback<ClickEvent>(onMainMenuClicked);
    }

    private void onMainMenuClicked(ClickEvent evt)
    {
        Debug.Log("Main");
        SceneManager.LoadScene("MainMenuScene");

    }
}
