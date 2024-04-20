using System;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public event Action<GameObject> OnHit;

    [SerializeField] private float explosionPower;
    [SerializeField] private float explosionRadius;

    private GameObject _attacker;
    private Rigidbody _rb;
    private HitEffectsLibrary _effectsLibrary;
    private bool _exploded = false;

    public void SetAttacker(GameObject attacker) => _attacker = attacker;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_rb.velocity = transform.forward * speed;
    }

    private void Start() => _effectsLibrary = HitEffectsLibrary.Instance;

    private void OnEnable()
    {
        _exploded = false;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_exploded)
        {
            if (other.gameObject == _attacker)
                return;

            _exploded = true;

            if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagable.Damage(_attacker, 100);
            }

            Explode();
        }
    }

    private void Explode()
    {
        if (_effectsLibrary != null)
            _effectsLibrary.SpawnExplosion(transform.position, 2);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            /*if (Physics.Raycast(transform.position, transform.position - rb.position, explosionRadius))
            {
                rb.AddExplosionForce(explosionPower,
                    transform.position, explosionRadius);

                IDamagable damagable = collider.GetComponent<IDamagable>();

                if (damagable != null)
                    damagable.Damage(null, Mathf.RoundToInt((transform.position - rb.position).magnitude) * 10);
            }*/

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionPower,
                    transform.position, explosionRadius);

            IDamagable damagable = collider.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.Damage(_attacker, 30);
        }
        _rb.velocity = Vector3.zero;
        OnHit?.Invoke(gameObject);
    }
}
