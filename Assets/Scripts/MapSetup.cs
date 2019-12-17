using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetup : MonoBehaviour
{
    public static MapSetup instance;
    public float mapSize = 1;
    public List<GameObject> SpawnPoints = new List<GameObject>();
    public int nbSpawnPoint = 0;
    public List<BoxCollider> colides = new List<BoxCollider>();

    public Vector2 vectorNorth = new Vector2();
    public Vector2 vectorSouth = new Vector2();
    public Vector2 vectorEast = new Vector2();
    public Vector2 vectorWest = new Vector2();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        nbSpawnPoint = SpawnPoints.Count;
    }
    
}
