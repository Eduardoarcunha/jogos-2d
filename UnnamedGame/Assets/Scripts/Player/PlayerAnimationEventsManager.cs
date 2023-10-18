using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerAnimationEventsManager : MonoBehaviour
{
    public static event Action OnStartAttack1Event;
    public static event Action OnEndAttack1Event;
    public static event Action OnStartAttack2Event;
    public static event Action OnEndAttack2Event;
    public static event Action OnStartRollEvent;
    public static event Action OnEndRollEvent;
    public static event Action OnHittedEvent;
    public static event Action OnEndHittedEvent;

    void OnAnimationStartAttack1()
    {
        OnStartAttack1Event?.Invoke();
    }

    void OnAnimationEndAttack1()
    {
        OnEndAttack1Event?.Invoke();
    }

    void OnAnimationStartAttack2()
    {
        OnStartAttack2Event?.Invoke();
    }

    void OnAnimationEndAttack2()
    {
        OnEndAttack2Event?.Invoke();
    }

    void OnAnimationStartRoll()
    {
        OnStartRollEvent?.Invoke();
    }

    void OnAnimationEndRoll()
    {
        OnEndRollEvent?.Invoke();
    }

    void OnAnimationHitted()
    {
        OnHittedEvent?.Invoke();
    }

    void OnAnimationEndHitted()
    {
        OnEndHittedEvent?.Invoke();
    }
}
