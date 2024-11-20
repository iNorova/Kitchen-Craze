using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Startbuttonloader : MonoBehaviour


{
    public GameObject loadingScreen; // UI GameObject for the loading screen
    public Slider progressBar;       // Optional: Progress bar slider
    public Text progressText;        // Optional: Progress percentage text
    public string GameplayScene;     // Name of the scene to load (e.g., Gameplay Scene)

    // Public method to start loading, triggered by the Start button
    public void StartGame()
    {
        StartCoroutine(LoadSceneAsync(GameplayScene));
    }

    private IEnumerator LoadSceneAsync(string GameplayScene)
    {
        // Activate the loading screen
        loadingScreen.SetActive(true);

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(GameplayScene);

        // Update loading progress while the scene loads
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize progress

            // Update progress bar
            if (progressBar != null)
                progressBar.value = progress;

            // Update progress text
            if (progressText != null)
                progressText.text = $"{progress * 100:0}%";

            yield return null; // Wait until the next frame
        }
    }
}
