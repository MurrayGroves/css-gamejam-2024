using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeartBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerController playerController;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts() {
        int heartsTomake = (int)(playerController.maxHealth / 100);
        for (int i = 0; i < heartsTomake; i++)
        {
            CreateEmptyHeart();
        }
    }

    public void Update()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            float ratio = ((float)i) / hearts.Count;
            float halfRatio = (i + 0.5f) / hearts.Count;

            if (playerController.health < ratio*playerController.maxHealth)
            {
                hearts[i].SetHeartImage(HeartStatus.Empty);
            } else if (playerController.health < halfRatio*playerController.maxHealth)
            {
                hearts[i].SetHeartImage(HeartStatus.Half);
            } else
            {
                hearts[i].SetHeartImage(HeartStatus.Full);
            }
        }
    }
    
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }
        
    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
