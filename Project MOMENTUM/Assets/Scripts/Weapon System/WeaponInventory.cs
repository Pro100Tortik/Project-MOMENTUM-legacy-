using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour, ISaveable
{
    public event Action<AudioClip> OnWeaponAdded;
    public event Action<WeaponDataSO> OnWeaponPickedUp;
    public event Action<WeaponDataSO> OnSecondWeaponPickedUp;

    [SerializeField] private List<WeaponDataSO> weaponList;
    [SerializeField] private List<WeaponDataSO> secondWeaponList;
    [SerializeField] private AudioClip pickupSound;
    private ClientStatus _status;
    private bool _pickedUp;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    public void AddWeapon(WeaponDataSO weapon)
    {
        if (!HaveWeapon(weapon))
        {
            weaponList.Add(weapon);
            OnWeaponPickedUp?.Invoke(weapon);
            OnWeaponAdded?.Invoke(pickupSound);
            _pickedUp = true;
        }
        else
        {
            AddSecondWeapon(weapon);
        }
    }

    public bool HaveWeapon(WeaponDataSO weapon)
    {
        if (weapon == null)
            return true;

        return weaponList.Contains(weapon);
    }

    public void AddSecondWeapon(WeaponDataSO weapon)
    {
        if (HaveSecondWeapon(weapon))
            return;

        secondWeaponList.Add(weapon);
        OnWeaponAdded?.Invoke(pickupSound);
        OnSecondWeaponPickedUp?.Invoke(weapon);
        _pickedUp = true;
    }

    public bool HaveSecondWeapon(WeaponDataSO weapon)
    {
        if (weapon == null)
            return true;

        return secondWeaponList.Contains(weapon);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_status.CurrentClientState == PlayerState.Dead) return;

        WeaponPickup weapon = other.GetComponent<WeaponPickup>();
        if (weapon)
        {
            AddWeapon(weapon.WeaponData);
            if (_pickedUp)
            {
                other.gameObject.SetActive(false);
                _pickedUp = false;
            }
        }
    }

    public void LoadData(GameData data)
    {
        weaponList = data.playerData.weapons;
        secondWeaponList = data.playerData.secondWeapons;
    }

    public void SaveData(GameData data)
    {
        data.playerData.weapons = weaponList;
        data.playerData.secondWeapons = secondWeaponList;
    }
}
