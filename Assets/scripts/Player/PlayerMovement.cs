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

    public bool canFireProjectile = false, canTurnInvincible = false;
    public bool isFiringProjectile = false;
    public GameObject projectile;
    public GameObject playerHitbox;
    public int direction; // for projectile
    Quaternion rotation = Quaternion.Euler(0, 0, 0);
    Vector3 offset = Vector3.zero;
    public bool isAttacking = false;
    //private Collider2D hitBox;
    //public Vector2 hitBoxSize = new Vector3(.5f, .5f), hitBoxLocation;

    public Inventory inv;
    public int level = 1;
    public int currentHealth = 3, currentMagic;
    public int maxHealth = 3, maxMagic;
    public int str = 1;
    
    public int exp;
    private int expThreshold;
    public int expIncrement = 30;
    public int healthIncrease = 1;
    public int strIncrease = 1;

    public HealthBar healthBar;
    public MagicBar magicBar;
    public TextMeshProUGUI levelDisplay;
    public TextMeshProUGUI strengthDisplay;

    private float moveHorizontal, moveVertical;
    Vector2 currentVelocity;

    public float invincibilityDuration = 1.0f;
    public bool isInvincible = false;
    Color spriteColor;

    ContactPoint2D contact;
    Vector3 collisionDirection;
    public float knockBackForce = 10, knockBackCounter, knockBackTotalTime;
    
    private void Start()
    { 

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();

        //playerHitbox = GameObject.Find("Player Attack");

        currentHealth = maxHealth;
        maxMagic = maxHealth; // Magic will be same value as health
        currentMagic = maxMagic;

        healthBar = GameObject.Find("Health Bar").GetComponent<HealthBar>();
        magicBar = GameObject.Find("Magic Bar").GetComponent<MagicBar>();
        levelDisplay = GameObject.Find("Level Display").GetComponent<TextMeshProUGUI>();
        strengthDisplay = GameObject.Find("Strength Display").GetComponent<TextMeshProUGUI>();

        healthBar.SetHealth(currentHealth);
        magicBar.SetMagic(currentMagic);

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else Debug.Log("healthBar not found.");
        if (magicBar != null)
        {
            magicBar.SetMaxMagic(maxMagic);
        }
        else Debug.Log("magicBar not found.");

        invincibilityDuration = .2f;
        knockBackForce = 10;
        knockBackTotalTime = .1f;
        setLevelDisplay();
        setStrengthDisplay();
        
        //Invoke("initializeItemEffects", .5f);
        initializeItemEffects();
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
        if (canFireProjectile && currentMagic > 0)
        {
            projectileAttack();
        }
        if (canTurnInvincible && currentMagic > 0)
        {
            tempInvincibility();
        }
        //switchControl();
        animate();
    }

    void FixedUpdate()
    {
        if (!isAttacking && knockBackCounter <= 0)
        {
            rb.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
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

    public void projectileAttack()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isFiringProjectile = true;
            animator.SetTrigger("Attack");
        }
    }

    public void tempInvincibility()
    {
        if (Input.GetButtonDown("Jump")) // Space
        {
            StartCoroutine(invincibilityTimer());
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

    public void takeMagicPoints(int value)
    {
        currentMagic -= value;

        magicBar.SetMagic(currentMagic);
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
        expThreshold = level * expIncrement; // Hardcoded increment each level
        if (exp >= expThreshold)
        {
            levelUp();
        }
    }

    public void levelUp()
    {
        level += 1;
        // Alternate between health and strength
        if (level % 2 == 0)
        {
            strUp(strIncrease);
        }
        else
        {
            healthUp(healthIncrease);
            magicUp(healthIncrease);
        }

        // Get leftover exp to keep towards next level up
        exp -= expThreshold;
        
        setLevelDisplay();
    }

    public void strUp(int amount)
    {
        str += amount;
        setStrengthDisplay();
    }

    public void healthUp(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        healthBar.SetHealth(currentHealth);
    }

    public void magicUp(int amount)
    {
        maxMagic += amount;
        currentMagic += amount;
        magicBar.SetMagic(currentMagic);
    }

    public void setLevelDisplay()
    {
        levelDisplay.text = level.ToString();
    }

    public void setStrengthDisplay()
    {
        strengthDisplay.text = str.ToString();
    }

    public void animate()
    {
        currentVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

        if (moveHorizontal < 0 && currentVelocity.x <= 0)
        {
            direction = 4;
            animator.SetInteger("DirectionX", -1);
        }
        else if (moveHorizontal > 0 && currentVelocity.x >= 0)
        {
            direction = 2;
            animator.SetInteger("DirectionX", 1);
        }
        else
        {
            animator.SetInteger("DirectionX", 0);
        }

        if (moveVertical < 0 && currentVelocity.y <= 0)
        {
            direction = 3;
            animator.SetInteger("DirectionY", -1);
        }
        else if (moveVertical > 0 && currentVelocity.y >= 0)
        {
            direction = 1;
            animator.SetInteger("DirectionY", 1);
        }
        else
        {
            animator.SetInteger("DirectionY", 0);
        }
    }

    public void isAttackingOn()
    {
        isAttacking = true;
    }

    public void isAttackingOff()
    {
        isAttacking = false;
    }

    public void refillHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    public void fireProjectile()
    {
        switch (direction)
        {
            case 1:
                offset = new Vector3(.1f, 0f, 0f);
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 2:
                offset = new Vector3(0f, -.1f, 0f);
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
        Debug.Log("fireProjectile called.");
        if (isFiringProjectile)
        {
            Debug.Log("isFiringProjectile");
            Instantiate(projectile, (playerHitbox.transform.position + offset), rotation);
            takeMagicPoints(1);
        }
        isFiringProjectile = false;
    }

    public void initializeItemEffects()
    {
        Debug.Log("Initializing inventory item effects...");
        Debug.Log("Inventory has: ");
        for (int i = 0; i < inv.inventory.Count; i++)
        {
            Debug.Log(inv.inventory[i]);
        }
        Debug.Log("What inventory needs for projectile attack: " + inv.projectileUpgrade);
        if (inv.inventory.Contains(inv.speedUpgrade))
        {
            speed *= .5f;
        }
        if (inv.inventory.Contains(inv.projectileUpgrade))
        {
            canFireProjectile = true;
            Debug.Log("Can fire projectiles: " + canFireProjectile);
        }
        if (inv.inventory.Contains(inv.InvincibilityUpgrade))
        {
            canTurnInvincible = true;
        }
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

        if (col.gameObject.CompareTag("Coin"))
        {
            CollectCoin(col.gameObject);
        }
    }

    private void CollectCoin(GameObject Coin)
    {
        // Disable the coin GameObject or remove it from the scene
        Coin.SetActive(false);
        // Add coins to the player's inventory or increase the score
    }


}