using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> positivePrefabs;
    public List<GameObject> negativePrefabs;
    public int initialPositiveCount = 3;
    public int initialNegativeCount = 3;
    public Vector2 spawnAreaSize;
    public Transform ground;

    private int currentPositiveCount;
    private int currentNegativeCount;

    private List<GameObject> positiveObjects = new List<GameObject>(); // Tracking positive objects
    private int stepCounter = 0; // Step counter
    private int cleanStepInterval = 50; // Clean tracking list every 50 steps

    private void Start()
    {
        //Debug.Log("Start called");
        currentPositiveCount = initialPositiveCount;
        currentNegativeCount = initialNegativeCount;
        SpawnObjects();
    }

    public void ResetEnvironment()
    {
        ClearObjects();
        stepCounter = 0;
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        // Spawning positive objects
        for (int i = 0; i < currentPositiveCount; i++)
        {
            if (positivePrefabs.Count > 0)
            {
                GameObject spawnedObject = SpawnObject(positivePrefabs[Random.Range(0, positivePrefabs.Count)]);
                positiveObjects.Add(spawnedObject); // Adding the positive object to the tracking list
            }
        }

        // Spawning negative objects
        for (int i = 0; i < currentNegativeCount; i++)
        {
            if (negativePrefabs.Count > 0)
            {
                SpawnObject(negativePrefabs[Random.Range(0, negativePrefabs.Count)]);
            }
        }
    }

    private GameObject SpawnObject(GameObject prefab)
    {
        Vector3 spawnPosition;

        do
        {
            spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                ground.position.y + 0.01f, // Added 0.01 to not overlap with the floor
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            );

        } while (Physics.CheckBox(spawnPosition, new Vector3(0.25f, 0.0f, 0.25f)));

        return Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
    }

    private void ClearObjects()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        positiveObjects.Clear();
    }

    public void IncreaseObjectCounts()
    {
        currentPositiveCount++;
        currentNegativeCount++;
    }

    public bool AllPositiveObjectsCollected()
    {
        stepCounter++; // Updating step counter

        // Updating the list only every cleanStepInterval steps
        if (stepCounter % cleanStepInterval == 0)
        {
            positiveObjects.RemoveAll(obj => obj == null); // Cleaning up destroyed objects
        }

        // Checking if all positive objects have been collected
        return positiveObjects.Count == 0;
    }
}
