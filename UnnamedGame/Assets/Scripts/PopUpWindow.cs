using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpWindow : MonoBehaviour
{
   public TMP_Text popUpText;

   private GameObject window;
   private Animator popUpAnimator;

   private void Start()
   {
    window = transform.GetChild(0).gameObject;
    popUpAnimator = window.GetComponent<Animator>();
    window.SetActive(false);
   }

   private void Update()
   {
        AnimatorStateInfo stateInfo = popUpAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ExitPopUp") && stateInfo.normalizedTime >= 0.9f)
        {
            window.SetActive(false);
        }
   }

   public void ShowPopUp(string text)
   {
        window.SetActive(true);
        popUpText.text = text;
        popUpAnimator.Play("EnterPopUp");
   }

   public void ExitPopUp()
   {
        popUpAnimator.Play("ExitPopUp");
   }
}
