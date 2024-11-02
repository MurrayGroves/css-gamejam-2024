using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D playerRB;
    private EnemyManager _enemyManagerGlobal;
    
    private float moveSpeed = 20f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private static int maxHealth = 100;
    private float health = maxHealth;
    private float alpha = 1.0f;
    private float turnSpeed = 1f;
    private float angleOffset = 10f;
    
    public Spawner spawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        playerRB = player.GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        moveSpeed = Random.Range(0.8f*moveSpeed, 1.2f*moveSpeed);
        turnSpeed = Random.Range(0.5f*turnSpeed, 1.5f*turnSpeed);
        angleOffset = Random.Range(-1.0f*angleOffset, 1.0f*angleOffset);
        _enemyManagerGlobal = GameObject.Find("GameMaster").GetComponent<EnemyManager>();
        _enemyManagerGlobal.RegisterEnemy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 lookDir = playerRB.position - rb.position;
        float angle = angleOffset + (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);
        rb.transform.rotation = Quaternion.Euler(0f, 0f, angle * (turnSpeed * Time.fixedDeltaTime));
        rb.AddForce(lookDir.normalized * (moveSpeed * Time.fixedDeltaTime));
        setColour(Color.white);
    }
    
    private void decreaseHealth(int damage)
    {
        health -= damage * Time.fixedDeltaTime;
        alpha = 0.35f + (0.65f * (health / maxHealth));
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void setColour(Color colour)
    {
        colour.a = alpha;
        sr.color = colour;
    }

    public void Damage(Vector2 direction, int force, int damage)
    {
        rb.AddForce(direction.normalized * (force * Time.fixedDeltaTime));
        decreaseHealth(damage);
        setColour(Color.red);
    }

    public void OnDestroy()
    {
        _enemyManagerGlobal.RemoveEnemy(gameObject);
        spawner.RemoveEnemy(gameObject);
    }
}
