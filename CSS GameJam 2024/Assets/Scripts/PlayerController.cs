using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;
    public GameObject weapon;

    public float sapSpeed = 10;
    public float sapperHealth = 100;
    public int money = 0;

    public int maxHealthBase = 500;
    public int maxHealth;
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    
    public int bulletDamageBase = 10;
    public int bulletDamage = 10;
    public int BulletDamage
    {
        get => bulletDamage;
        set => bulletDamage = value;
    }
    
    public int bulletForceBase = 10;
    public int bulletForce = 10;
    public int BulletForce
    {
        get => bulletForce;
        set => bulletForce = value;
    }
    
    public int pushDamageBase = 1;
    public int pushDamage = 1;
    public int PushDamage
    {
        get => pushDamage;
        set => pushDamage = value;
    }
    
    public int pushForceBase = 1000;
    public int pushForce = 1000;
    public int PushForce
    {
        get => pushForce;
        set => pushForce = value;
    }
    
    public int speedBase = 20000;
    public int speed = 20000;
    public int Speed
    {
        get => speed;
        set => speed = value;
    }
    
    public int healthRegenBase = 1;
    public int healthRegen = 1;
    public int HealthRegen
    {
        get => healthRegen;
        set => healthRegen = value;
    }
    
    public int bulletWidthBase = 1;
    public int bulletWidth = 1;
    public int BulletWidth
    {
        get => bulletWidth;
        set => bulletWidth = value;
    }
    
    public Dictionary<UpgradeType, int> Upgrades = new()
    {
        {UpgradeType.MaxHealth, 0},
        {UpgradeType.BulletDamage, 0},
        {UpgradeType.BulletForce, 0},
        {UpgradeType.PushDamage, 0},
        {UpgradeType.PushForce, 0},
        {UpgradeType.Speed, 0},
        {UpgradeType.HealthRegen, 0},
        {UpgradeType.BulletWidth, 0}
    };

    public Sprite lookUp;
    public Sprite lookUpRight;
    public Sprite lookRight;
    public Sprite lookDownRight;
    public Sprite lookDown;
    
    public int health;
    private Laser _laser;
    private Vector2 _movement;
    private Vector2 _lookDir;
    private SpriteRenderer _sr;
    private DungeonGenerator _dungeonGenerator;
    private Dictionary<GameObject, UpgradeShop> _upgradeShops;
    private bool _buyCooldown = false;
    
    private void Start()
    {
        health = MaxHealth;
        List<GameObject> shopObjs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Shop"));
        _upgradeShops = new Dictionary<GameObject, UpgradeShop>();
        foreach (GameObject shopObj in shopObjs)
        {
            _upgradeShops.Add(shopObj, shopObj.GetComponent<UpgradeShop>());
        }
    }
    
    private Tuple<GameObject, UpgradeShop, float> GetNearestShop(Vector2 position)
    {
        Tuple<GameObject, UpgradeShop, float> nearestShop = null;
        foreach (KeyValuePair<GameObject, UpgradeShop> shop in _upgradeShops)
        {
            float distance = Vector2.Distance(position, shop.Key.transform.position);
            if (nearestShop == null || distance < nearestShop.Item3)
            {
                nearestShop = new Tuple<GameObject, UpgradeShop, float>(shop.Key, shop.Value, distance);
            }
        }
        return nearestShop;
    }
    
    private void ResetBuyCooldown()
    {
        _buyCooldown = false;
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
                nearestSpawner.Item1.StartSapping(sapSpeed, sapperHealth);
            }
        }
        
        if (Keyboard.current.eKey.wasPressedThisFrame && !_buyCooldown)
        {
            Tuple<GameObject, UpgradeShop, float> nearestShop = GetNearestShop(transform.position);
            if (nearestShop.Item3 < 2)
            {
                nearestShop.Item2.Buy();
                _buyCooldown = true;
                Invoke(nameof(ResetBuyCooldown), 0.5f);
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
        
        rb.AddForce(_movement * (Speed * Time.fixedDeltaTime));
        cam.transform.position = new Vector3(rb.position.x, rb.position.y, -10);
        
        float angle = Mathf.Atan2(_lookDir.y, _lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        
        if (health < MaxHealth)
        {
            health += (int) (HealthRegen * Time.fixedDeltaTime);
        }
    }
    
    public void DealDamage(int damage)
    {
        health -= damage;
        _sr.color = new Color(1, 0, 0, 1);
        
        if (health <= 0)
        {
            _dungeonGenerator.GenerateDungeon();
        }
    }

    public void Reset()
    {
        health = MaxHealth;
        rb.position = new Vector2(0, -10);
    }
    
    public void GrantMoney(int amount)
    {
        money += amount;
    }
}
