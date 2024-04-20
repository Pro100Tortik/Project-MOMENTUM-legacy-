using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float eyesHeight = 0.75f;
    public float viewRadius = 20.0f;
    [Range(0, 360)]
    public float viewAngle = 100.0f;
    public LayerMask targetMask;
    public LayerMask levelMask;
    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

    public Transform TargetPosition { get; private set; }
    public bool CanSeeTarget {  get; private set; }

    private void Start() => StartCoroutine(FindTargetsWithDelay(0.2f));

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void FindVisibleTargets()
    {
        Vector3 pos = transform.position + Vector3.up * eyesHeight;
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = 
            Physics.OverlapSphere(pos, viewRadius, targetMask);

        foreach (Collider target in targetsInViewRadius)
        {
            Transform thisTarget = target.transform;
            Vector3 dirToTarget = (thisTarget.position - pos).normalized;
            float distanceToTarget = Vector3.Distance(pos, thisTarget.position);
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(pos, dirToTarget, distanceToTarget, levelMask))
                {
                    visibleTargets.Add(thisTarget);
                    TargetPosition = thisTarget;
                    CanSeeTarget = true;
                    return;
                }
            }
            else if (distanceToTarget <= 3)
            {
                if (!Physics.Raycast(pos, dirToTarget, distanceToTarget, levelMask))
                {
                    visibleTargets.Add(thisTarget);
                    TargetPosition = thisTarget;
                    CanSeeTarget = true;
                    return;
                }
            }
        }
        CanSeeTarget = false;
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
}
