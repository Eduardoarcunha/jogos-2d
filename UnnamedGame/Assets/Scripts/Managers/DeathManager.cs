using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private GameObject death;

    void Start()
    {
        GameManager.OnAfterGameStateChange += OnChangeGameState;
    }

    void OnChangeGameState(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.GameOver)
        {
            StartCoroutine(DeadScreen());
        }
        else
        {
            death.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    IEnumerator DeadScreen()
    {
        yield return new WaitForSeconds(2f);
        death.SetActive(true);
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }


    void OnDestroy()
    {
        GameManager.OnAfterGameStateChange -= OnChangeGameState;
    }
}
