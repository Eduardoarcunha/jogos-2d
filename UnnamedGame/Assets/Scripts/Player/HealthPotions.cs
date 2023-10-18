using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotions : MonoBehaviour
{
    public GameObject[] healthPotions;
    public Sprite emptyFlaskSprite;  // Use Sprite instead of Image

    public void SetPotions(int remainingPotions)
    {
        for (int i = 0; i < healthPotions.Length; i++)
        {
            Image potionImage = healthPotions[i].GetComponent<Image>();
            Animator potionAnimator = healthPotions[i].GetComponent<Animator>();
            if (i < remainingPotions)
            {
                potionImage.enabled = true;  // Show the full potion
            }
            else
            {
                potionAnimator.enabled = false;  // Disable the animator
                potionImage.sprite = emptyFlaskSprite;  // Set to empty flask image
                potionImage.enabled = true;  // Ensure it's still shown even if it's empty
            }
        }
    }
}
