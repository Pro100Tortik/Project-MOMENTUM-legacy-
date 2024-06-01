using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private bool isEnabled = false;
    [SerializeField] private AudioClip flashlightSound;
    [SerializeField] private AudioSource source;
    private Light _light = null;

    private void Awake()
    {
        _light = GetComponent<Light>();

        _light.enabled = isEnabled;
    }

    private void Update()
    {
        if (!player.CanReadInputs())
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            isEnabled = !isEnabled;
            _light.enabled = isEnabled;

            if (source == null)
                return;

            source.PlayOneShot(flashlightSound);
        }
    }
}
