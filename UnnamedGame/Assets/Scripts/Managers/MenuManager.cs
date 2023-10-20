using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private void Awake()
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
        AudioManager.instance.PlaySound("MenuMusic");
    }
    

    public void PlayGame()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }

    public void Tutorial()
    {
        AudioManager.instance.StopSound("MenuMusic");
        GameManager.instance.ChangeState(GameManager.GameState.Tutorial);
    }

}
