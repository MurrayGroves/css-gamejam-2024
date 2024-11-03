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
    public int maxHealth = 100;
    
    private List<GameObject> _enemies = new();
    private int spawnedCount = 0;
    private ParticleSystem _particleSystem;
    private float _health;
    private float _sapSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();
        _health = maxHealth;
    }
    
    private void Spawn()
    {
        for (int i = 0; i < spawnGroupSize; i++)
        {
            if (_enemies.Count >= maxAlive)
            {
                return;
            }
            if (spawnedCount >= maxSpawn)
            {
                return;
            }
            spawnedCount++;
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
        _health -= _sapSpeed * Time.fixedDeltaTime;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void StartSapping(float sapSpeed)
    {
        beingSapped = true;
        CancelInvoke(nameof(Spawn));
        _particleSystem.Play();
        _sapSpeed = sapSpeed;
    }
}
