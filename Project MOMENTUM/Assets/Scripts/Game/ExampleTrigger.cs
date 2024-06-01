using UnityEngine;

public class ExampleTrigger : MonoBehaviour
{

    [SerializeField] float maxdistance = 10f; 
    [SerializeField] float minDistance = 1.0f;
    [SerializeField] float maxstrength = 5f;

    private void Start()
    {
        Shake();
    }

    public void Shake()
    {
        float distance = GetDistanceToExplosion();
        float strength = CalculateMagnitude(distance);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(strength, 5, 0.3f, 0.5f);
    }

    float CalculateMagnitude(float dist)
    {
        dist = Mathf.Clamp(dist, 0f, maxdistance);
        float lerp = Mathf.InverseLerp(maxdistance, minDistance, dist);
        float finalMagnitude = Mathf.Lerp(0, maxstrength, lerp);
        return finalMagnitude;
    }
    
    float GetDistanceToExplosion()
    {
        return Vector3.Distance(transform.position, EZCameraShake.CameraShaker.Instance.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxdistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}
