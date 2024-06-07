using ProjectMOMENTUM;
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

    public static bool CanShoot(WeaponDataSO weaponData, float currentCoolDown, out float coolDownTimer)
    {
        coolDownTimer = currentCoolDown;

        if (Time.time < coolDownTimer)
            return false;

        coolDownTimer = Time.time + 1f / weaponData.primaryFireRate;

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

    public static void ShootingLogic(GameObject attacker, float damage, HitEffectsLibrary hitEffects, RaycastHit hitSomething, out IDamagable damaged)
    {
        damaged = null;

        IInteractable interactable = hitSomething.collider.GetComponent<IInteractable>();
        Interact(interactable);

        IHitbox hitbox = hitSomething.collider.GetComponent<IHitbox>();

        IDamagable damagable = hitSomething.collider.GetComponentInParent<IDamagable>();

        if (damagable != null && damagable == attacker.GetComponentInChildren<IDamagable>())
            return;

        hitEffects.SpawnParticles(hitSomething, 30);

        if (damagable == null)
            return;

        if (hitbox == null)
            return;

        damaged = damagable;
        damagable.Damage(attacker, damage * hitbox.DamageMultiplier());// * Random.Range(1, 3));
    }

    public static void AIShootingLogic(GameObject attacker, float damage, HitEffectsLibrary hitEffects, RaycastHit hitSomething, out IDamagable damaged)
    {
        damaged = null;

        Interact(hitSomething.collider.GetComponent<IInteractable>());

        IHitbox hitbox = hitSomething.collider.GetComponent<IHitbox>();

        if (hitSomething.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if (!rb.isKinematic)
            {
                rb.AddForce(-hitSomething.normal);
            }
        }

        IDamagable damagable = hitSomething.collider.GetComponentInParent<IDamagable>();

        if (hitEffects != null)
            hitEffects.SpawnParticles(hitSomething, 2);

        if (damagable == null)
            return;

        if (hitSomething.collider.gameObject == attacker)
            return;

        damaged = damagable;
        damagable.Damage(attacker, damage * (hitbox != null ? hitbox.DamageMultiplier() : 1));
    }
}
