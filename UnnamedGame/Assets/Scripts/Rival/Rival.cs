using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : MonoBehaviour
{
    // Player-related
    [SerializeField] private GameObject player;
    private int playerMask;

    // Components and scripts
    private int gameObjectId;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    [SerializeField] private BarthaSzabolcs.Tutorial_SpriteFlash.SimpleFlash flashEffect;
    [SerializeField] private Bar healthBar;

    // Attack attributes
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject fireMeteorPrefab;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private float safeDistance = 4f;
    [SerializeField] private float attackHitBox = 0.5f;
    [SerializeField] private float closeAttackRange = 1.8f;
    [SerializeField] private float longAttackRange = 3f;

    // Cooldowns    
    [SerializeField] private float allAttacksCooldown = 3f;
    [SerializeField] private float attack1Cooldown = 60f;
    [SerializeField] private float attack2Cooldown = 20f;
    [SerializeField] private float attack3Cooldown = 20f;
    [SerializeField] private float attack4Cooldown = 80f;
    [SerializeField] private float allAttacksCooldownRemaining = 0f;
    [SerializeField] private float attack1CooldownRemaining = 0f;
    [SerializeField] private float attack2CooldownRemaining = 0f;
    [SerializeField] private float attack3CooldownRemaining = 0f;
    [SerializeField] private float attack4CooldownRemaining = 0f;

    // Stats and conditions
    [SerializeField] private float maxLife = 50f;
    [SerializeField] private float life;
    [SerializeField] private bool isDead = false;  
    // [SerializeField] private bool canAttack = true;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool playerIsDead = false;

    private float signRivalToPlayer;
    private CurrentStateEnum currentState;

    private int attackId;

    private Dictionary<int, int> attackIdToDamage = new Dictionary<int, int>()
    {
        {1, 2},
        {2, 3},
        {3, 3},
        {4, 4}
    };

    public enum CurrentStateEnum
    {
        RunAway,
        Stay,
        RunTowards,
        Attack,
        Death,
        Winner
    }

    private void Awake()
    {
        gameObjectId = gameObject.GetInstanceID();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        maxLife = 50;
        life = maxLife;
        healthBar.UpdateBar(life, maxLife);

        PlayerCombat.OnDeathEvent += OnPlayerDeath;
        PlayerCombat.OnHitEnemyEvent += TakeDamage;
        RivalAnimationEventsManager.OnAttackHitEvent += OnHitAttack;
        RivalAnimationEventsManager.OnEndAttack1Event += OnEndAttack1;
        RivalAnimationEventsManager.OnEndAttack2Event += OnEndAttack2;
        RivalAnimationEventsManager.OnEndAttack3Event += OnEndAttack3;
        RivalAnimationEventsManager.OnEndAttack4Event += OnEndAttack4;
        RivalAnimationEventsManager.OnFlipEvent += OnFlip;
    }

    private void Update()
    {
        if (playerIsDead)
        {
            ChangeState(CurrentStateEnum.Winner);
            return;
        }
        if (isDead) return;
        UpdateAnimator();
        EvaluateAndChangeStateBasedOnDistance();
        UpdateCooldowns();
    }

    private void ChangeState(CurrentStateEnum newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        if (currentState == CurrentStateEnum.Winner){
            animator.SetTrigger("winnerTrigger");
            return;
        }

        bool isMoving = currentState == CurrentStateEnum.RunAway || currentState == CurrentStateEnum.RunTowards;
        animator.SetBool("isMoving", isMoving);    
    }

    private void UpdateAnimator()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - transform.position.x);
    
        if (currentState.Equals(CurrentStateEnum.RunAway))
        {
            transform.localScale = new Vector3(-signRivalToPlayer * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } 
        else 
        {
            transform.localScale = new Vector3(signRivalToPlayer * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void UpdateCooldowns()
    {
        attack1CooldownRemaining -= Time.fixedDeltaTime;
        attack2CooldownRemaining -= Time.fixedDeltaTime;
        attack3CooldownRemaining -= Time.fixedDeltaTime;
        attack4CooldownRemaining -= Time.fixedDeltaTime;
        allAttacksCooldownRemaining -= Time.fixedDeltaTime;
    }

    private void EvaluateAndChangeStateBasedOnDistance()
    {
        float distanceToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        bool inCloseRange = distanceToPlayer < closeAttackRange;
        bool inLongRange = distanceToPlayer > longAttackRange;
        bool inSafeDistance = distanceToPlayer > safeDistance;

        if (currentState == CurrentStateEnum.Attack || currentState == CurrentStateEnum.Death) return;

        if (inCloseRange)
        {
            HandleAttackInRange();
        }
        else
        {
            HandleAttackOutOfRange(inSafeDistance, inLongRange);
        }
    }

    private void HandleAttackInRange()
    {
        if (AttackIsNotOnCooldown(attack2CooldownRemaining, attack3CooldownRemaining) )
        {
            if (allAttacksCooldownRemaining <= 0)
            {
                Attack("attack2Trigger", "attack3Trigger", ref attack2CooldownRemaining, ref attack3CooldownRemaining, attack2Cooldown, attack3Cooldown, 2, 3);
            } 
            else 
            {
                ChangeState(CurrentStateEnum.Stay);
            }
        }
        else if (AttackIsNotOnCooldown(attack1CooldownRemaining, attack4CooldownRemaining))
        {
            ChangeState(CurrentStateEnum.RunAway);
        } 
        else 
        {
            ChangeState(CurrentStateEnum.Stay);
        }
    }
    
    private void HandleAttackOutOfRange(bool inSafeDistance, bool inLongRange)
    {
        if (inLongRange && AttackIsNotOnCooldown(attack1CooldownRemaining, attack4CooldownRemaining))
        {
            if (allAttacksCooldownRemaining <= 0)
            {
                Attack("attack1Trigger", "attack4Trigger", ref attack1CooldownRemaining, ref attack4CooldownRemaining, attack1Cooldown, attack4Cooldown, 1, 4);
            }
            else
            {
                ChangeState(CurrentStateEnum.Stay);
            }
        }
        else if (AttackIsNotOnCooldown(attack2CooldownRemaining, attack3CooldownRemaining))
        {
            ChangeState(CurrentStateEnum.RunTowards);
        }
        else if (inSafeDistance)
        {
            ChangeState(CurrentStateEnum.Stay);
        }
        else
        {
            ChangeState(CurrentStateEnum.RunAway);
        }
    }

    private bool AttackIsNotOnCooldown(float cooldown1, float cooldown2)
    {
        return cooldown1 <= 0 || cooldown2 <= 0;
    }

    private void Attack(string trigger1, string trigger2, ref float cooldownRemaining1, ref float cooldownRemaining2, float cooldown1, float cooldown2, int attackId1, int attackId2)
    {
        ChangeState(CurrentStateEnum.Attack);
        if (cooldownRemaining1 <= 0 && cooldownRemaining2 > 0)
        {
            if(trigger1 == "attack2Trigger") AudioManager.instance.PlaySound("BossSword");
            else if(trigger1 == "attack3Trigger") AudioManager.instance.PlaySound("DoublePierce");

            TriggerAttack(trigger1, ref cooldownRemaining1, cooldown1);
            attackId = attackId1;
        }
        else if (cooldownRemaining1 > 0 && cooldownRemaining2 <= 0)
        {
            if(trigger2 == "attack2Trigger") AudioManager.instance.PlaySound("BossSword");
            else if(trigger2 == "attack3Trigger") AudioManager.instance.PlaySound("DoublePierce");

            TriggerAttack(trigger2, ref cooldownRemaining2, cooldown2);
            attackId = attackId2;
        }
        else
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                if(trigger1 == "attack2Trigger") AudioManager.instance.PlaySound("BossSword");
                else if(trigger1 == "attack3Trigger") AudioManager.instance.PlaySound("DoublePierce");
                TriggerAttack(trigger1, ref cooldownRemaining1, cooldown1);
                attackId = attackId1;
            }
            else
            {
                if(trigger2 == "attack2Trigger") AudioManager.instance.PlaySound("BossSword");
                else if(trigger2 == "attack3Trigger") AudioManager.instance.PlaySound("DoublePierce");
                TriggerAttack(trigger2, ref cooldownRemaining2, cooldown2);
                attackId = attackId2;
            }
        }
    }

    private void OnHitAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackHitBox, playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(transform, attackIdToDamage[attackId]);
        }
        return;
    }
    
    private void TriggerAttack(string trigger, ref float cooldownRemaining, float cooldown)
    {
        animator.SetTrigger(trigger);
        cooldownRemaining = cooldown;
        allAttacksCooldownRemaining = allAttacksCooldown;
    }

    private void FixedUpdate()
    {
        UpdateRb();
    }

    private void UpdateRb()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - transform.position.x);
    
        if (currentState.Equals(CurrentStateEnum.RunAway))
        {
            rb.velocity = new Vector2(-signRivalToPlayer * speed, rb.velocity.y);
        }
        else if (currentState.Equals(CurrentStateEnum.RunTowards))
        {
            rb.velocity = new Vector2(signRivalToPlayer * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void OnEndAttack1()
    {
        AudioManager.instance.PlaySound("BossFire");
        GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);
        slash.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * Mathf.Sign(transform.localScale.x), 0);
        ChangeState(CurrentStateEnum.Stay);
    }

    private void OnEndAttack2()
    {
        
        ChangeState(CurrentStateEnum.RunTowards);
    } 
    private void OnEndAttack3() 
    {
        
        ChangeState(CurrentStateEnum.RunTowards);
    }

    private void OnEndAttack4()
    {
        AudioManager.instance.PlaySound("Meteor");
        for (int i = 0; i < 5; i++)
        {
            
            Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 5, 0);
            Vector2 randomSpeed = new Vector2(Random.Range(-1f,1f), -3f);
            float angle = Mathf.Atan2(randomSpeed.y, randomSpeed.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject fireMeteor = Instantiate(fireMeteorPrefab, randomPosition, rotation);
            fireMeteor.GetComponent<Rigidbody2D>().velocity = randomSpeed;
        }
        ChangeState(CurrentStateEnum.Stay);
    }

    private void OnFlip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    
    private void OnPlayerDeath()
    {
        playerIsDead = true;
        ChangeState(CurrentStateEnum.Stay);
    }

    private void TakeDamage(int id, int damage)
    {
        if (id == gameObjectId){
            life -= damage;
            healthBar.UpdateBar(life, maxLife);
            flashEffect.Flash();
            if (life <= 0)
            {
                OnDeath();
                ChangeState(CurrentStateEnum.Death);
            }
        }
        return;
    }

    private void OnDeath()
    {
        AudioManager.instance.PlaySound("BossDeathScream");
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        boxCollider.enabled = false;
        isDead = true;
        animator.SetTrigger("deathTrigger");
        GameManager.instance.ChangeState(GameManager.GameState.WonGame);
    }

    private void OnDestroy()
    {
        PlayerCombat.OnDeathEvent -= OnPlayerDeath;
        PlayerCombat.OnHitEnemyEvent -= TakeDamage;
        RivalAnimationEventsManager.OnAttackHitEvent -= OnHitAttack;
        RivalAnimationEventsManager.OnEndAttack1Event -= OnEndAttack1;
        RivalAnimationEventsManager.OnEndAttack2Event -= OnEndAttack2;
        RivalAnimationEventsManager.OnEndAttack3Event -= OnEndAttack3;
        RivalAnimationEventsManager.OnEndAttack4Event -= OnEndAttack4;
        RivalAnimationEventsManager.OnFlipEvent -= OnFlip;
    }
}
