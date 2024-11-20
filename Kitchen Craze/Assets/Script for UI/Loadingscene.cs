using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loadingscene : MonoBehaviour

{
    public GameObject loadingScreen; // UI GameObject for the loading screen
    public Slider progressBar;       // Optional: Progress bar slider
    public Text progressText;        // Optional: Progress percentage text
    public string homeScene;         // Name of the home screen scene
   

    private void Start()
    {
        // Automatically load the home screen at the start
        StartCoroutine(LoadSceneAsync(homeScene));
    }



    private IEnumerator LoadSceneAsync(string homeScene)
    {
        // Activate the loading screen UI
        loadingScreen.SetActive(true);

        // Begin loading the target scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(homeScene);

        // Update the loading progress
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Update the progress bar (if assigned)
            if (progressBar != null)
                progressBar.value = progress;

            // Update the progress text (if assigned)
            if (progressText != null)
                progressText.text = $"{progress * 100:0}%";

            yield return null;
        }
    }
}
