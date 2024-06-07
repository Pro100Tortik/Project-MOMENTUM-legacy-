using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField] private float explosionPower = 10f;
    [SerializeField] private float explosionRadius = 5f;
    private bool _exploded = false;

    private void OnEnable()
    {
        _exploded = false;
    }

    protected override void OnImpactAction(GameObject hitObject)
    {
        var damagable = hitObject.GetComponentInParent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(attacker, 100);
        }

        Explode();
    }

    protected override void OnImpactAction(Collision collision)
    {
        var damagable = collision.gameObject.GetComponentInParent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(attacker, 100);
        }

        Explode();
    }

    private void Explode()
    {
        if (_exploded)
            return;

        _exploded = true;

        if (effectsLibrary != null)
            effectsLibrary.SpawnExplosion(transform.position, 2);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionPower,
                    transform.position, explosionRadius, 1.5f, ForceMode.Impulse);

            if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                bool minDmg = damagable == attacker.GetComponentInChildren<IDamagable>();
                damagable.Damage(attacker, (minDmg ? 0.4f : 1f) * 30);
            }
        }
        rb.velocity = Vector3.zero;
        InvokeOnHit(gameObject);
    }
}
