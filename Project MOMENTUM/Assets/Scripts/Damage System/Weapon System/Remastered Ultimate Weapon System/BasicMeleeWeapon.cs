using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMOMENTUM
{
        public class BasicMeleeWeapon : Weapon
    {
        [SerializeField] private Transform attackPos;
        [SerializeField] private float radius = 1f;
        [SerializeField] private bool careAboutObstacles = true;
        private Collider[] _hitColliders = new Collider[32];
        private int _hitCount = 0;

        protected override void PrimaryAttack()
        {
            if (TryFindDamagables())
            {
                TryAttackDamagables();
            }
        }

        private bool TryFindDamagables()
        {
            _hitCount = Physics.OverlapSphereNonAlloc(attackPos.position, radius,
                _hitColliders, hitMask, QueryTriggerInteraction.Ignore);
            return _hitCount > 0;
        }

        private void TryAttackDamagables()
        {
            for (int i = 0; i < _hitCount; i++)
            {
                if (!_hitColliders[i].TryGetComponent(out IDamagable damageable))
                {
                    continue;
                }

                if (careAboutObstacles)
                {
                    bool hasObstacle = Physics.Linecast(attackPos.position, _hitColliders[i].transform.position, levelMask);

                    if (hasObstacle)
                    {
                        continue;
                    }
                }

                damageable.Damage(player.gameObject, weaponData.primaryDamage);
            }
        }

        private void OnDrawGizmos()
        {
            if (attackPos != null)
            {
                Gizmos.DrawWireSphere(attackPos.position, radius);
            }
        }
    }
}
