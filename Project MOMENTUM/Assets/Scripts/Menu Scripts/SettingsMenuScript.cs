using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private GameSettings settings => _settings.GameSettings;

    [Header("Inputs")]
    [SerializeField] private Slider sensitivity;

    [Header("Graphics")]
    //[SerializeField] private Toggle fullScreenToggle;

    [Header("Audio")]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider SFXVolume;
    private SettingsSaver _settings;

    private void Start()
    {
        _settings = SettingsSaver.Instance;

        InitializeVariables();

        DeveloperConsole.RegisterCommand("sensitivity", "<float>", "Changes mouse sensitivity.", args =>
        {
            if (float.TryParse(args[1], out var sens))
                SetSensitivity(sens);
        });

        //fullScreenToggle.isOn = Screen.fullScreen ? true : false;
    }

    private void OnDisable() => _settings.SaveSettings();

    public void SetSensitivity(float value)
    {
        settings.Sensitivity = value;
        InitializeVariables();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
        //settings.masterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        settings.MusicVolume = volume;
        InitializeVariables();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        settings.SFXVolume = volume;
        InitializeVariables();
    }

    public void ToggleFullScreen(bool isFullScreen) => Screen.fullScreen = isFullScreen;

    private void InitializeVariables()
    {
        sensitivity.value = settings.Sensitivity;

        audioMixer.GetFloat("Master", out float da);
        masterVolume.value = da;    

        musicVolume.value = settings.MusicVolume;

        SFXVolume.value = settings.SFXVolume;

        //Screen.fullScreen = settings.fullScreen;
    }
}
