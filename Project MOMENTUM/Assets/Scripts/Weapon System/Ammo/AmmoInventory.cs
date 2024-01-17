using System;
using System.Collections.Generic;
using UnityEngine;

public class AmmoInventory : MonoBehaviour, ISaveable
{
    public event Action<AudioClip> OnAmmoPickedUp;
    [SerializeField] private List<WeaponAmmo> ammoList;
    [SerializeField] private AudioClip pickupSound;
    private ClientStatus _status;
    private bool _pickedUp;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        WeaponAmmo ammo = ammoList[(int)ammoType];
        return ammo.ammo;
    }

    public int UseAmmo(AmmoType ammoType, int ammoAmount)
    {
        WeaponAmmo ammoBefore = ammoList[(int)ammoType];
        int used = Mathf.Min(ammoAmount, ammoBefore.ammo);
        ammoBefore.ammo -= used;
        ammoList[(int)ammoType] = ammoBefore;

        return used;
    }

    public int AddAmmo(AmmoType ammoType, int ammoAmount)
    {
        WeaponAmmo ammoBefore = ammoList[(int)ammoType];
        int added = Mathf.Min(ammoAmount, ammoBefore.maxAmmo - ammoBefore.ammo);

        if (added < 1)
            return 0;

        ammoBefore.ammo += added;
        ammoList[(int)ammoType] = ammoBefore;
        _pickedUp = true;
        OnAmmoPickedUp?.Invoke(pickupSound);

        return added;
    }

    private void UpdateAmmoInventory()
    {
        string[] ammoNames = Enum.GetNames(typeof(AmmoType));
        List<WeaponAmmo> invetory = new List<WeaponAmmo>(ammoNames.Length);
        for (int i = 0; i < ammoNames.Length; i++) 
        {
            WeaponAmmo ammo = ammoList.Find(entry => entry.ammoName == ammoNames[i]);
            ammo.ammoName = ammoNames[i];
            ammo.ammo = Mathf.Min(ammo.ammo, ammo.maxAmmo);
            invetory.Add(ammo);
        }
        ammoList = invetory;
    }

    private void OnValidate() => UpdateAmmoInventory();

    private void OnTriggerEnter(Collider other)
    {
        if (_status.CurrentClientState == PlayerState.Dead) return;

        AmmoPickup ammo = other.GetComponent<AmmoPickup>();
        if (ammo)
        {
            AddAmmo(ammo.AmmoType, ammo.AmmoAmount);
            if (_pickedUp)
            {
                other.gameObject.SetActive(false);
                _pickedUp = false;
            }
        }
    }

    public void LoadData(GameData data)
    {
        ammoList = data.playerData.ammo;
    }

    public void SaveData(GameData data)
    {
        data.playerData.ammo = ammoList;
    }
}
