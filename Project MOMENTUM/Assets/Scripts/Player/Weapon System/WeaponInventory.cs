using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public event Action<AudioClip> OnWeaponAdded;

    [SerializeField] private Player player;

    [Header("Weapon Inventory")]
    [SerializeField] private bool startWithPistol = true;
    [SerializeField] private List<WeaponDataSO> weaponList;
    [SerializeField] private AudioClip pickupSound;
    private bool _pickedUp;

    [Header("Weapon Switching")]
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private int startWeapon;

    private int _currentWeapon = 0;
    private int _lastWeapon;

    private void Awake()
    {
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

        DisableWeapons();
        SwitchWeapon(startWeapon);
    }

    #region Weapon Inventory
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
        }
    }

    public void AddWeapon(WeaponDataSO weapon, bool playSound = true)
    {
        if (!HaveWeapon(weapon))
        {
            weaponList.Add(weapon);
            ChangeOnPickup(weapon);

            if (playSound)
                OnWeaponAdded?.Invoke(pickupSound);

            _pickedUp = true;
        }
    }

    public bool HaveWeapon(WeaponDataSO weapon)
    {
        if (weapon == null)
            return true;

        return weaponList.Contains(weapon);
    }
    #endregion

    private void StartWithPistol()
    {
        WeaponDataSO pistol = Resources.Load<WeaponDataSO>("Weapon Datas/Pistol");
        AddWeapon(pistol, false);
    }

    private void Update()
    {
        if (player.IsDead)
        {
            DisableWeapons();
            return;
        }

        if (!player.CanReadInputs())
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchWeapon(3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchWeapon(4);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SwitchWeapon(5);

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeapon(_lastWeapon);
    }

    private void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex > weapons.Count - 1)
            return;

        if (!HaveWeapon(GetWeaponData(weaponIndex)))
            return;

        _lastWeapon = _currentWeapon;
        _currentWeapon = weaponIndex;

        DisableWeapons();
        EnableWeapon(_currentWeapon);
    }

    private void DisableWeapons()
    {
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }
    }

    private void EnableWeapon(int index) => weapons[index].SetActive(true);

    public WeaponDataSO GetCurrentWeaponData() => GetWeaponData(_currentWeapon);

    private WeaponDataSO GetWeaponData(int index)
    {
        if (index > weapons.Count) return null;

        var weapon = weapons[index].GetComponent<WeaponAbstract>();
        var data = weapon != null ? weapon.WeaponData() : null;
        return data;
    }

    private void ChangeOnPickup(WeaponDataSO weapon)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (GetWeaponData(i) == weapon)
            {
                DisableWeapons();
                EnableWeapon(i);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!player.CanReadInputs()) return;

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
}
