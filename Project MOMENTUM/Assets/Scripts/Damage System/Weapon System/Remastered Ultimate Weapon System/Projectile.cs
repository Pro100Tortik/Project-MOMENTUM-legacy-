using UnityEngine;
using System;
using ProjectMOMENTUM;

public class Projectile : MonoBehaviour
{
    public event Action<GameObject> OnHit;

    protected GameObject attacker;
    protected HitEffectsLibrary effectsLibrary;
    protected Rigidbody rb;
    protected Vector3 lastVel;
    private Collider _col;

    public void SetAttacker(GameObject attacker) => this.attacker = attacker;

    public void CalculateVelocity(Vector3 vel) => lastVel = vel;

    public Rigidbody GetRigidbody() => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Start()
    {
        effectsLibrary = HitEffectsLibrary.Instance;

        var colliders = attacker.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            Physics.IgnoreCollision(_col, col, true);
        }
    }

    protected void InvokeOnHit(GameObject gameObject) => OnHit?.Invoke(gameObject);

    protected virtual void OnImpactAction(GameObject hitObject) { }

    protected virtual void OnImpactAction(Collision collision) { }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == attacker)
            return;
    
        if (collision.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            if (damagable == attacker.GetComponentInChildren<IDamagable>())
                return;
        }
    
        OnImpactAction(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == attacker)
            return;
    
        if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            if (damagable == attacker.GetComponentInChildren<IDamagable>())
                return;
        }
    
        OnImpactAction(other.gameObject);
    }
}
