using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 1000000f;
    public Rigidbody2D rb;
    public Camera cam;
    public GameObject weapon;
    public static int MaxHealth = 1000;

    public Sprite lookUp;
    public Sprite lookUpRight;
    public Sprite lookRight;
    public Sprite lookDownRight;
    public Sprite lookDown;

    private int _health = MaxHealth;
    private Laser _laser;
    private Vector2 movement;
    private Vector2 lookDir;
    private SpriteRenderer _sr;
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        if (Keyboard.current.jKey.isPressed)
        {
            _laser.ShootLaserDamage();
        } else if (Keyboard.current.jKey.wasReleasedThisFrame)
        {
            _laser.RemoveLaserDamage();
        }
        
        if (Keyboard.current.kKey.isPressed)
        {
            _laser.TriggerLaserPush();
        } else if (Keyboard.current.kKey.wasReleasedThisFrame)
        {
            _laser.RemoveLaserPush();
        }

        if (movement.magnitude > 0)
        {
            lookDir = movement;
        }

        switch (lookDir.y)
        {
            case > 0 when lookDir.x == 0:
                _sr.sprite = lookUp;
                _sr.flipX = false;
                break;
            case > 0 when lookDir.x > 0:
                _sr.sprite = lookUpRight;
                _sr.flipX = false;
                break;
            case 0 when lookDir.x > 0:
                _sr.sprite = lookRight;
                _sr.flipX = false;
                break;
            case < 0 when lookDir.x > 0:
                _sr.sprite = lookDownRight;
                _sr.flipX = false;
                break;
            case < 0 when lookDir.x == 0:
                _sr.sprite = lookDown;
                _sr.flipX = false;
                break;
            case < 0 when lookDir.x < 0:
                _sr.sprite = lookDownRight;
                _sr.flipX = true;
                break;
            case 0 when lookDir.x < 0:
                _sr.sprite = lookRight;
                _sr.flipX = true;
                break;
            case > 0 when lookDir.x < 0:
                _sr.sprite = lookUpRight;
                _sr.flipX = true;
                break;
        }
    }
    
    public void FetchLaser()
    {
        _laser = weapon.GetComponent<Laser>();
    }

    public void Awake()
    {
        FetchLaser();
        _sr = gameObject.GetComponentInChildren<SpriteRenderer>();  
    }

    private void FixedUpdate()
    {
        _sr.color = new Color(1, 1, 1, 1);
        
        rb.AddForce(movement * (moveSpeed * Time.fixedDeltaTime));
        cam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
        
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    public void DealDamage(int damage)
    {
        _health -= damage;
        _sr.color = new Color(1, 0, 0, 1);
        
        if (_health <= 0)
        {
            _health = MaxHealth;
            rb.position = new Vector2(5, -7);
            GameObject gameMaster = GameObject.Find("GameMaster");
            gameMaster.GetComponent<EnemyManager>().ResetEnemies();
            DungeonGenerator dungeonGenerator = gameMaster.GetComponent<DungeonGenerator>();
            dungeonGenerator.difficulty = 1;
            dungeonGenerator.GenerateDungeon();
        }
    }
}
