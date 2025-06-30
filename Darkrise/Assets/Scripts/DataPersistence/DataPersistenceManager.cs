using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] string fileName;
    [SerializeField] bool useEncryption;
    [Header("Debugging Settings")]
    [SerializeField] bool initializeDataIfNull = false;
    public static DataPersistenceManager Instance { get; private set; }
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's already a data persistence manager in the scene. Deleting the most recent one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        DontDestroyOnLoad(this.gameObject);
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // load a save from the file data handler

        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull) // for debugging
        {
            NewGame();
        }

        // if no data, needs to make data

        if (this.gameData == null)
        {
            Debug.Log("No data found. Start a new game first.");
            return;
        }

        // send out the loaded data to everything that needs it

        foreach(IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No data found. Start a new game first.");
            return;
        }

        // pass the data to everything that needs to update

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence.SaveData(gameData);
        }
        // write it to a file

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
