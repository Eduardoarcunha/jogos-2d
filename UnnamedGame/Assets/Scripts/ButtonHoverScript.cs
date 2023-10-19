using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalScale;
    public Color hoverColor = new Color32(220, 161, 29, 255);
    private Color originalColor;
    private TMP_Text buttonText;

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
    }

    private void Update()
    {
        // Smoothly adjust the scale
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, hoverScale, Time.deltaTime * scaleSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, hoverColor, Time.deltaTime * colorChangeSpeed);
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, originalColor, Time.deltaTime * colorChangeSpeed);
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
}
