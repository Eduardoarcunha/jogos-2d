using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WonManager : MonoBehaviour
{
    [SerializeField] private GameObject won;
    [SerializeField] private TMP_Text titleText;

    void Start()
    {
        GameManager.OnAfterGameStateChange += OnChangeGameState;
    }

    void OnChangeGameState(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.WonGame)
        {
            StartCoroutine(WonScreen());
        }
        else
        {
            won.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    IEnumerator WonScreen()
    {
        yield return new WaitForSeconds(2f);
        won.SetActive(true);
        titleText.text = "You Won!";
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }


    void OnDestroy()
    {
        GameManager.OnAfterGameStateChange -= OnChangeGameState;
    }
}
