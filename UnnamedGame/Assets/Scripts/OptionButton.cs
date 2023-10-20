using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalScale;
    public Color hoverColor = new Color32(220, 161, 29, 255); // Color DCA11D
    private Color originalColor;
    private TMP_Text buttonText; // TextMeshProUGUI

    private Image buttonImage;
    private Color originalImageColor;

    // Variables for smooth scaling and color change
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
        Debug.Log(buttonText);
        Debug.Log(buttonImage);
        Debug.Log(isHovering);
        // Smoothly adjust the scale
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, hoverScale, Time.unscaledDeltaTime * scaleSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, hoverColor, Time.unscaledDeltaTime * colorChangeSpeed);
            }

            if (buttonImage != null)
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

            if (buttonImage != null)
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

    public void IncreaseDamage()
    {
        ProgressionManager.instance.IncreaseDamage();
    }

    public void IncreaseSpeed()
    {
        ProgressionManager.instance.IncreaseSpeed();
    }
}
