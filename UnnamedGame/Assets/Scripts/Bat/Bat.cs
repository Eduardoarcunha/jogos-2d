using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Transform attackPoint2;
    public Transform attackPoint3;

    public Transform raycastPoint;
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    public GameObject batProjectilePrefab;

    [Header("Properties")]
    private float speed = 3f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackPoint2Range = 0.8f;
    private float life = 6;
    private int attack2Damage = 4;
    private int expPoints = 3;

    [Header("Attack Settings")]
    private int nextAttack = 1;
    private float meleeAttackProbability = 0.7f;
    private float meleeAttackRange = 0.8f;

    [Header("Ranged Attack Settings")]
    private float rangeAttackMinDis = 1.5f;
    private float rangeAttackMaxDis = 4f;
    private float batProjectileSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int playerMask;
    private int gameObjectId;
    private float signPlayerToBat;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackCooldown = 1f;
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
        nextAttack = Random.value < meleeAttackProbability ? 2 : 3;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0), 6f, playerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            aware = true;
        }
        signPlayerToBat = Mathf.Sign(player.transform.position.x - transform.position.x);
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

            if (nextAttack == 2)
            {
                // Melee attack
                if (Vector2.Distance(transform.position, player.transform.position) <= meleeAttackRange)
                {
                    StartMeleeAttack();
                }
                else
                {
                    rb.velocity = new Vector2(signPlayerToBat * speed, rb.velocity.y);
                }
            }
            else if (nextAttack == 3)
            {
                // Ranged attack
                if (Vector2.Distance(transform.position, player.transform.position) < rangeAttackMinDis)
                {
                    rb.velocity = new Vector2(-signPlayerToBat * speed, rb.velocity.y);
                }
                else if (Vector2.Distance(transform.position, player.transform.position) > rangeAttackMaxDis)
                {
                    rb.velocity = new Vector2(signPlayerToBat * speed, rb.velocity.y);
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
        animator.SetTrigger("Attack3Trigger");
        nextAttack = Random.value < meleeAttackProbability ? 2 : 3;
    }


    private bool IsPlayerWithinRangedAttackRange()
    {
        return Vector2.Distance(transform.position, player.transform.position) <= attackRange;
    }

    private void StartMeleeAttack()
    {
        rb.velocity = Vector2.zero;
        isAttacking = true;
        animator.SetTrigger("Attack2Trigger");
        nextAttack = Random.value < meleeAttackProbability ? 2 : 3;
    }

    private void UpdateAnimator()
    {
        transform.localScale = new Vector3(signPlayerToBat * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnHitAttack2()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint3.position, attackPoint2Range, playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(transform, attack2Damage);
        }
    }

    private void OnHitAttack3()
    {
        GameObject batProjectile = Instantiate(batProjectilePrefab, attackPoint2.position, Quaternion.identity);
        batProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(batProjectileSpeed * Mathf.Sign(transform.localScale.x), 0);
    }

    private void OnEndAttack2()
    {
        attackCooldownRemaining = attackCooldown;
        isAttacking = false;
    }

    private void OnEndAttack3()
    {
        attackCooldownRemaining = attackCooldown;
        isAttacking = false;
    }


    private void TakeDamage(int id, int damage)
    {
        if (id == gameObjectId){
            AudioManager.instance.PlaySound("BatHit");
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

    private void DestroyBat()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PlayerCombat.OnHitEnemyEvent -= TakeDamage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint2.position, attackPoint2Range);
        Gizmos.DrawRay(raycastPoint.position, new Vector2(signPlayerToBat, 0) * 10f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
