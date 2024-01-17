using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    [SerializeField] private SettingsSaver config;
    [SerializeField] private AudioMixer audioMixer;
    private GameSettings settings => config.GameSettings;

    [Header("Inputs")]
    [SerializeField] private Slider sensitivity;

    [Header("Graphics")]
    //[SerializeField] private Toggle fullScreenToggle;

    [Header("Audio")]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider SFXVolume;

    private void Start()
    {
        InitializeVariables();
        //fullScreenToggle.isOn = Screen.fullScreen ? true : false;
    }

    private void OnDisable()
    {
        config.SaveSettings();
    }

    public void SetSensitivity(float value) => settings.sensitivity = value;

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
        settings.masterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        settings.musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        settings.SFXVolume = volume;
    }

    public void ToggleFullScreen(bool isFullScreen) => Screen.fullScreen = isFullScreen;

    private void InitializeVariables()
    {
        sensitivity.value = settings.sensitivity;

        masterVolume.value = settings.masterVolume;

        musicVolume.value = settings.musicVolume;

        SFXVolume.value = settings.SFXVolume;

        //Screen.fullScreen = settings.fullScreen;
    }
}
