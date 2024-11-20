using UnityEngine;

public class UniversalDragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 offset;

    // Sprites for dragging and idle states
    public Sprite defaultSprite;   // Set in the Inspector for idle state
    public Sprite draggingSprite;  // Set in the Inspector for dragging state

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set the initial sprite to default
        if (defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    void Update()
    {
        HandleInput();
        
        // Change the sprite based on dragging state
        if (isDragging)
        {
            spriteRenderer.sprite = draggingSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void HandleInput()
    {
        // Handle mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    startPosition = transform.position;
                    offset = startPosition - touchPosition;
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                transform.position = touchPosition + offset;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        // Handle mouse input for testing in Unity editor
        else if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                startPosition = transform.position;
                offset = startPosition - mousePosition;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition + offset;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}