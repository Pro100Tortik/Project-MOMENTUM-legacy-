using UnityEngine;

[RequireComponent(typeof(EnemyFOV), typeof(EnemyHealth))]
public class TurretAI : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private Vector2 upRotationLimit = new Vector2(-45, 45);
    [SerializeField] private Vector2 leftRightRotationLimit = new Vector2(-45, 45);
    [SerializeField] private AudioSource turretSource;
    [SerializeField] private AudioClip spotted;
    [SerializeField] private AudioClip fire;

    [Header("Combat Settings")]
    [SerializeField] private float damage = 2;
    [SerializeField] private float fireRate = 3;
    [SerializeField] private Transform gunpoint1;
    [SerializeField] private Transform gunpoint2;
    [SerializeField] private int ticksUntilAttack = 100; // When spotted target
    [SerializeField] private int attackTicks = 200;
    [SerializeField] private Vector2 spread = Vector2.one * 0.2f;
    [SerializeField] private LayerMask hitMask;

    private float _cooldownTimer;

    private bool _rightWeapon;

    private int _ticks;
    private int _attackTicks;

    private DrawBulletTracer _bulletTracer;
    private EnemyHealth _enemyHealth;
    private EnemyFOV _enemyFOV;
    private Transform _target;

    private void Awake()
    {
        _enemyFOV = GetComponent<EnemyFOV>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start() => _bulletTracer = DrawBulletTracer.Instance;

    private void FixedUpdate()
    {
        if (_enemyHealth.IsDead)
            return;

        if (_enemyFOV.CanSeeTarget)
        {
            _target = _enemyFOV.TargetPosition;
            _ticks++;

            if (_ticks >= ticksUntilAttack)
                _attackTicks = attackTicks;
        }
        else
        {
            _ticks = 0;

            if (_attackTicks > 0)
                _attackTicks--;
            else
                _target = null;
        }

        if (_target != null)
            Aim();

        if (_ticks >= ticksUntilAttack || _attackTicks > 0)
        {
            Shoot();
        }
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
        curRotation.y = Mathf.Clamp(curRotation.y, leftRightRotationLimit.x, leftRightRotationLimit.y);
        curRotation.z = 0f;
        transform.localRotation = Quaternion.Euler(curRotation); 
    }

    private float Vector3AngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal, Vector3 toZeroAngle)
    {
        Vector3 projected = Vector3.ProjectOnPlane(from - to, planeNormal);
        float projectedAngle = Vector3.SignedAngle(projected, toZeroAngle, planeNormal);
        return projectedAngle;
    }

    private void Shoot()
    {
        if (!CanShoot())
            return;

        turretSource.pitch = Random.Range(0.9f, 1.1f);
        turretSource.PlayOneShot(fire);

        Vector3 sprd = WeaponFunctions.GetSpread(spread);
        Vector3 dir = transform.forward + sprd;

        RaycastHit hit;
        Ray shootingRay = new Ray(transform.position, dir);
        if (Physics.Raycast(shootingRay, out hit, 100, hitMask, QueryTriggerInteraction.Ignore))
        {
            WeaponFunctions.AIShootingLogic(gameObject, damage, null, hit, out var damaged);
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint1.position : gunpoint2.position, hit.point);
        }
        else
        {
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint1.position : gunpoint2.position,
                (_rightWeapon ? gunpoint1.position : gunpoint2.position) + dir * 13);
        }

        _rightWeapon = !_rightWeapon;
    }

    private bool CanShoot()
    {
        if (Time.time < _cooldownTimer)
            return false;

        _cooldownTimer = Time.time + 1f / fireRate;
        return true;
    }
}
