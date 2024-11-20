using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Pausemenu : MonoBehaviour
{
    public GameObject PausePanel;
    public AudioSource BackgroundMusic;
    void Update()
    {

    }
    public void Pause()
{
    PausePanel.SetActive(true);
    Time.timeScale = 0; 
    BackgroundMusic.Pause();
}   
public void Continue()
{
    Time.timeScale = 1;
    BackgroundMusic.UnPause();
}


}