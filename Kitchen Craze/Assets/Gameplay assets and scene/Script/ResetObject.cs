using System.Collections.Generic;
using UnityEngine;

public class ObjectResetter : MonoBehaviour
{
    [System.Serializable]
    public class ResettableObject
    {
        public GameObject gameObject;
        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public Quaternion initialRotation;
        [HideInInspector] public Vector3 initialScale;
        [HideInInspector] public bool initialSpriteRendererState;
        [HideInInspector] public bool initialColliderState;
        [HideInInspector] public bool initialRigidbodyState;
    }

    [Header("Objects to Reset")]
    public List<ResettableObject> objectsToReset;

    private void Start()
    {
        // Save the initial states of all objects in the list.
        foreach (var obj in objectsToReset)
        {
            if (obj.gameObject != null)
            {
                obj.initialPosition = obj.gameObject.transform.position;
                obj.initialRotation = obj.gameObject.transform.rotation;
                obj.initialScale = obj.gameObject.transform.localScale;

                // Get initial states of components.
                var spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    obj.initialSpriteRendererState = spriteRenderer.enabled;
                }

                var collider = obj.gameObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    obj.initialColliderState = collider.enabled;
                }

                var rigidbody = obj.gameObject.GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    obj.initialRigidbodyState = rigidbody.simulated;
                }
            }
        }
    }

    // Method to reset objects, callable by a UI button
    public void ResetObjects()
    {
        foreach (var obj in objectsToReset)
        {
            if (obj.gameObject != null)
            {
                // Reset transform properties.
                obj.gameObject.transform.position = obj.initialPosition;
                obj.gameObject.transform.rotation = obj.initialRotation;
                obj.gameObject.transform.localScale = obj.initialScale;

                // Reset SpriteRenderer state.
                var spriteRenderer = obj.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = obj.initialSpriteRendererState;
                }

                // Reset Collider2D state.
                var collider = obj.gameObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = obj.initialColliderState;
                }

                // Reset Rigidbody2D state.
                var rigidbody = obj.gameObject.GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    rigidbody.simulated = obj.initialRigidbodyState;
                }
            }
        }

        Debug.Log("Objects have been reset to their initial states.");
    }
}
