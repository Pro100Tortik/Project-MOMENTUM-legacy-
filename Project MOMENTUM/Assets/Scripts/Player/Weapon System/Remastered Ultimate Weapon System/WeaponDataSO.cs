using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Cool things")]
    [Tooltip("Allow player shoot if holding attack button when switching")]
    public bool safety;
    public bool canBeDualWielded;
    public AmmoType ammoType;

    [Space]
    [Header("Weapon Stats")]
    public int damage;
    [Min(0)] public float range;

    [Tooltip("Bullets per second")]
    [Min(0)] public float fireRate;
    [Min(0)] public int bulletsPerShot;
    [Min(1)] public int pellets;
    [Min(0)] public Vector2 spread;

    [Header("Weapon Sounds")]
    public AudioClip fire1;
    public AudioClip empty;
}
