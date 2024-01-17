using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static void SpawnEffects(GameObject effect, RaycastHit hit, Vector3 rotation, float duration, bool setParent)
    {
        GameObject fx = Instantiate(effect, hit.point + hit.normal * 0.01f,
            Quaternion.FromToRotation(rotation, hit.normal), setParent ? hit.transform : null);

        Destroy(fx, duration);
    }
}
