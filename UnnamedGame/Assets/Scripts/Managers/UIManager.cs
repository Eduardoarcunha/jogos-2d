using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private TMP_Text endGameTitleText;
    [SerializeField] private GameObject pausePanel;

    [Header("UI Elements")]
    [SerializeField] private GameObject loreCanvas;
    public TMP_Text loreText;
    public float typingSpeed = 0.05f;
    public float delayAfterComplete = 2.0f;
    
    [Header("Lore Content")]
    [TextArea(5, 10)]
    public string loreContent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
        } else {
            Destroy(gameObject);
        }        
    }
    
    void Start()
    {
        GameManager.OnPauseOrResumeGame += OnPauseOrResumeGame;
        GameManager.OnAfterGameStateChange += OnChangeGameState;
    }

    void OnChangeGameState(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.WonGame)
        {
            endGameTitleText.text = "You Won!";
            StartCoroutine(EndGame());
        }
        else if (newGameState == GameManager.GameState.GameOver)
        {
            endGameTitleText.text = "Game Over";
            StartCoroutine(EndGame());
        }
        else
        {
            endGameCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
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

    public void InitializeGame()
    {
        loreCanvas.SetActive(true);
        StartCoroutine(DisplayLore());
    }

    IEnumerator DisplayLore()
    {
        loreText.text = "";
        for (int i = 0; i < loreContent.Length; i++)
        {
            loreText.text += loreContent[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(delayAfterComplete);
        loreCanvas.SetActive(false);
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        endGameCanvas.SetActive(true);
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }



    void OnDestroy()
    {
        GameManager.OnPauseOrResumeGame -= OnPauseOrResumeGame;
        GameManager.OnAfterGameStateChange -= OnChangeGameState;
    }
}
