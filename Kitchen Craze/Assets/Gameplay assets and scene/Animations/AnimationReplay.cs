using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
    }

    void OnMouseDown() // Trigger animation when the object is clicked
    {
        if (animator != null)
        {
            animator.ResetTrigger("PlayAnimation"); // Reset the trigger to ensure replay
            animator.SetTrigger("PlayAnimation");  // Set the trigger to start the animation
        }
        else
        {
            Debug.LogWarning("Animator not found on GameObject!");
        }
    }
}