using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAwareState : SkeletonBaseState
{
    private Transform raycastPoint;
    private Transform attackPoint;
    private GameObject player;

    private int playerMask;

    private float signPlayerToSkeleton;
    private float attackCooldown = 2f;
    private float attackCooldownRemaining = 0f;

    private bool playerInAttackRange;
    private bool isAttacking;
    
    public override void EnterState()
    {
        Debug.Log("Skeleton is aware of player");

        raycastPoint = skeleton.raycastPoint;
        attackPoint = skeleton.attackPoint;
        
        player = skeleton.player;
        playerMask = LayerMask.GetMask("Player");

        SkeletonAnimationEventsManager.OnEndAttack1Event += OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event += OnEndAttack1;

        PlayerCombat.OnHitEnemyEvent += TakeDamage;
    }

    public override void UpdateState()
    {
        signPlayerToSkeleton = Mathf.Sign(player.transform.position.x - skeleton.transform.position.x);
        if (!isAttacking & !skeleton.isDead)
        {
            UpdateAnimator();
        }
    }

    public override void FixedUpdateState()
    {
        if (!isAttacking && !skeleton.isDead){
            ChasePlayer();
        }

        if (playerInAttackRange && !isAttacking && attackCooldownRemaining <= 0f && !skeleton.isDead)
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
        PlayerCombat.OnHitEnemyEvent -= TakeDamage;
    }

    public override void OnDeath()
    {
        skeleton.rb.velocity = Vector2.zero;
        skeleton.rb.gravityScale = 0;
        skeleton.boxCollider.enabled = false;
        skeleton.isDead = true;
        skeleton.animator.SetTrigger("deathTrigger");
    }

    
    void ChasePlayer()
    {
        if (Mathf.Abs(skeleton.transform.position.x - player.transform.position.x) > 1)
        {
            skeleton.rb.velocity = new Vector2(signPlayerToSkeleton * skeleton.speed, skeleton.rb.velocity.y);
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
        skeleton.transform.localScale = new Vector3(signPlayerToSkeleton * Mathf.Abs(skeleton.transform.localScale.x), skeleton.transform.localScale.y, skeleton.transform.localScale.z);
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
