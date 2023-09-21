using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SkeletonAnimationEventsManager : MonoBehaviour
{
    public static event Action OnEndAttack1Event;

    void OnAnimationEndAttack1()
    {
        Debug.Log("OnAnimationEndAttack1");
        OnEndAttack1Event?.Invoke();
    }
}
