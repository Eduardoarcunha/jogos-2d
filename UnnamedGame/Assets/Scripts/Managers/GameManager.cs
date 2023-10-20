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

    public static event Action<bool> OnPauseOrResumeGame;
    
    public GameState gameState { get; private set;}

    private bool paused = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ChangeState(GameState.Menu);
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {   
        PlayerCombat.OnDeathEvent += PlayerDeath;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && gameState != GameState.Menu && gameState != GameState.GameOver) PauseOrResume();
    }



    public void ChangeState(GameState newGameState)
    {
        if (gameState == newGameState) return;

        OnBeforeGameStateChange?.Invoke(newGameState);
        GameState oldGameState = gameState;

        gameState = newGameState;
        switch (gameState)
        {
            case GameState.Menu:
                SceneLoaderManager.instance.LoadScene("MenuScene");
                break;
            case GameState.Tutorial:
                SceneLoaderManager.instance.LoadScene("TutorialScene");
                break;
            case GameState.LevelUp:
                break;
            case GameState.Play:
                if (oldGameState != GameState.LevelUp)
                {
                    SceneLoaderManager.instance.LoadScene("DarkFireCastle");
                }
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
        
        OnAfterGameStateChange?.Invoke(gameState);
    }

    public void PlayerDeath(){
        ChangeState(GameState.GameOver);
    }

    public void PauseOrResume(){
        paused = !paused;
        OnPauseOrResumeGame?.Invoke(paused);
    }

    public enum GameState
    {
        Menu = 0,
        Tutorial = 1,
        Play = 2,
        LevelUp = 3,
        GameOver = 4
    }

    void OnDestroy()
    {
        PlayerCombat.OnDeathEvent -= PlayerDeath;
    }
}