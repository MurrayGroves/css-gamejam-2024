using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private GameObject _player;
    private PlayerController _playerController;
    private Rigidbody2D _playerRb;
    private EnemyManager _enemyManagerGlobal;
    
    private float _moveSpeed = 20f;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private float _health;
    private float _alpha = 1.0f;
    private float _turnSpeed = 1f;
    private float _angleOffset = 10f;
    private bool _defendingSpawner = false;
    private GameObject _spawnerToDefend;
    private bool _dead = false;
    
    public Spawner spawner;
    public int damageToPlayer = 10;
    public int maxHealth = 100;
    public int damageToSapper = 10;
    public int moneyValue = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody2D>();
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _moveSpeed = Random.Range(0.8f*_moveSpeed, 1.2f*_moveSpeed);
        _turnSpeed = Random.Range(0.5f*_turnSpeed, 1.5f*_turnSpeed);
        _angleOffset = Random.Range(-1.0f*_angleOffset, 1.0f*_angleOffset);
        _enemyManagerGlobal = GameObject.Find("GameMaster").GetComponent<EnemyManager>();
        _enemyManagerGlobal.RegisterEnemy(gameObject);
        _playerController = _player.GetComponent<PlayerController>();
        _health = maxHealth;
        Image moneyImage = gameObject.GetComponentInChildren<Image>();
        moneyImage.enabled = false;
    }

    void FixedUpdate()
    {
        Vector2 target = _defendingSpawner ? _spawnerToDefend.transform.position : _playerRb.position;
        Vector2 lookDir = target - _rb.position;
        float angle = _angleOffset + (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);
        _rb.transform.rotation = Quaternion.Euler(0f, 0f, angle * (_turnSpeed * Time.fixedDeltaTime));
        _rb.AddForce(lookDir.normalized * (_moveSpeed * Time.fixedDeltaTime));
        SetColour(Color.white);
    }
    
    private void DecreaseHealth(int damage, bool instant)
    {
        if (_dead)
        {
            return;
        }
        
        _health -= damage * (instant ? 1 : Time.fixedDeltaTime);
        _alpha = 0.35f + (0.65f * (_health / maxHealth));
        if (_health <= 0)
        {
            _dead = true;
            _sr.enabled = false;
            _rb.mass = 1000f;
            _playerController.GrantMoney(moneyValue);
            TextMeshProUGUI moneyText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Image moneyImage = gameObject.GetComponentInChildren<Image>();
            moneyImage.enabled = true;
            moneyText.text = "+" + moneyValue;
            
            Invoke(nameof(DestroySelf), 0.5f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void SetColour(Color colour)
    {
        colour.a = _alpha;
        _sr.color = colour;
    }

    public void Damage(Vector2 direction, int force, int damage, bool instant)
    {
        if (_dead)
        {
            return;
        }
        
        _rb.AddForce(direction.normalized * (force * ( !instant ? Time.fixedDeltaTime : 1)));
        DecreaseHealth(damage, instant);
        SetColour(Color.red);
    }

    public void OnDestroy()
    {
        _enemyManagerGlobal.RemoveEnemy(gameObject);
        spawner.RemoveEnemy(gameObject);
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (_dead)
        {
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            _playerController.DealDamage(damageToPlayer);
        }

        if (other.CompareTag("Spawner"))
        {
            other.GetComponent<Spawner>().DamageSapper(damageToSapper * Time.fixedDeltaTime);
        }
    }
    
    public void DefendSpawner(GameObject spawner)
    {
        _defendingSpawner = true;
        _spawnerToDefend = spawner;
    }
    
    public void StopDefendingSpawner()
    {
        _defendingSpawner = false;
    }
}
