using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public Vector2 direction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = transform.position + new Vector3(direction.x, direction.y, 0) * (speed * Time.deltaTime);
        transform.position = newPos;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("LaserBullet collided with " + other.name);
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().Damage(direction, 1, damage, true);
        }
        
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
