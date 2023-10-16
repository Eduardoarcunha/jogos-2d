using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private float updateSpeed = 0.2f; // You can adjust this value for faster/slower updates

    private Coroutine updateHealthCoroutine;

    public void UpdateHealthBar(float health, float maxHealth)
    {
        if (updateHealthCoroutine != null)
        {
            StopCoroutine(updateHealthCoroutine);
        }

        updateHealthCoroutine = StartCoroutine(UpdateHealthBarRoutine(health / maxHealth));
    }

    private IEnumerator UpdateHealthBarRoutine(float targetFillAmount)
    {
        float elapsedDuration = 0f;
        float startingFillAmount = healthBarSprite.fillAmount;

        while (elapsedDuration < updateSpeed)
        {
            elapsedDuration += Time.deltaTime;
            healthBarSprite.fillAmount = Mathf.Lerp(startingFillAmount, targetFillAmount, elapsedDuration / updateSpeed);
            yield return null;
        }

        healthBarSprite.fillAmount = targetFillAmount; // Ensure the exact target value is set at the end
    }
}
