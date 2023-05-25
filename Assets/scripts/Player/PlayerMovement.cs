using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    private GameObject partner;
    [SerializeField]
    private GameObject mainCamera;

    public bool isBattle = false;
    public bool isAttacking = false;
    private Collider2D hitBox;
    public Vector2 hitBoxSize = new Vector2(.5f, .5f), hitBoxLocation;

    public int level = 1;
    public int currentHealth = 3;
    public int maxHealth = 3;
    public int str = 1;
    public HealthBar healthBar;

    public int exp;
    public int expThreshold = 100;
    public int healthIncrease = 5;
    //public int strIncrease = 1;

    public int gold;

    public TextMeshProUGUI Level;
    public TextMeshProUGUI Strength;

    private float moveHorizontal, moveVertical;
    Vector2 currentVelocity;

    private int currentTurn = 0;



    private void setIsBattle()
    {
        isBattle = gameObject.GetComponent<GameManager>().isBattle;
        Debug.Log("this is a battle");
    }

    private void setCurrentTurn()
    {
        currentTurn = gameObject.GetComponent<TurnManager>().getCurrentTurn();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Probably need to change these for scene transitions
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else Debug.Log("healthBar not found.");
        setLevel();
        setStrength();
    }

    private void OnEnable()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    }

    private void Update()
    {
        if (!isBattle)
        {
            if (!isAttacking)
            {
                moveHorizontal = Input.GetAxisRaw("Horizontal");
                moveVertical = Input.GetAxisRaw("Vertical");
            }
            else
            {
                moveHorizontal = 0f;
                moveVertical = 0f;
            }


            attack();
            switchControl();
            animate();
        }
        else
        {

        }
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }


    public void attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void switchControl()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            partner.GetComponent<PartnerFollow>().enabled = false;
            partner.GetComponent<PlayerMovement>().enabled = true;
            mainCamera.GetComponent<MainCamera>().player = partner;
            gameObject.GetComponent<PartnerFollow>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().enabled = false;
        }


    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void healByAmount(int amount)
    {
        if ((currentHealth + amount) >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void addExp(int expGains)
    {
        exp += expGains;

        if (exp >= expThreshold)
        {
            levelUp();
        }
    }

    public void levelUp()
    {
        level += 1;
        maxHealth += healthIncrease; // increase only hp, str gets increased from guy
        // str += strIncrease;

        // Get leftover exp to keep towards next level up
        exp -= expThreshold;

        setLevel();
    }

    public void strUp(int amount)
    {
        str += amount;
        setStrength();
    }

    public void setLevel()
    {
        Level.text = "Level: " + level.ToString();

    }

    public void setStrength()
    {
        Strength.text = "Strength: " + str.ToString();
    }

    public void animate()
    {
        // Animation variable code
        currentVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

        if (moveHorizontal < 0 && currentVelocity.x <= 0)
        {
            animator.SetInteger("DirectionX", -1);
        }
        else if (moveHorizontal > 0 && currentVelocity.x >= 0)
        {
            animator.SetInteger("DirectionX", 1);
        }
        else
        {
            animator.SetInteger("DirectionX", 0);
        }
        if (moveVertical < 0 && currentVelocity.y <= 0)
        {
            animator.SetInteger("DirectionY", -1);
        }
        else if (moveVertical > 0 && currentVelocity.y >= 0)
        {
            animator.SetInteger("DirectionY", 1);
        }
        else
        {
            animator.SetInteger("DirectionY", 0);
        }
    }



    public void isAttackingOn()
    {
        //Debug.Log("isAttackingOn()");
        isAttacking = true;
    }

    public void isAttackingOff()
    {
        //Debug.Log("isAttackingOff()");
        isAttacking = false;
    }

    public void refillHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Player collided with something");
        if (col.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with enemy.");
            if (col.gameObject.GetComponent<OverworldEnemy>() != null)
            {
                takeDamage(col.gameObject.GetComponent<OverworldEnemy>().str);
            }
            else
            { Debug.Log("Enemy does not have OverworldEnemy script"); }
        }
    }
}