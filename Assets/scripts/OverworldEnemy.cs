using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldEnemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public float knockBackForce, knockBackCounter, knockBackTotalTime;// Adjust the force as needed
    public SpriteRenderer spriteRenderer;

    public int health = 3;
    public int maxHealth = 3;
    public int str = 1;

    public GameObject player;

    public enum State { Idle, Move, Chase, Pain, Frozen };
    public State currentState;

    public Vector2 Kdirection;
    public Vector2 collisionDirection;

    public bool canMove = true;

    public Vector3 direction;
    private int rando;
    public float speed = 1f;
    public float timeBetweenChoices = 2f;
    private float lastChoice = 3f;
    public float timeBetweenDirectionChanges = 2f;
    private float lastChange = 3f;

    protected float lastXDirection = 1;
    protected float lastYDirection = 1;
    

    void Start()
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
        currentState = State.Move;
    }


    // Update is called once per frame
    void Update()
    {
        stateDecision();
        stateMachine();
        /*
        if (knockBackCounter <= 0 && canMove)
        {
            chase();
        }
        else if (canMove)
        {
            rb.velocity = Kdirection * knockBackForce;
            knockBackCounter -= Time.deltaTime;
        }
        */
    }

    public void stateDecision()
    {
        if (canMove)
        {
            if (knockBackCounter > 0)
            {
                currentState = State.Pain;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) < 10f)
            {
                currentState = State.Chase;
            }
            else if (Time.time - lastChoice > timeBetweenChoices)
            {
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
                lastChoice = Time.time;
            }
        }
        else
        {
            currentState = State.Frozen;
        }
    }

    public void stateMachine()
    {
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
            case State.Pain:
                pain();
                break;
            case State.Frozen:
                frozen();
                break;
        }
    }

    public void idle()
    {
        animator.SetTrigger("Idle");
    }

    public void move()
    {
        animator.SetTrigger("Moving");
        if (Time.time - lastChange > timeBetweenDirectionChanges)
        {
            randomDirection();
            direction.Normalize();
            lastChange = Time.time;
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    public void chase()
    {
        // Calculate the direction to the player
        animator.SetTrigger("Moving");
        direction = player.transform.position - transform.position;
        direction.Normalize();
        lastXDirection = direction.x;
        lastYDirection = direction.y;
        animateDirection();
        // Move towards the player
        rb.velocity = direction * speed;
    }

    public void pain()
    {
        rb.velocity = Kdirection * knockBackForce;
        knockBackCounter -= Time.deltaTime;
    }

    public void frozen()
    {
        rb.velocity = Vector2.zero;
    }

    public void takeDamage(int damage) 
    {
        health -= damage;
        spriteRenderer.color = Color.red;
        StartCoroutine(ResetColorAfterDelay(0.5f));
        if (health <= 0 && gameObject.CompareTag("Enemy"))
        {
            canMove = false;
            rb.velocity = Vector2.zero;
            gameObject.tag = "Untagged";
            //Debug.Log(gameObject.tag);
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
        //Debug.Log("deez");
        gameObject.SetActive(false);
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.color = Color.white; // Set it back to the original color
    }

    private void OnCollisionEnter2D(Collision2D col)
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
            if (col.gameObject.CompareTag("Player Attack"))
            {

                //Debug.Log("I got attacked!");
                takeDamage(player.GetComponent<PlayerMovement>().str);

                // Get the direction from this object to the other object
                Vector2 collisionDirection = col.transform.position - transform.position;
                

                // Normalize the direction vector if needed
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
    
    protected virtual void animateDirection()
    {
        animator.SetFloat("XDirection", lastXDirection);
    }
}