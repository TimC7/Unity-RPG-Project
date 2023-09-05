using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMovement player;
    public string originTag, targetTag;
    public int damage = 2;
    public float speed = 3f, destroyDelay = 10f;

    void Start()
    {
        Debug.Log("Fireball: Yo what up");
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        damage = player.str + 2; // arbitrary increase of 2
        rb.velocity = -transform.up * speed;
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            collision.SendMessage("takeDamage", damage);
        }
        if (!collision.CompareTag("Player Attack") && !collision.CompareTag(originTag))
        {
            Debug.Log(collision.tag);
            Destroy(this.gameObject);
        }
    }
}
