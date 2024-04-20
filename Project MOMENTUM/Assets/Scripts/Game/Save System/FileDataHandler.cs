using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionKeyWord = "momentum";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID)
    {
        if (profileID == null)
        {
            return null;
        }

        string fullPath = Path.Combine(_dataDirPath, profileID, _dataFileName);

        GameData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try
            {
                string dataToLoad = "";

                using FileStream stream = new FileStream(fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                dataToLoad = reader.ReadToEnd();

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError
                    ("Error when trying to load data to file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        if (profileID == null)
        {
            return;
        }

        string fullPath = Path.Combine(_dataDirPath, profileID, _dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);

            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError
                ("Error when trying to save data to file: " +  fullPath + "\n" + e);
        }
    }

    public void Delete(string  profileID)
    {
        if (profileID == null)
        {
            return;
        }

        string fullPath = Path.Combine(_dataDirPath, profileID, _dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                Directory.Delete(Path.GetDirectoryName(_dataDirPath), true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete profile data for profileID: " + 
                profileID + " at path " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;
            string fullPath = Path.Combine(_dataDirPath, profileID, _dataFileName);
            if (!File.Exists(fullPath))
                continue;

            GameData profileData = Load(profileID);

            if (profileData != null)
            {
                profileDictionary.Add(profileID, profileData);
            }
        }

        return profileDictionary;
    }

    public string GetRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach(KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileID = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null) 
            { 
                continue;
            }

            if (mostRecentProfileID == null)
            {
                mostRecentProfileID = profileID;
            }
            else
            {
                DateTime da = DateTime.FromBinary(profilesGameData[mostRecentProfileID].lastUpdated);
                DateTime newDa = DateTime.FromBinary(gameData.lastUpdated);

                if (newDa > da)
                {
                    mostRecentProfileID = profileID;
                }
            }
        }
        return mostRecentProfileID;
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionKeyWord[i % encryptionKeyWord.Length]);
        }
        return modifiedData;
    }
}
