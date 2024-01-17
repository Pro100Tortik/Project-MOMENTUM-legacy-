using System.Collections;
using UnityEngine;

public class RocketLauncher : WeaponAbstract
{
    [SerializeField] private GameObject attacker;
    //[SerializeField] private bool isPlayersWeapon = true;

    [Header("References")]
    [SerializeField] private WeaponDataSO weaponData;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform gunpoint;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private ParticleSystem muzzleFlash1;

    [Header("Settings")]
    [SerializeField] private float afterChangeCooldown = 0.35f;

    [Header("Animations")]
    [SerializeField] private Animator firstWeaponAnim;

    private ClientStatus _status;
    private float _coolDownTimer;
    private bool _safety;

    private PoolBase<GameObject> _projectilesPool;

    public GameObject Preload() => Instantiate(projectile);
    public void GetAction(GameObject effect) => effect.SetActive(true);
    public void ReturnAction(GameObject effect) => effect.SetActive(false);

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
        _projectilesPool = new PoolBase<GameObject>(Preload, GetAction, ReturnAction, 20);
    }

    private void OnEnable()
    {
        _coolDownTimer = Time.time + afterChangeCooldown;

        _safety = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) || !Input.GetKey(KeyCode.Mouse0))
        {
            _safety = false;
        }
    }

    private void FixedUpdate()
    {
        if (_status.CurrentClientState != PlayerState.Gameplay)
            return;

        if (_safety && Input.GetKey(KeyCode.Mouse0))
        {
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) &&
            WeaponFunctions.CanShoot(weaponData, ammoInventory, false, 1, _coolDownTimer, out _coolDownTimer))
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
        muzzleFlash1.Play();
        if (firstWeaponAnim != null)
        {
            firstWeaponAnim.CrossFade("Shot", 0.2f);
        }
    }

    private void Shoot()
    {
        RaycastHit hitSomething;

        Ray shootingRay = new Ray(playerCamera.position,
            playerCamera.forward + WeaponFunctions.GetSpread(weaponData.spread));

        if (Physics.Raycast(shootingRay, out hitSomething, weaponData.range, -1, QueryTriggerInteraction.Ignore))
        {
            SpawnProjectile(hitSomething.point);
        }
        else
            SpawnProjectile(playerCamera.position + playerCamera.forward * 50);
    }

    private void SpawnProjectile(Vector3 hit)
    {
        GameObject projectile = _projectilesPool.Get();
        projectile.transform.position = gunpoint.position;
        projectile.transform.forward = playerCamera.forward;
        projectile.GetComponent<Rigidbody>().velocity = playerCamera.forward * 40;

        projectile.GetComponent<ProjectileScript>().OnHit += ReturnRocket;

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

    public override bool WasDualWielded() => false;
}
