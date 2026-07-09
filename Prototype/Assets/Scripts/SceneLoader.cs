using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "YourBattleSceneName";
    [SerializeField] private CanvasGroup fadeGroup; // Assign the FadeOverlay in Inspector

    public void RestartGame()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        fadeGroup.gameObject.SetActive(true);

        float duration = 1.0f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }

        SceneManager.LoadScene(mainGameSceneName);
    }
}


