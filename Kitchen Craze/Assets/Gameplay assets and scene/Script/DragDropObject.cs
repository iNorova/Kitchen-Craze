using UnityEngine;

public class DragAndSwing : MonoBehaviour
{
    private Rigidbody2D rb;         // Reference to the object's Rigidbody2D
    private bool isDragging = false; // Is the object currently being dragged?
    private Vector2 offset;         // Offset between mouse and object during drag
    private Vector2 mousePosition;  // Mouse position in world coordinates

    public float dragSpeed = 10f;    // Speed at which the object follows the mouse
    public float sagTorque = 50f;    // Torque applied to create sag effect
    public float rotationDamping = 5f; // Damping to stabilize rotation

    void Start()
    {
        // Get the Rigidbody2D component for physics-based movement
        rb = GetComponent<Rigidbody2D>();

        // Disable gravity temporarily when dragging
        rb.gravityScale = 0;

        // Set the angular damping to smooth out rotation
        rb.angularDamping = rotationDamping;
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the mouse position in world coordinates
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Convert the mouse position to Vector2 to work with Rigidbody2D position (2D)
            Vector2 direction = (Vector2)mousePosition - rb.position;

            // Set the object's velocity towards the mouse position with drag speed
            rb.linearVelocity = direction * dragSpeed;

            // Optionally, apply rotation while dragging to simulate swinging (if you want)
            // You could also use the direction of dragging to determine rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    private void OnMouseDown()
    {
        // When the mouse is clicked on the object, start dragging
        isDragging = true;

        // Calculate the offset between the object and the mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = (Vector2)transform.position - (Vector2)mousePosition;

        // Allow free movement (remove any position constraints)
        rb.constraints = RigidbodyConstraints2D.None;
    }

    private void OnMouseUp()
    {
        // When the mouse is released, stop dragging
        isDragging = false;

        // Apply a random torque to simulate a "sag" effect (optional for realism)
        rb.AddTorque(Random.Range(-sagTorque, sagTorque));

        // Re-enable gravity and let the object fall naturally
        rb.gravityScale = 1;

        // Apply constraints to freeze position if you want it to stay in place
        // Remove this line if you want the object to move freely
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
