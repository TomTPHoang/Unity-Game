using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private float musicVolume = 0.05f;

    private void Start()
    {
        ActivateMainMenu(true);

        backgroundMusic.loop = true;
        backgroundMusic.volume = musicVolume;
        backgroundMusic.Play();
    }
    public void ActivateMainMenu(bool state)
    {
        mainMenu.SetActive(state);
        optionsMenu.SetActive(!state);
    }

    public void Play()
    {
        backgroundMusic.Stop();
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
