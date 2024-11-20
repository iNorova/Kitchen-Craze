using UnityEngine;
using TMPro;  // For TextMeshPro
using System.Collections;
using System.Collections.Generic;

public class CookingGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI ingredientsTextBox;

    [Tooltip("The object that will be clicked/touched to process the mix (e.g., the pan).")]
    public GameObject mixObject;

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

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Ensure all target objects are initially hidden
        foreach (var combo in combinations)
        {
            if (combo.cookedObject != null)
            {
                combo.cookedObject.SetActive(false);
            }
        }

        // Ensure Mix Object (Pan) is initially active
        if (mixObject != null)
        {
            // Ensure it has a Collider2D for interaction
            var collider = mixObject.GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError("Mix Object must have a Collider2D to detect clicks/touches!");
            }

            mixObject.SetActive(true); // Keep the pan visible at the start
        }
        else
        {
            Debug.LogError("Mix Object is not assigned in the Inspector!");
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

            // Check if two or more ingredients are in the pan, enable the Mix Object
            if (currentIngredients.Count >= 2)
            {
                EnableMixObject();
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

        // Optionally move the object out of view
        ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, -100);
    }

    private void UpdateTextBox()
    {
        ingredientsTextBox.text = string.Join(", ", currentIngredientNames);
    }

    private void EnableMixObject()
    {
        if (mixObject != null)
        {
            mixObject.SetActive(true); // Show the pan for interaction
        }
    }

    private void Update()
    {
        // Handle mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction(Input.mousePosition);
        }

        // Handle touch inputs
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Only handle the touch when it begins
            if (touch.phase == TouchPhase.Began)
            {
                HandleInteraction(touch.position);
            }
        }
    }

    private void HandleInteraction(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject == mixObject && mixObject.activeSelf)
            {
                ProcessMix();
            }
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
            // Re-enable Renderers and Colliders to reset ingredients
            var renderer = ingredient.GetComponent<Renderer>();
            var collider = ingredient.GetComponent<Collider2D>();

            if (renderer != null) renderer.enabled = true;
            if (collider != null) collider.enabled = true;

            // Reset ingredient position if needed
            ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, 0);
        }

        currentIngredientNames.Clear();
        currentIngredients.Clear();
        UpdateTextBox();

        // Keep Mix Object (Pan) active for further interactions
        if (mixObject != null)
        {
            mixObject.SetActive(true); // Keep the pan visible
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
