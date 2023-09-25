using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<int, int> OnHitEnemyEvent;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int damage = 2;
    [SerializeField] private Vector2 knockbackForce;


    [SerializeField] private LayerMask enemyLayers;

    private Rigidbody2D rb;

    private bool isAttacking = false;
    private bool isRolling = false;
    private bool isGrounded = false;
    private bool isHitted = false;
    private Animator animator;

    private void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        PlayerController.OnChangeGroundedState += HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndLightAttackEvent += OnEndLightAttackCombat;
        PlayerAnimationEventsManager.OnEndRollEvent += OnEndRollCombat;
        PlayerAnimationEventsManager.OnEndHittedEvent += OnEndHittedCombat;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerController.OnChangeGroundedState -= HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndLightAttackEvent -= OnEndLightAttackCombat;
        PlayerAnimationEventsManager.OnEndRollEvent -= OnEndRollCombat;
        PlayerAnimationEventsManager.OnEndHittedEvent -= OnEndHittedCombat;
    }

    private bool CanRollOrAttack()
    {
        return !isAttacking && !isRolling && !isHitted && isGrounded;
    }

    private void Update()
    {
        if (CanRollOrAttack())
        {
            if (Input.GetMouseButtonDown(0)) StartLightAttack();
            if (Input.GetKeyDown(KeyCode.Space)) Roll();
        }
    }

    private void StartLightAttack()
    {
        isAttacking = true;        
        animator.SetTrigger("attackTrigger");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) OnHitEnemyEvent?.Invoke(enemy.gameObject.GetInstanceID(), damage);
        }
    }
     
    private void OnEndLightAttackCombat()
    {
        isAttacking = false;
    }

    private void Roll()
    {
        isRolling = true;
        animator.SetTrigger("rollTrigger");
    }

    private void OnEndRollCombat()
    {
        isRolling = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isHitted) Hitted(collision.gameObject.transform, 1);
    }

    public void Hitted(Transform enemyPos, int damage)
    {
        animator.SetBool("isHitted", true);
        isHitted = true;
        isAttacking = false;
        isRolling = false;

        float knockbackDirectionSign = Mathf.Sign(transform.position.x - enemyPos.position.x);
        Vector2 knockbackDirection = new Vector2 (knockbackDirectionSign, 1);

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);              
    }

    private void OnEndHittedCombat()
    {
        isHitted = false;
        animator.SetBool("isHitted", false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void HandleChangeGroundedState(bool isGroundedState)
    {
        isGrounded = isGroundedState;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}