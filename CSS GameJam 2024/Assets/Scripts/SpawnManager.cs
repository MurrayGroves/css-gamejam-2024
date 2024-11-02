using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private List<GameObject> _enemies = new();
    private Camera _camera;
    
    public List<GameObject> ghostPrefabs;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnGhost", 0, 5);
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
    
    void SpawnGhost()
    {
        GameObject prefab = ghostPrefabs[Random.Range(0, ghostPrefabs.Count)];
        Bounds bounds = OrthographicBounds(_camera);
        int side = Random.Range(0, 4);
        Vector3 spawnPos = Vector2.zero;
        switch (side)
        {
            case 0:
                spawnPos.x = bounds.min.x - 3;
                spawnPos.y = Random.Range(bounds.min.y, bounds.max.y);
                break;
            case 1:
                spawnPos.x = bounds.max.x + 3;
                spawnPos.y = Random.Range(bounds.min.y, bounds.max.y);
                break;
            case 2:
                spawnPos.y = bounds.min.y - 3;
                spawnPos.x = Random.Range(bounds.min.x, bounds.max.x);
                break;
            case 3:
                spawnPos.y = bounds.max.y + 3;
                spawnPos.x = Random.Range(bounds.min.x, bounds.max.x);
                break;
        }
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
    
    public void RegisterEnemy(GameObject enemy)
    {
        _enemies.Add(enemy);
    }
    
    public void RemoveEnemy(GameObject enemy)
    {
        _enemies.Remove(enemy);
    }
}
