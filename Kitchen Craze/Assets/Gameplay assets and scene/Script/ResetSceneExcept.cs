using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetSceneExcept : MonoBehaviour
{
    [Header("Assign the objects to exclude from reset")]
    public string exclusionTag = "DoNotReset"; // Set the tag for objects you want to keep

    // Call this function when the reset button is pressed
    public void ResetScene()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Deactivate objects with the exclusion tag
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(exclusionTag))
            {
                // Make sure the object persists
                DontDestroyOnLoad(obj);
            }
        }

        // Reload the scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void Start()
    {
        Debug.Log($"Use the tag '{exclusionTag}' to mark objects that should not reset.");
    }
}
