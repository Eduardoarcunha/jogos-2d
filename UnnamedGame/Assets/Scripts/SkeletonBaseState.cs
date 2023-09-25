using UnityEngine;
using System.Collections;

public abstract class SkeletonBaseState
{
    protected SkeletonStateManager skeleton;

    public void InitializeState(SkeletonStateManager skelManager)
    {
        skeleton = skelManager;
        EnterState();
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public virtual void TakeDamage(int id, int damage)
    {
        if (id == skeleton.gameObjectId){
            skeleton.life -= damage;
            skeleton.flashEffect.Flash();
            if (skeleton.life <= 0)
            {
                OnDeath();
            }
        }
        return;
    }

    public virtual void OnDeath()
    {
        skeleton.rb.velocity = Vector2.zero;
        skeleton.rb.gravityScale = 0;
        skeleton.boxCollider.enabled = false;
        skeleton.isDead = true;
        skeleton.animator.SetTrigger("deathTrigger");
    }

}
