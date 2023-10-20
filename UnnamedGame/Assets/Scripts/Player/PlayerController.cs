using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("References")]
    private HealthPotions healthPotions; 
    private PlayerCombat playerCombat;   
    [SerializeField] private Transform groundCheck;    
    [SerializeField] private GameObject keyUI;

    [Header("Properties")]
    [SerializeField] private float airPenalty = 0.9f;
    [SerializeField] private float accelerationForce = 400;
    [SerializeField] private float rollForce = 10;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float gravityScale = 2.5f;
    private int remainingPotions = 5;
    private int potionHealAmount = 1;

    // State Variables
    private float horizontalInput;
    private bool verticalInput;
    private bool isGrounded = false;
    private bool isAttacking = false;
    private bool isRolling = false;
    private bool isHitted = false;
    private bool isDead = false;
    private bool paused = false;
    private Vector2 targetVelocity;
    private Vector3 velocity = Vector3.zero;

    public static event Action<bool> OnChangeGroundedState;
    public static event Action OnKeyCollect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        animator = GetComponent<Animator>();
        healthPotions = GetComponent<HealthPotions>();
        playerCombat = GetComponent<PlayerCombat>();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        PlayerAnimationEventsManager.OnStartAttack1Event += OnStartAttack;
        PlayerAnimationEventsManager.OnStartAttack2Event += OnStartAttack;
        PlayerAnimationEventsManager.OnStartRollEvent += OnStartRoll;
        PlayerAnimationEventsManager.OnEndRollEvent += OnEndRoll;
        PlayerAnimationEventsManager.OnHittedEvent += OnHitted;
        PlayerAnimationEventsManager.OnEndHittedEvent += OnEndHitted;
        PlayerCombat.OnDeathEvent += OnDeath;
        PlayerCombat.OnEndAttackEvent += OnEndAttack;
        GameManager.OnPauseOrResumeGame += OnPauseOrResumeGame;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerAnimationEventsManager.OnStartAttack1Event -= OnStartAttack;
        PlayerAnimationEventsManager.OnStartAttack2Event -= OnStartAttack;
        PlayerAnimationEventsManager.OnStartRollEvent -= OnStartRoll;
        PlayerAnimationEventsManager.OnEndRollEvent -= OnEndRoll;
        PlayerAnimationEventsManager.OnHittedEvent -= OnHitted;
        PlayerAnimationEventsManager.OnEndHittedEvent -= OnEndHitted;
        PlayerCombat.OnDeathEvent -= OnDeath;
        PlayerCombat.OnEndAttackEvent -= OnEndAttack;
        GameManager.OnPauseOrResumeGame -= OnPauseOrResumeGame;
    }

    private bool CanMoveOrAct()
    {
        return !isAttacking && !isRolling && !isHitted && !isDead;
    }

    private void Update()
    {
        if (isDead && isGrounded){
            rb.velocity = Vector3.zero;
        }

        IsGroundedCheck();

        if (!paused){
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput |= Input.GetKeyDown(KeyCode.W);

            if (Input.GetKeyDown(KeyCode.H) && remainingPotions > 0)
            {
                remainingPotions--;
                healthPotions.SetPotions(remainingPotions);
                playerCombat.Heal(potionHealAmount);
            }
        }
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (CanMoveOrAct())
        {
            float moveInput = horizontalInput * accelerationForce * Time.fixedDeltaTime;
            moveInput = !isGrounded ? moveInput * airPenalty : moveInput;

            if (verticalInput && isGrounded)
            {
                AudioManager.instance.PlaySound("Jump");
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
        if (horizontalInput != 0 && !isAttacking && !isRolling && !isHitted && !isDead)
        {
            int sign = Mathf.Sign(horizontalInput) > 0 ? 1 : -1;
            transform.localScale = new Vector3(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("isMoving", true);

            if (isGrounded && !AudioManager.instance.IsPlaying("Footsteps"))
            {
                AudioManager.instance.PlaySound("Footsteps");
            } else if (!isGrounded && AudioManager.instance.IsPlaying("Footsteps"))
            {
                AudioManager.instance.StopSound("Footsteps");
            }
        }
        else
        {
            // AudioManager.instance.FadeOutAndStop("Footsteps", 0.5f);
            AudioManager.instance.StopSound("Footsteps");
            animator.SetBool("isMoving", false);
        }

        if (!isGrounded && rb.velocity.y < 0 && !isDead && !isHitted && !isRolling)
        {
            animator.SetBool("isFalling", true);
        }
        
    }

    private void IsGroundedCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                if (!isGrounded) {
                    verticalInput = false;
                    isGrounded = true;
                    OnChangeGroundedState?.Invoke(true);
                    
                    if (isDead) {
                        rb.velocity = Vector3.zero;
                    } else {
                        animator.SetBool("isGrounded", true);
                        animator.SetBool("isFalling", false);
                    }
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

    private void OnStartAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
    }

    private void OnEndAttack()
    {
        isAttacking = false;
    }

    private void OnStartRoll()
    {
        isRolling = true;
        rb.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * rollForce, 0);
        gameObject.layer = LayerMask.NameToLayer("PlayerRolling");        
    }

    private void OnEndRoll()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        isRolling = false;
    }

    private void OnHitted()
    {
        isAttacking = false;
        isRolling = false;
        isHitted = true;
    }

    private void OnEndHitted()
    {
        isAttacking = false;
        isRolling = false;
        isHitted = false;
    }


    private void OnDeath()
    {
        isDead = true;
        isAttacking = false;
        isRolling = false;
        isHitted = false;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void IncreaseSpeed()
    {
        accelerationForce += 50;
    }

    private void OnPauseOrResumeGame(bool isPaused)
    {
        paused = isPaused;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            OnKeyCollect?.Invoke();
            Destroy(other.gameObject);
            keyUI.SetActive(true);
        }
    }
}