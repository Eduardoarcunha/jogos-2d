using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAwareState : SkeletonBaseState
{
    private float sign;
    private bool playerInAttackRange;
    private bool isAttacking;

    private float attackCooldown = 2f;
    private float attackCooldownRemaining = 0f;

    private int layerMask;

    
    public override void EnterState(SkeletonStateManager skeleton)
    {
        layerMask = LayerMask.GetMask("Player");

        SkeletonAnimationEventsManager.OnEndAttack1Event += () => { attackCooldownRemaining = attackCooldown; isAttacking = false; };
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        sign = Mathf.Sign(skeleton.player.transform.position.x - skeleton.transform.position.x);

        if (!isAttacking)
        {
            UpdateAnimator(skeleton);
        }
        
    }

    public override void FixedUpdateState(SkeletonStateManager skeleton)
    {
        if (!isAttacking){
            ChasePlayer(skeleton);
        }

        if (playerInAttackRange && !isAttacking && attackCooldownRemaining <= 0f)
        {
            AttackPlayer(skeleton);
        }
        attackCooldownRemaining -= Time.fixedDeltaTime;

    }

    public override void ExitState(SkeletonStateManager skeleton)
    {
        SkeletonAnimationEventsManager.OnEndAttack1Event -= () => { attackCooldownRemaining = attackCooldown; isAttacking = false;};
    }

    void ChasePlayer(SkeletonStateManager skeleton)
    {
        if (Mathf.Abs(skeleton.transform.position.x - skeleton.player.transform.position.x) > 4)
        {
            skeleton.rb.velocity = new Vector2(sign * skeleton.speed, 0);
            playerInAttackRange = false;
        }
        else
        {
            skeleton.rb.velocity = new Vector2(0, 0);
            playerInAttackRange = true;
        }
    }

    void UpdateAnimator(SkeletonStateManager skeleton)
    {
        skeleton.transform.localScale = new Vector3(sign * 1, 1, 1);
        if (skeleton.rb.velocity.x != 0)
        {
            
            skeleton.animator.SetBool("isMoving", true);
        }
        else
        {
            skeleton.animator.SetBool("isMoving", false);
        }
    }

    private void AttackPlayer(SkeletonStateManager skeleton)
    {

        isAttacking = true;        
        skeleton.animator.SetTrigger("attackTrigger");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(skeleton.attackPoint.position, skeleton.attackRange, layerMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }
    }

}
