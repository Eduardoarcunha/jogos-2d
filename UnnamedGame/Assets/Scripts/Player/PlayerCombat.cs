using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<int, int> OnHitEnemyEvent;
    public static event Action OnDeathEvent;

    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    public HealthBar healthBar;
    public HealthBar staminaBar;

    private int maxLife;
    private int life;
    private float maxStamina;
    private float stamina;

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
    private bool isDead = false;
    private float immunityTime = 2f;
    private float immunityTimeRemaining = 0f;
    private Animator animator;

    private void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        maxLife = 10;
        life = maxLife;

        maxStamina = 100;
        stamina = maxStamina;

        SubscribeToEvents();

        StartCoroutine(StaminaRecovery());
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
        return !isAttacking && !isRolling && !isHitted && isGrounded && !isDead;
    }

    private void Update()
    {
        if (CanRollOrAttack())
        {
            if (Input.GetMouseButtonDown(0)) StartLightAttack();
            if (Input.GetKeyDown(KeyCode.Space) && stamina > 20) Roll();
        }

        if (immunityTimeRemaining > 0 && !isDead) 
        {
            if (!flashEffect.IsFlashing())  // Check if a flash is not already happening
            {
                flashEffect.OscillateTransparency();
            }
            immunityTimeRemaining -= Time.deltaTime;
        }
    }

    private void StartLightAttack()
    {
        isAttacking = true;        
        AudioManager.instance.PlaySound("SwordSwing");
        animator.SetTrigger("Attack");

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
        AudioManager.instance.PlaySound("Roll");
        stamina -= 20;
        staminaBar.UpdateHealthBar(stamina, maxStamina);
        isRolling = true;
        animator.SetTrigger("Roll");
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
        if (isDead || immunityTimeRemaining > 0) return;
        life -= damage;
        healthBar.UpdateHealthBar(life, maxLife);
        immunityTimeRemaining = immunityTime;

        if (life <= 0) {
            OnDeathEvent?.Invoke();
            animator.SetTrigger("Death");
            isDead = true;
            isAttacking = false;
            isRolling = false;
        } else {
            animator.SetTrigger("Hitted");
            isHitted = true;
            isAttacking = false;
            isRolling = false;
        }

        
        float knockbackDirectionSign = Mathf.Sign(transform.position.x - enemyPos.position.x);
        Vector2 knockbackDirection = new Vector2 (knockbackDirectionSign, 1);
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);  
    }

    private void OnEndHittedCombat()
    {
        isHitted = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && !isHitted) Hitted(collision.gameObject.transform, 1);
    }

    private IEnumerator StaminaRecovery()
    {
        while (true)
        {
            if (stamina < maxStamina)
            {
                stamina += 5;
                if (stamina > maxStamina) stamina = maxStamina;
                // Update any UI or other components related to stamina here
                staminaBar.UpdateHealthBar(stamina, maxStamina);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void Heal(int healAmount)
    {
        AudioManager.instance.PlaySound("Heal");
        life += healAmount;
        if (life >= maxLife) life = maxLife;
        healthBar.UpdateHealthBar(life, maxLife);
    }

    public void SetCurrentLife(int life)
    {
        this.life = life;
        healthBar.UpdateHealthBar(life, maxLife);
    }
}