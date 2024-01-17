using UnityEngine;
using UnityEngine.Events;

public class PlayExplosionSound : MonoBehaviour
{
    [SerializeField] private UnityEvent Enabled;

    private void OnEnable() => Enabled.Invoke();
}
