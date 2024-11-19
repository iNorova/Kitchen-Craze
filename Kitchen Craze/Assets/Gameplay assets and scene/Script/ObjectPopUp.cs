using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CookingGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI ingredientsTextBox;
    public Button mixButton;

    [Tooltip("Delay in seconds before processing the mix.")]
    public float mixDelay = 4f;

    [System.Serializable]
    public class Combination
    {
        public List<GameObject> ingredientObjects;
        public GameObject cookedObject; // Object to activate when the combination matches
    }

    [Header("Combinations Settings")]
    public List<Combination> combinations = new List<Combination>();

    private List<string> currentIngredientNames = new List<string>();
    private List<GameObject> currentIngredients = new List<GameObject>();

    private void Start()
    {
        // Ensure all target objects are initially hidden
        foreach (var combo in combinations)
        {
            if (combo.cookedObject != null)
            {
                combo.cookedObject.SetActive(false);
            }
        }

        if (mixButton != null)
        {
            mixButton.onClick.AddListener(ProcessMix);
            mixButton.interactable = false;  // Initially disable the Mix Button
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FoodItem"))
        {
            AddIngredient(other.gameObject);
        }
    }

    public void AddIngredient(GameObject ingredient)
    {
        string ingredientName = ingredient.name.Replace("(Clone)", "").Trim(); // Strip "(Clone)" from the name
        if (!currentIngredientNames.Contains(ingredientName))
        {
            currentIngredientNames.Add(ingredientName);
            currentIngredients.Add(ingredient);
            UpdateTextBox();

            // Check if two or more ingredients are in the pan, enable the Mix Button
            if (currentIngredients.Count >= 2)
            {
                EnableMixButton();
            }
        }

        // Instead of destroying, disable the Renderer and Collider
        HideIngredient(ingredient);
    }

    private void HideIngredient(GameObject ingredient)
    {
        // Disable the Renderer and Collider to "hide" the ingredient
        var renderer = ingredient.GetComponent<Renderer>();
        var collider = ingredient.GetComponent<Collider2D>();

        if (renderer != null) renderer.enabled = false;
        if (collider != null) collider.enabled = false;

        // Optional: Move the object out of view (e.g., below the visible screen)
        ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, -100);
    }

    private void UpdateTextBox()
    {
        ingredientsTextBox.text = string.Join(", ", currentIngredientNames);
    }

    private void EnableMixButton()
    {
        if (mixButton != null)
        {
            mixButton.interactable = true;  // Enable the Mix Button when there are two or more ingredients
        }
    }

    private void ProcessMix()
    {
        StartCoroutine(MixCoroutine());
    }

    private IEnumerator MixCoroutine()
    {
        yield return new WaitForSeconds(mixDelay);

        foreach (var combo in combinations)
        {
            if (IsCombinationMatch(combo.ingredientObjects))
            {
                ActivateCombination(combo);
                yield break;
            }
        }

        Debug.LogWarning("No matching combination found!");
    }

    private bool IsCombinationMatch(List<GameObject> requiredIngredients)
    {
        if (requiredIngredients.Count != currentIngredients.Count) return false;

        foreach (var requiredIngredient in requiredIngredients)
        {
            string requiredName = requiredIngredient.name.Replace("(Clone)", "").Trim();
            bool found = currentIngredients.Exists(ingredient =>
                ingredient.name.Replace("(Clone)", "").Trim().Equals(requiredName, System.StringComparison.OrdinalIgnoreCase));

            if (!found) return false;
        }

        return true;
    }

    private void ActivateCombination(Combination combo)
    {
        if (combo.cookedObject != null && combo.cookedObject.CompareTag("Cooked"))
        {
            combo.cookedObject.SetActive(true);
            Debug.Log($"Activated: {combo.cookedObject.name}");
        }
        else
        {
            Debug.LogError("Combination is missing a target object, or target object is not tagged as 'Cooked'!");
        }

        ClearIngredients();
    }

    private void ClearIngredients()
    {
        foreach (var ingredient in currentIngredients)
        {
            // Re-enable Renderers and Colliders of ingredients to reset them
            var renderer = ingredient.GetComponent<Renderer>();
            var collider = ingredient.GetComponent<Collider2D>();

            if (renderer != null) renderer.enabled = true;
            if (collider != null) collider.enabled = true;

            // Move ingredient back to its original state if necessary
            ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, 0);
        }

        currentIngredientNames.Clear();
        currentIngredients.Clear();
        UpdateTextBox();

        // Disable Mix Button after ingredients are cleared
        if (mixButton != null)
        {
            mixButton.interactable = false;
        }
    }

    public void ResetGame()
    {
        ClearIngredients();

        foreach (var combo in combinations)
        {
            if (combo.cookedObject != null)
            {
                combo.cookedObject.SetActive(false);
            }
        }
    }
}
