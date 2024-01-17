using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable, ISaveable
{
    public event Action<AudioClip> OnDamage;
    public event Action<AudioClip> OnPickup;
    public event Action OnDeath;

    [Header("Damage Effect")]
    [SerializeField] private RawImage damageEffect;
    [SerializeField] private RawImage damageResistEffect;
    [SerializeField] private float fadeSpeed = 2.5f;

    [SerializeField, Range(0, 200)] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField, Range(0, 200)] private int armor = 0;
    [SerializeField] private int maxArmor = 100;
    private float _currentDamageResistFactor;
    private bool _isDead = false;
    private ArmorType _currentArmorType;

    [Header("Sounds")]
    [SerializeField] private float painTimerCooldown = 0.5f;
    [SerializeField] private AudioClip powerupArmor;
    [SerializeField] private AudioClip powerupHealth;
    [SerializeField] private AudioClip powerupDamageResist;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip armorSound;
    [SerializeField] private List<AudioClip> painSounds;

    private float _damageResistTimer;
    private float _painTimer;

    public int Health => health;
    public int Armor => armor;
    public ArmorType GetCurrentArmorType() => _currentArmorType;

    private void Awake()
    {
        damageEffect.color = new Color(1, 0, 0, 0);
        damageResistEffect.color = new Color(0, 1, 0, 0);
    }

    private void Update()
    {
        if (health <= 0 && !_isDead)
        {
            _isDead = true;
            health = 0;
            armor = 0;
            OnDeath?.Invoke();
        }

        _currentDamageResistFactor = GetDamageResistFactor(_currentArmorType);
    }

    private void FixedUpdate()
    {
        damageEffect.color = Color.Lerp(damageEffect.color, new Color(1, 0, 0, 0), Time.deltaTime * fadeSpeed);

        if (_damageResistTimer > 0)
            _damageResistTimer -= Time.deltaTime;
        else
            _damageResistTimer = 0;

        if (_damageResistTimer < 3)
        {
            damageResistEffect.color = Color.Lerp(damageResistEffect.color, new Color(0, 1, 0, 0), Time.deltaTime);
        }

        if (_painTimer > 0)
            _painTimer -= Time.deltaTime;
        else
            _painTimer = 0;
    }

    public void Damage(GameObject attacker, int damage)
    {
        if (_isDead) return;

        //if (status.GodMode)
        //    return;

        PainSoundTrigger();

        if (_damageResistTimer > 0)
        {
            return;
        }

        DamageLogic(damage);
    }

    private void PainSoundTrigger()
    {
        if (_painTimer <= 0)
        {
            _painTimer = painTimerCooldown;
            damageEffect.color = new Color(1, 0, 0, 0.4f);
            OnDamage?.Invoke(painSounds[UnityEngine.Random.Range(0, painSounds.Count - 1)]);
        }
    }

    private void DamageLogic(int damage)
    {
        if (armor > 0)
        {
            int damageToHealth = damage - Mathf.RoundToInt(damage * _currentDamageResistFactor);
            if (armor >= Mathf.RoundToInt(damage * _currentDamageResistFactor))
            {
                armor -= Mathf.RoundToInt(damage * _currentDamageResistFactor);
                health -= damageToHealth;
            }
            else
            {
                armor -= Mathf.RoundToInt(damage * _currentDamageResistFactor);
                health -= Mathf.Abs(armor) + damageToHealth;
                armor = 0;
                _currentArmorType = ArmorType.Green;
            }
        }
        else
        {
            health -= damage;
        }
    }

    private float GetDamageResistFactor(ArmorType currentarmor)
    {
        if (currentarmor == ArmorType.Blue)
        {
            return 0.50f;
        }
        else if (currentarmor == ArmorType.Red)
        {
            return 0.75f;
        }
        else
            return 0.33f;
    }

    private bool RestoreHealth(int restoreValue, bool clamp)
    {
        if (clamp)
        {
            int delta = maxHealth - health;

            if (delta <= 0)
                return false;

            if (restoreValue > delta)
                health = maxHealth;
            else
                health += restoreValue;

            health = Mathf.Clamp(health, 0, maxHealth);
        }
        else
        {
            int delta = 200 - health;

            if (delta <= 0)
                return false;

            if (restoreValue > delta)
                health = 200;
            else
                health += restoreValue;
        }
        return true;
    }

    private bool RestoreArmor(int restoreValue, bool clamp)
    {
        if (clamp)
        {
            int delta = maxArmor - armor;

            if (delta <= 0)
                return false;

            if (restoreValue > delta)
                armor = maxArmor;
            else
                armor += restoreValue;
            armor = Mathf.Clamp(armor, 0, maxArmor);
        }
        else
        {
            int delta = 200 - armor;

            if (delta <= 0)
                return false;

            if (restoreValue > delta)
                armor = 200;
            else
                armor += restoreValue;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        HealthArmorPickup pickup = other.GetComponent<HealthArmorPickup>();
        if (pickup)
        {
            if (pickup.Health)
            {
                if (RestoreHealth(pickup.GetHealValue(), true))
                {
                    OnPickup?.Invoke(healSound);
                    other.gameObject.SetActive(false);
                }
            }
            else
            {
                if (RestoreArmor(pickup.GetHealValue(), true))
                {
                    _currentArmorType = pickup.GetArmorType();
                    OnPickup?.Invoke(armorSound);
                    other.gameObject.SetActive(false);
                }
            }
        }

        Powerup powerup = other.GetComponent<Powerup>();
        if (powerup)
        {
            if (powerup.GetPowerupType == PowerupType.MegaHealth)
            {
                if (RestoreHealth(100, false))
                {
                    other.gameObject.SetActive(false);
                    OnPickup?.Invoke(powerupHealth);
                }
            }
            if (powerup.GetPowerupType == PowerupType.MegaArmor)
            {
                if (RestoreArmor(100, false))
                {
                    _currentArmorType = ArmorType.Blue;
                    other.gameObject.SetActive(false);
                    OnPickup?.Invoke(powerupArmor);
                }
            }
            if (powerup.GetPowerupType == PowerupType.DamageResist)
            {
                _damageResistTimer += 30.0f;
                damageResistEffect.color = new Color(0, 1, 0, 0.3f);
                other.gameObject.SetActive(false);
                OnPickup?.Invoke(powerupDamageResist);
            }
        }
    }

    public void LoadData(GameData data)
    {
        health = data.playerData.playerHealth;
        armor = data.playerData.playerArmor;
    }

    public void SaveData(GameData data)
    {
        data.playerData.playerHealth = health;
        data.playerData.playerArmor = armor;
    }
}
