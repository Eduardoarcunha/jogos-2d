using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("References")]
    public Transform raycastPoint;
    public Transform attackPoint;
    public GameObject player;
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;

    [Header("Properties")]
    private float speed = 2f;
    private float life = 10;
    // private float chaseRange = 5f;
    private int attack1Damage = 4;
    private int expPoints = 3;
    public float[] roamingPositions;
    private int roamingPositionIdx = 0;
    private float roamingPosition;
    
    [Header("Attack Settings")]
    public float attackRange = 1f;
    private float attackCooldown = 1.5f;
    private float attackCooldownRemaining = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private int playerMask;
    private int gameObjectId;
    private float signPlayerToSkeleton;
    private float signToNextRoamingPosition;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool aware = false;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerMask = LayerMask.GetMask("Player");
        gameObjectId = gameObject.GetInstanceID();

        PlayerCombat.OnHitEnemyEvent += TakeDamage;
    }

    private void Start()
    {
        roamingPosition = roamingPositions[roamingPositionIdx];
        signToNextRoamingPosition = Mathf.Sign(roamingPosition - transform.position.x);
    }

    private void Update()
    {
        signPlayerToSkeleton = Mathf.Sign(player.transform.position.x - transform.position.x);
        signToNextRoamingPosition = Mathf.Sign(roamingPosition - transform.position.x);

        SearchPlayer();
        if (!isDead) UpdateAnimator();

        if (Mathf.Abs(transform.position.x - roamingPosition) < 0.1f)
        {
            roamingPositionIdx = (roamingPositionIdx + 1) % roamingPositions.Length;
            roamingPosition = roamingPositions[roamingPositionIdx];
        }
    }

    private void FixedUpdate()
    {
        if (aware)
        {
            HandleAwareState();
        }
        else
        {
            HandleUnawareState();
        }
    }

    private void HandleAwareState()
    {
        
        if (!isAttacking & !isDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > attackRange)
            {
                rb.velocity = new Vector2(signPlayerToSkeleton * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
                if (attackCooldownRemaining <= 0f)
                {
                    isAttacking = true;
                    animator.SetTrigger("attackTrigger");
                }
            }
        }
        attackCooldownRemaining -= Time.fixedDeltaTime;
    }

    private void HandleUnawareState()
    {
        if (Mathf.Abs(transform.position.x - roamingPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Sign(roamingPosition - transform.position.x) * speed, rb.velocity.y);
        }
    }

    private void SearchPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0), 6f, playerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            aware = true;
        }
    }

    private void UpdateAnimator()
    {
        if (aware){
            transform.localScale = new Vector3(signPlayerToSkeleton * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else {
            transform.localScale = new Vector3(signToNextRoamingPosition * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        animator.SetBool("isMoving", rb.velocity.x != 0);
    }

    private void OnHitAttack1()
    {
        AudioManager.instance.PlaySound("SkeletonSlice");
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, playerMask);
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

    private void TakeDamage(int id, int damage)
    {
        if (id == gameObjectId)
        {
            life -= damage;
            AudioManager.instance.PlaySound("SkeletonHit");
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
        animator.SetTrigger("deathTrigger");
        ProgressionManager.instance.AddExp(expPoints);
        AudioManager.instance.PlaySound("BonesFalling");
    }

    private void DestroySkeleton()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SkeletonAnimationEventsManager.OnEndAttack1Event -= OnHitAttack1;
        SkeletonAnimationEventsManager.OnHitAttack1Event -= OnEndAttack1;
        PlayerCombat.OnHitEnemyEvent -= TakeDamage;
    }
}
