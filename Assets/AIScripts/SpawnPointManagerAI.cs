using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Spawning;
using UnityEngine;

public class SpawnPointManagerAI : MonoBehaviour
{
    public int numberOfEdges;
    public GameObject spawnPointPrefab;
    private PlayerSpawner spawner;
    [SerializeField] private GameObject spawnPoints;
    void Start()
    {
        spawner = GetComponent<PlayerSpawner>();
        spawner.Spawns = new Transform[numberOfEdges];
        // Example usage
        float radius = 10f;
        Vector3 origin = Vector3.zero;

        Transform[] corners = CalculatePolygonCorners(origin, numberOfEdges, radius);

        
        for (int i = 0; i < numberOfEdges; i++)
        {
            spawner.Spawns[i] = corners[i];
        }
    }

    Transform[] CalculatePolygonCorners(Vector3 origin, int numberOfEdges, float radius)
    {
        Transform[] corners = new Transform[numberOfEdges];

        float angleIncrement = 360f / numberOfEdges;
        for (int i = 0; i < numberOfEdges; i++)
        {
            float angle = i * angleIncrement;
            float x = origin.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = origin.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            GameObject cornerObject = new GameObject("SpawnPoint_" + i);
            cornerObject.transform.position = new Vector3(x, 0f, z);
            corners[i] = cornerObject.transform;
            cornerObject.transform.SetParent(spawnPoints.transform);
        }

        return corners;
    }
}
