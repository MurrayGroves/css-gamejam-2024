using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public int maxAlive;
    public int spawnGroupSize;
    public int maxSpawn;
    // Seconds between each spawn
    public float spawnInterval;
    public bool beingSapped = false;
    public int maxHealth = 200;
    
    private List<GameObject> _enemies = new();
    private int _spawnedCount = 0;
    private ParticleSystem _particleSystem;
    private float _health;
    private float _sapSpeed;
    private float _sapperHealth;
    private EnemyManager _enemyManagerGlobal;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();
        _health = maxHealth;
        _enemyManagerGlobal = GameObject.Find("GameMaster").GetComponent<EnemyManager>();
    }
    
    private void Spawn()
    {
        for (int i = 0; i < spawnGroupSize; i++)
        {
            if (_enemies.Count >= maxAlive)
            {
                return;
            }
            if (_spawnedCount >= maxSpawn)
            {
                return;
            }
            _spawnedCount++;
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector2 offset = Random.insideUnitCircle;
            GameObject enemy = Instantiate(prefab, transform.position + new Vector3(offset.x, offset.y, 0), Quaternion.identity);
            _enemies.Add(enemy);
            enemy.GetComponent<EnemyController>().spawner = this;
        }
    }
    
    public void RemoveEnemy(GameObject enemy)
    {
        _enemies.Remove(enemy);
    }

    private void FixedUpdate()
    {
        if (_sapperHealth <= 0 && beingSapped)
        {
            beingSapped = false;
            _particleSystem.Stop();
            _health = maxHealth;
            _enemyManagerGlobal.StopDefendingSpawner();
        }

        if (beingSapped)
        {
            _health -= _sapSpeed * Time.fixedDeltaTime;
            if (_health <= 0)
            {
                _enemyManagerGlobal.StopDefendingSpawner();
                Destroy(gameObject);
            }
        }
    }

    private void SpawnDefenders()
    {
        if (!beingSapped)
        {
            return;
        }
        
        const int defendersCount = 10;
        const float radius = 5;
        const float angleStep = 360f / defendersCount;
        for (var i = 0; i < 10; i++)
        {
            Vector2 offset = radius * (new Vector2(Mathf.Cos(i * angleStep), Mathf.Sin(i * angleStep)));
            GameObject defender = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position + new Vector3(offset.x, offset.y, 0), Quaternion.identity);
            _enemies.Add(defender);
            EnemyController enemyController = defender.GetComponent<EnemyController>();
            enemyController.spawner = this;
            enemyController.DefendSpawner(gameObject);
        }
        
        Invoke(nameof(SpawnDefenders), 10);
    }

    public void StartSapping(float sapSpeed, float sapperHealth)
    {
        beingSapped = true;
        CancelInvoke(nameof(Spawn));
        _particleSystem.Play();
        _sapSpeed = sapSpeed;
        _sapperHealth = sapperHealth;
        SpawnDefenders();
        _enemyManagerGlobal.DefendSpawner(gameObject);
    }
    
    public void DamageSapper(float damage)
    {
        _sapperHealth -= damage;
    }
}
