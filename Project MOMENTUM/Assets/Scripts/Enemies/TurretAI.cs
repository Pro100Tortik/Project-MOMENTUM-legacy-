using UnityEngine;

[RequireComponent(typeof(EnemyFOV))]
public class TurretAI : MonoBehaviour
{
    [SerializeField] private Vector2 upRotationLimit = new Vector2(-45, 45);
    private EnemyFOV _enemyFOV;
    private Transform _target;

    private void Awake() => _enemyFOV = GetComponent<EnemyFOV>();

    private void Update()
    {
        if (_enemyFOV.CanSeeTarget)
        {
            _target = _enemyFOV.TargetPosition;
        }
        else
        {
            _target = null;
        }

        if (_target != null)
            Aim();
    }

    private void Aim()
    {
        Vector3 newRotation = Vector3.zero;

        // Horizontal Aim
        float targetPlaneAngle = Vector3AngleOnPlane(_target.position, transform.position, -transform.up, transform.forward);
        newRotation.y = targetPlaneAngle;

        // UP/DOWN
        float upAngle = Vector3AngleOnPlane(_target.position, transform.position, -transform.right, transform.forward);
        newRotation.x = upAngle;

        transform.Rotate(newRotation, Space.Self);

        // Clamp X and reset Z rotation
        Vector3 curRotation = transform.localRotation.eulerAngles;
        curRotation.x = Mathf.Clamp(curRotation.x, upRotationLimit.x, upRotationLimit.y);
        curRotation.z = 0f;
        transform.localRotation = Quaternion.Euler(curRotation); 
    }

    private float Vector3AngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal, Vector3 toZeroAngle)
    {
        Vector3 projected = Vector3.ProjectOnPlane(from - to, planeNormal);
        float projectedAngle = Vector3.SignedAngle(projected, toZeroAngle, planeNormal);

        return projectedAngle;
    }
}
