using UnityEngine;

public class CloseMenu : MonoBehaviour

{
    // Reference to the menu panel
    public GameObject menuPanel;

    // Method to close the menu
    public void Close()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false); // Hide the menu
        }
    }
}
