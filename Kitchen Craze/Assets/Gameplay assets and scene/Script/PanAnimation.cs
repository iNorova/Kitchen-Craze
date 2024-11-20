using UnityEngine;

public class PanAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    void OnMouseDown() // This function is called when the sprite is clicked
    {
        if (animator != null)
        {
            // Trigger the 'OnClick' animation (start the pan animation)
            animator.SetTrigger("OnClick");
        }
    }
}
