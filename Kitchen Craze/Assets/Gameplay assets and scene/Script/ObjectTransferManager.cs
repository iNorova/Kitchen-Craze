using UnityEngine;

public class LoadObjectFromPrefab : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("The prefab of the object to load.")]
    public GameObject objectPrefab;

    [Tooltip("Position where the object will appear in the main scene.")]
    public Vector3 spawnPosition;

    // Function to call the object
    public void CallObject()
    {
        if (objectPrefab != null)
        {
            Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Object Prefab is not assigned in the Inspector!");
        }
    }
}
