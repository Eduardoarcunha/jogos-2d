using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SkeletonAnimationEventsManager : MonoBehaviour
{
    public static event Action OnEndAttack1Event;
    public static event Action OnHitAttack1Event;
    public static event Action OnDisappearEvent;

    void OnAnimationEndAttack1() =>  OnEndAttack1Event?.Invoke();

    private void OnAnimationHitAttack1() => OnHitAttack1Event?.Invoke();

    private void OnAnimationDisappear() => OnDisappearEvent?.Invoke();
}
