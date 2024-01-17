using UnityEngine;

public class SecretTrigger : MonoBehaviour
{
    public static int SecretsCounter {  get; private set; }
    public static int SecretsFound { get; private set; }

    private void OnEnable() => SecretsCounter++;

    public void SecretFound()
    {
        gameObject.SetActive(false);
        SecretsFound++;
    }

    private void OnDestroy()
    {
        SecretsFound = 0;
        SecretsCounter = 0;
    }
}
