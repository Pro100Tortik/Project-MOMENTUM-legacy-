using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponSlot
{
    public string Name;
    public List<GameObject> weapons;

    public void ActivateWeapon(int weaponIndex) => weapons[weaponIndex].SetActive(true);

    public WeaponDataSO GetWeaponInfo(int weaponIndex)
    {
        if (weapons.Count <= 0) return null;

        if (weaponIndex < 0 || weaponIndex > weapons.Count) return null;
        WeaponAbstract weapon = null;
        try
        {
            weapon = weapons[weaponIndex].GetComponent<WeaponAbstract>();
        }
        catch
        {
            Debug.Log("Zaebal");
        }

        return weapon != null ? weapon.WeaponData() : null;
    }
}
