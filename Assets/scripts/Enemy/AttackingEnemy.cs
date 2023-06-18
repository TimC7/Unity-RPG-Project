using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingEnemy : OverworldEnemy
{
    public bool isAttacking = false;
    protected override void Start()
    {
        health = 5;
        str = 2;
        //attackRange = 2f;
        isAttacking = false;
        base.Start();
    }

    protected override void stateDecision()
    {
        //Debug.Log("canMove = " + canMove);
        if (canMove)
        {
            if (knockBackCounter > 0)
            {
                currentState = State.Pain;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            {
                Debug.Log("setting state to attack.");
                currentState = State.Attack;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) < alertRange)
            {
                //Debug.Log("setting state to chase.");
                currentState = State.Chase;
            }
            else if (Time.time - lastChoice > timeBetweenChoices && patrolling)
            {
                idleOrMove();
                lastChoice = Time.time;
            }
        }
        else if (knockBackCounter > 0) // Could move this to beginning
        {
            currentState = State.Pain;
        }
        else
        {
            //Debug.Log("setting state to frozen.");
            currentState = State.Frozen;
        }
    }

    protected override void animateDirection()
    {
        animator.SetFloat("XDirection", lastXDirection);
        animator.SetFloat("YDirection", lastYDirection);
    }

    protected override void attack()
    {
        Debug.Log("In attack().");
        if (canMove)
        {
            //canMove = false;
            animator.SetTrigger("Attack");
            Debug.Log("setting attack trigger");
        }
    }

    private IEnumerator isAttackingFalse()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("setting isAttacking to false");
        isAttacking = false;
    }

    /*
    protected override void frozen()
    {
        
    }
    */
}
