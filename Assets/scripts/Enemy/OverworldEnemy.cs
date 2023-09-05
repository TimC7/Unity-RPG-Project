using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldEnemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public float knockBackForce, knockBackCounter, knockBackTotalTime;
    public SpriteRenderer spriteRenderer;

    public int health = 3;
    public int maxHealth = 3;
    public int str = 1;
    public int expReward = 10;

    public GameObject Coin;

    public GameObject player;
    public float alertRange = 7f;
    public float attackRange = .5f;
    public enum State { Idle, Move, Chase, Attack, Pain, Frozen };
    public State currentState;

    public Vector2 Kdirection;
    public Vector2 collisionDirection;

    public bool canMove = true, patrolling = true;

    public Vector3 direction;
    private int rando;
    public float speed = 1f;
    public float timeBetweenChoices = 2f;
    protected float lastChoice = 3f;
    public float timeBetweenDirectionChanges = 2f;
    protected float lastChange = 3f;

    protected float lastXDirection = 1;
    protected float lastYDirection = 1;
    
    public delegate void EnemyDefeatedEventHandler();

    public event EnemyDefeatedEventHandler OnEnemyDefeated;

    protected virtual void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        knockBackForce = 10;
        knockBackTotalTime = .1f;

        player = GameObject.FindWithTag("Player");

        randomDirection();
        direction.Normalize();
        //currentState = State.Move;
    }

    void Update()
    {
        stateDecision();
        stateMachine();
    }

    protected virtual void stateDecision()
    {
        if (canMove)
        {
            if (knockBackCounter > 0)
            {
                currentState = State.Pain;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) < alertRange)
            {
                currentState = State.Chase;
            }
            else if (Time.time - lastChoice > timeBetweenChoices && patrolling)
            {
                idleOrMove();
                lastChoice = Time.time;
            }
        }
        else
        {
            currentState = State.Frozen;
        }
    }

    protected virtual void stateMachine()
    {
        //Debug.Log("currentState is: " + currentState);
        switch (currentState)
        {
            case State.Idle:
                idle();
                break;
            case State.Move:
                move();
                break;
            case State.Chase:
                chase();
                break;
            case State.Attack:
                attack();
                break;
            case State.Pain:
                pain();
                break;
            case State.Frozen:
                frozen();
                break;
        }
    }

    protected virtual void idle()
    {
        animator.SetTrigger("Idle");
    }

    protected virtual void move()
    {
        //Debug.Log("We movin");
        animator.SetTrigger("Moving");
        if (Time.time - lastChange > timeBetweenDirectionChanges)
        {
            randomDirection();
            direction.Normalize();
            lastChange = Time.time;
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    protected virtual void chase()
    {
        animator.SetTrigger("Moving");

        direction = player.transform.position - transform.position;
        direction.Normalize();

        lastXDirection = direction.x;
        lastYDirection = direction.y;
        animateDirection();

        rb.velocity = direction * speed;
    }

    protected virtual void attack()
    {
        Debug.Log(gameObject + " is attacking.");
    }

    protected virtual void pain()
    {
        rb.velocity = Kdirection * knockBackForce;
        knockBackCounter -= Time.deltaTime;
    }

    protected virtual void frozen()
    {
        rb.velocity = Vector2.zero;
    }

    protected void idleOrMove()
    {
        //Debug.Log("in idleOrMove()");
        rando = Random.Range(1, 3);
        switch (rando)
        {
            case 1:
                currentState = State.Idle;
                break;
            case 2:
                currentState = State.Move;
                break;
        }
    }

    public virtual void takeDamage(int damage) 
    {
        health -= damage;
        spriteRenderer.color = Color.red;
        StartCoroutine(ResetColorAfterDelay(0.5f));
        if (health <= 0 && gameObject.CompareTag("Enemy"))
        {
            OnEnemyDefeated?.Invoke();
            canMove = false;
            rb.velocity = Vector2.zero;
            Instantiate(Coin, transform.position, Quaternion.identity);
            player.GetComponent<PlayerMovement>().addExp(expReward);
            gameObject.tag = "Untagged";
            animator.SetTrigger("Death"); //disabled object called from animation
        }
    }
    public void randomDirection()
    {
        rando = Random.Range(1, 5);
        switch (rando)
        {
            case 1:
                direction = new Vector3(0f, 1f, 0f);
                lastYDirection = 1;
                break;
            case 2:
                direction = new Vector3(1f, 0f, 0f);
                lastXDirection = 1;
                break;
            case 3:
                direction = new Vector3(0f, -1f, 0f);
                lastYDirection = -1;
                break;
            case 4:
                direction = new Vector3(-1f, 0f, 0f);
                lastXDirection = -1;
                break;
        }
        animateDirection();
    }

    public void disableObject()
    {
        Debug.Log("Disable object called.");
        gameObject.SetActive(false);
    }

    protected virtual void animateDirection()
    {
        animator.SetFloat("XDirection", lastXDirection);
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.color = Color.white; // Set it back to the original color
    }

    public void canMoveTrue()
    {
        canMove = true;
    }

    public void canMoveFalse()
    {
        canMove = false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            direction = -direction; 
        }
        if (col.gameObject.CompareTag("Player"))
        {
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player Attack"))
        {
            //Debug.Log("I got attacked!");
            takeDamage(player.GetComponent<PlayerMovement>().str);

            Vector2 collisionDirection = col.transform.position - transform.position;
                
            collisionDirection.Normalize();

            knockBackCounter = knockBackTotalTime;

            float dotUp = Vector2.Dot(collisionDirection, Vector2.up);
            float dotDown = Vector2.Dot(collisionDirection, Vector2.down);
            float dotLeft = Vector2.Dot(collisionDirection, Vector2.left);
            float dotRight = Vector2.Dot(collisionDirection, Vector2.right);

            // Set a threshold to determine the collision direction
            float angleThreshold = 0.5f; // Adjust this value based on your requirements

            // Check the dot products against the threshold to determine the collision direction
            if (dotUp >= angleThreshold)
            {
                Kdirection = Vector2.down;
                //Debug.Log("Trigger enter from up direction.");
            }
            else if (dotDown >= angleThreshold)
            {
                Kdirection = Vector2.up;
                //Debug.Log("Trigger enter from down direction.");
            }
            else if (dotLeft >= angleThreshold)
            {
                Kdirection = Vector2.right;
                //Debug.Log("Trigger enter from left direction.");
            }
            else if (dotRight >= angleThreshold)
            {
                Kdirection = Vector2.left;
                //Debug.Log(Kdirection);
            }
            else
            {
                //Debug.Log("Trigger enter from a non-perfect direction.");
            }
        }
    }
}