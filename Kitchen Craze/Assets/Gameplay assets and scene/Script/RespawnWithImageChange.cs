using UnityEngine;

public class RespawnWithImage : MonoBehaviour
{
    // Public variable to assign a specific sprite for respawning
    public Sprite respawnSprite; // Set this in the Inspector

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Optionally, initialize the sprite to the respawn sprite
        if (respawnSprite != null)
        {
            spriteRenderer.sprite = respawnSprite;
        }
    }

    public void Respawn()
    {
        // Ensure we are resetting the object to its starting position
        transform.position = Vector3.zero; // Set this to whatever initial position you prefer

        // Change the sprite to the respawnSprite
        if (respawnSprite != null)
        {
            spriteRenderer.sprite = respawnSprite;
        }

        // Reactivate the object
        gameObject.SetActive(true);
    }
}
