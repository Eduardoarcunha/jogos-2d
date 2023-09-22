using UnityEngine;

public class SkeletonUnawareState : SkeletonBaseState
{
    private Rigidbody2D rb;
    private Animator animator;

    private Transform raycastPoint;

    private int playerMask;
    private float roamingPosition;
    private int roamingPositionIdx = 0;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("Skeleton is unaware of player");

        rb = skeleton.GetRigidbody();
        animator = skeleton.GetAnimator();
        raycastPoint = skeleton.GetRaycastPoint();

        playerMask = LayerMask.GetMask("Player");
        roamingPosition = skeleton.roamingPositions[roamingPositionIdx];
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        SearchPlayer(skeleton);
        UpdateAnimator(skeleton);
    }

    public override void FixedUpdateState(SkeletonStateManager skeleton)
    {
        if (Mathf.Abs(skeleton.transform.position.x - roamingPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Sign(roamingPosition - skeleton.transform.position.x) * skeleton.speed, 0);
        }
    }

    public override void ExitState(SkeletonStateManager skeleton)
    {
    }

    private void SearchPlayer(SkeletonStateManager skeleton)
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, new Vector2(Mathf.Sign(skeleton.transform.localScale.x) * 1, 0), 6f, playerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            skeleton.SwitchState(skeleton.awareState);
        }
    }

    private void UpdateAnimator(SkeletonStateManager skeleton)
    {
        if (Mathf.Abs(skeleton.transform.position.x - roamingPosition) < 0.1f)
        {
            roamingPositionIdx = (roamingPositionIdx + 1) % skeleton.roamingPositions.Length;
            roamingPosition = skeleton.roamingPositions[roamingPositionIdx];
            animator.SetBool("isMoving", false);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Sign(roamingPosition - skeleton.transform.position.x), 0);
            skeleton.transform.localScale = new Vector3(Mathf.Sign(roamingPosition - skeleton.transform.position.x), 1, 1);
            animator.SetBool("isMoving", true);
        }
    }
}
