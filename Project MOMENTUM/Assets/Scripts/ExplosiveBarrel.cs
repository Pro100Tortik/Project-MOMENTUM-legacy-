using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 15;
    //[SerializeField] private int healthValueToBleed;
    [SerializeField] private float explosionPower = 500;
    [SerializeField] private float explosionRadius = 5;
    private bool _exploded = false;

    private void OnEnable()
    {
        Destroy(gameObject);
        _exploded = false;
    }

    public void Damage(GameObject attacker, int damage)
    {
        if (health <= 0)
        {
            Explode();
            _exploded = true;
            return;
        }

        health -= damage;
    }

    private void Explode()
    {
        if (_exploded)
            return;

        HitEffectsLibrary.Instance.SpawnExplosion(transform.position, 2);

        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up * 0.5f, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionPower,
                    transform.position, explosionRadius);

            IDamagable damagable = collider.GetComponent<IDamagable>();

            if (damagable != null && collider != GetComponent<Collider>())
                damagable.Damage(null, 30);
        }
        gameObject.SetActive(false);
    }
}
