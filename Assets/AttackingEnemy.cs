using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingEnemy : OverworldEnemy
{
    protected override void animateDirection()
    {
        //base.animateDirection();
        animator.SetFloat("XDirection", lastXDirection);
        animator.SetFloat("YDirection", lastYDirection);
    }
}
