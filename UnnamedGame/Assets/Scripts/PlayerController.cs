using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Transform trans;
    private Rigidbody2D rb;
    private Animator animator;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float airPenalty = 0.9f;
    [SerializeField] private float accelerationForce = 400;
    [SerializeField] private float jumpForce = 14;
    [SerializeField] private float unFreezeDelay = 0.3f;

    private float horizontalInput;
    private bool verticalInput;
    private bool isGrounded = false;
    private bool isAttacking = false;
    private bool isRolling = false;
    private Vector2 targetVelocity;
    private Vector3 velocity = Vector3.zero;


    public static event Action<bool> OnChangeGroundedState;

    void Start()
    {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        PlayerCombat.OnLightAttack += Attack;
        PlayerCombat.OnRoll += Roll;
    }

    private void Update()
    {
        IsGroundedCheck();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput |= Input.GetKeyDown(KeyCode.W);
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!isAttacking && !isRolling)
        {
            float moveInput = horizontalInput * accelerationForce * Time.fixedDeltaTime;

            if (!isGrounded)
            {
                moveInput *= airPenalty;
            }

            if (verticalInput && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            Move(moveInput);
        }
    }

    private void Move(float moveInput)
    {
        targetVelocity = new Vector2(moveInput, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);
    }

    private void UpdateAnimator()
    {
        if (horizontalInput != 0 && !isAttacking && !isRolling)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
    }

    private void IsGroundedCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                if (!isGrounded) {
                    verticalInput = false;
                    isGrounded = true;
                    OnChangeGroundedState?.Invoke(true);
                    animator.SetBool("isGrounded", true);
                    animator.SetBool("isFalling", false);
                    
                }
                return;
            }
        }
        if (isGrounded) {
           animator.SetBool("isGrounded", false);
            OnChangeGroundedState?.Invoke(false);
            isGrounded = false;
        }
    }

    private void Roll()
    {
        StopCoroutine(UnfreezePlayer());
        isRolling = true;
        rb.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * 12, 0);
        FreezePlayer();
        
    }

    private void Attack()
    {
        StopCoroutine(UnfreezePlayer());
        isAttacking = true;
        rb.velocity = Vector2.zero;
        FreezePlayer();
    }

    private void FreezePlayer()
    {
        StartCoroutine(UnfreezePlayer());
    }

    private IEnumerator UnfreezePlayer()
    {
        AnimatorStateInfo currentAnimState = animator.GetCurrentAnimatorStateInfo(0);
        float timeRemaining = (1.0f - currentAnimState.normalizedTime) * currentAnimState.length;
        yield return new WaitForSeconds(timeRemaining + unFreezeDelay);
        isAttacking = false;
        isRolling = false;
    }

    private void OnDestroy()
    {
        PlayerCombat.OnLightAttack -= FreezePlayer;
    }
}