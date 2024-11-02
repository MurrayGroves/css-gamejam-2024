using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D playerRB;
    private float moveSpeed = 10f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        playerRB = player.GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 lookDir = playerRB.position - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        rb.AddForce(lookDir.normalized * (moveSpeed * Time.fixedDeltaTime));
        sr.color = Color.white;
    }

    public void Push(Vector2 direction, int force)
    {
        rb.AddForce(direction.normalized * (force * Time.fixedDeltaTime));
        sr.color = Color.red;
    }
}
