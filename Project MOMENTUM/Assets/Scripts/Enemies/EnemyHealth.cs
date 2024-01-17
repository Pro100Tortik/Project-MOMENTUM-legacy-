using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    internal event Action<GameObject> OnDamaged;

    //[SerializeField] private GameObject parentGameobject;

    [Header("Sounds")]
    [SerializeField] private AudioSource painSource;
    [SerializeField] private AudioClip painSound;
    [SerializeField] private float painTimerCooldown = 0.4f;

    [Header("Parameters")]
    [SerializeField] private int health = 35;
    [SerializeField] private int overkillDamage = -15;
    private GibSpawner _gibSpawner;
    internal bool _isDead;
    private float _painTimer;

    private void Awake() => _gibSpawner = GetComponentInParent<GibSpawner>();

    public void SetHealth(int health) => this.health = health;

    public void SetOverkillDamage(int damage) => overkillDamage = damage;

    private void FixedUpdate()
    {
        if (_painTimer > 0)
            _painTimer -= Time.deltaTime;
        else
            _painTimer = 0;
    }

    public void Damage(GameObject attacker, int damage)
    {
        OnDamaged?.Invoke(attacker);
        int delta = health - damage;

        PlayPainSound();

        if (delta <= 0)
        {
            _isDead = true;
            health = delta;
        }

        if (delta <= overkillDamage)
        {
            Overkill();
            health = delta;
            return;
        }

        if (delta > 0)
        {
            health = delta;
        }
    }

    private void Die()
    {
        // Drop body
    }

    private void Overkill()
    {
        _gibSpawner.GIB();
        gameObject.SetActive(false);
    }

    private void PlayPainSound()
    {
        if (painSource != null && painSound != null)
        {
            if (_painTimer <= 0)
            {
                _painTimer = painTimerCooldown;
                painSource.PlayOneShot(painSound);
            }
        }
    }
}
