using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies = new();
    private bool _defendingSpawner = false;
    private GameObject _spawner;
 
    public void RegisterEnemy(GameObject enemy)
    {
        _enemies.Add(enemy);
    }
    
    public void RemoveEnemy(GameObject enemy)
    {
        _enemies.Remove(enemy);
    }

    public void ResetEnemies()
    {
        foreach (GameObject enemy in _enemies)
        {
            Destroy(enemy);
        }
        
        _enemies.Clear();
    }
    
    public void DefendSpawner(GameObject spawner)
    {
        _defendingSpawner = true;
        _spawner = spawner;
        
        foreach (GameObject enemy in _enemies)
        {
            enemy.GetComponent<EnemyController>().DefendSpawner(spawner);
        }
    }
    
    public void StopDefendingSpawner()
    {
        _defendingSpawner = false;
        _spawner = null;
        foreach (GameObject enemy in _enemies)
        {
            enemy.GetComponent<EnemyController>().StopDefendingSpawner();
        }
    }
}
