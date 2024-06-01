using ProjectMOMENTUM;
using System.Collections.Generic;
using UnityEngine;

public class BasicRaycastWeapon : Weapon
{
    [SerializeField] private Transform headPos;
    [SerializeField] private Transform gunpoint;
    [SerializeField] private bool isPiercing = false;
    private List<IDamagable> _damaged = new List<IDamagable>();
    private DrawBulletTracer _bulletTracer;
    private HitEffectsLibrary _hitEffects;

    private void Start()
    {
        _hitEffects = HitEffectsLibrary.Instance;
        _bulletTracer = DrawBulletTracer.Instance;
    }

    protected override void PrimaryAttack()
    {
        Shoot();
    }

    private void Shoot()
    {
        for (int i = 0; i < weaponData.pellets; i++)
        {
            Vector3 spread = WeaponFunctions.GetSpread(weaponData.spread);
            Vector3 dir = headPos.forward + spread;
            RaycastHit hit;

            Ray shootingRay = new Ray(headPos.position, headPos.forward + spread);

            float pierceDistance = GetDistanceToBlockingObject(dir, shootingRay);

            if (isPiercing)
            {
                _damaged.Clear();

                if (pierceDistance <= 0)
                    return;

                RaycastHit[] pierceHits = Physics.RaycastAll(shootingRay, pierceDistance);
                PiercingDamage(pierceHits);

                // If nobody pierced just shoot normal bullet to spawn decals and effects
                if (pierceHits.Length < 1)
                {
                    if (Physics.Raycast(shootingRay, out hit, weaponData.primaryRange, hitMask, QueryTriggerInteraction.Ignore))
                    {
                        WeaponFunctions.ShootingLogic(player.origin, weaponData.primaryDamage, _hitEffects, hit, out var damaged);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(shootingRay, out hit, weaponData.primaryRange, hitMask | levelMask, QueryTriggerInteraction.Ignore))
                {
                    WeaponFunctions.ShootingLogic(player.origin, weaponData.primaryDamage, _hitEffects, hit, out var damaged);
                    _bulletTracer.LaunchTrail(gunpoint.position, hit.point);
                }
                else
                {
                    _bulletTracer.LaunchTrail(gunpoint.position, gunpoint.position + dir * 13);
                }
            }
        }
    }

    private float GetDistanceToBlockingObject(Vector3 dir, Ray shootingRay)
    {
        RaycastHit hitBlocked;

        if (Physics.Raycast(shootingRay, out hitBlocked, weaponData.primaryRange, levelMask))
        {
            _bulletTracer.LaunchTrail(gunpoint.position, hitBlocked.point);
            return hitBlocked.distance;
        }
        else
        {
            _bulletTracer.LaunchTrail(gunpoint.position, gunpoint.position + dir * 13);
            return weaponData.primaryRange;
        }
    }

    private void PiercingDamage(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var dmgd = hits[i].collider.GetComponentInParent<IDamagable>();

            if (_damaged.Contains(dmgd))
                return;

            if (hits[i].point == null)
                return;

            WeaponFunctions.ShootingLogic(player.origin, weaponData.primaryDamage, _hitEffects, hits[i], out var damaged);

            if (damaged != null)
                _damaged.Add(damaged);

            _bulletTracer.LaunchTrail(gunpoint.position, hits[i].point);
        }
    }
}
