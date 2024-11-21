using UnityEngine;

public class ResetObject : MonoBehaviour
{
    [HideInInspector]
    public Vector3 originalPosition;

    private void Awake()
    {
        // Store the original position when the object is created
        originalPosition = transform.position;
    }
}
