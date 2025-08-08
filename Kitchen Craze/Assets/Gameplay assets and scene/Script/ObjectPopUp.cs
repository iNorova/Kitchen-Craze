using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CookingGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI ingredientsTextBox;

    [Tooltip("The object that will be clicked/touched to process the mix (e.g., the pot).")]
    public GameObject mixObject;

    [Tooltip("Delay in seconds before processing the mix.")]
    public float mixDelay = 4f;

    [Header("No Combination Objects")]
    public List<GameObject> objectsToDisplayOnNoCombination = new List<GameObject>();

    [Header("Trash Objects")]
    [Tooltip("List of 3 trash objects to randomly select from when no valid combination is found.")]
    public List<GameObject> trashObjects = new List<GameObject>();

    [Tooltip("Time (in seconds) the objects will be visible before hiding them.")]
    public float deactivationDelay = 1f;  // Adjustable timer for deactivation

    [System.Serializable]
    public class Combination
    {
        public List<GameObject> ingredientObjects;
        public GameObject cookedObject;
        public GameObject lockedStateObject;
        public GameObject unlockedStateObject;
    }

    [Header("Combinations Settings")]
    public List<Combination> combinations = new List<Combination>();

    private int completedDishes = 0;
    private List<string> currentIngredientNames = new List<string>();
    private List<GameObject> currentIngredients = new List<GameObject>();

    private Camera mainCamera;
    
    // Store all ingredient references for respawning
    private List<DragAndSwing> allIngredientReferences = new List<DragAndSwing>();

    private void Start()
    {
        mainCamera = Camera.main;

        // Find and store all ingredient references
        DragAndSwing[] allIngredients = FindObjectsOfType<DragAndSwing>();
        allIngredientReferences.AddRange(allIngredients);
        Debug.Log($"Found {allIngredientReferences.Count} ingredients to manage");

        foreach (var combo in combinations)
        {
            if (combo.cookedObject != null)
                combo.cookedObject.SetActive(false);

            if (combo.lockedStateObject != null)
                combo.lockedStateObject.SetActive(true);

            if (combo.unlockedStateObject != null)
                combo.unlockedStateObject.SetActive(false);
        }

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

            if (currentIngredients.Count >= 2)
            {
                EnableMixObject();
            }
        }

        // Hide the ingredient when it's added to the pot
        ingredient.SetActive(false);
    }

    // Call this method when ingredient dragging starts
    public void OnIngredientDragStart(GameObject ingredient)
    {
        var spriteRenderer = ingredient.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Set high sorting order to appear on top of everything
            spriteRenderer.sortingOrder = 200;
        }
    }

    // Call this method when ingredient dragging ends
    public void OnIngredientDragEnd(GameObject ingredient)
    {
        var spriteRenderer = ingredient.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Reset to normal sorting order
            spriteRenderer.sortingOrder = 0;
        }
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
        if (currentIngredientNames.Count > 0)
        {
            ingredientsTextBox.text = "Ingredients: " + string.Join(", ", currentIngredientNames);
        }
        else
        {
            ingredientsTextBox.text = "No ingredients added.";
        }
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
        if (currentIngredients.Count == 0)
        {
            Debug.Log("No ingredients in the pot!");
            ClearTextBox();
            DisplayNoCombinationObjects();
            return;
        }

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

        Debug.Log("No valid combination detected.");
        ClearTextBox();
        DisplayNoCombinationObjects();
        ClearIngredients();
        
        // Respawn all ingredients even when combination fails
        RespawnAllIngredients();
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

        UnlockPanels(combo);
        ClearIngredients();
        
        // Respawn all ingredients after successful combination
        RespawnAllIngredients();
    }

    private void UnlockPanels(Combination combo)
    {
        if (combo.lockedStateObject != null)
            combo.lockedStateObject.SetActive(false);

        if (combo.unlockedStateObject != null)
            combo.unlockedStateObject.SetActive(true);

        completedDishes++;
        Debug.Log($"Unlocked panel for dish: {combo.cookedObject.name} (Total completed dishes: {completedDishes})");

        CheckAllDishesUnlocked();
    }

    private void CheckAllDishesUnlocked()
    {
        bool allUnlocked = true;

        foreach (var combo in combinations)
        {
            if (combo.unlockedStateObject != null && !combo.unlockedStateObject.activeSelf)
            {
                allUnlocked = false;
                break;
            }
        }

        if (allUnlocked)
        {
            Debug.Log("All dishes are unlocked!");
        }
    }

    private void ClearIngredients()
    {
        if (currentIngredients.Count > 0)
        {
            foreach (var ingredient in currentIngredients)
            {
                var renderer = ingredient.GetComponent<Renderer>();
                var collider = ingredient.GetComponent<Collider2D>();

                // Reset the ingredient to its original state.
                if (renderer != null) renderer.enabled = true;
                if (collider != null) collider.enabled = true;

                // Reset position to the original position or some designated spawn point (e.g., ingredient's start position)
                ingredient.transform.position = new Vector3(ingredient.transform.position.x, ingredient.transform.position.y, 0);

                // Optionally, you can reset the rotation if needed.
                ingredient.transform.rotation = Quaternion.identity;
            }

            currentIngredientNames.Clear();
            currentIngredients.Clear();
        }

        if (mixObject != null)
        {
            mixObject.SetActive(true);
        }

        UpdateTextBox();
    }

    private void ClearTextBox()
    {
        if (ingredientsTextBox != null)
        {
            ingredientsTextBox.text = "No ingredients added.";
        }
    }

    private void DisplayNoCombinationObjects()
    {
        // Call the function to display and hide objects after each press.
        ToggleNoCombinationObjects();
    }

    private void ToggleNoCombinationObjects()
    {
        // Randomly select one trash object if available
        if (trashObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, trashObjects.Count);
            GameObject selectedTrashObject = trashObjects[randomIndex];
            
            // Show the randomly selected trash object
            selectedTrashObject.SetActive(true);
            var spriteRenderer = selectedTrashObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                // Set the sorting order to ensure it appears on top
                spriteRenderer.sortingOrder = 100;
            }

            // Trigger animation if the object has an Animator
            var animator = selectedTrashObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("Play");
                // Alternative: Reset and play the default animation
                animator.Play(0, 0, 0f);
            }

            // Hide the trash object after the deactivationDelay time
            StartCoroutine(HideTrashObjectAfterDelay(selectedTrashObject));
        }
        else
        {
            // Fallback to original behavior if no trash objects are assigned
            foreach (var obj in objectsToDisplayOnNoCombination)
            {
                obj.SetActive(true);
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                    // Set the sorting order to ensure it appears on top
                    spriteRenderer.sortingOrder = 100;
                }

                // Trigger animation if the object has an Animator
                var animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = true;
                    animator.SetTrigger("Play");
                    // Alternative: Reset and play the default animation
                    animator.Play(0, 0, 0f);
                }
            }

            // Hide the objects after the deactivationDelay time.
            StartCoroutine(HideNoCombinationObjectsAfterDelay());
        }
    }

    private IEnumerator HideTrashObjectAfterDelay(GameObject trashObject)
    {
        yield return new WaitForSeconds(deactivationDelay);

        // Stop animation if the object has an Animator
        var animator = trashObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Hide the trash object
        trashObject.SetActive(false);
        var spriteRenderer = trashObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            // Reset the sorting order when hiding
            spriteRenderer.sortingOrder = 0;
        }
    }

    private IEnumerator HideNoCombinationObjectsAfterDelay()
    {
        yield return new WaitForSeconds(deactivationDelay);  // Use the adjustable timer for deactivation

        // Hide the objects.
        foreach (var obj in objectsToDisplayOnNoCombination)
        {
            // Stop animation if the object has an Animator
            var animator = obj.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }

            obj.SetActive(false);
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
                // Reset the sorting order when hiding
                spriteRenderer.sortingOrder = 0;
            }
        }
    }

    private void RespawnAllIngredients()
    {
        // Respawn all ingredients using stored references
        foreach (DragAndSwing ingredient in allIngredientReferences)
        {
            if (ingredient != null)
            {
                ingredient.RespawnAfterCooking();
            }
        }
        
        Debug.Log($"Respawned {allIngredientReferences.Count} ingredients after cooking!");
    }
}