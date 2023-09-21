using UnityEngine;

public class SkeletonUnawareState : SkeletonBaseState
{
    private int layerMask;
    private float roamingPosition;
    private int roamingPositionIdx = 0;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("Skeleton is unaware of player");
        layerMask = LayerMask.GetMask("Player");
        roamingPosition = skeleton.roamingPositions[roamingPositionIdx];
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        SearchPlayer(skeleton);

        if (Mathf.Abs(skeleton.transform.position.x - roamingPosition) < 0.1f)
        {
            roamingPositionIdx = (roamingPositionIdx + 1) % skeleton.roamingPositions.Length;
            roamingPosition = skeleton.roamingPositions[roamingPositionIdx];
        }
        else
        {
            skeleton.rb.velocity = new Vector2(Mathf.Sign(roamingPosition - skeleton.transform.position.x), 0);
            skeleton.transform.localScale = new Vector3(Mathf.Sign(roamingPosition - skeleton.transform.position.x), 1, 1);
            skeleton.animator.SetBool("isMoving", true);
        }
    }

    public override void FixedUpdateState(SkeletonStateManager skeleton)
    {
        // FixedUpdate logic (if any)
    }

    public override void ExitState(SkeletonStateManager skeleton)
    {
        // Exit state logic (if any)
    }

    private void SearchPlayer(SkeletonStateManager skeleton)
    {
        RaycastHit2D hit = Physics2D.Raycast(skeleton.raycastPoint.position, new Vector2(Mathf.Sign(skeleton.transform.localScale.x) * 1, 0), 10f, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            skeleton.SwitchState(new SkeletonAwareState());
        }
    }
}
