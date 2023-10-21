using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Transform attackPoint1;
    public Transform attackPoint2;
    public Transform raycastPoint;
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    public GameObject bombPrefab;

    [Header("Properties")]
    private float speed = 3.5f;
    private float chaseRange = 5f;
    private float attackPoint1Range = 0.5f;
    private float attackPoint2Range = 0.5f;
    private float life = 10;
    private int attack1Damage = 3;
    private int expPoints = 5;

    [Header("Attack Settings")]
    private int nextAttack = 1;
    private float meleeAttackProbability = 0.5f;
    private float meleeAttackRange = 0.8f;

    [Header("Ranged Attack Settings")]
    private float rangeAttackMinDis = 2f;
    private float rangeAttackMaxDis = 6f;
    private float bombSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int playerMask;
    private int gameObjectId;
    private float signPlayerToGoblin;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackCooldown = 1.5f;
    private float attackCooldownRemaining = 0f;
    private float attackRange = .8f;
    private bool aware = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerMask = LayerMask.GetMask("Player");
        gameObjectId = gameObject.GetInstanceID();

        PlayerCombat.OnHitEnemyEvent += TakeDamage;
    }

    private void Start()
    {
        nextAttack = Random.value < meleeAttackProbability ? 1 : 2;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0), 6f, playerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            aware = true;
        }
        
        signPlayerToGoblin = Mathf.Sign(player.transform.position.x - transform.position.x);
        if (!isDead) UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!aware) return;
        if (Vector2.Distance(transform.position, player.transform.position) > chaseRange)
        {
            aware = false;
            return;
        }
        HandleMovementAndAttacks();
    }

    private void HandleMovementAndAttacks()
    {
        if (!isAttacking && attackCooldownRemaining <= 0f && !isDead)
        {

            if (nextAttack == 1)
            {
                // Melee attack
                if (Vector2.Distance(transform.position, player.transform.position) <= meleeAttackRange)
                {
                    StartMeleeAttack();
                }
                else
                {
                    rb.velocity = new Vector2(signPlayerToGoblin * speed, rb.velocity.y);
                }
            }
            else if (nextAttack == 2)
            {
                // Ranged attack
                if (Vector2.Distance(transform.position, player.transform.position) < rangeAttackMinDis)
                {
                    rb.velocity = new Vector2(-signPlayerToGoblin * speed, rb.velocity.y);
                }
                else if (Vector2.Distance(transform.position, player.transform.position) > rangeAttackMaxDis)
                {
                    rb.velocity = new Vector2(signPlayerToGoblin * speed, rb.velocity.y);
                }
                else
                {
                    StartRangedAttack();
                }
            }
        }

        attackCooldownRemaining -= Time.fixedDeltaTime;
    }



    private void StartRangedAttack()
    {
        rb.velocity = Vector2.zero;
        isAttacking = true;
        animator.SetTrigger("Attack2Trigger");
        nextAttack = Random.value < meleeAttackProbability ? 1 : 2;
    }


    private bool IsPlayerWithinRangedAttackRange()
    {
        return Vector2.Distance(transform.position, player.transform.position) <= attackRange;
    }

    private void StartMeleeAttack()
    {
        rb.velocity = Vector2.zero;
        isAttacking = true;
        animator.SetTrigger("Attack1Trigger");
        nextAttack = Random.value < meleeAttackProbability ? 1 : 2;
    }

    private void UpdateAnimator()
    {
        transform.localScale = new Vector3(signPlayerToGoblin * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        animator.SetBool("IsMoving", rb.velocity.x != 0);
    }

    private void OnHitAttack1()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint1.position, attackPoint1Range, playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(transform, attack1Damage);
        }
    }

    private void OnEndAttack1()
    {
        attackCooldownRemaining = attackCooldown;
        isAttacking = false;
    }

    private void OnEndAttack2()
    {
        attackCooldownRemaining = attackCooldown;
        GameObject bomb = Instantiate(bombPrefab, attackPoint1.position, Quaternion.identity);
        bomb.GetComponent<Rigidbody2D>().velocity = new Vector2(bombSpeed * Mathf.Sign(transform.localScale.x), 0);
        isAttacking = false;
    }


    private void TakeDamage(int id, int damage)
    {
        if (id == gameObjectId){
            AudioManager.instance.PlaySound("GoblinHit");
            life -= damage;
            flashEffect.Flash();
            if (life <= 0)
            {
                OnDeath();
            }
        }
        return;
    }

    private void OnDeath()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        boxCollider.enabled = false;
        isDead = true;
        animator.SetTrigger("DeathTrigger");
        ProgressionManager.instance.AddExp(expPoints);
    }

    private void DestroyGoblin()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PlayerCombat.OnHitEnemyEvent -= TakeDamage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint1.position, attackPoint1Range);
        Gizmos.DrawWireSphere(attackPoint2.position, attackPoint2Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
