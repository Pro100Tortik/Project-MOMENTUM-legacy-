using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] private bool isEnabled = false;
    [SerializeField] private AudioClip flashlightSound;
    [SerializeField] private AudioSource source;
    private Light _light = null;
    private ClientStatus _status;

    private void Awake()
    {
        _light = GetComponent<Light>();

        _light.enabled = isEnabled;
        _status = GetComponentInParent<ClientStatus>();
    }

    private void Update()
    {
        if (!_status.CanReadInputs())
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
