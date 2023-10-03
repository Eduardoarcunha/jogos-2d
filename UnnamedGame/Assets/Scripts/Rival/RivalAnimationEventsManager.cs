using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RivalAnimationEventsManager : MonoBehaviour
{
    public static event Action OnEndAttack2Event;
    public static event Action OnEndAttack3Event;

    void OnAnimationEndAttack2() => OnEndAttack2Event?.Invoke();
    void OnAnimationEndAttack3() => OnEndAttack3Event?.Invoke();
}
