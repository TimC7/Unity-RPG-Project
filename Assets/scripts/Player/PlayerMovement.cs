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
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    GameManager gm;

   // public OverworldEnemy goop;
   // public Vector3 jumpBackDirection = new Vector3(-1f, 0f, 0f);

    public bool isBattle = false;
    public bool isAttacking = false;
    private Collider2D hitBox;
    public Vector2 hitBoxSize = new Vector3(.5f, .5f), hitBoxLocation;

    public int level = 1;
    public int currentHealth = 3;
    public int maxHealth = 3;
    public int str = 1;
    

    public int exp;
    public int expThreshold = 100;
    public int healthIncrease = 5;
    //public int strIncrease = 1;

    public int gold;

    public HealthBar healthBar;
    public TextMeshProUGUI levelDisplay;
    public TextMeshProUGUI strengthDisplay;

    private float moveHorizontal, moveVertical;
    Vector2 currentVelocity;

    //private int currentTurn = 0;

    float invincibilityDuration = 1.0f;
    bool isInvincible = false;
    Color spriteColor;

    ContactPoint2D contact;
    Vector3 collisionDirection;
    public float knockBackForce = 10, knockBackCounter, knockBackTotalTime;
    

    private void Start()
    { 

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // Probably need to change these for scene transitions
        currentHealth = maxHealth;

        healthBar = GameObject.Find("Health Bar").GetComponent<HealthBar>();
        levelDisplay = GameObject.Find("Level Display").GetComponent<TextMeshProUGUI>();
        strengthDisplay = GameObject.Find("Strength Display").GetComponent<TextMeshProUGUI>();

        healthBar.SetHealth(currentHealth);

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else Debug.Log("healthBar not found.");
        invincibilityDuration = .2f;
        knockBackForce = 10;
        knockBackTotalTime = .1f;
        setLevel();
        setStrength();
    }

    private void OnEnable()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    }

    private void Update()
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
        //switchControl();
        animate();
    }

    void FixedUpdate()
    {
        if (!isAttacking && knockBackCounter <= 0)
        {
            rb.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
            //Debug.Log(rb.velocity);
        }
        else if (isAttacking)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = collisionDirection * knockBackForce;
            knockBackCounter -= Time.deltaTime;
        }
    }


    public void attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }
    }

    /*
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
    */

    public void takeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;

            healthBar.SetHealth(currentHealth);
            if (currentHealth <= 0)
                gameOver();
            else
                StartCoroutine(invincibilityTimer());
        }
    }

    public void gameOver()
    {
        gm.gameOver();
        gameObject.SetActive(false);
    }

    IEnumerator invincibilityTimer()
    {
        isInvincible = true;
        spriteColor = sr.color;
        spriteColor.a = 0.5f;
        sr.color = spriteColor;

        yield return new WaitForSeconds(invincibilityDuration);

        spriteColor.a = 1f;
        sr.color = spriteColor;
        isInvincible = false;
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
        levelDisplay.text = "Level: " + level.ToString();

    }

    public void setStrength()
    {
        strengthDisplay.text = "Strength: " + str.ToString();
    }

    public void animate()
    {
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
        //Debug.Log("Player collided with something");
        if (col.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Player collided with enemy.");
            contact = col.contacts[0];
            collisionDirection = contact.normal;
            knockBackCounter = knockBackTotalTime;
            if (col.gameObject.GetComponent<OverworldEnemy>() != null)
            {
                takeDamage(col.gameObject.GetComponent<OverworldEnemy>().str);
            }
            else
            { Debug.Log("Enemy does not have OverworldEnemy script"); }
        }
    }
}