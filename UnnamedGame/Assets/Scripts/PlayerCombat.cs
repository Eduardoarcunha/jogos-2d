using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action OnLightAttack;
    public static event Action OnRoll;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float rollCooldown = 0.5f;

    private bool isAttacking = false;
    private bool isRolling = false;
    private bool isGrounded = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        PlayerController.OnChangeGroundedState += HandleChangeGroundedState;
    }

    private void HandleChangeGroundedState(bool isGroundedState)
    {
        isGrounded = isGroundedState;
    }

    private void Update()
    {
        // left click input
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
        StartCoroutine(LightAttack());
    }

    private IEnumerator LightAttack()
    {
        OnLightAttack?.Invoke();
        animator.SetTrigger("attackTrigger");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }

        AnimatorStateInfo currentAnimState = animator.GetCurrentAnimatorStateInfo(0);
        float timeRemaining = (1.0f - currentAnimState.normalizedTime) * currentAnimState.length;

        yield return new WaitForSeconds(timeRemaining + attackCooldown);
        isAttacking = false;
    }

    private void Roll()
    {
        isRolling = true;
        StartCoroutine(Rolling());   
    }

    private IEnumerator Rolling()
    {
        OnRoll?.Invoke();
        animator.SetTrigger("rollTrigger");
        AnimatorStateInfo currentAnimState = animator.GetCurrentAnimatorStateInfo(0);
        float timeRemaining = (1.0f - currentAnimState.normalizedTime) * currentAnimState.length;
        yield return new WaitForSeconds(timeRemaining + rollCooldown);
        isRolling = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}