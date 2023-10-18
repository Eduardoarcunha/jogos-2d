using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStateManager : MonoBehaviour
{
    public GameObject player;
    public Transform attackPoint;
    public Transform raycastPoint;
    public BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;

    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public SpriteRenderer spriteRenderer { get; private set; }
    [HideInInspector] public BoxCollider2D boxCollider { get; private set; }
    
    public int gameObjectId;  
    public float[] roamingPositions;
    public float speed = 2f;

    public float attackRange = 0.5f;
    public float life = 10;
    public bool isDead = false;
      

    private SkeletonBaseState currentState;
    public SkeletonUnawareState unawareState = new SkeletonUnawareState();
    public SkeletonAwareState awareState = new SkeletonAwareState();

    private void Awake()
    {
        SkeletonAnimationEventsManager.OnDisappearEvent += DestroySkeleton;

        gameObjectId = gameObject.GetInstanceID();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        currentState = new SkeletonUnawareState();
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

    public void SwitchState(SkeletonBaseState newState)
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
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawRay(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0) * 10f);
    }

    public void StartAnyCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
