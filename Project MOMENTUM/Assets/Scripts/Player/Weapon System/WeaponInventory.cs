using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour, ISaveable
{
    public event Action<AudioClip> OnWeaponAdded;
    public event Action<WeaponDataSO> OnWeaponPickedUp;
    public event Action<WeaponDataSO> OnSecondWeaponPickedUp;

    [SerializeField] private bool startWithPistol = true;
    [SerializeField] private List<WeaponDataSO> weaponList;
    [SerializeField] private List<WeaponDataSO> secondWeaponList;
    [SerializeField] private AudioClip pickupSound;
    private ClientStatus _status;
    private bool _pickedUp;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();

        if (startWithPistol)
        {
            StartWithPistol();
        }
    }

    private void Start()
    {
        DeveloperConsole.RegisterCommand("groooovy", "", "Add all weapons.", args =>
        {
            AddAllWeapons();
        });
    }

    public void AddWeapon(WeaponDataSO weapon, bool playSound = true)
    {
        if (!HaveWeapon(weapon))
        {
            weaponList.Add(weapon);
            OnWeaponPickedUp?.Invoke(weapon);

            if (playSound)
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

    public void AddSecondWeapon(WeaponDataSO weapon, bool playSound = true)
    {
        if (HaveSecondWeapon(weapon))
            return;

        secondWeaponList.Add(weapon);

        if (playSound)
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

    private void StartWithPistol()
    {
        WeaponDataSO pistol = Resources.Load<WeaponDataSO>("Weapon Datas/Pistol");
        AddWeapon(pistol, false);
    }

    private void AddAllWeapons()
    {
        var weaponDatas = Resources.LoadAll<WeaponDataSO>("Weapon Datas");

        if (weaponDatas == null)
        {
            Debug.LogWarning("No WeaponData found.");
            return;
        }

        foreach (var weapon in weaponDatas)
        {
            AddWeapon(weapon);
            AddSecondWeapon(weapon);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_status.CanReadInputs()) return;

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
