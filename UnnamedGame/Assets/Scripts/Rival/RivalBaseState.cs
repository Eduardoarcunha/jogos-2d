using UnityEngine;
using System.Collections;

public abstract class RivalBaseState
{
    protected RivalStateManager rival;

    public void InitializeState(RivalStateManager rivalManager)
    {
        rival = rivalManager;
        EnterState();
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public virtual void TakeDamage(int id, int damage)
    {
        if (id == rival.gameObjectId){
            rival.life -= damage;
            rival.flashEffect.Flash();
            if (rival.life <= 0)
            {
                OnDeath();
            }
        }
        return;
    }

    public virtual void OnDeath()
    {
        rival.rb.velocity = Vector2.zero;
        rival.rb.gravityScale = 0;
        rival.boxCollider.enabled = false;
        rival.isDead = true;
        rival.animator.SetTrigger("deathTrigger");
    }

}
