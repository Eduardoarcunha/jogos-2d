using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAwareState : SkeletonBaseState
{
    private SkeletonStateManager skel;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform raycastPoint;
    private Transform attackPoint;
    private GameObject player;

    private float sign;
    private bool playerInAttackRange;
    private bool isAttacking;

    private float attackCooldown = 2f;
    private float attackCooldownRemaining = 0f;

    private int playerMask;

    
    public override void EnterState(SkeletonStateManager skeleton)
    {
        skel = skeleton;

        rb = skeleton.GetRigidbody();
        animator = skeleton.GetAnimator();
        
        raycastPoint = skeleton.GetRaycastPoint();
        attackPoint = skeleton.GetAttackPoint();
        
        player = skeleton.GetPlayer();
        playerMask = LayerMask.GetMask("Player");

        SkeletonAnimationEventsManager.OnEndAttack1Event += OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event += OnEndAttack1;
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        sign = Mathf.Sign(player.transform.position.x - skeleton.transform.position.x);

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
            isAttacking = true;
            animator.SetTrigger("attackTrigger");
        }
        attackCooldownRemaining -= Time.fixedDeltaTime;

    }

    public override void ExitState(SkeletonStateManager skeleton)
    {
        SkeletonAnimationEventsManager.OnEndAttack1Event -= OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event -= OnEndAttack1;
    }

    void ChasePlayer(SkeletonStateManager skeleton)
    {
        if (Mathf.Abs(skeleton.transform.position.x - player.transform.position.x) > 3)
        {
            rb.velocity = new Vector2(sign * skeleton.speed, 0);
            playerInAttackRange = false;
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            playerInAttackRange = true;
        }
    }

    void UpdateAnimator(SkeletonStateManager skeleton)
    {
        skeleton.transform.localScale = new Vector3(sign * 1, 1, 1);
        if (rb.velocity.x != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnHitAttack1()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(skel.transform, 1);
        }
    }

    private void OnEndAttack1()
    {
        attackCooldownRemaining = attackCooldown;
        isAttacking = false;
    }
}
