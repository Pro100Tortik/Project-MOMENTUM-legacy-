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

    public BodyPart GetBodyPart() => bodyPart;

    public float DamageMultiplier()
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                return 1.35f;

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
