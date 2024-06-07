using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField, Range(0, 200)] private float _health = 100;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField, Range(0, 200)] private float _armor = 0;
    [SerializeField] private int _maxArmor = 100;
    private float _currentDamageResistFactor;
    private ArmorType _currentArmorType;

    public bool IsDead { get; private set; } = false;
    public bool CanBeDamaged { get; private set; } = true;

    public float GetHealth() => _health;
    public float GetArmor() => _armor;

    public void Damage(GameObject attacker, float damage)
    {
        if (IsDead) return;

        if (!CanBeDamaged) return;

        _currentDamageResistFactor = GetDamageResistFactor(_currentArmorType);

        DamageLogic(damage);
    }

    private void RestoreHealth(int restoreValue, bool clamp)
    {
        if (clamp)
        {
            float delta = _maxHealth - _health;

            if (delta <= 0)
                return;

            if (restoreValue > delta)
                _health = _maxHealth;
            else
                _health += restoreValue;

            _health = Mathf.Clamp(_health, 0, _maxHealth);
        }
        else
        {
            float delta = 200 - _health;

            if (delta <= 0)
                return;

            if (restoreValue > delta)
                _health = 200;
            else
                _health += restoreValue;
        }
    }

    private void AddArmor(int restoreValue, bool clamp)
    {
        if (clamp)
        {
            float delta = _maxArmor - _armor;

            if (delta <= 0)
                return ;

            if (restoreValue > delta)
                _armor = _maxArmor;
            else
                _armor += restoreValue;

            _armor = Mathf.Clamp(_armor, 0, _maxArmor);
        }
        else
        {
            float delta = 200 - _armor;

            if (delta <= 0)
                return;

            if (restoreValue > delta)
                _armor = 200;
            else
                _armor += restoreValue;
        }

    }

    private void DamageLogic(float damage)
    {
        if (_armor > 0)
        {
            float damageToHealth = damage - Mathf.RoundToInt(damage * _currentDamageResistFactor);
            if (_armor >= Mathf.RoundToInt(damage * _currentDamageResistFactor))
            {
                _armor -= Mathf.RoundToInt(damage * _currentDamageResistFactor);
                _health -= damageToHealth;
            }
            else
            {
                _armor -= Mathf.RoundToInt(damage * _currentDamageResistFactor);
                _health -= Mathf.Abs(_armor) + damageToHealth;
                _armor = 0;
                _currentArmorType = ArmorType.Green;
            }
        }
        else
        {
            _health -= damage;
        }
    }

    private float GetDamageResistFactor(ArmorType currentarmor)
    {
        switch (currentarmor)
        {
            case ArmorType.Green:
                return 0.33f;

            case ArmorType.Blue:
                return 0.50f;

            case ArmorType.Red:
                return 0.75f;

            default:
                return 0;
        }
    }
}
