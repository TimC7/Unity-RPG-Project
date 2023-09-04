using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : AttackingEnemy
{
    public void isAttackingOff()
    {
        canMove = true;
    }

    public void isAttackingOn()
    {
        canMove = false;
    }
}