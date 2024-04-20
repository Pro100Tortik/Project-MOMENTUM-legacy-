using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    public event Action<GameObject> OnDamaged;
    public bool IsDead { get; private set; }

    [Header("Sounds")]
    [SerializeField] private AudioSource painSource;
    [SerializeField] private AudioClip painSound;
    [SerializeField] private float painTimerCooldown = 0.4f;

    [Header("Parameters")]
    [SerializeField] private float health = 35;
    [SerializeField] private int overkillDamage = -15;
    private GibSpawner _gibSpawner;
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

    public void Damage(GameObject attacker, float damage)
    {
        OnDamaged?.Invoke(attacker);
        float delta = health - damage;

        PlayPainSound();

        if (delta <= 0)
        {
            IsDead = true;
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
