using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add the TMP namespace
using System.Collections.Generic;

public class ResetButton : MonoBehaviour
{
    [Header("References")]
    public Button resetButton;  // Reference to the reset button

    // The list of ingredients (objects) to reset to their original state and position
    public List<GameObject> ingredientsToReset;

    // A list to store the initial state (position, rotation, visibility) of each ingredient
    private List<IngredientState> initialStates = new List<IngredientState>();

    // To store the current state of the text
    private string currentText;

    // To store the most recent ingredient added
    private string currentIngredient;

    [Header("UI References")]
    public TextMeshProUGUI ingredientIndicator;  // Reference to the TextMeshPro UI that needs to reset

    private void Start()
    {
        // Save the initial state of all ingredients to reset
        SaveInitialStates();

        // Save the current text state
        SaveCurrentText();

        // Ensure the button is set up and listen for the click event
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClicked);
        }
        else
        {
            Debug.LogError("Reset button is not assigned.");
        }
    }

    // This method is called when the reset button is clicked
    private void OnResetButtonClicked()
    {
        UndoReset();  // Call the UndoReset method to restore objects and text to the previous state
    }

    // Undo the reset by restoring the objects and text to their previous state
    private void UndoReset()
    {
        ResetIngredients();  // Resets the specific ingredients (objects) to their previous state
        ResetUI();  // Restores the UI text to its previous state
    }

    // Save the initial state of the ingredients (position, rotation, and visibility)
    private void SaveInitialStates()
    {
        initialStates.Clear();  // Clear the list of stored initial states

        foreach (var ingredient in ingredientsToReset)
        {
            if (ingredient != null)
            {
                IngredientState state = new IngredientState
                {
                    originalPosition = ingredient.transform.position,
                    originalRotation = ingredient.transform.rotation,
                    originalVisibility = ingredient.GetComponent<Renderer>().enabled,
                    // Save the state of Rigidbody2D and Collider2D if they exist
                    hasRigidbody2D = ingredient.GetComponent<Rigidbody2D>() != null,
                    hasCollider2D = ingredient.GetComponent<Collider2D>() != null
                };
                initialStates.Add(state);
            }
        }

        Debug.Log("Initial states have been saved.");
    }

    // Save the current state of the UI text
    private void SaveCurrentText()
    {
        if (ingredientIndicator != null)
        {
            currentText = ingredientIndicator.text;  // Store the current text to restore later
            currentIngredient = currentText;  // Store the most recent ingredient
        }
    }

    // Reset the ingredients back to their original state (position, rotation, visibility, Rigidbody2D, and Collider2D)
    private void ResetIngredients()
    {
        for (int i = 0; i < ingredientsToReset.Count; i++)
        {
            var ingredient = ingredientsToReset[i];
            if (ingredient != null && i < initialStates.Count)
            {
                // Reset position and rotation without involving physics
                ingredient.transform.position = initialStates[i].originalPosition;
                ingredient.transform.rotation = initialStates[i].originalRotation;

                // Reset visibility (make sure the object is visible)
                ingredient.GetComponent<Renderer>().enabled = initialStates[i].originalVisibility;

                // Ensure the object is not parented to anything else that could alter its position
                ingredient.transform.SetParent(null);

                // Check and restore Rigidbody2D and Collider2D components if necessary
                if (initialStates[i].hasRigidbody2D)
                {
                    Rigidbody2D rb2D = ingredient.GetComponent<Rigidbody2D>();
                    if (rb2D != null)
                    {
                        rb2D.isKinematic = false;  // Make sure the Rigidbody2D is not kinematic anymore
                    }
                }

                if (initialStates[i].hasCollider2D)
                {
                    Collider2D col2D = ingredient.GetComponent<Collider2D>();
                    if (col2D != null)
                    {
                        col2D.enabled = true;  // Ensure Collider2D is enabled
                    }
                }
            }
        }

        Debug.Log("Ingredients have been reset to their original positions, states, and components.");
    }

    // Reset the UI text (TextMeshPro) back to the previous state
    private void ResetUI()
    {
        if (ingredientIndicator != null)
        {
            // Reset the ingredient indicator text to its default or empty state
            ingredientIndicator.text = "";  // Clear the text

            // Then, update it with the most recent ingredient added (if applicable)
            if (!string.IsNullOrEmpty(currentIngredient))
            {
                ingredientIndicator.text = currentIngredient;  // Display the most recent ingredient
            }
        }
    }

    // Helper class to store the initial state of each ingredient
    [System.Serializable]
    public class IngredientState
    {
        public Vector3 originalPosition;
        public Quaternion originalRotation;
        public bool originalVisibility; // Track visibility (whether the Renderer was enabled)
        public bool hasRigidbody2D; // Track if the object has a Rigidbody2D
        public bool hasCollider2D; // Track if the object has a Collider2D
    }
}
