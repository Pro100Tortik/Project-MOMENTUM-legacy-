using UnityEngine;

public static class WeaponFunctions
{
    public static void Interact(IInteractable interactable)
    {
        if (interactable == null)
            return;

        if (interactable.CanInteractByShooting())
            interactable.Interact();
    }

    public static Vector3 GetSpread(Vector2 spread) =>
        new Vector3(Random.Range(-spread.x, spread.x),
            Random.Range(-spread.y, spread.y),
            Random.Range(-spread.x, spread.x));

    public static bool CanShoot(WeaponDataSO weaponData, bool dualWielded, float dualWieldedMultiplier, float currentCoolDown, out float coolDownTimer)
    {
        coolDownTimer = currentCoolDown;
        if (Time.time < coolDownTimer)
            return false;

        if (!dualWielded)
            coolDownTimer = Time.time + 1f / weaponData.fireRate;
        else
            coolDownTimer = Time.time + 1f / weaponData.fireRate / dualWieldedMultiplier;

        return true;
    }

    public static bool CheckAmmo(AmmoType thisAmmoType, AmmoInventory ammoInventory, int howManyBullets)
    {
        if (thisAmmoType == AmmoType.None)
            return true;

        if (ammoInventory.InfiniteAmmo)
            return true;

        int used = ammoInventory.UseAmmo
            (thisAmmoType, howManyBullets);

        return used > 0;
    }

    public static void ShootingLogic(GameObject attacker, WeaponDataSO weaponData,
        HitEffectsLibrary hitEffects, RaycastHit hitSomething, out IDamagable damaged)
    {
        damaged = null;

        IInteractable interactable = hitSomething.collider.GetComponent<IInteractable>();
        Interact(interactable);

        IHitbox hitbox = hitSomething.collider.GetComponent<IHitbox>();

        IDamagable damagable = hitSomething.collider.GetComponentInParent<IDamagable>();

        hitEffects.SpawnParticles(hitSomething, 2);

        if (damagable == null)
            return;

        if (hitbox == null)
            return;

        if (hitSomething.collider.gameObject == attacker)
            return;

        damaged = damagable;
        damagable.Damage(attacker, weaponData.damage * hitbox.DamageMultiplier());// * Random.Range(1, 3));
    }
}
