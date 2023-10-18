using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerAnimationEventsManager : MonoBehaviour
{
    public static event Action OnStartLightAttackEvent;
    public static event Action OnEndLightAttackEvent;
    public static event Action OnStartRollEvent;
    public static event Action OnEndRollEvent;
    public static event Action OnHittedEvent;
    public static event Action OnEndHittedEvent;


    void OnAnimationStartLightAttack()
    {
        OnStartLightAttackEvent?.Invoke();
    }

    void OnAnimationEndLightAttack()
    {
        OnEndLightAttackEvent?.Invoke();
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
