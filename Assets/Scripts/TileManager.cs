using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[] obstaclePrefabs;
    public GameObject iceCreamPrefab;
    public Transform playerTransform;

    private float zSpawn = 0;
    private float tileLength = 30;
    private int numberOfTiles = 6;
    private List<GameObject> activeTiles = new List<GameObject>();
    

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
{
    if (playerTransform.position.z - 30 > zSpawn - (numberOfTiles * tileLength))
    {
        SpawnTile();
        DeleteTile();
    }
}

    public void SpawnTile()
    {
        GameObject go = Instantiate(tilePrefab, transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);
        zSpawn += tileLength;
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}