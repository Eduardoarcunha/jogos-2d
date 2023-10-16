using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalPhaseOneState : RivalBaseState
{
    private GameObject player;
    private float signRivalToPlayer;
    private CurrentStateEnum currentState;

    private int attackId;

    private Dictionary<int, int> attackIdToDamage = new Dictionary<int, int>()
    {
        {1, 1},
        {2, 2},
        {3, 2},
        {4, 3}
    };

    public override void EnterState()
    {
        player = rival.player;
        
        PlayerCombat.OnHitEnemyEvent += onHitted;
        PlayerCombat.OnDeathEvent += OnPlayerDeath;

        RivalAnimationEventsManager.OnAttackHitEvent += OnHitAttack;
        RivalAnimationEventsManager.OnEndAttack1Event += OnEndAttack1;
        RivalAnimationEventsManager.OnEndAttack2Event += OnEndAttack2;
        RivalAnimationEventsManager.OnEndAttack3Event += OnEndAttack3;
        RivalAnimationEventsManager.OnEndAttack4Event += OnEndAttack4;
        RivalAnimationEventsManager.OnFlipEvent += OnFlip;
    }

    public override void UpdateState()
    {
        if (rival.playerIsDead)
        {
            ChangeState(CurrentStateEnum.Winner);
            return;
        }
        if (rival.isDead) return;
        UpdateAnimator();
        EvaluateAndChangeStateBasedOnDistance();
        UpdateCooldowns();
    }

    private void EvaluateAndChangeStateBasedOnDistance()
    {
        float distanceToPlayer = Mathf.Abs(rival.transform.position.x - player.transform.position.x);
        bool inCloseRange = distanceToPlayer < rival.closeAttackRange;
        bool inLongRange = distanceToPlayer > rival.longAttackRange;
        bool inSafeDistance = distanceToPlayer > rival.safeDistance;

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
        if (AttackIsNotOnCooldown(rival.attack2CooldownRemaining, rival.attack3CooldownRemaining) )
        {
            if (rival.allAttacksCooldownRemaining <= 0)
            {
                Attack("attack2Trigger", "attack3Trigger", ref rival.attack2CooldownRemaining, ref rival.attack3CooldownRemaining, rival.attack2Cooldown, rival.attack3Cooldown, 2, 3);
            } 
            else 
            {
                ChangeState(CurrentStateEnum.Stay);
            }
        }
        else if (AttackIsNotOnCooldown(rival.attack1CooldownRemaining, rival.attack4CooldownRemaining))
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
        if (inLongRange && AttackIsNotOnCooldown(rival.attack1CooldownRemaining, rival.attack4CooldownRemaining))
        {
            if (rival.allAttacksCooldownRemaining <= 0)
            {
                Attack("attack1Trigger", "attack4Trigger", ref rival.attack1CooldownRemaining, ref rival.attack4CooldownRemaining, rival.attack1Cooldown, rival.attack4Cooldown, 1, 4);
            }
            else
            {
                ChangeState(CurrentStateEnum.Stay);
            }
        }
        else if (AttackIsNotOnCooldown(rival.attack2CooldownRemaining, rival.attack3CooldownRemaining))
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
            TriggerAttack(trigger1, ref cooldownRemaining1, cooldown1);
            attackId = attackId1;
        }
        else if (cooldownRemaining1 > 0 && cooldownRemaining2 <= 0)
        {
            TriggerAttack(trigger2, ref cooldownRemaining2, cooldown2);
            attackId = attackId2;
        }
        else
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                TriggerAttack(trigger1, ref cooldownRemaining1, cooldown1);
                attackId = attackId1;
            }
            else
            {
                TriggerAttack(trigger2, ref cooldownRemaining2, cooldown2);
                attackId = attackId2;
            }
        }
    }

    private void OnHitAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(rival.attackPoint.position, rival.attackHitBox, rival.playerMask);
        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(rival.transform, attackIdToDamage[attackId]);
        }
        return;
    }
    
    private void TriggerAttack(string trigger, ref float cooldownRemaining, float cooldown)
    {
        rival.animator.SetTrigger(trigger);
        cooldownRemaining = cooldown;
        rival.allAttacksCooldownRemaining = rival.allAttacksCooldown;
    }

    public override void FixedUpdateState()
    {
        UpdateRb();
    }

    public override void ExitState()
    {
        PlayerCombat.OnHitEnemyEvent -= onHitted;

        RivalAnimationEventsManager.OnEndAttack1Event += OnEndAttack1;
        RivalAnimationEventsManager.OnEndAttack1Event += OnEndAttack1;
        RivalAnimationEventsManager.OnEndAttack2Event += OnEndAttack2;
        RivalAnimationEventsManager.OnEndAttack3Event += OnEndAttack3;
        RivalAnimationEventsManager.OnEndAttack4Event += OnEndAttack4;
    }

    public enum CurrentStateEnum
    {
        RunAway,
        Stay,
        RunTowards,
        Attack,
        Death,
        Winner
    }

    private void ChangeState(CurrentStateEnum newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        if (currentState == CurrentStateEnum.Winner){
            rival.animator.SetTrigger("winnerTrigger");
            return;
        }

        bool isMoving = currentState == CurrentStateEnum.RunAway || currentState == CurrentStateEnum.RunTowards;
        rival.animator.SetBool("isMoving", isMoving);    
    }

    private void UpdateAnimator()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - rival.transform.position.x);
    
        if (currentState.Equals(CurrentStateEnum.RunAway))
        {
            rival.transform.localScale = new Vector3(-signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);
        } 
        else 
        {
            rival.transform.localScale = new Vector3(signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);
        }
    }

    private void UpdateRb()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - rival.transform.position.x);
    
        if (currentState.Equals(CurrentStateEnum.RunAway))
        {
            rival.rb.velocity = new Vector2(-signRivalToPlayer * rival.speed, rival.rb.velocity.y);
        }
        else if (currentState.Equals(CurrentStateEnum.RunTowards))
        {
            rival.rb.velocity = new Vector2(signRivalToPlayer * rival.speed, rival.rb.velocity.y);
        }
        else
        {
            rival.rb.velocity = new Vector2(0, 0);
        }
    }

    void UpdateCooldowns()
    {
        rival.attack1CooldownRemaining -= Time.fixedDeltaTime;
        rival.attack2CooldownRemaining -= Time.fixedDeltaTime;
        rival.attack3CooldownRemaining -= Time.fixedDeltaTime;
        rival.attack4CooldownRemaining -= Time.fixedDeltaTime;
        rival.allAttacksCooldownRemaining -= Time.fixedDeltaTime;
    }

    void ResetCooldown(float cooldownRemaining, float cooldown)
    {
            cooldownRemaining = cooldown;
    }

    private void onHitted(int id, int damage)
    {
        if (id == rival.gameObjectId)
        {
            rival.life -= damage;
            rival.healthBar.UpdateHealthBar(rival.life, rival.maxLife);
            rival.flashEffect.Flash();
            if (rival.life <= 0)
            {
                OnDeath();
                ChangeState(CurrentStateEnum.Death);
            }
        }
    }

    private void OnEndAttack1()
    {
        rival.InstantiateSlash();
        ChangeState(CurrentStateEnum.Stay);
    }

    private void OnEndAttack2() => ChangeState(CurrentStateEnum.RunTowards);
    private void OnEndAttack3() => ChangeState(CurrentStateEnum.RunTowards);

    private void OnEndAttack4()
    {
        for (int i = 0; i < 5; i++)
        {
            rival.InstantiateFireMeteor();
        }
        ChangeState(CurrentStateEnum.Stay);
    }

    private void OnFlip()
    {
        rival.transform.localScale = new Vector3(-rival.transform.localScale.x, rival.transform.localScale.y, rival.transform.localScale.z);
    }
    
    private void OnPlayerDeath()
    {
        rival.playerIsDead = true;
        ChangeState(CurrentStateEnum.Stay);
    }
}
