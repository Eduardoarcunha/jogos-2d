using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseClick : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.instance.PauseOrResume();
    }
}
