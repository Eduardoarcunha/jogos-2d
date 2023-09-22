using UnityEngine;

public abstract class SkeletonBaseState
{
    public abstract void EnterState(SkeletonStateManager skeleton);

    public abstract void UpdateState(SkeletonStateManager skeleton);

    public abstract void FixedUpdateState(SkeletonStateManager skeleton);

    public abstract void ExitState(SkeletonStateManager skeleton);
    
}
