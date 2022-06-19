using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class SavedGameState
{
    public int Version = 1;

    public class SimpleSpawnerState
    {
        public class Entry
        {
            public PrimitiveType Type;
            public System.Tuple<float, float, float> Location;
        }

        public string ID;
        public List<Entry> SpawnedObjects = new List<Entry>();
    }

    public SimpleSpawnerState SpawnerState = new SimpleSpawnerState();
}

public enum ESaveSlot
{
    None,

    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5
}

public enum ESaveType
{
    Manual,
    Automatic
}

public interface ISaveable
{
    void PrepareForSave(SavedGameState gameState);
}

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] float AutoSaveInterval = 300f;

    public static SaveLoadManager Instance { get; private set; } = null;

    public SavedGameState SavedState { get; private set; } = null;
    ESaveSlot CurrentSlot  = ESaveSlot.None;

    List<ISaveable> SaveHandlers = new List<ISaveable>();
    float TimeUntilNextAutosave = 0f;

    bool GameInProgress = false;

    public bool HasSavedGames
    {
        get
        {
            // check if any slot has a valid save
            var allSlots = System.Enum.GetValues(typeof(ESaveSlot));
            foreach(var slot in allSlots)
            {
                var slotEnum = (ESaveSlot)slot;

                if (slotEnum == ESaveSlot.None)
                    continue;

                if (DoesSaveExist(slotEnum, ESaveType.Manual))
                    return true;
                if (DoesSaveExist(slotEnum, ESaveType.Automatic))
                    return true;
            }

            return false;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadPersistentLevel()
    {
        // check if persistent level is already present
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            if (SceneManager.GetSceneAt(sceneIndex).name == "SaveLoadPersistentLevel")
                return;
        }

        // no persistent level present - load it name
        SceneManager.LoadScene("SaveLoadPersistentLevel", LoadSceneMode.Additive);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found a duplicate SaveLoadManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGameInProgress(bool newValue)
    {
        GameInProgress = newValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (SavedState != null && GameInProgress)
        {
            TimeUntilNextAutosave -= Time.deltaTime;

            // time to autosave?
            if (TimeUntilNextAutosave <= 0)
            {
                TimeUntilNextAutosave = AutoSaveInterval;

                RequestSave(CurrentSlot, ESaveType.Automatic);
            }
        }
    }

    public void RegisterHandler(ISaveable handler)
    {
        if (!SaveHandlers.Contains(handler))
            SaveHandlers.Add(handler);
    }

    public void DeregisterHandler(ISaveable handler)
    {
        SaveHandlers.Remove(handler);
    }

    public string GetLastSavedTime(ESaveSlot slot, ESaveType saveType)
    {
        var lastSavedTime = File.GetLastWriteTime(GetSaveFilePath(slot, saveType));

        return $"{lastSavedTime.ToLongDateString()} @ {lastSavedTime.ToLongTimeString()}";
    }

    string GetSaveFilePath(ESaveSlot slot, ESaveType saveType)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveFile_Slot{(int)slot}_{saveType.ToString()}.json");
    }

    public void RequestSave(ESaveSlot slot, ESaveType saveType)
    {
        SavedGameState savedState = new SavedGameState();

        // populate the saved state
        foreach(var handler in SaveHandlers)
        {
            if (handler == null)
                continue;

            handler.PrepareForSave(savedState);
        }

        var filePath = GetSaveFilePath(slot, saveType);

        File.WriteAllText(filePath, JsonConvert.SerializeObject(savedState, Formatting.Indented));
    }

    public bool DoesSaveExist(ESaveSlot slot, ESaveType saveType)
    {
        return File.Exists(GetSaveFilePath(slot, saveType));
    }

    public void RequestLoad(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);

        CurrentSlot = slot;
        SavedState = JsonConvert.DeserializeObject<SavedGameState>(File.ReadAllText(filePath));

        TimeUntilNextAutosave = AutoSaveInterval;
    }

    public void ClearSave()
    {
        SavedState = null;
        CurrentSlot = ESaveSlot.None;
    }
}
