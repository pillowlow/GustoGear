using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints = new Transform[4]; // Array to hold the spawn points
    [SerializeField] private GameObject[] prefabs = new GameObject[4]; // Array to hold the prefabs

    // Method to generate units at specified positions
    public void genUnit()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null && prefabs[i] != null)
            {
                // Instantiate the prefab at the position and rotation of the spawn point
                Instantiate(prefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            }
            else
            {
                Debug.LogError($"Spawn point or prefab at index {i} is null.");
            }
        }
    }
}
