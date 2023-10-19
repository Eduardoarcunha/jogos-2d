using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public string[] popUpsText;
    public PopUpWindow popUpWindowScript;
    [SerializeField] private GameObject player;
    [SerializeField] private Bar healthBar;

    private int popUpIndex;


    private float timeBetweenPopUps = .5f;
    private float timeBeginning = 1f;

    private Coroutine showPopUpAfterDelayCoroutine;

    void Start()
    {
        popUpIndex = 0;
        popUpWindowScript.ShowPopUp(popUpsText[popUpIndex]);
    }

    void Update()
    {
        if (popUpIndex == 0 && Input.GetAxis("Horizontal") != 0)
        {
            popUpIndex++;
            popUpWindowScript.ExitPopUp();
            showPopUpAfterDelayCoroutine = StartCoroutine(ShowPopUpAfterDelay());
        }

        if (popUpIndex == 1 && Input.GetKeyDown(KeyCode.W) && showPopUpAfterDelayCoroutine == null)
        {
            popUpIndex++;
            popUpWindowScript.ExitPopUp();
            showPopUpAfterDelayCoroutine = StartCoroutine(ShowPopUpAfterDelay());
        }

        if (popUpIndex == 2 && Input.GetMouseButtonDown(0) && showPopUpAfterDelayCoroutine == null)
        {
            popUpIndex++;
            popUpWindowScript.ExitPopUp();
            showPopUpAfterDelayCoroutine = StartCoroutine(ShowPopUpAfterDelay());
        }

        if (popUpIndex == 3 && Input.GetKeyDown(KeyCode.Space) && showPopUpAfterDelayCoroutine == null)
        {
            popUpIndex++;
            popUpWindowScript.ExitPopUp();
            showPopUpAfterDelayCoroutine = StartCoroutine(ShowPopUpAfterDelay());
            healthBar.SetBar(4,5);
        }

        if (popUpIndex == 4 && Input.GetKeyDown(KeyCode.H) && showPopUpAfterDelayCoroutine == null)
        {
            popUpIndex++;
            popUpWindowScript.ExitPopUp();
            showPopUpAfterDelayCoroutine = StartCoroutine(ShowPopUpAfterDelay());
        }

        if (popUpIndex == 5 && Input.GetKeyDown(KeyCode.Return) && showPopUpAfterDelayCoroutine == null)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Menu);
        }
    }

    IEnumerator ShowPopUpAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenPopUps);
        if (popUpIndex < popUpsText.Length)
        {
            popUpWindowScript.ShowPopUp(popUpsText[popUpIndex]);
        } 
        yield return new WaitForSeconds(timeBeginning);
        showPopUpAfterDelayCoroutine = null;
    }
}
