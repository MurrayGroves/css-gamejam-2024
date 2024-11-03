using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public Vector2 direction;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = transform.position + new Vector3(direction.x, direction.y, 0) * (speed * Time.deltaTime);
        transform.position = newPos;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
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
