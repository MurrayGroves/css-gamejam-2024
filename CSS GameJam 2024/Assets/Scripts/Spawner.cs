using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<GameObject> _enemies = new();
    private int spawnedCount = 0;
    
    public List<GameObject> enemyPrefabs;
    public int maxAlive;
    public int spawnGroupSize;
    public int maxSpawn;
    // Seconds between each spawn
    public float spawnInterval;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
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
    
}
