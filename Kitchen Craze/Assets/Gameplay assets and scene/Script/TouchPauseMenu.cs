using UnityEngine;
using UnityEngine.UI;

public class TouchPauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // The Pause Menu UI Panel
    public Button resumeButton;     // Resume button in the UI
    public Button quitButton;       // Quit button in the UI

    public string pauseAssetTag = "PauseAsset"; // Tag for the pause button sprite
    
    private bool isPaused = false;

    void Start()
    {
        // Make sure the Pause Menu is inactive at the start
        pauseMenuUI.SetActive(false);

        // Add listeners for the buttons
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitMenu);
    }

    void Update()
    {
        // Check if there's a touch on the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Convert touch position to world coordinates
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            // Use a raycast to check if the touch hits a collider with the specified tag
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            // Debug log to check if anything was hit
            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);

                // Check if the hit object has the correct tag
                if (hit.collider.CompareTag(pauseAssetTag))
                {
                    Debug.Log("Pause button tapped!");
                    TogglePauseMenu();
                }
                else
                {
                    Debug.Log("Tapped object is not the pause button.");
                }
            }
            else
            {
                Debug.Log("No object hit.");
            }
        }
    }

    // Toggle the pause menu and game pause state
    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);

        // Pause or resume the game
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // Resume the game by hiding the pause menu
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    // Quit the menu without affecting game time
    public void QuitMenu()
    {
        pauseMenuUI.SetActive(false);
    }
}
