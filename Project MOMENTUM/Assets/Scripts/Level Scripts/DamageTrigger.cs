using UnityEngine;

namespace ProjectMOMENTUM
{
    public sealed class DamageTrigger : TriggerZoneAbstract
    {
        [SerializeField] private float damage = 5f;
        [SerializeField] private float damageSpeed = 2f;
        private float _timer;

        protected override void OnEnter(Collider other) { }

        protected override void OnExit(Collider other) { }

        protected override void OnStay(Collider other)
        {
            var damagable = GetComponentInChildren<IDamagable>();
            if (damagable != null)
            {
                _timer += Time.deltaTime;

                if (_timer >= damageSpeed)
                {
                    damagable.Damage(null, damage);
                    _timer = 0;
                }
            }
        }
    }
}
