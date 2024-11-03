using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 1000000f;
    public Rigidbody2D rb;
    public Camera cam;
    public GameObject weapon;
    public int maxHealth = 1000;
    public float sapSpeed = 10;

    public Sprite lookUp;
    public Sprite lookUpRight;
    public Sprite lookRight;
    public Sprite lookDownRight;
    public Sprite lookDown;

    private int _health;
    private Laser _laser;
    private Vector2 _movement;
    private Vector2 _lookDir;
    private SpriteRenderer _sr;
    private DungeonGenerator _dungeonGenerator;
    
    private void Start()
    {
        _health = maxHealth;
    }
    
    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
        
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

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Tuple<Spawner, float> nearestSpawner = _dungeonGenerator.GetNearestSpawner(transform.position);
            if (nearestSpawner.Item2 < 2 && !nearestSpawner.Item1.beingSapped)
            {
                nearestSpawner.Item1.StartSapping(sapSpeed);
            }
        }

        if (_movement.magnitude > 0)
        {
            _lookDir = _movement;
        }

        switch (_lookDir.y)
        {
            case > 0 when _lookDir.x == 0:
                _sr.sprite = lookUp;
                _sr.flipX = false;
                break;
            case > 0 when _lookDir.x > 0:
                _sr.sprite = lookUpRight;
                _sr.flipX = false;
                break;
            case 0 when _lookDir.x > 0:
                _sr.sprite = lookRight;
                _sr.flipX = false;
                break;
            case < 0 when _lookDir.x > 0:
                _sr.sprite = lookDownRight;
                _sr.flipX = false;
                break;
            case < 0 when _lookDir.x == 0:
                _sr.sprite = lookDown;
                _sr.flipX = false;
                break;
            case < 0 when _lookDir.x < 0:
                _sr.sprite = lookDownRight;
                _sr.flipX = true;
                break;
            case 0 when _lookDir.x < 0:
                _sr.sprite = lookRight;
                _sr.flipX = true;
                break;
            case > 0 when _lookDir.x < 0:
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
        GameObject gameMaster = GameObject.Find("GameMaster");
        gameMaster.GetComponent<EnemyManager>().ResetEnemies();
        _dungeonGenerator = gameMaster.GetComponent<DungeonGenerator>();
    }

    private void FixedUpdate()
    {
        _sr.color = new Color(1, 1, 1, 1);
        
        rb.AddForce(_movement * (moveSpeed * Time.fixedDeltaTime));
        cam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
        
        float angle = Mathf.Atan2(_lookDir.y, _lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    public void DealDamage(int damage)
    {
        _health -= damage;
        _sr.color = new Color(1, 0, 0, 1);
        
        if (_health <= 0)
        {
            _dungeonGenerator.GenerateDungeon();
        }
    }

    public void Reset()
    {
        _health = maxHealth;
        rb.position = new Vector2(5, -7);
    }
}
