using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalStateManager : MonoBehaviour
{
    public GameObject player;
    public int playerMask;
    public Transform attackPoint;

    // Scripts
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    public HealthBar healthBar;

    public GameObject fireMeteorPrefab;
    public GameObject slashPrefab;

    [HideInInspector] public int gameObjectId;
    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public SpriteRenderer spriteRenderer { get; private set; }
    [HideInInspector] public BoxCollider2D boxCollider { get; private set; }
    
    // Attributes
    [HideInInspector] public float maxLife;
    [HideInInspector] public float life;
    public bool isDead = false;  
    public bool canAttack = true;
    public float speed = 3f;
    public float safeDistance = 4f;
    public float attackHitBox = 0.5f;
    public float closeAttackRange = 1.8f;
    public float longAttackRange = 3f;
    [HideInInspector] public float allAttacksCooldown;
    [HideInInspector] public float attack1Cooldown;
    [HideInInspector] public float attack2Cooldown;
    [HideInInspector] public float attack3Cooldown;
    [HideInInspector] public float attack4Cooldown;
    public float allAttacksCooldownRemaining = 0f;
    public float attack1CooldownRemaining = 0f;
    public float attack2CooldownRemaining = 0f;
    public float attack3CooldownRemaining = 0f;
    public float attack4CooldownRemaining = 0f;
    public bool playerIsDead = false;


    private RivalBaseState currentState;
    public RivalPhaseOneState phaseOneState = new RivalPhaseOneState();

    private void Awake()
    {
        gameObjectId = gameObject.GetInstanceID();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        playerMask = LayerMask.GetMask("Player");

        currentState = phaseOneState;
        currentState.InitializeState(this);
    }

    private void Start()
    {
        maxLife = 50;
        life = maxLife;

        allAttacksCooldown = 20f;
        attack1Cooldown = 70f;
        attack2Cooldown = 30f;
        attack3Cooldown = 30f;
        attack4Cooldown = 100f;
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(RivalBaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.InitializeState(this);
    }

    public void InstantiateFireMeteor()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 5, 0);
        Vector2 randomSpeed = new Vector2(Random.Range(-1f,1f), -3f);


        float angle = Mathf.Atan2(randomSpeed.y, randomSpeed.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject fireMeteor = Instantiate(fireMeteorPrefab, randomPosition, rotation);

        // Assuming fireMeteor has a Rigidbody2D component
        Rigidbody2D rb = fireMeteor.GetComponent<Rigidbody2D>();
        rb.velocity = randomSpeed;
    }

    public void InstantiateSlash()
    {
        GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);
        slash.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * Mathf.Sign(transform.localScale.x), 0);
    }

    private void DestroySkeleton()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SkeletonAnimationEventsManager.OnDisappearEvent -= DestroySkeleton;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackHitBox);
    }

    public void StartAnyCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
