using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateWeaponScript : WeaponAbstract
{
    [SerializeField] private GameObject attacker;

    [Header("References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private LayerMask levelMask;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private WeaponDataSO weaponData;
    [SerializeField] private Transform gunpoint;
    [SerializeField] private Transform gunpoint2;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;
    private HitEffectsLibrary _hitEffects;

    [Header("Settings")]
    [SerializeField] private bool haveSafetyLock = false;
    [SerializeField] private bool isPiercing = false;
    [SerializeField] private float afterChangeCooldown = 0.7f;
    [SerializeField] private float dualWieldedFireRateMultiplier = 1.65f;

    [Header("Dual Wielding")]
    [SerializeField] private GameObject secondWeapon;

    [Header("Animations")]
    [SerializeField] private Animator firstWeaponAnim;
    [SerializeField] private Animator secondWeaponAnim;
    private DrawBulletTracer _bulletTracer;

    private ClientStatus _status;
    private bool _dualWielded;
    private bool _rightWeapon;
    private float _coolDownTimer;
    private bool _safety;

    private List<IDamagable> _damaged = new List<IDamagable>();

    private PoolBase<GameObject> _projectilesPool;
    public GameObject Preload() => Instantiate(projectile);
    public void GetAction(GameObject effect) => effect.SetActive(true);
    public void ReturnAction(GameObject effect) => effect.SetActive(false);

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();

        if (projectile == null)
            return;

        _projectilesPool = new PoolBase<GameObject>(Preload, GetAction, ReturnAction, 20);
    }

    private void Start()
    {
        _hitEffects = HitEffectsLibrary.Instance;
        _bulletTracer = DrawBulletTracer.Instance;
    }

    private void OnEnable()
    {
        _coolDownTimer = Time.time + afterChangeCooldown;
        if (haveSafetyLock)
        {
            _safety = false;
            return;
        }
        _safety = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) || !Input.GetKey(KeyCode.Mouse0))
        {
            _safety = false;
        }

        if (secondWeapon != null)
        {
            _dualWielded = secondWeapon.activeSelf;
        }
        _rightWeapon = _dualWielded ? _rightWeapon : true;
    }

    private void FixedUpdate()
    {
        if (!_status.CanReadInputs())
            return;

        if (haveSafetyLock)
        {
            if (_safety && Input.GetKey(KeyCode.Mouse0))
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) &&
            WeaponFunctions.CanShoot(weaponData, _dualWielded,
            dualWieldedFireRateMultiplier, _coolDownTimer, out _coolDownTimer))
        {
            if (!WeaponFunctions.CheckAmmo(weaponData.ammoType, ammoInventory, weaponData.bulletsPerShot))
            {
                weaponSource.PlayOneShot(weaponData.empty);
                return;
            }

            if (projectile != null)
            {
                SpawnProjectile();
            }
            else
            {
                Shoot();
            }

            weaponSource.PlayOneShot(weaponData.fire1);

            PlayAnimationsAndEffects();
        }
    }

    private void PlayAnimationsAndEffects()
    {
        if (_dualWielded)
        {
            _rightWeapon = !_rightWeapon;
            if (!_rightWeapon)
            {
                muzzleFlash1.Play();
                if (firstWeaponAnim != null)
                {
                    firstWeaponAnim.CrossFade("Shot", 0.2f);
                }
            }
            else
            {
                muzzleFlash2.Play();
                if (secondWeaponAnim != null)
                {
                    secondWeaponAnim.CrossFade("Shot", 0.2f);
                }
            }
        }
        else
        {
            muzzleFlash1.Play();
            if (firstWeaponAnim != null)
            {
                firstWeaponAnim.CrossFade("Shot", 0.2f);
            }
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < weaponData.pellets; i++)
        {
            Vector3 spread = WeaponFunctions.GetSpread(weaponData.spread);
            Vector3 dir = playerCamera.forward + spread;
            RaycastHit hit;

            Ray shootingRay = new Ray(playerCamera.position, playerCamera.forward + spread);

            float pierceDistance = GetDistanceToBlockingObject(dir, shootingRay);

            if (isPiercing)
            {
                _damaged.Clear();

                if (pierceDistance > 0)
                {
                    RaycastHit[] pierceHits = Physics.RaycastAll(shootingRay, pierceDistance);
                    PiercingDamage(pierceHits);

                    // If nobody pierced just shoot normal bullet to spawn decals and effects
                    if (pierceHits.Length < 1)
                    {
                        if (Physics.Raycast(shootingRay, out hit, weaponData.range, hitMask, QueryTriggerInteraction.Ignore))
                        {
                            WeaponFunctions.ShootingLogic(attacker, weaponData, _hitEffects, hit, out var damaged);
                        }
                    }
                }
            }
            else
            {
                if (Physics.Raycast(shootingRay, out hit, weaponData.range, hitMask, QueryTriggerInteraction.Ignore))
                {
                    WeaponFunctions.ShootingLogic(attacker, weaponData, _hitEffects, hit, out var damaged);
                    _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position, hit.point);
                }
                else
                {
                    _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position,
                        (_rightWeapon ? gunpoint.position : gunpoint2.position) + dir * 13);
                }
            }
        }
    }

    private float GetDistanceToBlockingObject(Vector3 dir, Ray shootingRay)
    {
        RaycastHit hitBlocked;

        if (Physics.Raycast(shootingRay, out hitBlocked, weaponData.range, levelMask))
        {
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position, hitBlocked.point);
            return hitBlocked.distance;
        }
        else
        {
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position,
                (_rightWeapon ? gunpoint.position : gunpoint2.position) + dir * 13);
            return weaponData.range;
        }
    }

    private void PiercingDamage(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var dmgd = hits[i].collider.GetComponentInParent<IDamagable>();
            if (_damaged.Contains(dmgd))
            {
                Debug.Log($"{dmgd} was found");

                return;
            }

            if (hits[i].point == null)
                return;

            WeaponFunctions.ShootingLogic(attacker, weaponData, _hitEffects, hits[i], out var damaged);

            if (damaged != null)
            {
                _damaged.Add(damaged);
                Debug.Log($"{damaged} was damaged");
            }
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position, hits[i].point);
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectile = _projectilesPool.Get();

        var proj = projectile.GetComponent<ProjectileScript>();
        proj.SetAttacker(attacker);
        proj.OnHit += ReturnRocket;

        projectile.transform.position = gunpoint.position;
        projectile.transform.forward = playerCamera.forward;
        projectile.GetComponent<Rigidbody>().velocity = playerCamera.forward * 40;

        StartCoroutine(ReturnIfMiss(projectile, 30));
    }

    private void ReturnRocket(GameObject proj)
    {
        _projectilesPool.Return(proj);
    }

    private IEnumerator ReturnIfMiss(GameObject proj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnRocket(proj);
    }

    public override WeaponDataSO WeaponData() => weaponData;

    public override bool WasDualWielded() => _dualWielded;
}
