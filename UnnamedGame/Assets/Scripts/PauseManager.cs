using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    void Start()
    {
        GameManager.OnPauseOrResumeGame += OnPauseOrResumeGame;
    }

    void OnPauseOrResumeGame(bool paused)
    {
        if (paused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void OnDestroy()
    {
        GameManager.OnPauseOrResumeGame -= OnPauseOrResumeGame;
    }
}
