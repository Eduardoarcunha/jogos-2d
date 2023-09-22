using UnityEngine;

public abstract class SkeletonBaseState
{
    public abstract void EnterState(SkeletonStateManager skelManager);

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public abstract void TakeDamage(int damage);
    
}
