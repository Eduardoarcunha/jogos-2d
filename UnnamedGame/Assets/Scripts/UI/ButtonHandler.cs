using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalScale;
    public Color hoverColor = new Color32(220, 161, 29, 255);
    private Color originalColor;
    private TMP_Text buttonText;

    private Image buttonImage;
    private Color originalImageColor;

    private float scaleSpeed = 2f;
    private float colorChangeSpeed = 4f;
    private bool isHovering = false;

    private void Start()
    {
        originalScale = transform.localScale;
        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            originalColor = buttonText.color;
        }
        buttonImage = GetComponentInChildren<Image>();
        if (buttonImage != null)
        {
            originalImageColor = buttonImage.color;
        }
    }

    private void Update()
    {
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, hoverScale, Time.unscaledDeltaTime * scaleSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, hoverColor, Time.unscaledDeltaTime * colorChangeSpeed);
            }

            if (buttonImage != null && gameObject.CompareTag("ImageButton"))
            {
                buttonImage.color = Color.Lerp(buttonImage.color, hoverColor, Time.unscaledDeltaTime * colorChangeSpeed);
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, originalColor, Time.unscaledDeltaTime * colorChangeSpeed);
            }
            if (buttonImage != null && gameObject.CompareTag("ImageButton"))
            {
                buttonImage.color = Color.Lerp(buttonImage.color, originalImageColor, Time.unscaledDeltaTime * colorChangeSpeed);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        AudioManager.instance.PlaySound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }


    #region Button Clicks

    public void PlayGame()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }

    public void Tutorial()
    {
        AudioManager.instance.StopSound("MenuMusic");
        GameManager.instance.ChangeState(GameManager.GameState.Tutorial);
    }

    public void ReturnToMenu()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Menu);
    }

    public void RestartGame()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Play);
    }

    public void PauseOrResume()
    {
        GameManager.instance.PauseOrResume();
    }

    public void IncreaseDamage()
    {
        ProgressionManager.instance.IncreaseDamage();
    }

    public void IncreaseSpeed()
    {
        ProgressionManager.instance.IncreaseSpeed();
    }


    #endregion
}
