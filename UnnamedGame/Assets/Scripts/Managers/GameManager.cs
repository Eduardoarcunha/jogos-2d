using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action<GameState> OnBeforeGameStateChange;
    public static event Action<GameState> OnAfterGameStateChange;
    
    public GameState gameState { get; private set;}

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
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newGameState)
    {
        if (gameState == newGameState) return;

        OnBeforeGameStateChange?.Invoke(newGameState);

        gameState = newGameState;
        switch (gameState)
        {
            case GameState.Menu:
                SceneManager.LoadScene("MenuScene");
                break;
            case GameState.Tutorial:
                SceneManager.LoadScene("TutorialScene");
                break;
            case GameState.Play:
                SceneManager.LoadScene("DarkFireCastle");
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
        
        OnAfterGameStateChange?.Invoke(gameState);
    }

    public enum GameState
    {
        Menu = 0,
        Tutorial = 1,
        Play = 2,
        GameOver = 3
    }

}