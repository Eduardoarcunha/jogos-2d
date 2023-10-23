using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Image barSprite;
    [SerializeField] private float updateSpeed = 0.2f;

    private Coroutine updateBarCoroutine;

    public void SetBar(float value, float maxValue)
    {
        barSprite.fillAmount = value / maxValue;
    }

    public void UpdateBar(float value, float maxValue)
    {
        if (updateBarCoroutine != null)
        {
            StopCoroutine(updateBarCoroutine);
        }

        updateBarCoroutine = StartCoroutine(UpdateBarRoutine(value / maxValue));
    }

    private IEnumerator UpdateBarRoutine(float targetFillAmount)
    {
        float elapsedDuration = 0f;
        float startingFillAmount = barSprite.fillAmount;

        while (elapsedDuration < updateSpeed)
        {
            elapsedDuration += Time.deltaTime;
            barSprite.fillAmount = Mathf.Lerp(startingFillAmount, targetFillAmount, elapsedDuration / updateSpeed);
            yield return null;
        }

        barSprite.fillAmount = targetFillAmount;
    }
}
