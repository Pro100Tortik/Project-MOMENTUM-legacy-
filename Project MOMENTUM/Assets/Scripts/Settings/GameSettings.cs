using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public int language = 0;
    [Range(0.1f, 10.0f)] public float sensitivity = 1.5f;
    [Range(-80.0f, 10.0f)] public float masterVolume = 1.0f;
    [Range(-80.0f, 10.0f)] public float musicVolume = 1.0f;
    [Range(-80.0f, 10.0f)] public float SFXVolume = 1.0f;
    public bool fullScreen = false;
}
