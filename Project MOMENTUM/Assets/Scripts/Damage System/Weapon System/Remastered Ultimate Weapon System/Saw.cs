using UnityEngine;

public class Saw : Projectile
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private int maxRicochets = 4;
    private int _currentHits = 0;

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        _currentHits = 0;
    }

    protected override void OnImpactAction(GameObject hitObject)
    {
        var damagable = hitObject.GetComponentInParent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(attacker, damage);
        }
    }

    protected override void OnImpactAction(Collision collision)
    {
        if (_currentHits > maxRicochets)
        {
            InvokeOnHit(gameObject);
            return;
        }

        _currentHits++;
        var curSpeed = lastVel.magnitude;
        var dir = Vector3.Reflect(lastVel, collision.contacts[0].normal);
        rb.velocity = dir.normalized * Mathf.Max(curSpeed, 0);
        lastVel = rb.velocity;
    }
}
