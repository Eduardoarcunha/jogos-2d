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
            
        } else {
            Destroy(gameObject);
        }
        
        PlayerCombat.OnDeathEvent += PlayerDeath;
    }

    void Start()
    {   
        if (SceneManager.GetActiveScene().name == "MenuScene"){
            ChangeState(GameState.Menu);
            AudioManager.instance.PlaySound("MenuMusic");
        }
        else if (SceneManager.GetActiveScene().name == "TutorialScene") ChangeState(GameState.Tutorial);
        else if (SceneManager.GetActiveScene().name == "DarkFireCastle") ChangeState(GameState.Play);
        else if (SceneManager.GetActiveScene().name == "GameOverScene") ChangeState(GameState.GameOver);
        else if (SceneManager.GetActiveScene().name == "WonGameScene") ChangeState(GameState.WonGame);
        else ChangeState(GameState.Menu);
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
                AudioManager.instance.PlaySound("MenuMusic");
                break;
            case GameState.Tutorial:
                SceneLoaderManager.instance.LoadScene("TutorialScene");
                break;
            case GameState.LevelUp:
                break;
            case GameState.Play:
                if (oldGameState != GameState.LevelUp)
                {
                    AudioManager.instance.StopSound("MenuMusic");
                    SceneLoaderManager.instance.LoadScene("DarkFireCastle");
                }
                break;
            case GameState.GameOver:
                AudioManager.instance.StopSound("BossMusic");
                break;
            case GameState.WonGame:
                AudioManager.instance.StopSound("BossMusic");
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
        GameOver = 4,
        WonGame = 5
    }

    void OnDestroy()
    {
        PlayerCombat.OnDeathEvent -= PlayerDeath;
    }
}