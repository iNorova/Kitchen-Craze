using UnityEngine;
using TMPro; // For TextMeshPro
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
        public List<GameObject> ingredientObjects; // Ingredients needed for this combination
        public GameObject cookedObject; // Resulting object (e.g., dish) to activate
        public GameObject lockedStateObject; // Object representing the locked state in the panel
        public GameObject unlockedStateObject; // Object representing the unlocked state in the panel
    }

    [Header("Combinations Settings")]
    public List<Combination> combinations = new List<Combination>();

    [Header("Game Progression")]
    [Tooltip("Total number of dishes needed to complete the game.")]
    public int totalDishes = 20;

    private int completedDishes = 0; // Tracks how many dishes are completed
    private List<string> currentIngredientNames = new List<string>();
    private List<GameObject> currentIngredients = new List<GameObject>();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Initialize all locked/unlocked objects
        foreach (var combo in combinations)
        {
            if (combo.cookedObject != null)
                combo.cookedObject.SetActive(false);

            if (combo.lockedStateObject != null)
                combo.lockedStateObject.SetActive(true); // Show locked state initially

            if (combo.unlockedStateObject != null)
                combo.unlockedStateObject.SetActive(false); // Hide unlocked state initially
        }

        // Ensure Mix Object (Pan) is initially active
        if (mixObject != null)
        {
            var collider = mixObject.GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError("Mix Object must have a Collider2D to detect clicks/touches!");
            }

            mixObject.SetActive(true);
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
        string ingredientName = ingredient.name.Replace("(Clone)", "").Trim();
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

        HideIngredient(ingredient);
    }

    private void HideIngredient(GameObject ingredient)
    {
        var renderer = ingredient.GetComponent<Renderer>();
        var collider = ingredient.GetComponent<Collider2D>();

        if (renderer != null) renderer.enabled = false;
        if (collider != null) collider.enabled = false;

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
            mixObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction(Input.mousePosition);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
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

        // Unlock associated panels
        UnlockPanels(combo);

        ClearIngredients();
    }

    private void UnlockPanels(Combination combo)
    {
        if (combo.lockedStateObject != null)
            combo.lockedStateObject.SetActive(false); // Hide locked state

        if (combo.unlockedStateObject != null)
            combo.unlockedStateObject.SetActive(true); // Show unlocked state

        completedDishes++;
        Debug.Log($"Unlocked panel for dish: {combo.cookedObject.name} (Total completed dishes: {completedDishes})");

        if (completedDishes == totalDishes)
        {
            Debug.Log("All dishes are completed!");
            ResetGame();
        }
    }

    private void ClearIngredients()
    {
        foreach (var ingredient in currentIngredients)
        {
            var renderer = ingredient.GetComponent<Renderer>();
            var collider = ingredient.GetComponent<Collider2D>();

            if (renderer != null) renderer.enabled = true;
            if (collider != null) collider.enabled = true;

            ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, 0);
        }

        currentIngredientNames.Clear();
        currentIngredients.Clear();
        UpdateTextBox();

        if (mixObject != null)
        {
            mixObject.SetActive(true);
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

            if (combo.lockedStateObject != null)
            {
                combo.lockedStateObject.SetActive(true); // Reset to locked state
            }

            if (combo.unlockedStateObject != null)
            {
                combo.unlockedStateObject.SetActive(false); // Reset to hidden unlocked state
            }
        }

        completedDishes = 0; // Reset progression
        Debug.Log("Game reset!");
    }
}