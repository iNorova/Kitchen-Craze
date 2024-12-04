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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
    }

    private void OnMouseDown()
    {
        isDragging = true;

        // Bring the object to the top layer for rendering
        spriteRenderer.sortingOrder = 100;

        // Disable collisions with other ingredients
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);

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

        // Apply a random torque to simulate a "sag" effect (optional for realism)
        rb.AddTorque(Random.Range(-sagTorque, sagTorque));

        // Re-enable gravity and let the object fall naturally
        rb.gravityScale = 1;

        // Optionally freeze rotation after dragging
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging && collision.gameObject.CompareTag(potTag))
        {
            // When the ingredient collides with the pot
            Debug.Log("Ingredient added to pot: " + gameObject.name);
            // Add your logic here for handling the ingredient
        }
    }
}
