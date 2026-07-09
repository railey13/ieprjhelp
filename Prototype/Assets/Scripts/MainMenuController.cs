using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument doc;
    private Button start;
    private Button Controls;
    private Button exit;
    //private Button Main;
    private bool isPlay = false;

    void OnEnable()
    {
        
        if (isPlay == true)
            gameObject.SetActive(false);
        start = doc.rootVisualElement.Q<Button>("Start");
        Controls = doc.rootVisualElement.Q<Button>("Controls");
        exit = doc.rootVisualElement.Q<Button>("Exit");
        //Main = doc.rootVisualElement.Q<Button>("Main");
        start.RegisterCallback<ClickEvent>(onStartClicked);
        Controls.RegisterCallback<ClickEvent>(onControlsClicked);
        exit.RegisterCallback<ClickEvent>(onExitClicked);
        //Main.RegisterCallback<ClickEvent>(onMainMenuClicked);

    }

    //private void onMainMenuClicked(ClickEvent evt) {
    //    Debug.Log("Main");
    //    SceneManager.LoadScene("MainMenu");

    //}

    private void onStartClicked(ClickEvent evt)
    {
        
        SceneManager.LoadScene("ValScene");
        isPlay = true;
    }
    private void onExitClicked(ClickEvent evt)
    {
        Debug.Log("Exit");
        Application.Quit();
    }
    private void onControlsClicked(ClickEvent evt)
    {
        
        Debug.Log("Controls");
        SceneManager.LoadScene("Controls");
    }
}
