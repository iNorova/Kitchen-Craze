using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHome : MonoBehaviour
{
    public string homeSceneName = "UI FOR KITCHEN CRAZE"; // Name of the home screen scene

    public void OnBackToHomeButtonPressed()
    {
        Debug.Log("Back to Home Button Pressed. Starting the unload process.");
        StartCoroutine(UnloadCurrentAndLoadHome());
    }

    private IEnumerator UnloadCurrentAndLoadHome()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Debug log for current scene
        Debug.Log($"Current active scene: {currentScene.name}");

        if (currentScene.name != homeSceneName) // Prevent trying to unload the home screen
        {
            // Start unloading the current scene
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            if (unloadOperation == null)
            {
                Debug.LogError("Unload operation failed. Scene may already be unloaded or invalid.");
                yield break;
            }

            // Wait until the scene is fully unloaded
            Debug.Log($"Unloading scene: {currentScene.name}");
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
            Debug.Log("Scene unloaded successfully.");
        }
        else
        {
            Debug.LogWarning("The home scene is already active. Skipping unload.");
        }

        // Load the home screen scene
        Debug.Log($"Loading home scene: {homeSceneName}");
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(homeSceneName);

        if (loadOperation == null)
        {
            Debug.LogError("Load operation failed. Check if the home scene name is correct.");
            yield break;
        }

        // Wait until the home screen scene is fully loaded
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        Debug.Log("Home scene loaded successfully.");
    }
}
