using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{

    private Transform trans;
    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private float airPenalty = 0.75f;

    private float moveSpeed = 300;
    private float jumpForce = 6;
    [SerializeField] private Transform groundCheck;    
    
    private bool isGrounded = false;
    private Vector2 targetVelocity;
    private Vector3 velocity = Vector3.zero;


    void Start()
    {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        IsGroundedCheck();
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {

            if (horizontalInput < 0){
                transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = new Vector3(1, 1, 1);
            }

            animator.SetBool("isMoving", true);
        } else {
            animator.SetBool("isMoving", false);
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            Move(horizontalInput * moveSpeed * Time.fixedDeltaTime);
        } else {
            Move(horizontalInput * moveSpeed * Time.fixedDeltaTime * airPenalty);
        } 
        
    }


    void Move(float moveInput)
    {
        targetVelocity = new Vector2(moveInput, rb.velocity.y);      
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.1f);
    }


    void IsGroundedCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject){
                isGrounded = true;
                animator.SetBool("isGrounded", true);
                return;
            }
        }
        animator.SetBool("isGrounded", false);
        isGrounded = false;
    }
}
