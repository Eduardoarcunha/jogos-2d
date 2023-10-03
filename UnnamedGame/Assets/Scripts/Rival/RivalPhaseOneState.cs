using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalPhaseOneState : RivalBaseState
{
    private GameObject player;
    private float signRivalToPlayer;
    private currentStateEnum currentState;

    public override void EnterState()
    {
        Debug.Log("Rival is in phase one");
        player = rival.player;

        RivalAnimationEventsManager.OnEndAttack2Event += () => { ChangeState(currentStateEnum.stay); };
        RivalAnimationEventsManager.OnEndAttack3Event += () => { ChangeState(currentStateEnum.stay); };
    }

    public override void UpdateState()
    {
        Debug.Log(currentState);   
        UpdateAnimator();

        if (currentState != currentStateEnum.attack)
        {    
            bool inRange = Mathf.Abs(rival.transform.position.x - player.transform.position.x) < rival.attackRange;
            bool inSafeDistance = Mathf.Abs(rival.transform.position.x - player.transform.position.x) > rival.safeDistance;
            Debug.Log(inRange);
            if (inRange)
            {
                if (rival.attack2CooldownRemaining <= 0 || rival.attack3CooldownRemaining <= 0)
                {
                    Debug.Log("Attacked 2 or 3");
                    int random = Random.Range(0, 2);
                    if (random == 0)
                    {
                        ChangeState(currentStateEnum.attack);
                        rival.animator.SetTrigger("attack2Trigger");
                        rival.attack2CooldownRemaining = rival.attack2Cooldown;
                    }
                    else
                    {
                        ChangeState(currentStateEnum.attack);
                        rival.animator.SetTrigger("attack3Trigger");
                        rival.attack3CooldownRemaining = rival.attack3Cooldown;   
                    }
                }
                else
                {
                    Debug.Log("Run away player");
                    ChangeState(currentStateEnum.runAway);
                }
            }
            else
            {
                if (rival.attack1CooldownRemaining <= 0 || rival.attack4CooldownRemaining <= 0)
                {
                    Debug.Log("Attacked 1 or 4");
                    int random = Random.Range(0, 2);
                    if (random == 0)
                    {
                        // rival.animator.SetTrigger("attack1Trigger");
                        rival.attack1CooldownRemaining = rival.attack1Cooldown;
                    }
                    else
                    {
                        // rival.animator.SetTrigger("attack4Trigger");
                        rival.attack4CooldownRemaining = rival.attack4Cooldown;
                    }
                }
                else if (rival.attack2CooldownRemaining <= 0 || rival.attack3CooldownRemaining <= 0)
                {
                    Debug.Log("Run towards player");
                    ChangeState(currentStateEnum.runTowards);
                }
                else if (inSafeDistance)
                {
                    Debug.Log("Safe Distance");
                    ChangeState(currentStateEnum.stay);
                }
                else
                {
                    Debug.Log("Run away player");
                    ChangeState(currentStateEnum.runAway);
                }
            }
        }

        UpdateCooldowns();
    }

    public override void FixedUpdateState()
    {
        UpdateRb();
    }

    public override void ExitState()
    {
        Debug.Log("Rival is in phase one");
        RivalAnimationEventsManager.OnEndAttack2Event -= () => { ChangeState(currentStateEnum.stay); };
        RivalAnimationEventsManager.OnEndAttack3Event -= () => { ChangeState(currentStateEnum.stay); };
    }

    public enum currentStateEnum
    {
        runAway,
        stay,
        runTowards,
        attack
    }

    private void ChangeState(currentStateEnum newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case currentStateEnum.runAway:
                rival.animator.SetBool("isMoving", true);
                break;
            case currentStateEnum.stay:
                rival.animator.SetBool("isMoving", false);
                break;
            case currentStateEnum.runTowards:
                rival.animator.SetBool("isMoving", true);
                break;
            case currentStateEnum.attack:
                rival.animator.SetBool("isMoving", false);
                break;
        }
    
    }

    private void UpdateAnimator()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - rival.transform.position.x);
    
        if (currentState.Equals(currentStateEnum.runAway))
        {
            rival.transform.localScale = new Vector3(-signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);
        }
        else if (currentState.Equals(currentStateEnum.runTowards))
        {
            rival.transform.localScale = new Vector3(signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);
        }
        else if (currentState.Equals(currentStateEnum.stay))
        {
            rival.transform.localScale = new Vector3(signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);            
        }
        else if (currentState.Equals(currentStateEnum.attack))
        {
            rival.transform.localScale = new Vector3(signRivalToPlayer * Mathf.Abs(rival.transform.localScale.x), rival.transform.localScale.y, rival.transform.localScale.z);
        }

    }

    private void UpdateRb()
    {
        signRivalToPlayer = Mathf.Sign(player.transform.position.x - rival.transform.position.x);
    
        if (currentState.Equals(currentStateEnum.runAway))
        {
            rival.rb.velocity = new Vector2(-signRivalToPlayer * rival.speed, rival.rb.velocity.y);
        }
        else if (currentState.Equals(currentStateEnum.runTowards))
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
    }

    void ResetCooldown(float cooldownRemaining, float cooldown)
    {
            cooldownRemaining = cooldown;
    }
}
