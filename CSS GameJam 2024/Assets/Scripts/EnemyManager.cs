using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies = new();
 
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
}
