using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public bool safetyLock;
    public AmmoType primaryAmmoType;
    public AmmoType secondaryAmmoType;

    [Header("Weapon Stats")]
    [Min(1)] public int pellets;
    [Min(0)] public Vector2 spread;

    [Header("Primary Attack")]
    public bool isPrimaryAutomatic = true;
    public int primaryDamage;
    [Min(0)] public float primaryRange;
    [Min(0)] public float primaryFireRate;
    public bool primaryUseAmmo = true;
    [Min(0)] public int bulletsPerShotPrimary;

    [Header("Secondary Attack")]
    public bool isSecondaryAutomatic = true;
    public int secondaryDamage;
    [Min(0)] public float secondaryRange;
    [Min(0)] public float secondaryFireRate;
    public bool secondaryUseAmmo = true;
    [Min(0)] public int bulletsPerShotSecondary;

    [Header("Weapon Sounds")]
    public AudioClip fire1;
    public AudioClip fire2;
    public AudioClip empty;
}
