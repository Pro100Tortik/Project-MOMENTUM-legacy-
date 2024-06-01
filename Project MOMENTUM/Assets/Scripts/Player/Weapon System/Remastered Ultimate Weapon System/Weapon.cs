using UnityEngine;

public class Weapon : WeaponAbstract
{
    [Header("References")]
    [SerializeField] protected Player player;
    [SerializeField] protected WeaponDataSO weaponData;
    [SerializeField] protected AudioSource weaponSource;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private float afterChangeCooldown = 0.25f;
    [SerializeField] protected LayerMask levelMask;
    [SerializeField] protected LayerMask hitMask;

    protected AmmoInventory ammoInventory => player.ammoInventory;
    private bool _safetyLock = false;
    private float _primaryAttackCooldown;
    private float _secondaryAttackCooldown;
    private bool _isShooting = false;
    private bool _isShootingPrimary = false;
    private bool _isShootingSecondary = false;

    protected virtual void PrimaryAttack() { }

    protected virtual void SecondaryAttack() { }

    protected virtual void PrimaryAttackUp() { }

    protected virtual void SecondaryAttackUp() { }

    protected virtual bool IsPrimaryReady()
    {
        if (Time.time < _primaryAttackCooldown)
            return false;

        _primaryAttackCooldown = Time.time + 1f / weaponData.primaryFireRate;

        return true;
    }

    protected virtual bool IsSecondaryReady()
    {
        // If primary attack is not ready you can't use secondary. Like scoping in CS.
        if (Time.time < Mathf.Max(_primaryAttackCooldown, _secondaryAttackCooldown))
            return false;

        _secondaryAttackCooldown = Time.time + 1f / weaponData.secondaryFireRate;

        return true;
    }

    protected virtual bool HaveAmmo(bool isUsingAmmo, int bulletsPerShot)
    {
        if (!isUsingAmmo)
            return true;

        if (weaponData.primaryAmmoType == AmmoType.None)
            return true;

        if (ammoInventory.InfiniteAmmo)
            return true;

        int used = ammoInventory.UseAmmo(weaponData.primaryAmmoType, bulletsPerShot);

        return used > 0;
    }

    private void OnEnable()
    {
        // Delay after the player has taken out the gun, so he can't shoot instantly.
        if (IsPrimaryReady())
            _primaryAttackCooldown = Time.time + afterChangeCooldown;
        if (IsSecondaryReady())
            _secondaryAttackCooldown = Time.time + afterChangeCooldown;

        // Disallow shooting if weapon have safety lock.
        if (!weaponData.safetyLock)
        {
            _safetyLock = false;
            return;
        }

        _safetyLock = true;
    }

    private void Update() 
    {
        // Check if player can hold button to initiate attack
        if (weaponData.isPrimaryAutomatic) _isShootingPrimary = Input.GetKey(KeyCode.Mouse0);
        else _isShootingPrimary = Input.GetKeyDown(KeyCode.Mouse0);

        if (weaponData.isSecondaryAutomatic) _isShootingSecondary = Input.GetKey(KeyCode.Mouse1);
        else _isShootingSecondary = Input.GetKeyDown(KeyCode.Mouse1);

        if (Input.GetKeyUp(KeyCode.Mouse0))
            PrimaryAttackUp();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            SecondaryAttackUp();

        _isShooting = _isShootingPrimary || _isShootingSecondary;

        if (weaponData.safetyLock)
        {
            // Disable safety lock so player can shoot
            if (!_isShooting)
            {
                _safetyLock = false;
            }
        }

        ShootingLogic();
    }

    private void ShootingLogic()
    {
        // Disallow game do anything if player paused game or something.
        if (!player.CanReadInputs())
            return;

        // Disallow shooting if the gun have safety lock.
        if (weaponData.safetyLock && _safetyLock && (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)))
            return;

        if (_isShootingPrimary && IsPrimaryReady())
        {
            weaponSource.pitch = Random.Range(0.9f, 1.1f);

            _isShootingPrimary = false;
            if (HaveAmmo(weaponData.primaryUseAmmo, weaponData.bulletsPerShotPrimary))
            {
                PrimaryAttack();
                muzzleFlash?.Play();
                weaponSource?.PlayOneShot(weaponData.fire1);
            }
            else
            {
                weaponSource?.PlayOneShot(weaponData.empty);
            }
        }

        if (_isShootingSecondary && IsSecondaryReady())
        {
            weaponSource.pitch = Random.Range(0.9f, 1.1f);

            _isShootingSecondary = false;
            if (HaveAmmo(weaponData.secondaryUseAmmo, weaponData.bulletsPerShotSecondary))
            {
                SecondaryAttack();
                //muzzleFlash?.Play();
                weaponSource?.PlayOneShot(weaponData.fire2);
            }
            else
            {
                weaponSource?.PlayOneShot(weaponData.empty);
            }
        }
    }

    public override WeaponDataSO WeaponData() => weaponData;
}
