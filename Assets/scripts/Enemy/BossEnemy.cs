using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BossEnemy : AttackingEnemy
{
    public int teleportCounter, teleportThreshold = 3;
    public float teleportDistance = 8f, originalAttackRange;
    public bool canFireProjectile = false;
    public GameObject projectile;
    Quaternion rotation = Quaternion.Euler(0, 0, 0);
    Vector3 offset = Vector3.zero;
    private int idirection;
    public GameObject bossHitbox;
    public Color defaultColor;
    public delegate void BossDefeatedEventHandler();
    public event BossDefeatedEventHandler OnDarkMaxDefeated;
    
    protected override void Start()
    {
        base.Start();
        originalAttackRange = attackRange;
        defaultColor = spriteRenderer.color;
    }
    public override void takeDamage(int damage) 
    {
        health -= damage;

        spriteRenderer.color = Color.red;
        teleportCounter++;
        if (health <= 0)
        {
            OnDarkMaxDefeated?.Invoke();
            canMove = false;
            rb.velocity = Vector2.zero;
            //Instantiate(Coin, transform.position, Quaternion.identity);
            player.GetComponent<PlayerMovement>().addExp(expReward);
            gameObject.tag = "Untagged";
            animator.SetTrigger("Death"); //disabled object called from animation
        }
        else
            StartCoroutine(takeDamageBehavior(0.5f));
    }

    private IEnumerator takeDamageBehavior(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.color = defaultColor;

        if (teleportCounter >= teleportThreshold)
        {
            if (Kdirection == Vector2.left)
            {
                transform.position = player.transform.position + (Vector3.right * teleportDistance);
            }
            else if (Kdirection == Vector2.right)
            {
                transform.position = player.transform.position + (Vector3.left * teleportDistance);
            }
            else if (Kdirection == Vector2.up)
            {
                transform.position = player.transform.position + (Vector3.down * teleportDistance);
            }
            else if (Kdirection == Vector2.down)
            {
                transform.position = player.transform.position + (Vector3.up * teleportDistance);
            }

            if (teleportCounter % 2 == 0)
            {
                canFireProjectile = true;
                projectileAttack();
            }
            else
            {
                attackRange = originalAttackRange;
                canFireProjectile = false;
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            Vector3 Kdirection3D = new Vector3(Kdirection.x, Kdirection.y, 0.0f);
            transform.position = player.transform.position + (Kdirection3D * teleportDistance);
        }
    }

    public void isAttackingOff()
    {
        canMove = true;
    }

    public void isAttackingOn()
    {
        canMove = false;
    }

    public void projectileAttack()
    {
        attackRange *= 5;
    }

    public void fireProjectile()
    {
        directionToSwitch();
        switch (idirection)
        {
            case 1:
                offset = new Vector3(.1f, 0f, 0f);
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 2:
                offset = new Vector3(0f, .1f, 0f);
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 3:
                offset = new Vector3(-.1f, 0f, 0f);
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 4:
                offset = new Vector3(0f, -.1f, 0f);
                rotation = Quaternion.Euler(0, 0, -90);
                break;
        }
        Debug.Log("Dark Max fireProjectile() called.");
        if (canFireProjectile)
        {
            Instantiate(projectile, (bossHitbox.transform.position + offset), rotation);
        }
    }
    
    private void directionToSwitch()
    {
        Debug.Log("Dark Max directionToSwitch() called.");


        float angleToRight = Vector2.SignedAngle(Vector2.right, direction);
        float angleToLeft = Vector2.SignedAngle(Vector2.left, direction);
        float angleToUp = Vector2.SignedAngle(Vector2.up, direction);
        float angleToDown = Vector2.SignedAngle(Vector2.down, direction);


        if (Mathf.Abs(angleToRight) < 45f)
        {
            idirection = 2;
            Debug.Log("Choosing right");
        }
        else if (Mathf.Abs(angleToLeft) < 45f)
        {
            idirection = 4;
            Debug.Log("Choosing left");
        }
        else if (Mathf.Abs(angleToUp) < 45f)
        {
            idirection = 1;
            Debug.Log("Choosing up");
        }
        else if (Mathf.Abs(angleToDown) < 45f)
        {
            idirection = 3;
            Debug.Log("Choosing down");
        }
        else
        {
            Debug.Log("No specific cardinal direction");
        }
    }
}