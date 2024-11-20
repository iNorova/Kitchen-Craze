using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Pausemenu : MonoBehaviour
{
    public GameObject PausePanel;
    
    void Update()
    {

    }
    public void Pause()
{
    PausePanel.SetActive(true);
    Time.timeScale = 0; 
    
}   
public void Continue()
{
    Time.timeScale = 1;
    
}


}