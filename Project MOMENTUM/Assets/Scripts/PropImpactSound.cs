using UnityEngine;

public class PropImpactSound : MonoBehaviour
{
    [SerializeField] private float hitVelocity = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float volume = 0.5f;
    [SerializeField] private AudioClip[] sounds;
    private AudioSource _source;

    private void Awake()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.volume = volume;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > hitVelocity 
            && !collision.collider.CompareTag("Player"))
        if (sounds != null)
        {
            _source.clip = sounds[Random.Range(0, sounds.Length - 1)];
            _source.Play();
        }
    }
}
