using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public enum CellSide
{
    Top,
    Right,
    Bottom,
    Left
}

public class Cell
{
    public Dictionary<CellSide, bool> connectorsUsed = new()
    {
        {CellSide.Top, false},
        {CellSide.Right, false},
        {CellSide.Bottom, false},
        {CellSide.Left, false}
    };
    public GameObject room;
    public List<Spawner> spawners = new List<Spawner>();
}

public class DungeonGenerator : MonoBehaviour
{
    public GameObject lobby;
    public GameObject[] roomPrefabs;
    public int difficulty = 1;

    private List<Cell> _cells;
    private EnemyManager _enemyManager;
    
    public CellSide OppositeSide(CellSide side)
    {
        return side switch
        {
            CellSide.Top => CellSide.Bottom,
            CellSide.Right => CellSide.Left,
            CellSide.Bottom => CellSide.Top,
            CellSide.Left => CellSide.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cells = new List<Cell>();
        _enemyManager = GameObject.Find("GameMaster").GetComponent<EnemyManager>();
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        _enemyManager.ResetEnemies();
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.Reset();
        
        foreach (Cell cell in _cells)
        {
            if (cell.room != lobby)
            {
                Destroy(cell.room);
            }
        }
        _cells.Clear();
        Cell lobbyCell = new Cell();
        lobbyCell.room = lobby;
        lobbyCell.connectorsUsed[CellSide.Bottom] = true;
        lobbyCell.connectorsUsed[CellSide.Left] = true;
        lobbyCell.connectorsUsed[CellSide.Right] = true;
        _cells.Add(lobbyCell);
        
        for (int i=0; i<difficulty; i++)
        {
            List<Cell> candidates = new();
            foreach (Cell cell in _cells)
            {
                if (cell.connectorsUsed.Any(kvp => kvp.Value == false))
                {
                    candidates.Add(cell);
                }
            }
            
            Cell toJoin = candidates[Random.Range(0, candidates.Count)];
            List<CellSide> cellSides = toJoin.connectorsUsed.Where(kvp => kvp.Value == false).Select(kvp => kvp.Key).ToList();
            CellSide side = cellSides[Random.Range(0, cellSides.Count)];
            
            toJoin.connectorsUsed[side] = true;
            Cell newCell = new Cell();
            newCell.room = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
            newCell.spawners = newCell.room.GetComponentsInChildren<Spawner>().ToList();
            newCell.connectorsUsed[OppositeSide(side)] = true;
            switch (side)
            {
                case CellSide.Bottom:
                    newCell.room.transform.position = toJoin.room.transform.position + new Vector3(0, -14, 0);
                    break;
                case CellSide.Left:
                    newCell.room.transform.position = toJoin.room.transform.position + new Vector3(-14, 0, 0);
                    break;
                case CellSide.Right:
                    newCell.room.transform.position = toJoin.room.transform.position + new Vector3(14, 0, 0);
                    break;
                case CellSide.Top:
                    newCell.room.transform.position = toJoin.room.transform.position + new Vector3(0, 14, 0);
                    break;
            }
            
            _cells.Add(newCell);
        }
    }

    public Tuple<Spawner, float> GetNearestSpawner(Vector2 position)
    {
        List<Spawner> spawners = new();
        foreach (Cell cell in _cells)
        {
            spawners.AddRange(cell.spawners);
        }
        
        Spawner nearestSpawner = null;
        float nearestDistance = float.MaxValue;
        foreach (Spawner spawner in spawners)
        {
            if (spawner)
            {
                float distance = Vector2.Distance(position, spawner.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSpawner = spawner;
                }
            }
        }

        return new Tuple<Spawner, float>(nearestSpawner, nearestDistance);
    }

    private void FixedUpdate()
    {
        bool allSpawnersDestroyed = true;
        foreach (Cell cell in _cells)
        {
            foreach (Spawner spawner in cell.spawners)
            {
                if (spawner)
                {
                    allSpawnersDestroyed = false;
                    break;
                }
            }
        }
        
        if (allSpawnersDestroyed)
        {
            difficulty++;
            GenerateDungeon();
        }
    }
}
