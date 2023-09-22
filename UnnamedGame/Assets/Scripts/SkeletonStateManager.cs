using UnityEngine;

public class SkeletonStateManager : MonoBehaviour
{
    public GameObject player;
    public Transform attackPoint;
    public Transform raycastPoint;

    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }

    public float[] roamingPositions;
    public float speed = 2f;
    public float attackRange = 0.5f;

    private SkeletonBaseState currentState;
    public SkeletonUnawareState unawareState = new SkeletonUnawareState();
    public SkeletonAwareState awareState = new SkeletonAwareState();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentState = new SkeletonUnawareState();
        currentState.EnterState(this);
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
        currentState.EnterState(this);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawRay(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0) * 10f);
    }
}
