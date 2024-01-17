using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableSaving = false;
    [SerializeField] private bool initializeIfNull = false;

    [Header("Save")]
    [SerializeField] private string folderName;
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData _gameData;
    private List<ISaveable> saveables;
    private FileDataHandler _dataHandler;
    private string selectedProfileID = "";

    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("I got you");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _dataHandler = new FileDataHandler
            (Application.persistentDataPath, fileName, useEncryption);
        InitializeSelectedProfileID();
    }

    private void Start()
    {
        ChangeSelectedProfileID(folderName);
        NewGame();
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
        saveables = FindAllSaveables();
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        selectedProfileID = newProfileID;
        LoadGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
        Debug.Log("new game");
    }

    public void DeleteProfileData(string profileID)
    {
        _dataHandler.Delete(profileID);
        InitializeSelectedProfileID();
        LoadGame();
    }

    private void InitializeSelectedProfileID()
    {
        selectedProfileID = _dataHandler.GetRecentlyUpdatedProfileID();
    }

    private void LoadGame()
    {
        if (disableSaving)
        {
            return;
        }

        _gameData = _dataHandler.Load(selectedProfileID);

        if (_gameData == null && initializeIfNull)
        {
            NewGame();
        }

        if (_gameData == null)
        {
            Debug.Log("no data load");
            return;
        }

        foreach (ISaveable saveable in saveables)
        {
            saveable.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        if (disableSaving)
        {
            return;
        }

        if (_gameData == null)
        {
            Debug.Log("no data save");
            return;
        }

        foreach (ISaveable saveable in saveables)
        {
            saveable.SaveData(_gameData);
        }

        _gameData.lastUpdated = System.DateTime.Now.ToBinary();

        _dataHandler.Save(_gameData, selectedProfileID);
    }

    private void OnApplicationQuit() => SaveGame();

    private List<ISaveable> FindAllSaveables()
    {
        IEnumerable<ISaveable> saveables = 
            FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();

        return new List<ISaveable>(saveables);
    }

    public bool HasGameData()
    {
        return _gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesData()
    {
        return _dataHandler.LoadAllProfiles();
    }
}
