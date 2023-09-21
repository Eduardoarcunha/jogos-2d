using UnityEngine;

public class SkeletonStateManager : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public Animator animator;

    public Transform attackPoint;
    public Transform raycastPoint;

    public float[] roamingPositions;
    public float speed = 2f;

    public float attackRange = 0.5f;

    private SkeletonBaseState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentState = new SkeletonUnawareState();
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(SkeletonBaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawRay(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0) * 10f);
    }
}
