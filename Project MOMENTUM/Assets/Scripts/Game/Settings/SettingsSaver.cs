using System.IO;
using UnityEngine;

public class SettingsSaver : MonoBehaviour
{
    [SerializeField] private bool IsBuild = true;
    [SerializeField] private GameSettings settings;

    private string path = "";
    private string persistentPath = "";

    public GameSettings GameSettings => settings;

    private void Awake()
    {
#if UNITY_EDITOR
        IsBuild = false;
#endif

        SetPaths();
        LoadSettings();
    }

    // Creates new file with default values
    private void CreateGameSettings() => settings = new GameSettings();
    
    private void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "config.cfg";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "config.cfg";
    }

    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
        string savePath = IsBuild ? persistentPath : path;

        string json = JsonUtility.ToJson(settings, true);

        using StreamWriter write = new StreamWriter(savePath);
        write.Write(json);
        write.Close();
    }

    [ContextMenu("Load Settings")]
    public void LoadSettings()
    {
        if (!File.Exists(IsBuild ? persistentPath : path))
        {
            CreateGameSettings();
            return;
        }

        using StreamReader reader = new StreamReader(IsBuild ? persistentPath : path);
        string json = reader.ReadToEnd();
        reader.Close();

        settings = JsonUtility.FromJson<GameSettings>(json);
    }

    [ContextMenu("Reset Settings")]
    public void ResetSettings() => CreateGameSettings();

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
