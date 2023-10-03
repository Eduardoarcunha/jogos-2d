using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalStateManager : MonoBehaviour
{
    public GameObject player;
    public Transform attackPoint;
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;

    [HideInInspector] public int gameObjectId;
    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public SpriteRenderer spriteRenderer { get; private set; }
    [HideInInspector] public BoxCollider2D boxCollider { get; private set; }
    
    // Attributes
    public float life = 10;
    public bool isDead = false;  
    public bool canAttack = true;
    public float speed = 3f;
    public float safeDistance = 4f;
    public float attackHitBox = 0.5f;
    public float attackRange = 2f;
    public float attack1Cooldown = 2f;
    public float attack2Cooldown = 2f;
    public float attack3Cooldown = 2f;
    public float attack4Cooldown = 2f;
    public float attack1CooldownRemaining = 0f;
    public float attack2CooldownRemaining = 0f;
    public float attack3CooldownRemaining = 0f;
    public float attack4CooldownRemaining = 0f;


    private RivalBaseState currentState;
    public RivalPhaseOneState phaseOneState = new RivalPhaseOneState();

    private void Awake()
    {
        gameObjectId = gameObject.GetInstanceID();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        currentState = phaseOneState;
        currentState.InitializeState(this);
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
