using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor; // Required for using SceneAsset
#endif

public class ReturnToSpecificScene : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The button that triggers the scene change.")]
    public Button returnButton;

#if UNITY_EDITOR
    [Header("Scene Settings")]
    [Tooltip("Drag the scene you want to load here.")]
    public SceneAsset defaultScene; // Allows you to drag and drop a scene
#endif

    private string defaultSceneName; // Stores the scene name at runtime

    void Start()
    {
#if UNITY_EDITOR
        // Get the scene name from the dragged SceneAsset
        if (defaultScene != null)
        {
            defaultSceneName = defaultScene.name;
        }
        else
        {
            Debug.LogError("Default scene is not assigned in the Inspector!");
            return;
        }
#else
        if (string.IsNullOrEmpty(defaultSceneName))
        {
            Debug.LogError("Default scene name is missing! Ensure the script is set up properly.");
        }
#endif

        // Add listener to the button
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(LoadDefaultScene);
        }
        else
        {
            Debug.LogError("Return button not assigned in the Inspector!");
        }
    }

    void LoadDefaultScene()
    {
        if (!string.IsNullOrEmpty(defaultSceneName))
        {
            // Load the specified scene
            SceneManager.LoadScene(defaultSceneName);
        }
        else
        {
            Debug.LogError("Default scene name is not set!");
        }
    }
}

