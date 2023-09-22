using UnityEngine;

public class SkeletonStateManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform raycastPoint;

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

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public Transform GetAttackPoint()
    {
        return attackPoint;
    }

    public Transform GetRaycastPoint()
    {
        return raycastPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawRay(raycastPoint.position, new Vector2(Mathf.Sign(transform.localScale.x) * 1, 0) * 10f);
    }
}
