using UnityEngine;

public class PanAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public string panIdleAnimationName = "lidclose";
    public string panClickAnimationName = "panwhenpressed";
    public string panLidIdleAnimationName = "lidclose";
    public string panLidClickAnimationName = "panwhenpressed";
    public float clickAnimationDuration = 2f;
    
    [Header("Animation Delays")]
    public float panAnimationDelay = 0f;
    public float panLidAnimationDelay = 0f;
    
    [Header("Components")]
    public Animator panAnimator;
    public Animator panLidAnimator;
    
    private bool isAnimating = false;
    private float animationTimer = 0f;
    private string currentPanAnimation = "";
    private string currentPanLidAnimation = "";
    private bool panAnimationStarted = false;
    private bool panLidAnimationStarted = false;

    void Start()
    {
        // Get the Animator components if not assigned
        if (panAnimator == null)
        {
            panAnimator = GetComponent<Animator>();
        }
        
        // Find PanLid GameObject and get its Animator
        if (panLidAnimator == null)
        {
            GameObject panLid = GameObject.Find("PanLid");
            if (panLid != null)
            {
                panLidAnimator = panLid.GetComponent<Animator>();
            }
        }
        
        // Start with idle animations
        ResetToIdle();
    }

    void Update()
    {
        // Handle delayed animation starts
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            
            // Start Pan animation after delay
            if (!panAnimationStarted && animationTimer >= panAnimationDelay)
            {
                if (panAnimator != null)
                {
                    PlayPanAnimation(panClickAnimationName);
                }
                panAnimationStarted = true;
            }
            
            // Start PanLid animation after delay
            if (!panLidAnimationStarted && animationTimer >= panLidAnimationDelay)
            {
                if (panLidAnimator != null)
                {
                    PlayPanLidAnimation(panLidClickAnimationName);
                }
                panLidAnimationStarted = true;
            }
            
            // Auto-reset to idle after animation completes
            if (animationTimer >= clickAnimationDuration)
            {
                ResetToIdle();
            }
        }
    }

    void OnMouseDown()
    {
        if (!isAnimating)
        {
            TriggerClickAnimation();
        }
    }

    // Method to play Pan animation
    public void PlayPanAnimation(string animationName)
    {
        if (panAnimator != null && !string.IsNullOrEmpty(animationName))
        {
            // Reset the animator state before playing new animation
            panAnimator.Rebind();
            panAnimator.Update(0f);
            
            panAnimator.Play(animationName);
            currentPanAnimation = animationName;
        }
    }

    // Method to play PanLid animation
    public void PlayPanLidAnimation(string animationName)
    {
        if (panLidAnimator != null && !string.IsNullOrEmpty(animationName))
        {
            // Reset the animator state before playing new animation
            panLidAnimator.Rebind();
            panLidAnimator.Update(0f);
            
            panLidAnimator.Play(animationName);
            currentPanLidAnimation = animationName;
        }
    }

    // Method to manually trigger the click animations
    public void TriggerClickAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            animationTimer = 0f;
            panAnimationStarted = false;
            panLidAnimationStarted = false;
            
            // Animations will start based on their individual delays in Update()
        }
    }

    // Method to reset both to idle state
    public void ResetToIdle()
    {
        isAnimating = false;
        animationTimer = 0f;
        panAnimationStarted = false;
        panLidAnimationStarted = false;
        
        // Reset both animations to idle
        if (panAnimator != null)
        {
            PlayPanAnimation(panIdleAnimationName);
        }
        
        if (panLidAnimator != null)
        {
            PlayPanLidAnimation(panLidIdleAnimationName);
        }
    }

    // Get current animation states
    public string GetCurrentPanAnimation()
    {
        return currentPanAnimation;
    }

    public string GetCurrentPanLidAnimation()
    {
        return currentPanLidAnimation;
    }

    // Check if currently animating
    public bool IsAnimating()
    {
        return isAnimating;
    }
}
