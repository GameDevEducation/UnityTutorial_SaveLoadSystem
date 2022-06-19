using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour, ISaveable
{
    [SerializeField] string ID = "SimpleSpawner-1";
    [SerializeField] int NumObjects = 5;
    List<System.Tuple<GameObject, PrimitiveType>> SpawnedObjects = new List<System.Tuple<GameObject, PrimitiveType>>();

    public void PrepareForSave(SavedGameState gameState)
    {
        gameState.SpawnerState.ID = ID;

        // add an entry for each of the spawned objects
        foreach(var spawnedGOInfo in SpawnedObjects)
        {
            var location = spawnedGOInfo.Item1.transform.position;

            gameState.SpawnerState.SpawnedObjects.Add(new SavedGameState.SimpleSpawnerState.Entry()
            {
                Location = new System.Tuple<float, float, float>(location.x, location.y, location.z),
                Type = spawnedGOInfo.Item2
            });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveLoadManager.Instance.RegisterHandler(this);

        // are we loading from a save file?
        if (SaveLoadManager.Instance.SavedState != null)
        {
            var spawnerState = SaveLoadManager.Instance.SavedState.SpawnerState;

            if (spawnerState.ID == ID)
            {
                foreach(var entry in spawnerState.SpawnedObjects)
                {
                    PrimitiveType typeToSpawn = entry.Type;
                    Vector3 location = new Vector3(entry.Location.Item1, entry.Location.Item2, entry.Location.Item3);

                    SpawnObject(typeToSpawn, location);
                }

                return;
            }
        }

        var availableTypes = System.Enum.GetValues(typeof(PrimitiveType));
        for (int index = 0; index < NumObjects; index++)
        {
            // pick a random type
            int typeIndex = Random.Range(0, availableTypes.Length);
            PrimitiveType typeToSpawn = (PrimitiveType)availableTypes.GetValue(typeIndex);
            Vector3 location = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));

            SpawnObject(typeToSpawn, location);
        }
    }

    void SpawnObject(PrimitiveType typeToSpawn, Vector3 location)
    {
        var newGO = GameObject.CreatePrimitive(typeToSpawn);
        newGO.transform.position = location;

        SpawnedObjects.Add(new System.Tuple<GameObject, PrimitiveType>(newGO, typeToSpawn));
    }

    void OnDestroy()
    {
        SaveLoadManager.Instance.DeregisterHandler(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
