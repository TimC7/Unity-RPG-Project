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

    public enum State { Idle, Move };
    public State currentState;

    public Vector2 Kdirection;
    public Vector2 collisionDirection;

    public Vector3 direction;
    private int rando;
    public float speed = 1f;
    public float timeBetweenChoices = 2f;
    private float lastChoice = 3f;
    public float timeBetweenDirectionChanges = 2f;
    private float lastChange = 3f;

    private int lastXDirection = 1;

    void Start()
    {
        
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
        if (knockBackCounter <= 0)
        {

            // Check if the player is within a certain distance
            if (Vector3.Distance(transform.position, player.transform.position) < 10f)
            {
                // Calculate the direction to the player
                animator.SetTrigger("Moving");
                direction = player.transform.position - transform.position;
                direction.Normalize();
                if (direction.x > 0)
                    lastXDirection = 1;
                else
                    lastXDirection = -1;
                animator.SetFloat("Direction", lastXDirection);

                // Move towards the player
                rb.velocity = direction * speed;
            }

            else
            {
                // Stop moving if the player is far away
                //rb.velocity = Vector2.zero;
                decision();
                switch (currentState)
                {
                    case State.Idle:
                        animator.SetTrigger("Idle");
                        break;
                    case State.Move:
                        animator.SetTrigger("Moving");
                        if (Time.time - lastChange > timeBetweenDirectionChanges)
                        {
                            randomDirection();
                            direction.Normalize();
                            lastChange = Time.time;
                        }
                        transform.position += direction * speed * Time.deltaTime;
                        break;
                }
            }
        }
        else
        {
            rb.velocity = Kdirection * knockBackForce;
            knockBackCounter -= Time.deltaTime;
            //Debug.Log(Kdirection);
        }
    }

    public void decision()
    {
        if (Time.time - lastChoice > timeBetweenChoices)
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

    public void randomDirection()
    {
        rando = Random.Range(1, 5);
        switch (rando)
        {
            case 1:
                direction = new Vector3(0f, 1f, 0f);
                
                break;
            case 2:
                direction = new Vector3(1f, 0f, 0f);
                lastXDirection = 1;
                break;
            case 3:
                direction = new Vector3(0f, -1f, 0f);
                break;
            case 4:
                direction = new Vector3(-1f, 0f, 0f);
                lastXDirection = -1;
                break;
        }

        animator.SetFloat("Direction", lastXDirection);
    }


    public void takeDamage(int damage) //, Vector3 jumpBackDirection posible parameter
    {
        health -= damage;
        //animator.SetTrigger("Damage");
        spriteRenderer.color = Color.red;
        StartCoroutine(ResetColorAfterDelay(0.5f)); // Adjust the duration as needed
        //rb.AddForce(new Vector2(0f, -1f) * pushBackForce, ForceMode2D.Impulse); // Adjust the push back direction as needed (still needs to be adjusted)
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
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
            Debug.Log("This frikkn guy");
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player Attack"))
        {
            if (col.gameObject.CompareTag("Player Attack"))
            {

                Debug.Log("I got attacked!");
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
                    // Trigger enter from the up direction
                    Debug.Log("Trigger enter from up direction.");
                }
                else if (dotDown >= angleThreshold)
                {
                    Kdirection = Vector2.up;
                    // Trigger enter from the down direction
                    Debug.Log("Trigger enter from down direction.");
                }
                else if (dotLeft >= angleThreshold)
                {
                    Kdirection = Vector2.right;
                    // Trigger enter from the left direction
                    Debug.Log("Trigger enter from left direction.");
                }
                else if (dotRight >= angleThreshold)
                {
                    Kdirection = Vector2.left;

                    // Trigger enter from the right direction
                    Debug.Log(Kdirection);
                }
                else
                {
                    // Collision from a non-perfect direction
                    Debug.Log("Trigger enter from a non-perfect direction.");
                }
            }
        }
    }
}
