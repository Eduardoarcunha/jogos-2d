using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager instance;

    [Header("References")]
    public Bar progressBar;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject levelUpPanel;

    private int currentLevel = 1;
    private int currentExp = 0;
    private int currentExpToNextLevel = 10;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        progressBar.SetBar(currentExp, currentExpToNextLevel);
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        progressBar.UpdateBar(currentExp, currentExpToNextLevel);
        if (currentExp >= currentExpToNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentLevel++;
        currentExp = 0;
        currentExpToNextLevel = currentLevel * 10;
        progressBar.SetBar(currentExp, currentExpToNextLevel);
        GameManager.instance.ChangeState(GameManager.GameState.LevelUp);
        StartCoroutine(LevelUpCoroutine());   
    }

    private IEnumerator LevelUpCoroutine()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true); 
    }

    public void IncreaseDamage()
    {
        playerCombat.IncreaseDamage();
        Time.timeScale = 1f;
        levelUpPanel.SetActive(false);
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }

    public void IncreaseSpeed()
    {
        playerController.IncreaseSpeed();
        Time.timeScale = 1f;
        levelUpPanel.SetActive(false);
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }
}
