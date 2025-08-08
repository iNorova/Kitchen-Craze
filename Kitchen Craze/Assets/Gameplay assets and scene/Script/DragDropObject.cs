using UnityEngine;

public class DragAndSwing : MonoBehaviour
{
    private Rigidbody2D rb;         // Reference to the object's Rigidbody2D
    private Collider2D col;        // Reference to the object's Collider2D
    private bool isDragging = false; // Is the object currently being dragged?
    private Vector2 offset;         // Offset between mouse and object during drag
    private Vector2 mousePosition;  // Mouse position in world coordinates

    public float dragSpeed = 10f;    // Speed at which the object follows the mouse
    public float sagTorque = 50f;    // Torque applied to create sag effect
    public float rotationDamping = 5f; // Damping to stabilize rotation

    public string potTag = "Pot";   // Tag of the pot object

    private int defaultSortingOrder; // Default sorting order for the object
    private SpriteRenderer spriteRenderer; // SpriteRenderer to change sorting order

    // Respawn variables
    private Vector3 originalPosition; // Store the original position
    private Quaternion originalRotation; // Store the original rotation
    private bool shouldRespawn = false; // Flag to trigger respawn

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store original position and rotation for respawning
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Disable gravity temporarily when dragging
        rb.gravityScale = 0;

        // Set the angular damping to smooth out rotation
        rb.angularDamping = rotationDamping;

        // Store the default sorting order
        defaultSortingOrder = spriteRenderer.sortingOrder;
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the mouse position in world coordinates
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate direction towards mouse position
            Vector2 direction = (Vector2)mousePosition - rb.position;

            // Set the object's velocity towards the mouse position with drag speed
            rb.linearVelocity = direction * dragSpeed;

            // Rotate the object towards the dragging direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }

        // Check if object has fallen off screen and should respawn
        if (shouldRespawn && transform.position.y < -10f) // Adjust -10f based on your screen size
        {
            RespawnObject();
        }


    }

    private void OnMouseDown()
    {
        isDragging = true;

        // Bring the object to the very top layer for rendering (above shelves)
        spriteRenderer.sortingOrder = 200;

        // Disable collisions with other ingredients
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);

        // Temporarily disable the collider to pass through shelves
        if (col != null)
        {
            col.enabled = false;
        }

        // Calculate the offset between the object and the mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = (Vector2)transform.position - (Vector2)mousePosition;

        // Allow free movement (remove any position constraints)
        rb.constraints = RigidbodyConstraints2D.None;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // Reset the sorting order
        spriteRenderer.sortingOrder = defaultSortingOrder;

        // Re-enable collisions with other ingredients
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, false);

        // Re-enable the collider
        if (col != null)
        {
            col.enabled = true;
        }

        // Ignore collisions with all layers EXCEPT the pot layer
        // This way it only detects the pot/pan
        for (int i = 0; i < 32; i++) // Unity has 32 layers
        {
            if (i != LayerMask.NameToLayer("Pot")) // Replace "Pot" with your pot's layer name
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i, true);
            }
        }

        // Apply a random torque to simulate a "sag" effect (optional for realism)
        rb.AddTorque(Random.Range(-sagTorque, sagTorque));

        // Re-enable gravity and let the object fall naturally
        rb.gravityScale = 1;

        // Set flag to respawn when object falls off screen
        shouldRespawn = true;

        // Optionally freeze rotation after dragging
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging && collision.gameObject.CompareTag(potTag))
        {
            // When the ingredient collides with the pot
            Debug.Log("Ingredient added to pot: " + gameObject.name);
            
            // Let the cooking system handle the ingredient
            // Don't hide it immediately - let the cooking system do that
            shouldRespawn = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDragging && other.CompareTag(potTag))
        {
            // When the ingredient enters the pot trigger
            Debug.Log("Ingredient added to pot (trigger): " + gameObject.name);
            
            // Let the cooking system handle the ingredient
            // Don't hide it immediately - let the cooking system do that
            shouldRespawn = false;
        }
    }

    // Public method to be called by the cooking system when a combination is completed
    public void RespawnAfterCooking()
    {
        RespawnObject();
    }

    private void RespawnObject()
    {
        // Reset position and rotation to original
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        
        // Reset velocity and angular velocity
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Reset respawn flag
        shouldRespawn = false;
        
        // Reset gravity scale
        rb.gravityScale = 0;
        
        // Reset constraints to allow movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Re-enable the collider when respawning
        if (col != null)
        {
            col.enabled = true;
        }
        
        // Re-enable collisions with all layers when respawning
        for (int i = 0; i < 32; i++)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, i, false);
        }
        
        // Ensure the object is active and visible
        gameObject.SetActive(true);
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        // Reset sorting order to default
        spriteRenderer.sortingOrder = defaultSortingOrder;
        
        Debug.Log("Object respawned: " + gameObject.name);
    }
}
