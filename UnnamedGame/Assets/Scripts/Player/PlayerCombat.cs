using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<int, int> OnHitEnemyEvent;
    public static event Action OnEndAttackEvent;
    public static event Action OnDeathEvent;

    public CameraBehavior cameraBh;

    [Header("References")]
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    public Bar healthBar;
    public Bar staminaBar;

    [Header("Properties")]
    [SerializeField] private int maxLife;
    
    [SerializeField] private float maxStamina;
    [SerializeField] private int life;
    [SerializeField] private float stamina;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int damage = 1;

    [Header("Others Settings")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private LayerMask enemyLayers;


    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isRolling = false;
    private bool isGrounded = false;
    private bool isHitted = false;
    private bool isDead = false;
    private bool paused = false;
    private bool attackCombo = false;
    private float immunityTime = 2f;
    private float immunityTimeRemaining = 0f;
    private Coroutine blinkRoutine;

    private Animator animator;

    private void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        life = maxLife;
        stamina = maxStamina;

        SubscribeToEvents();

        StartCoroutine(StaminaRecovery());
    }

    private void SubscribeToEvents()
    {
        PlayerController.OnChangeGroundedState += HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndAttack1Event += OnEndAttack1;
        PlayerAnimationEventsManager.OnEndAttack2Event += OnEndAttack2;
        PlayerAnimationEventsManager.OnEndRollEvent += OnEndRollCombat;
        PlayerAnimationEventsManager.OnEndHittedEvent += OnEndHittedCombat;
        GameManager.OnPauseOrResumeGame += OnPauseOrResumeGame;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerController.OnChangeGroundedState -= HandleChangeGroundedState;
        PlayerAnimationEventsManager.OnEndAttack2Event -= OnEndAttack2;
        PlayerAnimationEventsManager.OnEndRollEvent -= OnEndRollCombat;
        PlayerAnimationEventsManager.OnEndHittedEvent -= OnEndHittedCombat;
        GameManager.OnPauseOrResumeGame -= OnPauseOrResumeGame;
    }

    private bool CanRollOrAttack()
    {
        return !isAttacking && !isRolling && !isHitted && isGrounded && !isDead;
    }

    private void Update()
    {
        if (paused) return;
        if (isAttacking && !attackCombo) {
            if (Input.GetMouseButtonDown(0)) {
                attackCombo = true;
                animator.SetTrigger("Attack2");
            }
        }

        if (CanRollOrAttack())
        {
            if (Input.GetMouseButtonDown(0)) StartAttack("Attack1");
            if (Input.GetKeyDown(KeyCode.Space) && stamina > 20) Roll();
        }

        if (life >= 4 && blinkRoutine != null)
        {
            AudioManager.instance.StopSound("HeartBeat");
            StopCoroutine(blinkRoutine);
            blinkRoutine = null; // Reset the reference
            // Ensure the panel color is reset to its original state
            cameraBh.ResetPanelColor();
        }
    }

    private void StartAttack(string attackType)
    {
        decreaseStamina(2);
        isAttacking = true;        
        AudioManager.instance.PlaySound("SwordSwing");
        animator.SetTrigger(attackType);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if (hitEnemies.Length == 0) return;

        StartCoroutine(cameraBh.Shake(0.3f, 0.01f));
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) OnHitEnemyEvent?.Invoke(enemy.gameObject.GetInstanceID(), damage);
        }
    }

    private void OnEndAttack1()
    {
        if (attackCombo) {
            animator.SetTrigger("Attack2");
            StartAttack("Attack2");
        } else {
            isAttacking = false;
            OnEndAttackEvent?.Invoke();
        }
    }
     
    private void OnEndAttack2()
    {
        isAttacking = false;
        attackCombo = false;
        OnEndAttackEvent?.Invoke();
    }

    private void Roll()
    {
        AudioManager.instance.PlaySound("Roll");
        stamina -= 20;
        staminaBar.UpdateBar(stamina, maxStamina);
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
        StartCoroutine(cameraBh.Shake(0.3f, 0.05f));
        life -= damage;
        AudioManager.instance.PlaySound("PlayerHit");
        if (life < 4 && blinkRoutine == null)
        {
            blinkRoutine = StartCoroutine(cameraBh.BlinkRoutine());
            AudioManager.instance.PlaySound("HeartBeat");
        }
        
        healthBar.UpdateBar(life, maxLife);

        StartCoroutine(StartImmunity());

        if (life <= 0) {
            AudioManager.instance.StopSound("HeartBeat");
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

    private IEnumerator StartImmunity()
    {
        float tolerance = .8f;
        immunityTimeRemaining = immunityTime;
        gameObject.layer = LayerMask.NameToLayer("PlayerRolling");   

        while (immunityTimeRemaining > 0)
        {
            if (!flashEffect.IsFlashing())
            {
                flashEffect.OscillateTransparency();
            }

            immunityTimeRemaining -= Time.deltaTime;
            yield return null; 
        }
        yield return new WaitForSeconds(tolerance);
        gameObject.layer = LayerMask.NameToLayer("Player");   
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
        if (collision.gameObject.CompareTag("Spike") && !isHitted) Hitted(collision.gameObject.transform, 1);
        if (collision.gameObject.CompareTag("DeathCollider"))
        {
            isDead = true;
            OnDeathEvent?.Invoke();
        }

    }

    private IEnumerator StaminaRecovery()
    {
        while (true)
        {
            if (stamina < maxStamina)
            {
                stamina += 5;
                if (stamina > maxStamina) stamina = maxStamina;
                staminaBar.UpdateBar(stamina, maxStamina);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void Heal(int healAmount)
    {
        AudioManager.instance.PlaySound("Heal");
        life += healAmount;
        if (life >= maxLife) life = maxLife;
        healthBar.UpdateBar(life, maxLife);
    }

    public void SetCurrentLife(int life)
    {
        this.life = life;
        healthBar.UpdateBar(life, maxLife);
    }

    public void IncreaseDamage()
    {
        damage++;
    }

    public void decreaseStamina(int amount)
    {
        stamina -= amount;
        staminaBar.UpdateBar(stamina, maxStamina);
    }

    private void OnPauseOrResumeGame(bool value){
        paused = value;
    }

}