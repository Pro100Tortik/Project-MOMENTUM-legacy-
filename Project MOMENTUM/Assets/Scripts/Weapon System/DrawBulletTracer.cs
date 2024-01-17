using System.Collections;
using UnityEngine;

public class DrawBulletTracer : MonoBehaviour
{
    public static DrawBulletTracer Instance;

    [SerializeField] private GameObject bulletTrail;
    private GameObjectPool _trailsPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _trailsPool = new GameObjectPool(bulletTrail, 50);
    }

    public void LaunchTrail(Vector3 startpos, Vector3 hitpoint)
    {
        GameObject trail = _trailsPool.Get();
        trail.transform.position = startpos;
        StartCoroutine(SpawnTrail(trail, hitpoint));
    }

    private IEnumerator SpawnTrail(GameObject Trail, Vector3 HitPoint)
    {
        TrailRenderer trailRen = Trail.GetComponent<TrailRenderer>();
        Vector3 startPosition = trailRen.transform.position;
        float distance = Vector3.Distance(trailRen.transform.position, HitPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            trailRen.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= 100 * Time.deltaTime;
            yield return null;
        }
        trailRen.transform.position = HitPoint;
        _trailsPool.Return(Trail);
    }
}
