using UnityEngine;

public class HealingItem : Weapon
{
    private PlayerHealth health => player.playerHealth;

    protected override void PrimaryAttack()
    {
        health.RestoreHealth(weaponData.primaryDamage, true);
    }
}
