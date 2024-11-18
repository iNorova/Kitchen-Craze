using UnityEngine;
using TMPro;  // For TextMeshPro

public class IngredientDropper : MonoBehaviour
{
    public TextMeshProUGUI boardText;  // Reference to the TextMeshProUGUI component to show dropped ingredients
    private bool isInPan = false;  // To check if the ingredient is dropped in the pan

    // Called when the ingredient collides with the pan's collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ingredient"))  // Check if the other object is tagged as 'Ingredient'
        {
            // Get the name of the ingredient asset from the hierarchy
            string ingredientName = other.gameObject.name;

            // Despawn the ingredient
            Destroy(other.gameObject);  // Destroy the dropped ingredient

            // Update the board with the ingredient name
            AddIngredientToBoard(ingredientName);
        }
    }

    // Function to update the board when an ingredient is dropped into the pan
    void AddIngredientToBoard(string ingredientName)
    {
        if (boardText != null)
        {
            // Add the ingredient to the board's text (e.g., a list of dropped items)
            boardText.text += ingredientName + "\n";  // Display on a new line
        }
    }
}
