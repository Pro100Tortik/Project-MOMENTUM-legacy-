using UnityEngine;

[RequireComponent(typeof(DifficultyObjectDisabler), typeof(AmmoPickup))]
public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponDataSO weaponData;
    public WeaponDataSO WeaponData => weaponData;
}
