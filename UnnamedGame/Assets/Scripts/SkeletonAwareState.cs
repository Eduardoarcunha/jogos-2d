using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAwareState : SkeletonBaseState
{
    private SkeletonStateManager skeleton;

    private Transform raycastPoint;
    private Transform attackPoint;
    private GameObject player;

    private int playerMask;

    private float signPlayerToSkeleton;
    private float attackCooldown = 2f;
    private float attackCooldownRemaining = 0f;

    private bool playerInAttackRange;
    private bool isAttacking;
    
    public override void EnterState(SkeletonStateManager skelManager)
    {
        skeleton = skelManager;
        
        raycastPoint = skeleton.raycastPoint;
        attackPoint = skeleton.attackPoint;
        
        player = skeleton.player;
        playerMask = LayerMask.GetMask("Player");

        SkeletonAnimationEventsManager.OnEndAttack1Event += OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event += OnEndAttack1;
    }

    public override void UpdateState()
    {
        signPlayerToSkeleton = Mathf.Sign(player.transform.position.x - skeleton.transform.position.x);
        if (!isAttacking)
        {
            UpdateAnimator();
        }
    }

    public override void FixedUpdateState()
    {
        if (!isAttacking){
            ChasePlayer();
        }

        if (playerInAttackRange && !isAttacking && attackCooldownRemaining <= 0f)
        {
            isAttacking = true;
            skeleton.animator.SetTrigger("attackTrigger");
        }
        attackCooldownRemaining -= Time.fixedDeltaTime;
    }

    public override void ExitState()
    {
        SkeletonAnimationEventsManager.OnEndAttack1Event -= OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event -= OnEndAttack1;
    }

    public override void TakeDamage(int damage)
    {
        return;
    }
    
    void ChasePlayer()
    {
        if (Mathf.Abs(skeleton.transform.position.x - player.transform.position.x) > 3)
        {
            skeleton.rb.velocity = new Vector2(signPlayerToSkeleton * skeleton.speed, 0);
            playerInAttackRange = false;
        }
        else
        {
            skeleton.rb.velocity = new Vector2(0, 0);
            playerInAttackRange = true;
        }
    }

    void UpdateAnimator()
    {
        skeleton.transform.localScale = new Vector3(signPlayerToSkeleton * 1, 1, 1);
        if (skeleton.rb.velocity.x != 0)
        {
            skeleton.animator.SetBool("isMoving", true);
        }
        else
        {
            skeleton.animator.SetBool("isMoving", false);
        }
    }

    private void OnHitAttack1()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(skeleton.transform, 1);
        }
    }

    private void OnEndAttack1()
    {
        attackCooldownRemaining = attackCooldown;
        isAttacking = false;
    }
}
