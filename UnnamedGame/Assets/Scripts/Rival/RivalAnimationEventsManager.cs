using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RivalAnimationEventsManager : MonoBehaviour
{
    public static event Action OnAttackHitEvent;
    public static event Action OnEndAttack1Event;
    public static event Action OnEndAttack2Event;
    public static event Action OnEndAttack3Event;
    public static event Action OnEndAttack4Event;
    public static event Action OnFlipEvent;

    void OnAnimationAttackHit() => OnAttackHitEvent?.Invoke();
    void OnAnimationEndAttack1() => OnEndAttack1Event?.Invoke();
    void OnAnimationEndAttack2() => OnEndAttack2Event?.Invoke();
    void OnAnimationEndAttack3() => OnEndAttack3Event?.Invoke();
    void OnAnimationEndAttack4() => OnEndAttack4Event?.Invoke();
    void OnAnimationFlip() => OnFlipEvent();
}
