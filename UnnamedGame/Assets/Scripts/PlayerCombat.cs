using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    private bool isAttacking = false;
    private bool isRolling = false;
    private bool isGrounded = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        PlayerController.OnChangeGroundedState += HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndLightAttackEvent += OnEndLightAttack;
        PlayerAnimationEventsManager.OnEndRollEvent += OnEndRoll;
    }

    private void HandleChangeGroundedState(bool isGroundedState)
    {
        isGrounded = isGroundedState;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isRolling && isGrounded)
        {
            StartLightAttack();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isRolling && isGrounded)
        {
            Roll();
        }
    }

    private void StartLightAttack()
    {
        isAttacking = true;        
        animator.SetTrigger("attackTrigger");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }
    }
     
    private void OnEndLightAttack()
    {
        isAttacking = false;
    }


    private void Roll()
    {
        isRolling = true;
        animator.SetTrigger("rollTrigger");
    }

    private void OnEndRoll()
    {
        isRolling = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnDestroy()
    {
        PlayerController.OnChangeGroundedState -= HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndLightAttackEvent -= OnEndLightAttack;
        PlayerAnimationEventsManager.OnEndRollEvent -= OnEndRoll;
    }
}