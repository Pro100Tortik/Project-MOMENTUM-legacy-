using System.Data;
using UnityEngine;

public class Rifle : WeaponAbstract
{
    [SerializeField] private GameObject attacker;
    [SerializeField] private bool isPlayersWeapon = true;

    [Header("References")]
    [SerializeField] private LayerMask shootThrough;
    [SerializeField] private LayerMask levelMask;
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

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    private void Start()
    {
        _hitEffects = HitEffectsLibrary.Instance;
        _bulletTracer = DrawBulletTracer.Instance;
    }

    private void OnEnable()
    {
        _coolDownTimer = Time.time + afterChangeCooldown;
    }

    private void Update()
    {
        if (secondWeapon != null)
        {
            _dualWielded = secondWeapon.activeSelf;
        }
        _rightWeapon = _dualWielded ? _rightWeapon : true;
    }

    private void FixedUpdate()
    {
        if (_status.CurrentClientState != PlayerState.Gameplay)
            return;

        if (Input.GetKey(KeyCode.Mouse0) &&
            WeaponFunctions.CanShoot(weaponData, ammoInventory, _dualWielded,
            dualWieldedFireRateMultiplier, _coolDownTimer, out _coolDownTimer))
        {
            if (!WeaponFunctions.CheckAmmo(weaponData.ammoType, ammoInventory, weaponData.bulletsPerShot))
            {
                weaponSource.PlayOneShot(weaponData.empty);
                return;
            }

            Shoot();

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
        Vector3 spread = WeaponFunctions.GetSpread(weaponData.spread);
        Vector3 dir = playerCamera.forward + spread;

        Ray shootingRay = new Ray(playerCamera.position, playerCamera.forward + spread);

        float pierceDistance = GetDistanceToBlockingObject(dir, shootingRay);

        // Pierce if distance more then 0
        if (pierceDistance > 0)
        {
            RaycastHit[] pierceHits = Physics.RaycastAll(shootingRay, pierceDistance);
            PiercingDamage(pierceHits);
            if (pierceHits.Length < 1)
            {
                RaycastHit hitSomething;

                if (Physics.Raycast(shootingRay, out hitSomething, weaponData.range, -1, QueryTriggerInteraction.Ignore))
                {
                    WeaponFunctions.ShootingLogic(isPlayersWeapon, attacker, weaponData, _hitEffects, hitSomething);
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
            return 0;
        }
    }

    private void PiercingDamage(RaycastHit[] hits)
    {

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].point == null)
                return;

            WeaponFunctions.ShootingLogic(isPlayersWeapon, attacker, weaponData, _hitEffects, hits[i]);
            _bulletTracer.LaunchTrail(_rightWeapon ? gunpoint.position : gunpoint2.position, hits[i].point);
        }
    }

    public override WeaponDataSO WeaponData() => weaponData;

    public override bool WasDualWielded() => _dualWielded;
}
