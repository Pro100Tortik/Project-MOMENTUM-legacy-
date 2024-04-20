using TMPro;
using UnityEngine;

public class IntermissionScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text levelName;
    [SerializeField] private TMP_Text secretsFound;
    [SerializeField] private TMP_Text secretsAll;
    [SerializeField] private TMP_Text killsConfirmed;
    [SerializeField] private TMP_Text killsAll;

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        levelName.text = GameFunctions.GetCurrentScene().name;
        secretsFound.text = SecretTrigger.SecretsFound.ToString();
        secretsAll.text = SecretTrigger.SecretsCounter.ToString();
        killsConfirmed.text = 0.ToString();
        killsAll.text = 0.ToString();
    }
}
