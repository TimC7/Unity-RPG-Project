using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : AttackingEnemy
{
    int lastXInt, lastYInt;

    protected override void move()
    {
                //Debug.Log("We movin");
        //animator.SetTrigger("Moving");
        if (Time.time - lastChange > timeBetweenDirectionChanges)
        {
            randomDirection();
            direction.Normalize();
            lastChange = Time.time;
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void chase()
    {
        //animator.SetTrigger("Moving");

        direction = player.transform.position - transform.position;
        direction.Normalize();

        lastXDirection = direction.x;
        lastYDirection = direction.y;
        animateDirection();

        rb.velocity = direction * speed;
    }

    protected override void animateDirection()
    {
        lastXInt = (int)lastXDirection;
        lastYInt = (int)lastYDirection;
        animator.SetInteger("DirectionX", lastXInt);
        animator.SetInteger("DirectionY", lastYInt);
    }
}
