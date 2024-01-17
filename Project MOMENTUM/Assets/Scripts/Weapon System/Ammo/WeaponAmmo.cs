using UnityEngine;

[System.Serializable]
public struct WeaponAmmo
{
    [HideInInspector] public string ammoName;
    public int ammo;
    public int maxAmmo;
}
