using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SkeletonUnawareState : SkeletonBaseState
{
    private Transform raycastPoint;

    private int playerMask;
    private int roamingPositionIdx = 0;
    private float roamingPosition;

    public override void EnterState()
    {
        Debug.Log("Skeleton is unaware of player");
        raycastPoint = skeleton.raycastPoint;

        playerMask = LayerMask.GetMask("Player");
        roamingPosition = skeleton.roamingPositions[roamingPositionIdx];

        PlayerCombat.OnHitEnemyEvent += TakeDamage;
    }

    public override void UpdateState()
    {
        SearchPlayer();
        UpdateAnimator();
    }

    public override void FixedUpdateState()
    {
        if (Mathf.Abs(skeleton.transform.position.x - roamingPosition) < 0.1f)
        {
            skeleton.rb.velocity = Vector2.zero;
        }
        else
        {
            skeleton.rb.velocity = new Vector2(Mathf.Sign(roamingPosition - skeleton.transform.position.x) * skeleton.speed, skeleton.rb.velocity.y);
        }
    }

    public override void ExitState()
    {
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

    private void SearchPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, new Vector2(Mathf.Sign(skeleton.transform.localScale.x) * 1, 0), 6f, playerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            skeleton.SwitchState(skeleton.awareState);
        }
    }

    private void UpdateAnimator()
    {
        if (Mathf.Abs(skeleton.transform.position.x - roamingPosition) < 0.1f)
        {
            roamingPositionIdx = (roamingPositionIdx + 1) % skeleton.roamingPositions.Length;
            roamingPosition = skeleton.roamingPositions[roamingPositionIdx];
            skeleton.animator.SetBool("isMoving", false);
        }
        else
        {
            skeleton.transform.localScale = new Vector3(Mathf.Sign(roamingPosition - skeleton.transform.position.x), 1, 1);
            skeleton.animator.SetBool("isMoving", true);
        }
    }
}
