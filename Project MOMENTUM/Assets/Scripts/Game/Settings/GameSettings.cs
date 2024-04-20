using UnityEngine;

[System.Serializable]
public class GameSettings
{
    [Range(0.1f, 10.0f)] public float Sensitivity = 1.5f;
    //[Range(-80.0f, 10.0f)] public float masterVolume = 1.0f;
    [Range(-80.0f, 10.0f)] public float MusicVolume = 1.0f;
    [Range(-80.0f, 10.0f)] public float SFXVolume = 1.0f;
    public bool EnableWeaponSway = true;
    public bool EnableWeaponBob = true;
}
