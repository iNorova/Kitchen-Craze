using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor; // Required for using SceneAsset
#endif

public class ReturnToSpecificScene : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The button that triggers the scene change.")]
    public Button returnButton;

    [Header("Groups to Reset")]
    [Tooltip("List of GameObjects or groups to reset to their default state.")]
    public List<GameObject> groupsToReset; // List to specify which groups to reset

    [Header("Panels to Exclude from Reset")]
    [Tooltip("Panels that should not be reset when the game resets.")]
    public List<GameObject> panelsToExcludeFromReset; // List to specify which panels should not be reset

#if UNITY_EDITOR
    [Header("Scene Settings")]
    [Tooltip("Drag the scene you want to load here.")]
    public SceneAsset defaultScene; // Allows you to drag and drop a scene
#endif

    private string defaultSceneName; // Stores the scene name at runtime
    private int completedDishes = 0; // Tracks completed dishes

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
            returnButton.onClick.AddListener(HandleResetButton);
        }
        else
        {
            Debug.LogError("Return button not assigned in the Inspector!");
        }
    }

    void HandleResetButton()
    {
        // Save cookbook progression before resetting
        SaveCookbookProgression();

        // Temporarily disable panels and proceed with scene change
        DisablePanelsBeforeSceneLoad();

        // Reset specific groups to default
        ResetGroupsToDefault();

        // Load the specified scene
        LoadDefaultScene();
    }

    void LoadDefaultScene()
    {
        if (!string.IsNullOrEmpty(defaultSceneName))
        {
            // Load the scene and add a listener for scene loading
            SceneManager.sceneLoaded += RestorePanelsState;
            SceneManager.LoadScene(defaultSceneName);
        }
        else
        {
            Debug.LogError("Default scene name is not set!");
        }
    }

    void SaveCookbookProgression()
    {
        // Save all unlocked dish states
        var gameManagers = Object.FindObjectsByType<CookingGameManager>(FindObjectsSortMode.None); // Updated method
        foreach (var gameManager in gameManagers)
        {
            foreach (var combo in gameManager.combinations)
            {
                string key = $"Unlocked_{combo.cookedObject.name}";
                bool isUnlocked = combo.unlockedStateObject != null && combo.unlockedStateObject.activeSelf;
                PlayerPrefs.SetInt(key, isUnlocked ? 1 : 0);
            }
        }

        PlayerPrefs.Save(); // Ensure the data is stored persistently
    }

    // Disable panels before the scene loads to prevent reset
    private void DisablePanelsBeforeSceneLoad()
    {
        foreach (var panel in panelsToExcludeFromReset)
        {
            if (panel != null)
            {
                // Mark the panels as "don't destroy" on load, so they persist across scenes.
                DontDestroyOnLoad(panel);

                // Disable the panels temporarily to prevent them from being reset.
                panel.SetActive(false);
            }
        }
    }

    // Restore the state of panels after the scene has loaded
    private void RestorePanelsState(Scene scene, LoadSceneMode mode)
    {
        // After the scene is loaded, we enable the panels again
        foreach (var panel in panelsToExcludeFromReset)
        {
            if (panel != null)
            {
                // Re-enable the panel and its contents
                panel.SetActive(true);
            }
        }

        // Remove the sceneLoaded event listener to avoid repeated calls
        SceneManager.sceneLoaded -= RestorePanelsState;
    }

    // Reset specific groups to their default state
    private void ResetGroupsToDefault()
    {
        foreach (var group in groupsToReset)
        {
            if (group != null)
            {
                // Reset the group to its default state.
                // This could involve setting the active state or other properties.
                ResetGroupState(group);
            }
        }
    }

    // Example function for resetting a group's state
    private void ResetGroupState(GameObject group)
    {
        // For this example, let's set the group to inactive (default state)
        group.SetActive(false);

        // You can expand this function to reset more specific properties if needed.
        // For instance, you might want to reset specific components or child objects within the group.
    }

    public void ResetGame()
{
    // Reset all except panels in the exclude list
    var gameManagers = Object.FindObjectsByType<CookingGameManager>(FindObjectsSortMode.None); // Updated method

    foreach (var gameManager in gameManagers)
    {
        foreach (var combo in gameManager.combinations)
        {
            // Reset the cooked object only if it's NOT in the exclusion list
            if (combo.cookedObject != null && !panelsToExcludeFromReset.Contains(combo.cookedObject))
            {
                combo.cookedObject.SetActive(false); // Reset to default state
            }

            // Reset the locked state object only if it's NOT in the exclusion list
            if (combo.lockedStateObject != null && !panelsToExcludeFromReset.Contains(combo.lockedStateObject))
            {
                combo.lockedStateObject.SetActive(true); // Reset to locked state
            }

            // Reset the unlocked state object only if it's NOT in the exclusion list
            if (combo.unlockedStateObject != null && !panelsToExcludeFromReset.Contains(combo.unlockedStateObject))
            {
                combo.unlockedStateObject.SetActive(false); // Reset to hidden unlocked state
            }
        }
    }

    completedDishes = 0; // Reset progression
    Debug.Log("Game reset, except specified panels and their contents!");
}


}

