using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplateProjects;

public class UIManager : MonoBehaviour
{
    private bool isPaused = false;

    [SerializeField] private GameObject hudCanvas = null;
    [SerializeField] private GameObject pauseCanvas = null;
    [SerializeField] private GameObject endCanvas = null;

    private PlayerStats stats;

    private void Start()
    {
        Time.timeScale = 1;
        GetReferences();
        SetActiveHud(true);
    }

    private void Update()
    {
        if(!stats.IsDead())
        {
            if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
                SetActivePause(true);
            else if(Input.GetKeyDown(KeyCode.Escape) && isPaused)
                SetActivePause(false);
        }
    }

    public void SetActiveHud(bool state)
    {
            hudCanvas.SetActive(state);
            endCanvas.SetActive(!state);

            if(!stats.IsDead())
                pauseCanvas.SetActive(!state);
    }

    public void SetActivePause(bool state)
    {
        pauseCanvas.SetActive(state);
        hudCanvas.SetActive(!state);

        Time.timeScale = state ? 0 : 1;
        if(state)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isPaused = state;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void GetReferences()
    {
        stats = GetComponent<PlayerStats>();
    }
}
