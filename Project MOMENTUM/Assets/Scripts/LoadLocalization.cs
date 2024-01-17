using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LoadLocalization : MonoBehaviour
{
    [SerializeField] private SettingsSaver settings;

    private void Start()
    {
        StartCoroutine(SetLocale(settings.GameSettings.language));
    }

    private IEnumerator SetLocale(int id)
    {
        yield return LocalizationSettings.InitializationOperation;
        if (id < 0)
        {
            id = 0;
        }

        if (LocalizationSettings.AvailableLocales.Locales.Count - 1 < id)
        {
            id = LocalizationSettings.AvailableLocales.Locales.Count - 1;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }
}
