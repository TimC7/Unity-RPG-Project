using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldEnemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public int health = 3;
    public int maxHealth = 3;
    public int str = 1;

    public GameObject player;

    public enum State { Idle, Move };
    public State currentState;

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

        player = GameObject.FindWithTag("Player");

        randomDirection();
        direction.Normalize();
        currentState = State.Move;
        
    }

    // Update is called once per frame
    void Update()
    {

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

    private void FixedUpdate()
    {
        
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

    public void takeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            gameObject.SetActive(false);
        }
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
                
            }
        }
    }
    
}
