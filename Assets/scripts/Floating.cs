using UnityEngine;
using TMPro;

public class Floating : MonoBehaviour
{
    public float speed = 1.0f;
    public float amplitude = 0.5f; 
    private float initialY;  
    public TextMeshProUGUI youwinn;
    private void Start()
    {
        initialY = transform.position.y;
        youwinn = GameObject.Find("youwinnUI").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * amplitude + initialY;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            youwinn.text = "YOU WINN!";
        }
    }

}
