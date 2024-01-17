using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int playerHealth;
    public int playerArmor;

    public List<WeaponDataSO> weapons;
    public List<WeaponDataSO> secondWeapons;
    public List<WeaponAmmo> ammo;

    public PlayerData()
    {
        playerHealth = 100;
        playerArmor = 0;

        weapons = new List<WeaponDataSO>();
        secondWeapons = new List<WeaponDataSO>();
    }
}
