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

    private float horizontalInput;
    private bool isGrounded = false;
    private bool isAttacking = false;
    private Vector2 targetVelocity;
    private Vector3 velocity = Vector3.zero;


    public static event Action<bool> OnChangeGroundedState;

    void Start()
    {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        PlayerCombat.OnLightAttack += FreezePlayer;
    }

    private void Update()
    {
        CheckInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        float moveInput = horizontalInput * accelerationForce * Time.fixedDeltaTime;
        
        if (!isAttacking)
        {
            if (!isGrounded)
            {
                moveInput *= airPenalty;
            }

            Move(moveInput);
        }
    }

    private void CheckInput()
    {
        IsGroundedCheck();
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
    }

    private void Move(float moveInput)
    {
        targetVelocity = new Vector2(moveInput, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);
    }

    private void UpdateAnimator()
    {
        if (horizontalInput != 0 && !isAttacking)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void IsGroundedCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                isGrounded = true;
                OnChangeGroundedState?.Invoke(true);
                animator.SetBool("isGrounded", true);
                animator.SetBool("isFalling", false);
                return;
            }
        }

        animator.SetBool("isGrounded", false);
        OnChangeGroundedState?.Invoke(false);
        isGrounded = false;
    }

    private void FreezePlayer()
    {
        StopCoroutine(UnfreezePlayer());
        isAttacking = true;
        rb.velocity = Vector2.zero;
        StartCoroutine(UnfreezePlayer());
    }

    private IEnumerator UnfreezePlayer()
    {
        AnimatorStateInfo currentAnimState = animator.GetCurrentAnimatorStateInfo(0);
        float timeRemaining = (1.0f - currentAnimState.normalizedTime) * currentAnimState.length;
        yield return new WaitForSeconds(timeRemaining + 0.3f);
        isAttacking = false;
    }

    private void OnDestroy()
    {
        PlayerCombat.OnLightAttack -= FreezePlayer;
    }
}