using UnityEngine;

public enum BodyPart
{
    None,
    Head,
    Hand,
    Body,
    Leg
}

public class Hitbox : MonoBehaviour, IHitbox
{
    [SerializeField] private BodyPart bodyPart;
    [SerializeField] private float damageMultiplier = 1f;
    private Collider _collider;

    private void OnValidate() => TryGetComponent<Collider>(out _collider);

    private void OnDrawGizmos()
    {
        if (_collider == null)
            return;

        Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
    }

    public float DamageMultiplier()
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                return 2f;

            case BodyPart.Hand:
                return 0.75f;

            case BodyPart.Body:
                return 1f;

            case BodyPart.Leg:
                return 0.5f;

            default:
                return damageMultiplier;
        }
    }
}
